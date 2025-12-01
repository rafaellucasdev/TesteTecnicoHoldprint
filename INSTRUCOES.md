# Instruções de Execução - Teste Técnico

## 🎯 Opções de Execução

Você pode executar este projeto de duas formas:

### Opção 1: Execução Local (Recomendado para desenvolvimento)

#### Pré-requisitos
1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Instale o [MongoDB Community Edition](https://www.mongodb.com/try/download/community)

#### Passos

1. **Inicie o MongoDB**
   - Windows: O MongoDB geralmente inicia automaticamente como serviço
   - Linux/Mac: Execute `sudo systemctl start mongod` ou `brew services start mongodb-community`
   - Verifique se está rodando na porta 27017

2. **Clone e execute o projeto**
   ```bash
   cd ContasAPagar.API
   dotnet restore
   dotnet run
   ```

3. **Acesse a API**
   - Swagger UI: http://localhost:5000/swagger
   - API: http://localhost:5000/api

### Opção 2: Usando Docker Compose (Mais fácil)

#### Pré-requisitos
1. Instale o [Docker Desktop](https://www.docker.com/products/docker-desktop)

#### Passos

1. **Execute o Docker Compose**
   ```bash
   docker-compose up -d
   ```

   Isso irá:
   - Criar um container com MongoDB
   - Buildar e executar a API
   - Configurar a rede entre os containers

2. **Acesse a API**
   - Swagger UI: http://localhost:5000/swagger
   - API: http://localhost:5000/api

3. **Para parar os containers**
   ```bash
   docker-compose down
   ```

## 📋 Testando a API

### Usando Swagger (Interface Gráfica)

1. Acesse http://localhost:5000/swagger
2. Teste os endpoints diretamente pela interface

### Usando Postman

1. Importe o arquivo `ContasAPagar.postman_collection.json` no Postman
2. Os endpoints já estarão configurados e prontos para uso

### Exemplo de Fluxo Completo

#### 1. Criar um Fornecedor
```bash
curl -X POST "http://localhost:5000/api/suppliers" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Fornecedor ABC LTDA",
    "document": "12345678901234",
    "email": "contato@abc.com"
  }'
```

**Resposta esperada (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Fornecedor ABC LTDA",
  "document": "12345678901234",
  "email": "contato@abc.com",
  "createdAt": "2024-12-01T10:00:00Z"
}
```

**Importante:** Guarde o `id` retornado para usar nos próximos passos!

#### 2. Criar uma Conta a Pagar
```bash
curl -X POST "http://localhost:5000/api/payables" \
  -H "Content-Type: application/json" \
  -d '{
    "supplierId": "550e8400-e29b-41d4-a716-446655440000",
    "description": "Pagamento de serviço de consultoria",
    "dueDate": "2024-12-31",
    "amount": 2500.00
  }'
```

**Resposta esperada (201 Created):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "supplierId": "550e8400-e29b-41d4-a716-446655440000",
  "description": "Pagamento de serviço de consultoria",
  "dueDate": "2024-12-31T00:00:00Z",
  "amount": 2500.00,
  "status": "Pending",
  "paymentDate": null,
  "createdAt": "2024-12-01T10:05:00Z"
}
```

#### 3. Listar Contas a Pagar
```bash
curl -X GET "http://localhost:5000/api/payables?page=1&pageSize=10&status=Pending"
```

**Resposta esperada (200 OK):**
```json
{
  "data": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "supplierId": "550e8400-e29b-41d4-a716-446655440000",
      "description": "Pagamento de serviço de consultoria",
      "dueDate": "2024-12-31T00:00:00Z",
      "amount": 2500.00,
      "status": "Pending",
      "paymentDate": null,
      "createdAt": "2024-12-01T10:05:00Z"
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1
}
```

#### 4. Marcar Conta como Paga
```bash
curl -X PATCH "http://localhost:5000/api/payables/660e8400-e29b-41d4-a716-446655440001/pay"
```

**Resposta esperada (200 OK):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "supplierId": "550e8400-e29b-41d4-a716-446655440000",
  "description": "Pagamento de serviço de consultoria",
  "dueDate": "2024-12-31T00:00:00Z",
  "amount": 2500.00,
  "status": "Paid",
  "paymentDate": "2024-12-01T10:10:00Z",
  "createdAt": "2024-12-01T10:05:00Z"
}
```

#### 5. Tentar Cancelar Conta Paga (Erro Esperado)
```bash
curl -X PATCH "http://localhost:5000/api/payables/660e8400-e29b-41d4-a716-446655440001/cancel"
```

**Resposta esperada (400 Bad Request):**
```json
{
  "message": "Não é possível cancelar uma conta que já foi paga",
  "timestamp": "2024-12-01T10:15:00Z"
}
```

## 🔍 Verificando os Dados no MongoDB

Se quiser verificar os dados diretamente no banco:

### MongoDB Compass (Interface Gráfica)
1. Instale o [MongoDB Compass](https://www.mongodb.com/products/compass)
2. Conecte em `mongodb://localhost:27017`
3. Acesse o banco `ContasAPagarDB`
4. Veja as collections `suppliers` e `payables`

### MongoDB Shell
```bash
# Conectar ao MongoDB
mongosh

# Selecionar o banco de dados
use ContasAPagarDB

# Ver fornecedores
db.suppliers.find().pretty()

# Ver contas a pagar
db.payables.find().pretty()

# Filtrar contas pendentes
db.payables.find({ status: "Pending" }).pretty()
```

## ✅ Checklist de Funcionalidades

- [x] Cadastro de Fornecedor com validação de documento único
- [x] Validação de CPF/CNPJ (11 ou 14 dígitos)
- [x] Validação de email opcional
- [x] Cadastro de Conta a Pagar
- [x] Verificação de existência do fornecedor
- [x] Status inicial sempre Pending
- [x] Listagem de Contas com filtros:
  - [x] Por SupplierId
  - [x] Por período de vencimento
  - [x] Por Status
  - [x] Paginação
- [x] Marcar conta como paga
- [x] Preenchimento automático de PaymentDate
- [x] Validação: não pagar conta já paga ou cancelada
- [x] Cancelar conta
- [x] Validação: não cancelar conta já paga
- [x] Tratamento de erros consistente
- [x] Separação de camadas (Controllers, Services, Repositories)
- [x] Índices no MongoDB para performance

## 📊 Estrutura do Banco de Dados

O banco `ContasAPagarDB` será criado automaticamente com as seguintes collections:

### suppliers
- Índice único em `document`
- Campos: id, name, document, email, createdAt

### payables
- Índice composto em (supplierId, dueDate, status)
- Campos: id, supplierId, description, dueDate, amount, status, paymentDate, createdAt

## 🐛 Troubleshooting

### Erro: "Unable to connect to MongoDB"
- Verifique se o MongoDB está rodando: `mongosh` ou verifique o serviço
- Confirme que está na porta 27017
- No Windows, verifique os serviços do sistema

### Erro: "Port 5000 is already in use"
- Mude a porta no `launchSettings.json`
- Ou pare o processo que está usando a porta 5000

### Erro ao buildar o projeto
- Execute `dotnet restore` na pasta ContasAPagar.API
- Verifique se tem o .NET 8 instalado: `dotnet --version`

## 📞 Suporte

Para dúvidas ou problemas, verifique:
1. README.md - Documentação completa da API
2. Swagger UI - Documentação interativa dos endpoints
3. Logs da aplicação - Verifique o console onde a API está rodando

## 🎓 Considerações Técnicas

Este projeto demonstra:
- Arquitetura em camadas bem definida
- Repository Pattern para abstração do banco
- Service Layer para lógica de negócio
- FluentValidation para validações
- Exception Handling middleware customizado
- Boas práticas de código .NET
- Documentação com Swagger/OpenAPI
- Containerização com Docker
- Índices apropriados no MongoDB

Desenvolvido como teste técnico para demonstração de habilidades em .NET e MongoDB.

