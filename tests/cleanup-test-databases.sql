-- Script to cleanup old test databases
-- Run this manually if you have leftover test databases

-- Terminate all connections to test databases
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname LIKE 'test_novels_%'
  AND pid <> pg_backend_pid();

-- Drop all test databases
DO $$
DECLARE
    db_name TEXT;
BEGIN
    FOR db_name IN 
        SELECT datname 
        FROM pg_database 
        WHERE datname LIKE 'test_novels_%'
    LOOP
        EXECUTE 'DROP DATABASE IF EXISTS "' || db_name || '"';
        RAISE NOTICE 'Dropped database: %', db_name;
    END LOOP;
END $$;

-- Verify cleanup
SELECT datname 
FROM pg_database 
WHERE datname LIKE 'test_novels_%';
