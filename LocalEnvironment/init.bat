docker pull mcr.microsoft.com/mssql/server:2017-latest
docker pull mcr.microsoft.com/mssql-tools:latest
docker pull dbeaver/cloudbeaver:latest
docker pull rnwood/smtp4dev:v3
docker-compose -p photobook-api-local up --force-recreate --build