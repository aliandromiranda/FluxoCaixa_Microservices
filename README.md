Fluxo de Caixa – Microservices 

Este projeto implementa um sistema de Fluxo de Caixa utilizando arquitetura de microsserviços, com dois serviços independentes que se comunicam via RabbitMQ:

TransactionsService: responsável pelo lançamento de créditos e débitos.

ConsolidatedService: responsável pelo relatório consolidado diário.

--

1. Pré-requisitos

Instalar localmente:

- .NET 7 ou .NET 8 SDK
- Erlang 
- RabbitMQ Server

--

2. Execute localmente, via cmd ou PowerShell, digitando:

cd src/TransactionsService
dotnet run

Em outra sessão do cmd ou Powershell, digite para o Consolidado:

cd src/ConsolidatedService
dotnet run

--

3. Fluxo de Comunicação entre os Serviços
TransactionsService:

-Recebe POST de lançamentos
- Publica evento no RabbitMQ
  
ConsolidatedService

- Possui um BackgroundService que consome eventos
- Atualiza o saldo diário no banco SQLite
- Disponibiliza endpoint de relatório

Acesse o TransactionService via http://localhost:5001/swagger
Para acessar o ConsolitadedService, acesse http://localhost:5002/swagger


