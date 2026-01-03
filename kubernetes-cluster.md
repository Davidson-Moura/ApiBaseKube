# Configurando o cluster

## Atualizar o linux
sudo apt update && sudo apt upgrade -y

## Ajustes de kernel e rede
sudo modprobe br_netfilter
sudo tee /etc/sysctl.d/k8s.conf <<EOF
net.bridge.bridge-nf-call-iptables=1
net.ipv4.ip_forward=1
net.bridge.bridge-nf-call-ip6tables=1
EOF
sudo sysctl --system

## Desabilitar swap
sudo swapoff -a
sudo sed -i '/swap/d' /etc/fstab

## Instalar K3s no Control Plane
curl -sfL https://get.k3s.io | \
INSTALL_K3S_EXEC="\
--write-kubeconfig-mode=644 \
--disable traefik \
--disable servicelb \
--node-name k3s-master" \
sh -

### Validar
sudo kubectl get nodes
sudo systemctl status k3s

### Obter o token do cluster
sudo cat /var/lib/rancher/k3s/server/node-token

## Instalar os Workers (Pular, não precisa em um único nó)
curl -sfL https://get.k3s.io | \
K3S_URL=https://10.0.0.10:6443 \
K3S_TOKEN=K10203b515fd5e76a06323b607fdbef2ce5d7acb700e564d79a0ba50adc00961a6e::server:a5a16319a8234534bbebd17fe3f22cf2 \
INSTALL_K3S_EXEC="--node-name k3s-worker-1" \
sh -

## LoadBalancer
kubectl apply -f https://raw.githubusercontent.com/metallb/metallb/v0.14.5/config/manifests/metallb-native.yaml

## Segurança
sudo ufw allow 22
sudo ufw allow 6443
sudo ufw allow 80
sudo ufw allow 443
sudo ufw enable

## Observabilidade
kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml

## Instalar docker e helm
sudo apt update && sudo apt upgrade -y

snap install docker

sudo apt-get install curl gpg apt-transport-https --yes
curl -fsSL https://packages.buildkite.com/helm-linux/helm-debian/gpgkey | gpg --dearmor | sudo tee /usr/share/keyrings/helm.gpg > /dev/null
echo "deb [signed-by=/usr/share/keyrings/helm.gpg] https://packages.buildkite.com/helm-linux/helm-debian/any/ any main" | sudo tee /etc/apt/sources.list.d/helm-stable-debian.list
sudo apt-get update
sudo apt-get install helm

## salvar configuração do k3s
export KUBECONFIG=/etc/rancher/k3s/k3s.yaml

echo 'export KUBECONFIG=/etc/rancher/k3s/k3s.yaml' >> ~/.bashrc
source ~/.bashrc


## Adicionar nginx controller ao cluster
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/cloud/deploy.yaml
kubectl get pods -n ingress-nginx -w

kubectl patch svc ingress-nginx-controller \
-n ingress-nginx \
-p '{"spec":{"type":"ClusterIP"}}'


kubectl edit deployment ingress-nginx-controller -n ingress-nginx
    - Procure por spec.template.spec e adicione:

    hostNetwork: true
    dnsPolicy: ClusterFirstWithHostNet


    - Ficará assim:

    spec:
    template:
        spec:
        hostNetwork: true
        dnsPolicy: ClusterFirstWithHostNet
        containers:
        - name: controller
            ports:
            - containerPort: 80
            - containerPort: 443


## Atualizar 
sudo apt update
sudo apt upgrade k3s


## Limpar linux
rm -rf /root/.ssh
mkdir /root/.ssh
chmod 700 /root/.ssh
touch /root/.ssh/authorized_keys
chmod 600 /root/.ssh/authorized_keys
chown -R root:root /root/.ssh

## Copiar a chave ssh para o arquivo:
- exemplo: ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAI.... comentario
- nano /root/.ssh/authorized_keys
## Normalizar o final da linha
- dos2unix /root/.ssh/authorized_keys 2>/dev/null || true
## reiniciar o ssh
- systemctl restart ssh

## Testar 
kubectl get ingress
kubectl get svc
kubectl get svc -n ingress-nginx
kubectl get pods -n ingress-nginx

kubectl get pods -n metallb-system

helm list -A












# Utilizar certificados

## Instalar o cert-manager
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.14.5/cert-manager.yaml
kubectl get pods -n cert-manager

## Configurar o cert
### Arquivo
- cluster-issuer.yaml
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    email: davidson@inovy.com.br
    server: https://acme-v02.api.letsencrypt.org/directory
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx

### Aplicar
kubectl apply -f cluster-issuer.yaml
kubectl get clusterissuer
kubectl describe clusterissuer letsencrypt-prod

### Verificar o certificado

kubectl describe challenge
kubectl logs -n cert-manager deploy/cert-manager
kubectl logs -n ingress-nginx deploy/ingress-nginx-controller


### Criar Secret de conexão
kubectl create secret generic db-connection --from-literal=CONNECTION_STRING="Host=postgres;Port=5432;Database=appdb;Username=app;Password=secret"
