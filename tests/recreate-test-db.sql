-- Drop and recreate test database with new schema
-- Run this in pgAdmin or psql connected to 'postgres' database

\c postgres

-- Terminate connections
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'test_novels_shared_v2'
  AND pid <> pg_backend_pid();

-- Drop old database
DROP DATABASE IF EXISTS test_novels_shared_v2;

-- Note: The new database will be created automatically by tests with the updated schema
