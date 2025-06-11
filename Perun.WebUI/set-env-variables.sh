#!/bin/sh

# Replace env variable placeholders like $API_URL with values from environment variables
envsubst < /usr/share/nginx/html/index.html > /usr/share/nginx/html/index.tmp && \
mv /usr/share/nginx/html/index.tmp /usr/share/nginx/html/index.html

exec "$@"
