# Order Management System

API simples para gerenciamento de pedidos em .NET 8.

## Sobre

Sistema básico para controle de produtos, clientes e pedidos.

## Estrutura

```
src/
├── Domain/          # Entidades e regras de negócio
├── Application/     # Casos de uso e DTOs
├── Infrastructure/  # Banco de dados e repositórios
└── API/             # Controllers e middleware
```

## Como executar

1. Configure a connection string no `appsettings.json`
2. Execute `dotnet run` na pasta da API
3. Acesse http://localhost:5000/swagger

Usa Repository Pattern e Unit of Work pra gerenciar transações.

# Order Management System

API para gerenciamento de pedidos (ASP.NET Core / .NET 8).

Resumo
------
Sistema simples para gerenciar clientes, produtos e pedidos. Código organizado em camadas (Domain / Application / Infrastructure / API) com testes automatizados e migrações EF incluídas.

Pré-requisitos
---------------
- .NET 8 SDK
- PostgreSQL (local ou via Docker)
- (Opcional) dotnet-ef CLI para gerenciar migrations

Instale o dotnet-ef globalmente se for usar as ferramentas de EF Core:

```powershell
dotnet tool install --global dotnet-ef
# ou, se o projeto usar um tool manifest:
dotnet tool restore
```

Configuração
-------------
1. Copie o arquivo de exemplo para criar sua configuração local (não comitar segredos):

```powershell
cp appsettings.json.example appsettings.json
```

2. Configure a connection string em `appsettings.json` ou use variáveis de ambiente / User Secrets (recomendado para dev local):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=OrderDb;Username=postgres;Password=sua_senha"
# Ou (PowerShell) via variáveis de ambiente temporárias:
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=OrderDb;Username=postgres;Password=sua_senha"
```

Rodando localmente
-------------------
1. Aplique as migrations (a partir da raiz do repositório):

```powershell
dotnet ef database update --project src/OrderManagement.Infrastructure --startup-project src/OrderManagement.API
```

2. Inicie a API:

```powershell
dotnet run --project src/OrderManagement.API
```

3. Abra o Swagger UI. O URL é exibido no output do `dotnet run` (normalmente `https://localhost:5001` ou `http://localhost:5000`) e a documentação estará em `/swagger`.

Testes
------
Rode os testes a partir da raiz do repositório:

```powershell
dotnet test tests/OrderManagement.Tests/OrderManagement.Tests.csproj
```

ou simplesmente:

```powershell
dotnet test
```

API — endpoints principais
--------------------------
- `GET/POST/PUT/DELETE /api/produtos`  — CRUD de produtos
- `GET/POST/PUT/DELETE /api/clientes`  — CRUD de clientes
- `GET/POST/PUT/DELETE /api/pedidos`   — CRUD de pedidos

Exemplo de request para criar um pedido:

```json
POST /api/pedidos
{
  "clienteId": 1,
  "itens": [
    { "produtoId": 1, "quantidade": 2 },
    { "produtoId": 2, "quantidade": 1 }
  ]
}
```

O total do pedido é calculado automaticamente e o estoque é atualizado quando o pedido é criado.

Observability (logs)
---------------------
O projeto usa Serilog para logs estruturados. Em ambiente local os logs aparecem no console. Se o projeto estiver configurado para persistir logs em arquivo, verifique a pasta `logs/`.

Notas de arquitetura e decisões principais
---------------------------------------
- Clean Architecture: separação clara entre Domain/Application/Infrastructure/API.
- Repositórios + Unit of Work para persistência.
- Validações de negócio colocadas no domínio (ex.: verificação de estoque, VO de Email).
- Migrations EF disponíveis em `src/OrderManagement.Infrastructure/Migrations`.

Comandos úteis
-------------

```powershell
# Criar migration
dotnet ef migrations add NomeDaMigration --project src/OrderManagement.Infrastructure --startup-project src/OrderManagement.API

# Aplicar migrations
dotnet ef database update --project src/OrderManagement.Infrastructure --startup-project src/OrderManagement.API

# Rodar a API
dotnet run --project src/OrderManagement.API

# Rodar com hot reload
dotnet watch run --project src/OrderManagement.API