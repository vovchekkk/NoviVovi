-- Migration: Add hide_character_id column to Steps table
-- Run this in pgAdmin or psql connected to 'Novels' database

\c Novels

-- Add new column
ALTER TABLE "Steps" ADD COLUMN "hide_character_id" UUID;

-- Add foreign key
ALTER TABLE "Steps"
ADD FOREIGN KEY ("hide_character_id") REFERENCES "Characters"("id") ON DELETE SET NULL;

-- Verify
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'Steps' 
ORDER BY ordinal_position;
