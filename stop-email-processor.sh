#!/bin/bash
set -e

echo "🧹 Removing email-processor cron job..."

# Create a backup of the current crontab
crontab -l > /tmp/current_cron || true

# Remove any line that mentions 'email-processor'
sed -i '/email-processor/d' /tmp/current_cron

# Install the cleaned crontab
crontab /tmp/current_cron

# Clean up
rm /tmp/current_cron

echo "✅ Cron job removed."
