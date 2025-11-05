#!/usr/bin/env bash
set -e

# Load variables from .env if it exists
if [ -f "/app/.env" ]; then
  echo "Loading .env from /app/.env"
  export $(grep -v '^#' /app/.env | xargs)
fi

echo "Starting DiffErgoSum API with DB_DRIVER=${DB_DRIVER:-inmemory}"

exec dotnet DiffErgoSum.dll
