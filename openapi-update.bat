@echo off

echo Downloading OpenAPI specifications updated from local APIs...
echo ---
mkdir openapi

rem Organization API
echo Downloading Organization OpenAPI specifications...
curl --output openapi/organization-api.v1.json --url https://localhost:20610/openapi/v1.json

rem Project API
echo Downloading Project OpenAPI specifications...
curl --output openapi/project-api.v1.json --url https://localhost:20611/openapi/v1.json

rem Observability API
echo Downloading Observability OpenAPI specifications...
curl --output openapi/observability-api.v1.json --url https://localhost:20612/openapi/v1.json

rem Evaluation API
echo Downloading Evaluation OpenAPI specifications...
curl --output openapi/evaluation-api.v1.json --url https://localhost:20613/openapi/v1.json

echo OpenAPI specifications updated.
echo ---