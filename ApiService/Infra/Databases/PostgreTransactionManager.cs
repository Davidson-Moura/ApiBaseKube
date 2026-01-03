using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Databases;
using ApiService.Domain.Security;
using System.Data.Common;

namespace ApiService.Infra.Databases
{
    public class PostgreTransactionManager : IPostgreTransactionManager
    {
        private readonly IConnectionContext _dbContext;
        private readonly IClaimContext _claimContext;
        private readonly Dictionary<string, CountManager> _countManagerDictionary = new();

        private PostgreConnection _contextConnection;
        private DbTransaction _dbTransaction;
        private bool _inTransaction = false;
        private bool _completed = false;

        public PostgreTransactionManager(
            IConnectionContext dbContext,
            IClaimContext claimContext)
        {
            _dbContext = dbContext;
            _claimContext = claimContext;
        }

        public IDbContextTransaction Transaction => _contextConnection?.Database.CurrentTransaction;

        public IPostgreTransactionManager BeginTransaction()
        {
            if (!_inTransaction)
            {
                _contextConnection = _dbContext.PostgreConnection;
                _inTransaction = true;
            }

            var id = _claimContext.CurrentHttpConnectionId ?? "0";

            if (!_countManagerDictionary.TryGetValue(id, out var countManager))
            {
                countManager = new CountManager();
                _countManagerDictionary.Add(id, countManager);
            }

            lock (countManager)
            {
                if (countManager.IsEmpty())
                {
                    _contextConnection?.Database.BeginTransaction();
                }
                countManager.Add();
                countManager.AddInstance();
            }

            return this;
        }

        public void Complete()
        {
            var id = _claimContext.CurrentHttpConnectionId ?? "0";

            if (!_countManagerDictionary.TryGetValue(id, out var countManager) || countManager.IsEmpty())
                throw new InvalidOperationException("A transação não foi finalizada, pois não há transação aberta");

            lock (countManager)
            {
                countManager.Remove();

                if (!countManager.IsEmpty())
                    return;

                if (_contextConnection is null)
                    throw new InvalidOperationException("Não há conexão para finalizar a transaction");

                _contextConnection?.Database.CommitTransaction();
                _contextConnection?.Database.CloseConnection();

                _dbTransaction = null;
                _inTransaction = false;
                _completed = true;
            }
        }

        public void Dispose()
        {
            if (_completed || !_inTransaction) return;

            var id = _claimContext.CurrentHttpConnectionId ?? "0";

            if (!_countManagerDictionary.TryGetValue(id, out var countManager))
            {
                if (_inTransaction)
                {
                    _contextConnection?.Database?.RollbackTransaction();
                    _contextConnection?.Database?.CloseConnection();
                    _dbTransaction = null;
                }
                return;
            }

            lock (countManager)
            {
                countManager.RemoveInstance();

                if (!countManager.IsInstanceEmpty())
                    return;

                if (countManager.IsEmpty())
                    return;

                if (!_inTransaction || _contextConnection == null)
                    return;

                _contextConnection?.Database.RollbackTransaction();
                _contextConnection?.Database.CloseConnection();

                _countManagerDictionary.Remove(id);
                _dbTransaction = null;
                _inTransaction = false;
            }
        }

        private class CountManager
        {
            private int _count;
            private int _instanceCount;

            public void Add() => _count++;
            public void Remove() => _count--;
            public bool IsEmpty() => _count == 0;
            public bool IsSingle() => _count == 1;

            public void AddInstance() => _instanceCount++;
            public void RemoveInstance() => _instanceCount--;

            public bool IsInstanceEmpty() => _instanceCount == 0;
        }
    }
}
