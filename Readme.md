# Gerar kubernete
- aspire publish -o k8s-artifacts

## Pastas
- Chart.yaml → metadados do chart
- values.yaml → configurações (imagem, portas, replicas)
- templates/ → Deployments, Services, etc (parametrizados)

## Precisamos do helm para executar o kubernetes
    - Obs: O Helm não constrói imagem, ele apenas referencia.
        - Veja se todas as imagens estão geradas: docker images
        - Se não tiver, construir na pasta do projeto:  dotnet publish -c Release -p:PublishProfile=DefaultContainer
            - O projetos precisam ter: <EnableSdkContainerSupport>true</EnableSdkContainerSupport>

### Gerar os containers
- dotnet publish Web.csproj -c Release -t:PublishContainer
- dotnet publish ApiService.csproj -c Release -t:PublishContainer

### Instalar app
    - Instalar o app:
        - helm install clinit-more ./k8s-artifacts
        - helm uninstall clinit-more

### Testar
    - Ver a porta no node: kubectl get svc
    - Expor a porta no local: 
        kubectl port-forward svc/webfrontend-service 8081:8081
        kubectl port-forward svc/apiservice-service 8080:8080
    - Acessar: http://localhost:8080

    - Ver se os pods estão rodando:  kubectl get pods
    - Ver logs:
        kubectl logs deployment/webfrontend-deployment
        kubectl logs deployment/apiservice-deployment

    - Verificar ingress:
        - Precisa te o ingress instalado no kubernetes:
            kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.14.1/deploy/static/provider/cloud/deploy.yaml

        kubectl get ingress
        kubectl describe ingress clinit-ingress
        kubectl port-forward svc/nginx-service 8080:80 //Expor ingress

    - Verificar endpoints disponíveis:
        kubectl get endpoints apiservice-service webfrontend-service

    - Acessar em:
        - localhost
        - localhost/api


### Subir imagens para o linux

#### Fazer login
- ssh root@85.209.92.205

#### Enviar os arquivos helm para a VM
- scp -r k8s-artifacts root@85.209.92.205:/root/

#### gerar .tar
- docker save apiservice:latest -o apiservice.tar
- docker save webfrontend:latest -o webfrontend.tar

- scp *.tar root@85.209.92.205:/root/

#### importar docker (Não precisa)
docker load < apiservice.tar
docker load < webfrontend.tar

#### importar o k3s (Só se precisar importar do docker)
docker save apiservice:latest | ctr -n k8s.io images import -
docker save webfrontend:latest | ctr -n k8s.io images import -
#### Importar direto para o k8s

ctr -n k8s.io images import apiservice.tar
ctr -n k8s.io images import webfrontend.tar

#### Verificar se agora está visivel a imagem:
- kubectl get pods -o wide
- k3s ctr images list | grep apiservice
- k3s ctr images list | grep webfrontend
- kubectl get svc
- ss -tulnp
- kubectl get ingress
- kubectl describe ingress clinit-ingress

- helm upgrade --install clinit-more .

#### Listar as aplicações Helm
- helm list -A

#### Instalar a aplicação kubernetes
- helm install clinit-more ./k8s-artifacts
- helm uninstall clinit-more

#### teste na VM
- curl http://localhost/

### Veficar as requisições que acontecem no ingress

- kubectl get pods -n ingress-nginx
- kubectl logs -n ingress-nginx <nome-do-pod>
- kubectl logs -n ingress-nginx ingress-nginx-controller-6c6d87df48-59v6l

### Remover artefaos na VM
rm -r k8s-artifacts/


### Verificar pods
kubectl logs -f apiservice-deployment-5cf96997cb-ks825

## Postgres no docker
docker run -d --name postgres-db -e POSTGRES_PASSWORD=inovy -e POSTGRES_DB=appdb -v postgres_data:/var/lib/postgresql/data -p 5432:5432 postgres:16

Host=localhost;Port=5432;Database=appdb;Username=postgres;Password=inovy


### Criar migração
dotnet ef migrations add InitialCreate
### Adicionar cada migração
dotnet ef migrations add AdminUserTables
### Atualiza o banco de dados no ambiente
dotnet ef database update

### Migrar em produção
dotnet ef migrations script \
  --project ApiService.Infra \
  --startup-project ApiService.Api \
  --idempotent \
  -o migrate.sql

psql -f migrate.sql

### Voltar todas as migrações
dotnet ef database update 0

### Apagar tudo e criar de novo
dotnet ef database drop --force
dotnet ef database update

### Apagar todas as migrações criadas, mas não altera o banco
- deletar a pasta Migrations do projeto

- deletar a tabela de migrações
DELETE FROM "__EFMigrationsHistory";

- Criar a primeira migração de novo
dotnet ef migrations add InitialCreate
dotnet ef database update