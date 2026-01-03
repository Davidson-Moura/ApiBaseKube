using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class Cryptography
    {
        private static string _key = "";
        private static string _shortKey = "";
        public static void SetKey(string key ) => _key = key;
        public static void SetShortKey(string key ) => _shortKey = key;
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                100_000,
                HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(32);

            return $"100000.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }
        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            byte[] computedHash = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(
                storedHashBytes,
                computedHash);
        }
        public static string SHA256(string input)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = System.Security.Cryptography.SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(input));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        public static string EncryptString(string text)
        {
            return EncryptString(text, _key);
        }
        private static string EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aes = Aes.Create())
            {
                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(text);
                            }
                        }

                        var iv = aes.IV;

                        var encrypted = ms.ToArray();

                        var result = new byte[iv.Length + encrypted.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            return DecryptString(cipherText, _key);
        }
        private static string DecryptString(string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16]; // AES usa IV de 16 bytes
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            var _key = Encoding.UTF8.GetBytes(key);

            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(_key, iv))
            using (var ms = new MemoryStream(cipher))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }

        public static byte[] EncryptFile(string fileName)
        {
            try
            {
                var path = Path.GetDirectoryName(fileName);
                var outputFile = Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + ".lic");
                File.Create(outputFile).Close();
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(_shortKey);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(fileName, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
                return File.ReadAllBytes(outputFile);
            }
            catch
            {
                throw;
            }
        }

        public static byte[] EncryptFile(MemoryStream file)
        {
            try
            {

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(_shortKey);

                string cryptFile = Path.GetTempFileName();
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);



                int data;
                while ((data = file.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                file.Close();
                cs.Close();
                fsCrypt.Close();
                return file.ToArray();
            }
            catch
            {
                throw;
            }
        }



        public static byte[] DecryptFile(string fileName)
        {

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(_shortKey);

            FileStream fsCrypt = new FileStream(fileName, FileMode.Open);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(key, key),
                CryptoStreamMode.Read);

            var outputFile = Path.Combine(Path.GetDirectoryName(fileName)
                , Path.GetFileNameWithoutExtension(fileName) + ".lic");
            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();

            return File.ReadAllBytes(outputFile);
        }

        public static MemoryStream EncryptFileMemory(MemoryStream memorySource)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(_shortKey);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                var output = new MemoryStream();
                CryptoStream cs = new CryptoStream(output,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);


                int data;
                while ((data = memorySource.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                memorySource.Close();
                cs.Close();

                return output;
            }
            catch
            {
                throw;
            }
        }

        public static MemoryStream DecryptFileMemory(MemoryStream encrytedFile, bool closeStream = false)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(_shortKey);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(encrytedFile,
                RMCrypto.CreateDecryptor(key, key),
                CryptoStreamMode.Read);

            var fsOut = new MemoryStream();

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            cs.Close();
            if (closeStream) encrytedFile.Close();

            return fsOut;
        }

        public static string GenerateFileHash(Stream stream)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
        public static string SanitializeHash(this Guid id)
        {
            return id.ToString().SanitializeHash();
        }
        public static string SanitializeHash(this string id)
        {
            return id.Replace("-", "").ToLowerInvariant();
        }
        public static string GenerateSecureToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenBytes = new byte[32];
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes);
            }
        }
        public static string EncryptP(string plainText, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    var result = Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
                    return result;
                }
            }
        }
        private static string DecryptP(string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            var iv = fullCipher.Take(16).ToArray();
            var cipherBytes = fullCipher.Skip(16).ToArray();

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
        
    }
}
