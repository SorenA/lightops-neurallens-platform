// Generate API clients

import { execSync } from "node:child_process";
import { styleText } from "node:util";

function generateApiClient(apiName, apiSpec) {
  try {
    console.log(styleText('green', `Generating ${apiName} API client...`))

    execSync(
      `npx @openapitools/openapi-generator-cli generate \
        -i ${apiSpec} \
        -g typescript-fetch \
        -o ./packages/api-clients/${apiName}`,
    )

    console.log(styleText('green', 'Generation completed.'))
  } catch (error) {
    if (error instanceof Error) {
      console.error(
        styleText('red', `Error occurred while generating ${apiName} API client from ${apiSpec}:`),
        error.message,
      )
    }
  }
}

generateApiClient('auth', '../../openapi/auth-api.v1.json')
generateApiClient('evaluation', '../../openapi/evaluation-api.v1.json')
generateApiClient('observability', '../../openapi/observability-api.v1.json')
generateApiClient('organization', '../../openapi/organization-api.v1.json')
generateApiClient('workspace', '../../openapi/workspace-api.v1.json')