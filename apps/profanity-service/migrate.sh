#!/usr/bin/env bash
# Usage:  ./migrate.sh <MigrationName>
# Example: ./migrate.sh AddSeverityColumn
#
# What this does:
#   1. Adds an EF Core migration (code → C# diff files, never touches the DB)
#   2. Exports just the SQL delta for that migration
#   3. Strips EF-internal __EFMigrationsHistory table ops (Flyway owns versioning)
#   4. Saves the clean SQL as the next sql/V{n}__<Name>.sql file ready for Flyway
#
# The EF migration files in src/Profanity.Data/Migrations are kept — they are
# needed so the NEXT call to this script can diff from the correct baseline.

set -euo pipefail

MIGRATION_NAME="${1:?Usage: ./migrate.sh <MigrationName>}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DATA_PROJECT="$SCRIPT_DIR/src/Profanity.Data/Profanity.Data.csproj"
STARTUP_PROJECT="$SCRIPT_DIR/src/Profanity.Api/Profanity.Api.csproj"
MIGRATIONS_DIR="$SCRIPT_DIR/src/Profanity.Data/Migrations"
SQL_DIR="$SCRIPT_DIR/sql"

# ── 1. Determine next Flyway version number ───────────────────────────────────
LATEST_V=$(find "$SQL_DIR" -name "V[0-9]*__*.sql" 2>/dev/null \
  | grep -oP '(?<=/V)\d+(?=__)' | sort -n | tail -1)
NEXT_V=$(( ${LATEST_V:-0} + 1 ))

# ── 2. Record the last EF migration name BEFORE adding the new one ────────────
#      Files are named: <timestamp>_<MigrationName>.cs  (exclude Designer + Snapshot)
mkdir -p "$MIGRATIONS_DIR"
PREV_MIGRATION=$(find "$MIGRATIONS_DIR" -maxdepth 1 -name "*.cs" \
  ! -name "*Designer*" ! -name "*Snapshot*" 2>/dev/null \
  | sort | tail -1 \
  | sed 's|.*/[0-9]*_||; s|\.cs$||') || true

# ── 3. Add the new EF Core migration ─────────────────────────────────────────
echo "→ Adding EF migration '$MIGRATION_NAME' ..."
dotnet ef migrations add "$MIGRATION_NAME" \
  --project       "$DATA_PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --output-dir    Migrations

# ── 4. Export SQL delta (previous → new) ─────────────────────────────────────
TMP_SQL=$(mktemp --suffix=.sql)
echo "→ Generating SQL ..."

if [[ -n "$PREV_MIGRATION" ]]; then
  dotnet ef migrations script "$PREV_MIGRATION" "$MIGRATION_NAME" \
    --project         "$DATA_PROJECT" \
    --startup-project "$STARTUP_PROJECT" \
    -o "$TMP_SQL"
else
  # First migration — script from an empty database
  dotnet ef migrations script \
    --project         "$DATA_PROJECT" \
    --startup-project "$STARTUP_PROJECT" \
    -o "$TMP_SQL"
fi

# ── 5. Strip EF-internal history ops → write Flyway file ─────────────────────
# Removes:
#   - The CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (...); block
#   - INSERT INTO / VALUES lines that reference __EFMigrationsHistory
FLYWAY_FILE="$SQL_DIR/V${NEXT_V}__${MIGRATION_NAME}.sql"

awk '
  # start skipping when we see the EF history table creation
  /CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"/ { skip=1 }
  # stop skipping after the closing );
  skip && /^\);/ { skip=0; next }
  skip { next }
  # drop INSERT INTO and VALUES rows referencing the history table
  /INSERT INTO.*__EFMigrationsHistory/ { next }
  /VALUES.*_MigrationName|[0-9]{14}_/ { next }
  { print }
' "$TMP_SQL" > "$FLYWAY_FILE"
rm "$TMP_SQL"

# Remove leading blank lines
sed -i '/./,$!d' "$FLYWAY_FILE"

echo ""
echo "✓ Created: sql/$(basename "$FLYWAY_FILE")"
echo ""
echo "  Review the file, then apply it:"
echo "  docker compose up profanity-flyway"
