# API de Contas a Pagar

API REST desenvolvida em .NET 10 com MongoDB para gerenciamento de contas a pagar e fornecedores.

## 🚀 Tecnologias

- .NET 10
- MongoDB Driver 2.23.1
- FluentValidation 11.9.0
- Swagger/OpenAPI
- ASP.NET Core

## 📋 Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/thank-you/sdk-10.0.100-windows-x64-installer)
- [MongoDB](https://www.mongodb.com/try/download/community) (versão 4.4 ou superior)

## 🔧 Instalação e Execução

### 1. Clone o repositório

```bash
git clone <https://github.com/rafaellucasdev/TesteTecnicoHoldprint.git>
cd TesteTecnicoHoldprint
```

### 2. Configure o MongoDB

Certifique-se de que o MongoDB está rodando localmente na porta padrão (27017). (Se tiver o docker instalado, é só executar:  docker run -d -p 27017:27017 --name mongodb mongo:7.0

Se necessário, ajuste a connection string no arquivo `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ContasAPagarDB"
  }
}
```

### 3. Restaure as dependências

```bash
dotnet restore
```

### 4. Execute a aplicação

```bash
cd ContasAPagar.API
dotnet run
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: http://localhost:5000/swagger

## 📚 Documentação da API

### Fornecedores (Suppliers)

#### POST /api/suppliers
Cria um novo fornecedor

**Request Body:**
```json
{
  "name": "Fornecedor Exemplo LTDA",
  "document": "12345678901",
  "email": "contato@exemplo.com"
}
```

**Response:** 201 Created
```json
{
  "id": "guid",
  "name": "Fornecedor Exemplo LTDA",
  "document": "12345678901",
  "email": "contato@exemplo.com",
  "createdAt": "2024-01-01T10:00:00Z"
}
```

#### GET /api/suppliers
Lista todos os fornecedores

**Response:** 200 OK
```json
[
  {
    "id": "guid",
    "name": "Fornecedor Exemplo LTDA",
    "document": "12345678901",
    "email": "contato@exemplo.com",
    "createdAt": "2024-01-01T10:00:00Z"
  }
]
```

#### GET /api/suppliers/{id}
Obtém um fornecedor específico

**Response:** 200 OK

---

### Contas a Pagar (Payables)

#### POST /api/payables
Cria uma nova conta a pagar

**Request Body:**
```json
{
  "supplierId": "guid-do-fornecedor",
  "description": "Pagamento de serviço X",
  "dueDate": "2024-12-31",
  "amount": 1500.50
}
```

**Response:** 201 Created
```json
{
  "id": "guid",
  "supplierId": "guid-do-fornecedor",
  "description": "Pagamento de serviço X",
  "dueDate": "2024-12-31T00:00:00Z",
  "amount": 1500.50,
  "status": "Pending",
  "paymentDate": null,
  "createdAt": "2024-01-01T10:00:00Z"
}
```

#### GET /api/payables
Lista contas a pagar com filtros opcionais

**Query Parameters:**
- `supplierId` (opcional): Filtrar por fornecedor
- `startDueDate` (opcional): Data inicial de vencimento
- `endDueDate` (opcional): Data final de vencimento
- `status` (opcional): Pending, Paid ou Canceled
- `page` (padrão: 1): Número da página
- `pageSize` (padrão: 10): Itens por página

**Exemplo:**
```
GET /api/payables?status=Pending&page=1&pageSize=10
```

**Response:** 200 OK
```json
{
  "data": [
    {
      "id": "guid",
      "supplierId": "guid-do-fornecedor",
      "description": "Pagamento de serviço X",
      "dueDate": "2024-12-31T00:00:00Z",
      "amount": 1500.50,
      "status": "Pending",
      "paymentDate": null,
      "createdAt": "2024-01-01T10:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

#### GET /api/payables/{id}
Obtém uma conta a pagar específica

**Response:** 200 OK

#### PATCH /api/payables/{id}/pay
Marca uma conta a pagar como paga

**Response:** 200 OK
```json
{
  "id": "guid",
  "supplierId": "guid-do-fornecedor",
  "description": "Pagamento de serviço X",
  "dueDate": "2024-12-31T00:00:00Z",
  "amount": 1500.50,
  "status": "Paid",
  "paymentDate": "2024-01-15T10:30:00Z",
  "createdAt": "2024-01-01T10:00:00Z"
}
```

#### PATCH /api/payables/{id}/cancel
Cancela uma conta a pagar

**Response:** 200 OK

---

## ⚙️ Regras de Negócio

### Fornecedores
- Nome é obrigatório (máx. 200 caracteres)
- Documento (CPF/CNPJ) é obrigatório e único
- Email é opcional, mas deve ser válido quando informado
- Não é permitido cadastrar dois fornecedores com o mesmo documento

### Contas a Pagar
- SupplierId deve existir no banco de dados
- Descrição é obrigatória (máx. 500 caracteres)
- Data de vencimento é obrigatória
- Valor deve ser maior que zero
- Status inicial é sempre "Pending"
- Não é possível pagar uma conta já paga ou cancelada
- Não é possível cancelar uma conta já paga

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas:

```
ContasAPagar.API/
├── Controllers/          # Endpoints da API
├── Services/            # Lógica de negócio
├── Repositories/        # Acesso aos dados (MongoDB)
├── Models/              # Entidades de domínio
├── DTOs/                # Objetos de transferência de dados
├── Validators/          # Validações com FluentValidation
├── Middleware/          # Middlewares customizados
├── Exceptions/          # Exceções customizadas
└── Program.cs           # Configuração da aplicação
```

### Padrões Utilizados

- **Repository Pattern**: Abstração do acesso a dados
- **Service Layer**: Encapsulamento da lógica de negócio
- **DTO Pattern**: Separação entre modelos de domínio e API
- **Dependency Injection**: Injeção de dependências nativa do .NET
- **Exception Handling**: Middleware global para tratamento de erros

## 🧪 Testando a API

### Via Swagger
Acesse http://localhost:5000/swagger para testar os endpoints através da interface gráfica.

### Via cURL

**Criar um fornecedor:**
```bash
curl -X POST "http://localhost:5000/api/suppliers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Fornecedor Teste",
    "document": "12345678901",
    "email": "teste@email.com"
  }'
```

**Criar uma conta a pagar:**
```bash
curl -X POST "http://localhost:5000/api/payables" \
  -H "Content-Type: application/json" \
  -d '{
    "supplierId": "GUID_DO_FORNECEDOR",
    "description": "Conta de teste",
    "dueDate": "2024-12-31",
    "amount": 1000.00
  }'
```

**Marcar conta como paga:**
```bash
curl -X PATCH "http://localhost:5000/api/payables/{id}/pay"
```

## 📝 Validações

A API utiliza FluentValidation para validações:

- Campos obrigatórios não podem ser vazios
- Documento deve ser CPF (11 dígitos) ou CNPJ (14 dígitos)
- Email deve ter formato válido
- Valores monetários devem ser maiores que zero
- Validações de regras de negócio são feitas na camada de serviço

## 🔒 Tratamento de Erros

Todos os erros são tratados pelo `ExceptionHandlingMiddleware` e retornam respostas padronizadas:

**400 Bad Request**: Validações ou regras de negócio
```json
{
  "message": "Descrição do erro",
  "timestamp": "2024-01-01T10:00:00Z"
}
```

**404 Not Found**: Recurso não encontrado
```json
{
  "message": "Recurso não encontrado",
  "timestamp": "2024-01-01T10:00:00Z"
}
```

**500 Internal Server Error**: Erros inesperados
```json
{
  "message": "Ocorreu um erro interno no servidor",
  "timestamp": "2024-01-01T10:00:00Z"
}
```

## 📦 Estrutura do MongoDB

### Collections

**suppliers**
```json
{
  "_id": "string",
  "name": "string",
  "document": "string",
  "email": "string (optional)",
  "createdAt": "datetime"
}
```

**payables**
```json
{
  "_id": "string",
  "supplierId": "string",
  "description": "string",
  "dueDate": "datetime",
  "amount": "decimal",
  "status": "string (Pending|Paid|Canceled)",
  "paymentDate": "datetime (optional)",
  "createdAt": "datetime"
}
```

### Índices

- `suppliers.document`: Índice único
- `payables`: Índice composto (supplierId, dueDate, status)

## 👨‍💻 Autor

Desenvolvido como teste técnico para demonstração de conhecimentos em .NET e MongoDB.

## 📄 Licença

Este projeto foi desenvolvido para fins de avaliação técnica.

