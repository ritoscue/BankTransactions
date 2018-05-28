# BankTransactions
Pre - Conditions:
1) Asp .Net Core installed
2) Microsoft Sql Server


Steps:


1) Execute on own SQL Server next scripts:

    1.1   01_TR_CreateDatabase.sql
    1.2   02_TR_CreateTables.sql


2) Run Web Api project
    2.0   Change connection string on file /src/Services/TransactionsApi/appsetting.json
    2.1   Open on cmd promnt this path:  /src/Services/TransactionsApi
    2.2   Run command: dotnet run (Please check swagger page for methods: https://WebApiUri/swagger)
    3.3   First time, it's loaded seed for Transaction, wait a moment.


3) Run MVC Web project

    3.1   Change tags TransactionsApiUrl and connectionString on file    /src/Web/TransactionsWeb/appsettings.json for Web api uri
    3.2   Open on cmd promnt this path:  /src/Web/TransactionsWeb
    3.3   Run command: dotnet run
