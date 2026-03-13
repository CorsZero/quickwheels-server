#!/bin/bash

source /home/ubuntu/.env

docker run --rm \
--network infra_quickwheels-network \
-e PGHOST=vehicle-db \
-e PGUSER=$DB_USER \
-e PGPASSWORD=$DB_PASSWORD \
-e AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID \
-e AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY \
-e AWS_DEFAULT_REGION=$AWS_REGION \
postgres:16-alpine sh -c "
apk add --no-cache aws-cli && \\
pg_dump -U $DB_USER $VEHICLE_DB_NAME | gzip | \
aws s3 cp - s3://quickwheels-db-backups/postgres/vehicle/vehicle-\$(date +%F).sql.gz
"