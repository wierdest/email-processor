#!/bin/bash
set -e

ENV_FILE="$(pwd)/.env"
RUN_SCRIPT="$(pwd)/run-email-processor.sh"
LOG_FILE="$(pwd)/cron.log"

# 🕒 Intervalo de execução (formato cron)
CRON_SCHEDULE="* * * * *"
CRONLINE="$CRON_SCHEDULE $RUN_SCRIPT >> $LOG_FILE 2>&1"

# 🐳 Build image
echo "🐳 Building email processor image..."
podman build -t email-processor -f Dockerfile .

# 🧽 Ensure log file exists
touch "$LOG_FILE"

# 📅 Register cron job if not already registered
echo "📅 Checking for existing cron job..."
CURRENT_CRONTAB="$(crontab -l 2>/dev/null || true)"

if ! echo "$CURRENT_CRONTAB" | grep -Fxq "$CRONLINE"; then
  echo "📅 Adding cron job to crontab..."
  (echo "$CURRENT_CRONTAB"; echo "$CRONLINE") | crontab -
else
  echo "📅 Cron job already exists, skipping..."
fi

echo "✅ Email processor scheduled to run: $CRON_SCHEDULE"
