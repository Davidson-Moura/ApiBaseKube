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
    - Instalar o app: helm install minha-app .
        - Se for atualizar: helm upgrade minha-app .
        - Desinstalar se necessário: helm uninstall minha-app

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
        kubectl describe ingress minha-app-ingress
        kubectl port-forward svc/nginx-service 8080:80 //Expor ingress

    - Verificar endpoints disponíveis:
        kubectl get endpoints apiservice-service webfrontend-service

    - Acessar em:
        - localhost
        - localhost/api



        