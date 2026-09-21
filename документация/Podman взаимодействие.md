
Порядок действий:

1. Установить **Podman** на Windows и проверить:

   ```powershell
   podman --version
   podman machine init
   podman machine start
   podman info
   ```

2. Скачать образ PostgreSQL:

   ```powershell
   podman pull postgres:17
   ```

3. Создать контейнер PostgreSQL примерно так:

   ```powershell
   podman run -d `
     --name idm-postgres `
     -e POSTGRES_USER=idm `
     -e POSTGRES_PASSWORD=idm_password `
     -e POSTGRES_DB=idm `
     -p 5432:5432 `
     -v idm-postgres-data:/var/lib/postgresql/data `
     postgres:17
   ```

4. Проверить:

   ```powershell
   podman ps
   ```

   Затем:

   ```powershell
   podman logs idm-postgres
   ```

5. Подключиться к БД:

   ```text
   Host: localhost
   Port: 5432
   Database: idm
   Username: idm
   Password: idm_password
   ```

6. В эту БД уже выполнить нашу схему IDM:

   ```text
   systems
      ↓
   roles
      ↓
   ...
   ```

7. Обязательно проверить **volume**: удалить контейнер, создать его снова с тем же `idm-postgres-data` и убедиться, что таблицы/данные не исчезли. Это как раз хороший практический кусок по Podman.


Для **IDM-002** тебе сейчас пригодится вот такая шпаргалка по Podman.

| Команда                             | Что делает                                     |
| ----------------------------------- | ---------------------------------------------- |
| `podman machine start`              | Запустить виртуальную машину Podman на Windows |
| `podman machine stop`               | Остановить VM Podman                           |
| `podman ps`                         | Показать запущенные контейнеры                 |
| `podman ps -a`                      | Показать все контейнеры, включая остановленные |
| `podman start idm-postgres`         | Запустить существующий PostgreSQL-контейнер    |
| `podman stop idm-postgres`          | Остановить контейнер                           |
| `podman restart idm-postgres`       | Перезапустить контейнер                        |
| `podman logs idm-postgres`          | Посмотреть логи                                |
| `podman logs -f idm-postgres`       | Смотреть логи в реальном времени               |
| `podman exec -it idm-postgres bash` | Зайти внутрь контейнера                        |
| `podman rm idm-postgres`            | Удалить остановленный контейнер                |
| `podman rm -f idm-postgres`         | Принудительно остановить и удалить             |
| `podman images`                     | Посмотреть скачанные образы                    |
| `podman pull postgres:17`           | Скачать образ PostgreSQL                       |
| `podman volume ls`                  | Посмотреть volumes                             |
| `podman inspect idm-postgres`       | Вся информация о контейнере                    |

### Что ты будешь использовать каждый день

Запустил компьютер:

```powershell
podman machine start
podman start idm-postgres
```

Проверил:

```powershell
podman ps
```

Перезапустить PostgreSQL:

```powershell
podman restart idm-postgres
```

Если что-то сломалось:

```powershell
podman logs idm-postgres
```

или:

```powershell
podman logs -f idm-postgres
```

Закончил работу:

```powershell
podman stop idm-postgres
podman machine stop
```

И важный момент: **`stop` ≠ `rm`**.

```text
stop
 ↓
контейнер существует
данные существуют
можно сделать start

rm
 ↓
контейнер удалён
```

При этом если PostgreSQL хранит данные в **volume**, удаление самого контейнера не обязано удалять данные:

```text
Image: postgres:17
        ↓
Container: idm-postgres  ← можно удалить
        ↓
Volume: idm-postgres-data ← данные остаются
```



### Миграция

```bash
Get-Content .\database\migrations\001_initial_schema.sql -Raw |
    podman exec -i idm-postgres psql -U idm -d idm -v ON_ERROR_STOP=1
```
