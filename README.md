# UzunTec Database Abstraction

A powerful .NET Standard 2.0 library that provides a unified interface for executing SQL queries across multiple database engines. Write your database code once and run it on **Oracle**, **PostgreSQL**, **MySQL**, **SQLite**, and **SQL Server** without changing your code.

## Features

- 🎯 **Multi-Database Support**: Works seamlessly with Oracle, PostgreSQL, MySQL, SQLite, and SQL Server
- 🔄 **Async/Await Support**: Full async/await pattern support for all operations
- 📄 **Automatic Pagination**: Built-in pagination with database-specific optimizations
- 🔒 **Transaction Support**: Easy transaction management
- 🛡️ **Parameterized Queries**: Safe parameter handling with automatic SQL injection prevention
- 📊 **Type-Safe Results**: Strongly-typed result handling with `DataResultTable` and `DataResultRecord`
- 🔧 **Flexible Configuration**: Customizable options for different database dialects

## Installation

This package is published on [NuGet.org](https://www.nuget.org/packages/UzunTec.DatabaseAbstraction/) as `UzunTec.DatabaseAbstraction`.

Install the package via NuGet:

```bash
dotnet add package UzunTec.DatabaseAbstraction
```

Or via Package Manager:

```powershell
Install-Package UzunTec.DatabaseAbstraction
```

Or via PackageReference in your `.csproj`:

```xml
<PackageReference Include="UzunTec.DatabaseAbstraction" Version="0.5.0" />
```

## Quick Start

### Basic Setup

```csharp
using System.Data.Common;
using UzunTec.Utils.DatabaseAbstraction;
using Npgsql; // For PostgreSQL
// or using MySql.Data.MySqlClient; // For MySQL
// or using System.Data.SqlClient; // For SQL Server

// Create connection builder
string connectionString = "Server=localhost;Database=mydb;User Id=user;Password=pass;";
ConnectionBuilder connectionBuilder = new ConnectionBuilder(
    NpgsqlFactory.Instance, // Use appropriate factory for your database
    connectionString
);

// Create DbQueryBase instance
IDbQueryBase db = new DbQueryBase(connectionBuilder, DatabaseDialect.PostgreSQL);
```

### Using with Existing DbConnection

```csharp
using (DbConnection connection = /* your connection */)
{
    IDbQueryBase db = new DbQueryBase(connection, DatabaseDialect.PostgreSQL);
    // Use db here
}
```

## Supported Database Engines

| Database | Dialect Enum | Provider Factory |
|----------|--------------|------------------|
| **PostgreSQL** | `DatabaseDialect.PostgreSQL` | `NpgsqlFactory.Instance` |
| **MySQL** | `DatabaseDialect.MySql` | `MySqlClientFactory.Instance` |
| **SQL Server** | `DatabaseDialect.SqlServer` | `SqlClientFactory.Instance` |
| **SQLite** | `DatabaseDialect.SQLite` | `SQLiteFactory.Instance` |
| **Oracle** | `DatabaseDialect.Oracle` | `OracleClientFactory.Instance` |

## Core Operations

### Querying Data

#### Get Multiple Records

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

#### Get Single Record

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

#### Execute Scalar

```csharp
string query = "SELECT COUNT(*) FROM Users WHERE Status = @Status";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("Status", "Active")
};

object count = db.ExecuteScalar(query, parameters);
int userCount = Convert.ToInt32(count);
```

### Modifying Data

#### Insert

```csharp
string query = @"INSERT INTO Users (UserName, Email, CreatedDate) 
                 VALUES (@UserName, @Email, @CreatedDate)";

var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserName", "John Doe"),
    new DataBaseParameter("Email", "john@example.com"),
    new DataBaseParameter("CreatedDate", DateTime.Now)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

#### Update

```csharp
string query = @"UPDATE Users 
                 SET Email = @Email 
                 WHERE UserId = @UserId";

var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("Email", "newemail@example.com"),
    new DataBaseParameter("UserId", 123)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

#### Delete

```csharp
string query = "DELETE FROM Users WHERE UserId = @UserId";
var parameters = new DataBaseParameter[]
{
    new DataBaseParameter("UserId", 123)
};

int rowsAffected = db.ExecuteNonQuery(query, parameters);
```

## Async Operations

All operations have async counterparts. It's recommended to use `ConfigureAwait(false)` for better performance in library code:

```csharp
// Async query
DataResultTable result = await db.GetResultTableAsync(query, parameters).ConfigureAwait(false);

// Async single record
DataResultRecord record = await db.GetSingleRecordAsync(query, parameters).ConfigureAwait(false);

// Async execute
int rowsAffected = await db.ExecuteNonQueryAsync(query, parameters).ConfigureAwait(false);

// Async scalar
object count = await db.ExecuteScalarAsync(query, parameters).ConfigureAwait(false);
```

## Pagination

### Limit Results

```csharp
// Get top 10 records
DataResultTable result = db.GetResultTable("SELECT * FROM Users ORDER BY CreatedDate DESC", 10);
```

### Offset and Count

```csharp
// Get 20 records starting from offset 40
DataResultTable result = db.GetLimitedRecords(
    "SELECT * FROM Users ORDER BY UserId", 
    40,  // offset
    20   // count
);
```

### Page-Based Pagination

```csharp
// Get page 3 with 10 items per page
DataResultTable result = db.GetPagedResultTable(
    "SELECT * FROM Users ORDER BY UserId",
    3,   // page number (1-based)
    10   // page size
);
```

The library automatically handles pagination syntax differences between databases:
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

// Async version
DataResultTable result = await db.GetResultTableFromProcedureAsync(procedureName, parameters);
```

## Transactions

```csharp
try
{
    db.BeginTransaction();
    
    // Perform multiple operations
    db.ExecuteNonQuery(insertQuery, insertParams);
    db.ExecuteNonQuery(updateQuery, updateParams);
    
    db.CommitTransaction();
}
catch (Exception ex)
{
    db.RollbackTransaction();
    // Handle error
}
```

## Working with Results

### DataResultTable

`DataResultTable` is a collection of `DataResultRecord` objects:

```csharp
DataResultTable table = db.GetResultTable(query);

// Access fields metadata
foreach (var field in table.Fields)
{
    Console.WriteLine($"{field.Key}: {field.Value}");
}

// Convert to strongly-typed list using BuildList
// You can pass a method reference or lambda
List<User> users = table.BuildList(record => new User
{
    UserCode = record.GetValue<int>("cod_user"),
    UserName = record.GetString("user_name"),
    UserCodRef = record.GetNullableValue<long>("cod_user_ref")
});

// Or use a method reference
List<User> users2 = table.BuildList(BuildUserFromRecord);

// Get single record (if table has exactly one row, returns null otherwise)
DataResultRecord single = table.SingleRecord();
```

### DataResultRecord

`DataResultRecord` provides type-safe access to column values. Column names are case-sensitive and should match your database column names exactly:

```csharp
DataResultRecord record = db.GetSingleRecord(query, parameters);

// Get string (handles NULL automatically)
string name = record.GetString("user_name");  // Note: column names match DB case

// Get value types
int id = record.GetValue<int>("cod_user");
DateTime date = record.GetValue<DateTime>("input_date");

// Get nullable value types
long? nullableId = record.GetNullableValue<long>("cod_user_ref");

// Get enum (from char/int values stored in database)
StatusUser status = record.GetEnum<StatusUser>("user_status");
StatusUser? nullableStatus = record.GetNullableEnum<StatusUser>("optional_status");
```

## Parameters

### Creating Parameters

```csharp
// Simple parameter (prefix @ is optional when creating the parameter)
var param = new DataBaseParameter("UserId", 123);
// or
var param2 = new DataBaseParameter("@UserId", 123);  // Both work the same

// Parameter with nullable value (automatically converted to DBNull)
var nullableParam = new DataBaseParameter("UserCodRef", (long?)null);
var nullableParam2 = new DataBaseParameter("UserCodRef", 423423423432L);  // long? value

// Parameter with direction (for stored procedures)
var outputParam = new DataBaseParameter("Result", null, ParameterDirection.Output);

// Enum values should be converted to their underlying type (char, int, etc.)
var statusParam = new DataBaseParameter("USER_STATUS", (char)StatusUser.Admin);
```

### Parameter Naming

The library automatically handles parameter prefixes:
- Use `@` prefix in your SQL queries (works for most databases)
- For Oracle, the library automatically converts `@` to `:`
- You can also use `#` or `:` prefixes - they will be normalized

```csharp
// All these work the same way:
new DataBaseParameter("@UserId", 123)
new DataBaseParameter("#UserId", 123)
new DataBaseParameter(":UserId", 123)
new DataBaseParameter("UserId", 123)  // Prefix is optional
```

## Advanced Configuration

### Custom Options

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

### Options Explained

- **Dialect**: The database engine you're using
- **UseLockedCommands**: Use locked commands for SQL Server/MySQL (default: false when using DbConnection)
- **AutoCloseConnection**: Automatically close connections after operations (default: true for SQL Server/Oracle)
- **SortQueryParameters**: Sort parameters alphabetically (useful for Oracle)
- **QueryParameterIdentifier**: Character used in your SQL queries (`@` by default)
- **DialectParameterIdentifier**: Character used by the database (`@` or `:` for Oracle)

## Complete Example

Here's a complete example based on real usage patterns:

```csharp
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UzunTec.Utils.DatabaseAbstraction;
using Npgsql;

// Model class
public class User
{
    public int UserCode { get; set; }
    public string UserName { get; set; }
    public long? UserCodRef { get; set; }
    public string PasswordMd5 { get; set; }
    public DateTime InputDate { get; set; }
    public StatusUser Status { get; set; }
}

// Repository class
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

// Usage
var connectionBuilder = new ConnectionBuilder(
    NpgsqlFactory.Instance,
    "Server=localhost;Database=mydb;User Id=user;Password=pass;"
);

var db = new DbQueryBase(connectionBuilder, DatabaseDialect.PostgreSQL);
var repository = new UserRepository(db);

User user = repository.GetUser(123);
List<User> users = repository.GetUsers(1, 10);
```

## Requirements

- .NET Standard 2.0 or higher
- Appropriate database provider NuGet package:
  - PostgreSQL: `Npgsql`
  - MySQL: `MySql.Data`
  - SQL Server: `System.Data.SqlClient` or `Microsoft.Data.SqlClient`
  - SQLite: `System.Data.SQLite`
  - Oracle: `Oracle.ManagedDataAccess`

## License

Copyright © 2019 Uzun Technology

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Repository

GitHub: https://github.com/uztec/database-abstraction
