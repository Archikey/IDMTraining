
        CREATE TABLE systems (
            id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            name_system VARCHAR(255) NOT NULL,
            description TEXT,
            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            is_active BOOLEAN NOT NULL DEFAULT TRUE,
            system_type VARCHAR(20) NOT NULL
    CHECK (system_type IN ('Postgres', 'Service', 'Linux', 'CRM', 
    'Other'))
        );
        CREATE TABLE roles (
            id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            name_role VARCHAR(255) NOT NULL,
            description TEXT,
            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            system_id INTEGER NOT NULL,
            FOREIGN KEY (system_id) REFERENCES systems(id) ON DELETE RESTRICT,
            CONSTRAINT uq_roles_system_name
    UNIQUE (system_id, name_role)
        );

        CREATE TABLE accounts (
            id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
            login_account VARCHAR(255) NOT NULL,
            account_type VARCHAR(20) NOT NULL
    CHECK (account_type IN ('Regular', 'Service', 'Privileged')),
            display_name VARCHAR(255) NOT NULL,
            created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            is_active BOOLEAN NOT NULL DEFAULT TRUE,
            system_id INTEGER NOT NULL,
            FOREIGN KEY (system_id) REFERENCES systems(id) ON DELETE RESTRICT,
            CONSTRAINT uq_accounts_system_login
    UNIQUE (system_id, login_account)

        );

        CREATE TABLE accounts_roles(
            accounts_id INTEGER NOT NULL,
            roles_id INTEGER NOT NULL,
            granted_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            FOREIGN KEY (accounts_id) REFERENCES accounts(id) ON DELETE CASCADE,
            FOREIGN KEY (roles_id) REFERENCES roles(id) ON DELETE CASCADE,
            PRIMARY KEY (accounts_id, roles_id)
        );

 




