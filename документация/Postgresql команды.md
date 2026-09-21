#БД


| SQL                            | Что делает                                                   | Пример / смысл                                     |
| ------------------------------ | ------------------------------------------------------------ | -------------------------------------------------- |
| `CREATE TABLE`                 | Создаёт таблицу                                              | `CREATE TABLE accounts (...)`                      |
| `INTEGER`                      | Целое число                                                  | Обычно для `id`                                    |
| `GENERATED ALWAYS AS IDENTITY` | PostgreSQL сам генерирует последовательные значения          | Современная замена `SERIAL` для `id`               |
| `PRIMARY KEY`                  | Первичный ключ: уникальный идентификатор строки              | `id ... PRIMARY KEY`                               |
| `VARCHAR(255)`                 | Строка с ограничением длины                                  | `login_account VARCHAR(255)`                       |
| `TEXT`                         | Строка без заданного ограничения длины                       | Хорошо для `description`                           |
| `BOOLEAN`                      | `TRUE` / `FALSE`                                             | `is_active BOOLEAN`                                |
| `TIMESTAMPTZ`                  | Дата и время с учётом часового пояса                         | Для `created_at`, `updated_at`                     |
| `NOT NULL`                     | Запрещает `NULL`                                             | Поле обязательно должно иметь значение             |
| `DEFAULT`                      | Значение по умолчанию                                        | `DEFAULT TRUE`                                     |
| `NOW()`                        | Текущие дата и время                                         | `DEFAULT NOW()`                                    |
| `CHECK`                        | Проверяет допустимость значения                              | `CHECK (account_type IN (...))`                    |
| `IN (...)`                     | Проверяет, входит ли значение в набор                        | `IN ('Regular', 'Service')`                        |
| `FOREIGN KEY`                  | Создаёт внешний ключ — связь с другой таблицей               | `FOREIGN KEY (system_id)`                          |
| `REFERENCES`                   | Указывает таблицу и колонку, на которую ссылается FK         | `REFERENCES systems(id)`                           |
| `UNIQUE`                       | Запрещает дубликаты                                          | `UNIQUE (system_id, login_account)`                |
| `CONSTRAINT name`              | Даёт ограничению собственное имя                             | `CONSTRAINT uq_accounts_system_login UNIQUE (...)` |
| `ON DELETE RESTRICT`           | Запрещает удалить родителя, пока существуют зависимые строки | Нельзя удалить `System`, пока у неё есть Accounts  |
| `ON DELETE CASCADE`            | При удалении родителя автоматически удаляет зависимые строки | Удалили Account → удалились его `accounts_roles`   |

Особенно запомни разницу между тремя ограничениями:

```sql
PRIMARY KEY (id)
```

**Кто эта строка?** Уникально идентифицирует её.

```sql
FOREIGN KEY (system_id) REFERENCES systems(id)
```

**С кем эта строка связана?** `Account` ссылается на существующую `System`.

```sql
UNIQUE (system_id, login_account)
```

**Что нельзя повторять?** В одной системе нельзя иметь два одинаковых логина.

Причём составной `UNIQUE` — важная штука. Например:

```text
system_id | login_account
----------+--------------
1         | postgres       OK
1         | admin          OK
2         | postgres       OK
1         | postgres       ERROR
```

`postgres` может существовать в разных системах, но два `postgres` внутри **одной** системы запрещены.

И ещё одна конструкция, которую стоит прямо запомнить целиком:

```sql
account_type VARCHAR(20) NOT NULL
    CHECK (account_type IN ('Regular', 'Service', 'Privileged'))
```

Она говорит PostgreSQL:

> поле строковое, максимум 20 символов, `NULL` запрещён, а из строк разрешены только три конкретных значения.

Поэтому:

```sql
'Regular'       -- OK
'Service'       -- OK
'Privileged'    -- OK
'SuperAdmin228' -- ERROR
```
