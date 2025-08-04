#!/bin/bash
set -e

LOCKFILE="/tmp/email-processor.lock"
LOCK_MAX_AGE_MINUTES=15

# 🧹 Check for an existing lock file
if [ -e "$LOCKFILE" ]; then
  LOCK_PID=$(cat "$LOCKFILE")

  # If process is still running, skip
  if kill -0 "$LOCK_PID" 2>/dev/null; then
    echo "⏳ Another instance is already running (PID $LOCK_PID). Skipping..."
    exit 1
  fi

  # Check if lock file is stale
  AGE_MINUTES=$(( ( $(date +%s) - $(stat -c %Y "$LOCKFILE") ) / 60 ))
  if [ "$AGE_MINUTES" -lt "$LOCK_MAX_AGE_MINUTES" ]; then
    echo "⚠️ Found stale lock file from non-running process, age: ${AGE_MINUTES} min. Aborting to be safe."
    exit 1
  else
    echo "🧼 Removing stale lock file (age: ${AGE_MINUTES} min)..."
    rm -f "$LOCKFILE"
  fi
fi

# 🧷 Acquire lock
echo $$ > "$LOCKFILE"
trap 'rm -f "$LOCKFILE"' EXIT

echo "⏱️ Running scheduled email processor at $(date)"
podman run --rm --env-file "$(dirname "$0")/.env" email-processor
