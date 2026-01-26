# UzunTec Database Abstraction

Uma biblioteca poderosa .NET Standard 2.0 que fornece uma interface unificada para executar consultas SQL em múltiplos mecanismos de banco de dados. Escreva seu código de banco de dados uma vez e execute-o em **Oracle**, **PostgreSQL**, **MySQL**, **SQLite** e **SQL Server** sem alterar seu código.

## Funcionalidades

- 🎯 **Suporte Multi-Banco**: Funciona perfeitamente com Oracle, PostgreSQL, MySQL, SQLite e SQL Server
- 🔄 **Suporte Async/Await**: Suporte completo ao padrão async/await para todas as operações
- 📄 **Paginação Automática**: Paginação integrada com otimizações específicas por banco de dados
- 🔒 **Suporte a Transações**: Gerenciamento fácil de transações
- 🛡️ **Consultas Parametrizadas**: Manipulação segura de parâmetros com prevenção automática de SQL injection
- 📊 **Resultados Type-Safe**: Manipulação de resultados fortemente tipada com `DataResultTable` e `DataResultRecord`
- 🔧 **Configuração Flexível**: Opções personalizáveis para diferentes dialetos de banco de dados

## Instalação

Este pacote está publicado no [NuGet.org](https://www.nuget.org/packages/UzunTec.DatabaseAbstraction/) como `UzunTec.DatabaseAbstraction`.

Instale o pacote via NuGet:

```bash
dotnet add package UzunTec.DatabaseAbstraction
```

Ou via Package Manager:

```powershell
Install-Package UzunTec.DatabaseAbstraction
```

Ou via PackageReference no seu `.csproj`:

```xml
<PackageReference Include="UzunTec.DatabaseAbstraction" Version="0.5.0" />
```

## Início Rápido

### Configuração Básica

```csharp
using System.Data.Common;
using UzunTec.Utils.DatabaseAbstraction;
using Npgsql; // Para PostgreSQL
// ou using MySql.Data.MySqlClient; // Para MySQL
// ou using System.Data.SqlClient; // Para SQL Server

// Criar connection builder
string connectionString = "Server=localhost;Database=mydb;User Id=user;Password=pass;";
ConnectionBuilder connectionBuilder = new ConnectionBuilder(
    NpgsqlFactory.Instance, // Use o factory apropriado para seu banco de dados
    connectionString
);

// Criar instância DbQueryBase
IDbQueryBase db = new DbQueryBase(connectionBuilder, DatabaseDialect.PostgreSQL);
```

### Usando com DbConnection Existente

```csharp
using (DbConnection connection = /* sua conexão */)
{
    IDbQueryBase db = new DbQueryBase(connection, DatabaseDialect.PostgreSQL);
    // Use db aqui
}
```

## Bancos de Dados Suportados

| Banco de Dados | Enum Dialect | Provider Factory |
|----------------|--------------|-------------------|
| **PostgreSQL** | `DatabaseDialect.PostgreSQL` | `NpgsqlFactory.Instance` |
| **MySQL** | `DatabaseDialect.MySql` | `MySqlClientFactory.Instance` |
| **SQL Server** | `DatabaseDialect.SqlServer` | `SqlClientFactory.Instance` |
| **SQLite** | `DatabaseDialect.SQLite` | `SQLiteFactory.Instance` |
| **Oracle** | `DatabaseDialect.Oracle` | `OracleClientFactory.Instance` |

## Operações Principais

### Consultando Dados

#### Obter Múltiplos Registros

```csharp
string query = "SELECT * FROM Users WHERE Status = @Status";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("Status", "Active")
};

DataResultTable result = db.GetResultTable(query, parameters);

foreach (DataResultRecord record in result)
{
    string name = record.GetString("UserName");
    int id = record.GetValue<int>("UserId");
}
```

#### Obter Registro Único

```csharp
string query = "SELECT * FROM Users WHERE UserId = @UserId";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserId", 123)
};

DataResultRecord record = db.GetSingleRecord(query, parameters);

if (record != null)
{
    string name = record.GetString("UserName");
    DateTime created = record.GetValue<DateTime>("CreatedDate");
}
```

#### Executar Scalar

```csharp
string query = "SELECT COUNT(*) FROM Users WHERE Status = @Status";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("Status", "Active")
};

object count = db.ExecuteScalar(query, parameters);
int userCount = Convert.ToInt32(count);
```

### Modificando Dados

#### Inserir

```csharp
string query = @"INSERT INTO Users (UserName, Email, CreatedDate) 
                 VALUES (@UserName, @Email, @CreatedDate)";

var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserName", "João Silva"),
    new DataBaseParameter("Email", "joao@exemplo.com"),
    new DataBaseParameter("CreatedDate", DateTime.Now)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

#### Atualizar

```csharp
string query = @"UPDATE Users 
                 SET Email = @Email 
                 WHERE UserId = @UserId";

var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("Email", "novoemail@exemplo.com"),
    new DataBaseParameter("UserId", 123)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

#### Deletar

```csharp
string query = "DELETE FROM Users WHERE UserId = @UserId";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserId", 123)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

## Operações Assíncronas

Todas as operações têm versões assíncronas. É recomendado usar `ConfigureAwait(false)` para melhor performance em código de biblioteca:

```csharp
// Consulta assíncrona
DataResultTable result = await db.GetResultTableAsync(query, parameters).ConfigureAwait(false);

// Registro único assíncrono
DataResultRecord record = await db.GetSingleRecordAsync(query, parameters).ConfigureAwait(false);

// Execução assíncrona
int rowsAffected = await db.ExecuteNonQueryAsync(query, parameters).ConfigureAwait(false);

// Scalar assíncrono
object count = await db.ExecuteScalarAsync(query, parameters).ConfigureAwait(false);
```

## Paginação

### Limitar Resultados

```csharp
// Obter os 10 primeiros registros
DataResultTable result = db.GetResultTable("SELECT * FROM Users ORDER BY CreatedDate DESC", 10);
```

### Offset e Count

```csharp
// Obter 20 registros começando do offset 40
DataResultTable result = db.GetLimitedRecords(
    "SELECT * FROM Users ORDER BY UserId", 
    40,  // offset
    20   // count
);
```

### Paginação Baseada em Páginas

```csharp
// Obter página 3 com 10 itens por página
DataResultTable result = db.GetPagedResultTable(
    "SELECT * FROM Users ORDER BY UserId",
    3,   // número da página (baseado em 1)
    10   // tamanho da página
);
```

A biblioteca automaticamente trata as diferenças de sintaxe de paginação entre bancos de dados:
- **SQL Server**: `OFFSET ... ROWS FETCH NEXT ... ROWS ONLY`
- **MySQL**: `LIMIT offset, count`
- **PostgreSQL**: `LIMIT count OFFSET offset`
- **SQLite**: `LIMIT count OFFSET offset`
- **Oracle**: `OFFSET ... ROWS FETCH NEXT ... ROWS ONLY`

## Stored Procedures

```csharp
string procedureName = "GetUserOrders";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserId", 123),
    new DataBaseParameter("OrderStatus", "Pending")
};

DataResultTable result = db.GetResultTableFromProcedure(procedureName, parameters);

// Versão assíncrona
DataResultTable result = await db.GetResultTableFromProcedureAsync(procedureName, parameters);
```

## Transações

```csharp
try
{
    db.BeginTransaction();
    
    // Executar múltiplas operações
    db.ExecuteNonQuery(insertQuery, insertParams);
    db.ExecuteNonQuery(updateQuery, updateParams);
    
    db.CommitTransaction();
}
catch (Exception ex)
{
    db.RollbackTransaction();
    // Tratar erro
}
```

## Trabalhando com Resultados

### DataResultTable

`DataResultTable` é uma coleção de objetos `DataResultRecord`:

```csharp
DataResultTable table = db.GetResultTable(query);

// Acessar metadados dos campos
foreach (var field in table.Fields)
{
    Console.WriteLine($"{field.Key}: {field.Value}");
}

// Converter para lista fortemente tipada usando BuildList
// Você pode passar uma referência de método ou lambda
List<User> users = table.BuildList(record => new User
{
    UserCode = record.GetValue<int>("cod_user"),
    UserName = record.GetString("user_name"),
    UserCodRef = record.GetNullableValue<long>("cod_user_ref")
});

// Ou usar uma referência de método
List<User> users2 = table.BuildList(BuildUserFromRecord);

// Obter registro único (se a tabela tiver exatamente uma linha, retorna null caso contrário)
DataResultRecord single = table.SingleRecord();
```

### DataResultRecord

`DataResultRecord` fornece acesso type-safe aos valores das colunas. Os nomes das colunas são case-sensitive e devem corresponder exatamente aos nomes das colunas do seu banco de dados:

```csharp
DataResultRecord record = db.GetSingleRecord(query, parameters);

// Obter string (trata NULL automaticamente)
string name = record.GetString("user_name");  // Nota: nomes de colunas correspondem ao case do DB

// Obter tipos de valor
int id = record.GetValue<int>("cod_user");
DateTime date = record.GetValue<DateTime>("input_date");

// Obter tipos de valor nullable
long? nullableId = record.GetNullableValue<long>("cod_user_ref");

// Obter enum (de valores char/int armazenados no banco de dados)
StatusUser status = record.GetEnum<StatusUser>("user_status");
StatusUser? nullableStatus = record.GetNullableEnum<StatusUser>("optional_status");
```

## Parâmetros

### Criando Parâmetros

```csharp
// Parâmetro simples (prefixo @ é opcional ao criar o parâmetro)
var param = new DataBaseParameter("UserId", 123);
// ou
var param2 = new DataBaseParameter("@UserId", 123);  // Ambos funcionam da mesma forma

// Parâmetro com valor nullable (automaticamente convertido para DBNull)
var nullableParam = new DataBaseParameter("UserCodRef", (long?)null);
var nullableParam2 = new DataBaseParameter("UserCodRef", 423423423432L);  // valor long?

// Parâmetro com direção (para stored procedures)
var outputParam = new DataBaseParameter("Result", null, ParameterDirection.Output);

// Valores enum devem ser convertidos para seu tipo subjacente (char, int, etc.)
var statusParam = new DataBaseParameter("USER_STATUS", (char)StatusUser.Admin);
```

### Nomenclatura de Parâmetros

A biblioteca trata automaticamente os prefixos de parâmetros:
- Use o prefixo `@` em suas consultas SQL (funciona para a maioria dos bancos)
- Para Oracle, a biblioteca automaticamente converte `@` para `:`
- Você também pode usar prefixos `#` ou `:` - eles serão normalizados

```csharp
// Todos estes funcionam da mesma forma:
new DataBaseParameter("@UserId", 123)
new DataBaseParameter("#UserId", 123)
new DataBaseParameter(":UserId", 123)
new DataBaseParameter("UserId", 123)  // Prefixo é opcional
```

## Configuração Avançada

### Opções Personalizadas

```csharp
var options = new AbstractionOptions
{
    Dialect = DatabaseDialect.PostgreSQL,
    UseLockedCommands = false,
    AutoCloseConnection = true,
    SortQueryParameters = false,
    QueryParameterIdentifier = '@',
    DialectParameterIdentifier = '@'
};

IDbQueryBase db = new DbQueryBase(connectionBuilder, options);
```

### Opções Explicadas

- **Dialect**: O mecanismo de banco de dados que você está usando
- **UseLockedCommands**: Usar comandos bloqueados para SQL Server/MySQL (padrão: false ao usar DbConnection)
- **AutoCloseConnection**: Fechar conexões automaticamente após operações (padrão: true para SQL Server/Oracle)
- **SortQueryParameters**: Ordenar parâmetros alfabeticamente (útil para Oracle)
- **QueryParameterIdentifier**: Caractere usado em suas consultas SQL (`@` por padrão)
- **DialectParameterIdentifier**: Caractere usado pelo banco de dados (`@` ou `:` para Oracle)

## Exemplo Completo

Aqui está um exemplo completo baseado em padrões de uso reais:

```csharp
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UzunTec.Utils.DatabaseAbstraction;
using Npgsql;

// Classe modelo
public class User
{
    public int UserCode { get; set; }
    public string UserName { get; set; }
    public long? UserCodRef { get; set; }
    public string PasswordMd5 { get; set; }
    public DateTime InputDate { get; set; }
    public StatusUser Status { get; set; }
}

// Classe repositório
public class UserRepository
{
    private readonly IDbQueryBase db;

    public UserRepository(IDbQueryBase dbQueryBase)
    {
        this.db = dbQueryBase;
    }

    public User GetUser(int userCode)
    {
        string query = @"SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS
                         FROM USER_TEST
                         WHERE COD_USER = @COD_USER";
        
        var parameters = new DataBaseParameter[]
        {
            new DataBaseParameter("COD_USER", userCode)
        };

        DataResultRecord record = db.GetSingleRecord(query, parameters);
        
        if (record == null) return null;

        return BuildUserFromRecord(record);
    }

    public async Task<User> GetUserAsync(int userCode)
    {
        string query = @"SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS
                         FROM USER_TEST
                         WHERE COD_USER = @COD_USER";
        
        var parameters = new DataBaseParameter[]
        {
            new DataBaseParameter("COD_USER", userCode)
        };

        DataResultRecord record = await db.GetSingleRecordAsync(query, parameters).ConfigureAwait(false);
        
        if (record == null) return null;

        return BuildUserFromRecord(record);
    }

    public List<User> GetUsers(int page, int pageSize)
    {
        string query = @"SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS
                         FROM USER_TEST
                         ORDER BY USER_NAME";
        
        DataResultTable result = db.GetPagedResultTable(query, page, pageSize);

        return result.BuildList(BuildUserFromRecord);
    }

    public bool CreateUser(User user)
    {
        string query = @"INSERT INTO USER_TEST (COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS)
                         VALUES(@COD_USER, @USER_NAME, @COD_USER_REF, @PASSWORD_MD5, @INPUT_DATE, @USER_STATUS)";

        var parameters = new DataBaseParameter[]
        {
            new DataBaseParameter("COD_USER", user.UserCode),
            new DataBaseParameter("USER_NAME", user.UserName),
            new DataBaseParameter("COD_USER_REF", user.UserCodRef),
            new DataBaseParameter("PASSWORD_MD5", user.PasswordMd5),
            new DataBaseParameter("INPUT_DATE", user.InputDate),
            new DataBaseParameter("USER_STATUS", (char)user.Status)
        };

        int rowsAffected = db.ExecuteNonQuery(query, parameters);
        return rowsAffected == 1;
    }

    private User BuildUserFromRecord(DataResultRecord record)
    {
        return new User
        {
            UserCode = record.GetValue<int>("cod_user"),
            UserName = record.GetString("user_name"),
            UserCodRef = record.GetNullableValue<long>("cod_user_ref"),
            PasswordMd5 = record.GetString("password_md5"),
            InputDate = record.GetValue<DateTime>("input_date"),
            Status = record.GetEnum<StatusUser>("user_status")
        };
    }
}

// Uso
var connectionBuilder = new ConnectionBuilder(
    NpgsqlFactory.Instance,
    "Server=localhost;Database=mydb;User Id=user;Password=pass;"
);

var db = new DbQueryBase(connectionBuilder, DatabaseDialect.PostgreSQL);
var repository = new UserRepository(db);

User user = repository.GetUser(123);
List<User> users = repository.GetUsers(1, 10);
```

## Requisitos

- .NET Standard 2.0 ou superior
- Pacote NuGet do provedor de banco de dados apropriado:
  - PostgreSQL: `Npgsql`
  - MySQL: `MySql.Data`
  - SQL Server: `System.Data.SqlClient` ou `Microsoft.Data.SqlClient`
  - SQLite: `System.Data.SQLite`
  - Oracle: `Oracle.ManagedDataAccess`

## Licença

Copyright © 2019 Uzun Technology

## Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para enviar um Pull Request.

## Repositório

GitHub: https://github.com/uztec/database-abstraction
