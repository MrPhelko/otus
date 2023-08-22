# Инструкция по запуску через minikube

* добавить репу на bitnami ``helm repo add bitnami https://charts.bitnami.com/bitnami``
* установить postgresql c помощью helm ``helm install postgresql-dev -f .\postgresql\values.yaml bitnami/postgresql``
* запустить деплой на k8s ``kubectl apply -f . ``