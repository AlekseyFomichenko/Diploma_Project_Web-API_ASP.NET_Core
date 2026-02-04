-- Создание БД и пользователя для проекта (выполнить в pgAdmin 4, подключившись к серверу как postgres).
-- Выполнять по шагам: сначала блок 1 на любом подключении, затем блок 2 (при необходимости — вручную переключиться на diplomaproject).

-- 1) Создать пользователя postgres1 (если ещё нет)
DO
$$
BEGIN
  IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'postgres1') THEN
    CREATE ROLE postgres1 WITH LOGIN PASSWORD 'example';
  END IF;
END
$$;

-- 2) Создать базу diplomaproject (запустить один раз; при повторном запуске будет ошибка "already exists" — это нормально)
CREATE DATABASE diplomaproject OWNER postgres1;

-- 3) Выдать права на схему public (выполнить после переключения в Query Tool на базу diplomaproject)
GRANT ALL ON SCHEMA public TO postgres1;
GRANT CREATE ON SCHEMA public TO postgres1;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO postgres1;
