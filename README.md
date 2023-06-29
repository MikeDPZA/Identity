# Identity Microservice
This is a Microservice intended to handle Authentication via [AWS Cognito](https://aws.amazon.com/cognito/) or [Azure AD B2C](https://learn.microsoft.com/en-us/azure/active-directory-b2c/)

## Intent
The intent of this service is to handle user authentication, SSO and to a degree user management. To use this service it has to be hosted via some container orchestrator like Kubernetes. This service however is still in development

## Roadmap
### Cognito
[x] Cognito Sign on \
[x] Cognito Client Credentials \
[  ] User Management

### ADB2C
[  ] Azure Sign on \
[  ] Azure Client Credentials \
[  ] User Management

### Nuget Package
[  ] ServiceCollection Registration
