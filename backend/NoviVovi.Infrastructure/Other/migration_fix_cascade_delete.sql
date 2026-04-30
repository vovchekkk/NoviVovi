-- =====================================================
-- MIGRATION: Fix CASCADE DELETE constraints
-- Date: 2026-04-30
-- Description: Changes ON DELETE SET NULL to ON DELETE CASCADE
--              for non-nullable fields in domain model
-- =====================================================

-- IMPORTANT: This migration will fail if there are orphaned records
-- Make sure to clean up any orphaned data before running this migration

BEGIN;

-- 1. Fix Replicas.speaker_id: SET NULL → CASCADE
-- Domain: Replica.Speaker is NOT nullable
ALTER TABLE "Replicas"
DROP CONSTRAINT IF EXISTS "Replicas_speaker_id_fkey";

ALTER TABLE "Replicas"
ADD CONSTRAINT "Replicas_speaker_id_fkey"
FOREIGN KEY ("speaker_id") REFERENCES "Characters"("id") ON DELETE CASCADE;

-- 2. Fix Steps.background_id: SET NULL → CASCADE
-- Domain: ShowBackgroundStep.BackgroundObject is NOT nullable
ALTER TABLE "Steps"
DROP CONSTRAINT IF EXISTS "Steps_background_id_fkey";

ALTER TABLE "Steps"
ADD CONSTRAINT "Steps_background_id_fkey"
FOREIGN KEY ("background_id") REFERENCES "Backgrounds"("id") ON DELETE CASCADE;

-- 3. Fix Steps.character_id: SET NULL → CASCADE
-- Domain: ShowCharacterStep.CharacterObject is NOT nullable
ALTER TABLE "Steps"
DROP CONSTRAINT IF EXISTS "Steps_character_id_fkey";

ALTER TABLE "Steps"
ADD CONSTRAINT "Steps_character_id_fkey"
FOREIGN KEY ("character_id") REFERENCES "StepCharacter"("id") ON DELETE CASCADE;

-- 4. Fix Steps.hide_character_id: SET NULL → CASCADE
-- Domain: HideCharacterStep.Character is NOT nullable
ALTER TABLE "Steps"
DROP CONSTRAINT IF EXISTS "Steps_hide_character_id_fkey";

ALTER TABLE "Steps"
ADD CONSTRAINT "Steps_hide_character_id_fkey"
FOREIGN KEY ("hide_character_id") REFERENCES "Characters"("id") ON DELETE CASCADE;

-- 5. Fix Steps.next_label_id: SET NULL → CASCADE
-- Domain: JumpTransition.TargetLabel is NOT nullable
ALTER TABLE "Steps"
DROP CONSTRAINT IF EXISTS "Steps_next_label_id_fkey";

ALTER TABLE "Steps"
ADD CONSTRAINT "Steps_next_label_id_fkey"
FOREIGN KEY ("next_label_id") REFERENCES "Labels"("id") ON DELETE CASCADE;

-- 6. Fix CharacterStates.transform_id: SET NULL → CASCADE
-- Domain: CharacterState.LocalTransform is NOT nullable
ALTER TABLE "CharacterStates"
DROP CONSTRAINT IF EXISTS "CharacterStates_transform_id_fkey";

ALTER TABLE "CharacterStates"
ADD CONSTRAINT "CharacterStates_transform_id_fkey"
FOREIGN KEY ("transform_id") REFERENCES "Transforms"("id") ON DELETE CASCADE;

-- 7. Fix Backgrounds.transform_id: SET NULL → CASCADE
-- Domain: BackgroundObject.Transform is NOT nullable (via SceneObject)
ALTER TABLE "Backgrounds"
DROP CONSTRAINT IF EXISTS "Backgrounds_transform_id_fkey";

ALTER TABLE "Backgrounds"
ADD CONSTRAINT "Backgrounds_transform_id_fkey"
FOREIGN KEY ("transform_id") REFERENCES "Transforms"("id") ON DELETE CASCADE;

-- 8. Fix StepCharacter.transform_id: SET NULL → CASCADE
-- Domain: CharacterObject.Transform is NOT nullable (via SceneObject)
ALTER TABLE "StepCharacter"
DROP CONSTRAINT IF EXISTS "StepCharacter_transform_id_fkey";

ALTER TABLE "StepCharacter"
ADD CONSTRAINT "StepCharacter_transform_id_fkey"
FOREIGN KEY ("transform_id") REFERENCES "Transforms"("id") ON DELETE CASCADE;

COMMIT;

-- =====================================================
-- VERIFICATION QUERIES
-- Run these to verify the migration was successful
-- =====================================================

-- Check all foreign key constraints on Steps table
-- SELECT conname, conrelid::regclass, confrelid::regclass, confdeltype
-- FROM pg_constraint
-- WHERE conrelid = '"Steps"'::regclass AND contype = 'f';

-- Check all foreign key constraints on Transform references
-- SELECT conname, conrelid::regclass, confrelid::regclass, confdeltype
-- FROM pg_constraint
-- WHERE confrelid = '"Transforms"'::regclass AND contype = 'f';

-- Expected confdeltype values:
-- 'c' = CASCADE
-- 'n' = SET NULL
-- 'r' = RESTRICT
-- 'a' = NO ACTION
