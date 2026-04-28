-- Emergency PostgreSQL Recovery Script
-- Run this to fix corrupted database issues

-- Step 1: Connect to postgres database (not the corrupted one)
\c postgres

-- Step 2: Force terminate all connections to test database
SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'test_novels_shared'
  AND pid <> pg_backend_pid();

-- Step 3: Drop the corrupted database with FORCE
DROP DATABASE IF EXISTS test_novels_shared WITH (FORCE);

-- Step 4: Verify it's gone
SELECT datname FROM pg_database WHERE datname LIKE 'test_novels%';

-- Step 5: Clean up any orphaned databases
DO $$
DECLARE
    db_name TEXT;
BEGIN
    FOR db_name IN 
        SELECT datname 
        FROM pg_database 
        WHERE datname LIKE 'test_novels_%'
    LOOP
        EXECUTE 'DROP DATABASE IF EXISTS "' || db_name || '" WITH (FORCE)';
        RAISE NOTICE 'Dropped database: %', db_name;
    END LOOP;
END $$;

-- Done! Now you can run tests again
