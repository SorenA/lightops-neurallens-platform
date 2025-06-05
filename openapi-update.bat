@echo off

echo Downloading OpenAPI specifications updated from local APIs...
echo ---
mkdir openapi

rem Organization API
echo Downloading Organization OpenAPI specifications...
curl --output openapi/organization-api.v1.json --url https://localhost:20610/openapi/v1.json

rem Workspace API
echo Downloading Workspace OpenAPI specifications...
curl --output openapi/workspace-api.v1.json --url https://localhost:20611/openapi/v1.json

rem Observability API
echo Downloading Observability OpenAPI specifications...
curl --output openapi/observability-api.v1.json --url https://localhost:20612/openapi/v1.json

rem Evaluation API
echo Downloading Evaluation OpenAPI specifications...
curl --output openapi/evaluation-api.v1.json --url https://localhost:20613/openapi/v1.json

rem Ingest API
echo Downloading Ingest OpenAPI specifications...
curl --output openapi/ingest-api.v1.json --url https://localhost:20614/openapi/v1.json

rem Auth API
echo Downloading Auth OpenAPI specifications...
curl --output openapi/auth-api.v1.json --url https://localhost:20615/openapi/v1.json

echo OpenAPI specifications updated.
echo ---