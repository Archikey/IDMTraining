## Dapper установка

Для добавления Dapper:

```powershell
dotnet add .\src\Idm.Application\Idm.Application.csproj package Dapper
```

Для file-based `MigrateDatabase.cs` в начале:

```csharp
#:package Npgsql@10.0.3
#:package Dapper
#:project ../src/Idm.Application/Idm.Application.csproj
```

Запуск:

```powershell
dotnet run .\scripts\MigrateDatabase.cs
```

Проверить установленные пакеты проекта:

```powershell
dotnet list .\src\Idm.Application\Idm.Application.csproj package
```

И после установки на всякий:

```powershell
dotnet restore
dotnet build
```

## **API Dapper**

Основные методы Dapper, которые нам понадобятся:

| Метод                            | Для чего                           | Типичный результат  |
| -------------------------------- | ---------------------------------- | ------------------- |
| `ExecuteAsync()`                 | `INSERT`, `UPDATE`, `DELETE`, DDL  | `int` affected rows |
| `QueryAsync<T>()`                | получить 0..N строк                | `IEnumerable<T>`    |
| `QueryFirstAsync<T>()`           | взять первую строку, ошибка если 0 | `T`                 |
| `QueryFirstOrDefaultAsync<T>()`  | первая строка или default          | `T?`                |
| `QuerySingleAsync<T>()`          | должна быть **ровно 1** строка     | `T`                 |
| `QuerySingleOrDefaultAsync<T>()` | должна быть 0..1 строка            | `T?`                |
| `ExecuteScalarAsync<T>()`        | одно значение                      | `T`                 |

Например, вместо нашего Npgsql:

```csharp
await using var command = connection.CreateCommand();
command.CommandText = "...";

var parameter = command.CreateParameter();
// ...
command.Parameters.Add(parameter);

var result = await command.ExecuteNonQueryAsync();
```

Dapper позволяет:

```csharp
var result = await connection.ExecuteAsync(
    sql,
    new { systemId = 1 });
```

Параметры он сопоставляет по имени:

```sql
WHERE system_id = @systemId
```

```csharp
new { systemId = 1 }
```

Для чтения списка:

```csharp
var accounts = await connection.QueryAsync<Account>(sql);
```

Dapper пытается сопоставить **имена колонок результата** со свойствами объекта. Поэтому SQL aliases нам будут полезны:

```sql
SELECT
    id,
    login_account AS Login,
    display_name AS DisplayName
FROM accounts;
```

И ещё две вещи, которые скоро понадобятся:

```csharp
using Dapper;
```

и транзакцию можно передавать параметром:

```csharp
await connection.ExecuteAsync(
    sql,
    parameters,
    transaction: transaction);
```

### Для нашего мигратора пока нужны всего 3

Не учим весь Dapper сразу:

```csharp
ExecuteAsync()       // выполнить CREATE TABLE / SQL миграции
QueryAsync<T>()      // получить применённые миграции
ExecuteScalarAsync<T>() // пригодится для отдельных значений
```

А транзакции будем делать через Npgsql/`DbTransaction`, после чего передавать transaction в Dapper.

И важная модель в голове:

```text
Npgsql
    ↓
соединение с PostgreSQL
типы PostgreSQL
транзакции
низкоуровневые возможности

Dapper
    ↓
Execute...
Query...
параметры
mapping SQL → C# object
```

Dapper **не заменяет Npgsql**. Он сидит поверх него и избавляет нас от большей части `Command → Parameter → Reader → GetString(0) → GetInt32(1)`.
