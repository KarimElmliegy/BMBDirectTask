# Azure deployment

The application is now set up to run as one web application. The API serves `EcommerceFrontEnd` from its `wwwroot` folder, and the frontend calls `/api`. This is the simplest production arrangement because the browser uses one origin and does not need a separate CORS setup.

## Recommended: App Service + Azure SQL

1. Create an Azure SQL Database and its logical SQL server. Allow the App Service outbound access in the SQL server firewall. For initial testing, you can temporarily allow Azure services, then tighten the firewall later.
2. Create an Azure App Service for Linux with a .NET 6 runtime, or deploy the included Dockerfile to a Linux Web App for Containers. .NET 6 is out of support, so plan to retarget the projects to a supported .NET version before production.
3. Deploy the repository with the API project as the web project. For a Docker deployment, build from the repository root because the Dockerfile copies all four project files and the frontend directory.
4. In App Service > Environment variables, add these settings. App Service converts `__` into nested JSON configuration keys:

   `ASPNETCORE_ENVIRONMENT=Production`

   `ConnectionStrings__DefaultConnection=<Azure SQL connection string>`

   `Jwt__Issuer=BMBAssessment.API`

   `Jwt__Audience=BMBAssessment.Client`

   `Jwt__Key=<random secret of at least 32 characters>`

5. Apply the EF Core migrations to Azure SQL before the first request. From the repository root, after installing the EF tool, run:

   `dotnet ef database update --project BMBAssessment.Infrastructure --startup-project BMBAssessment`

   Use the Azure SQL connection string through the `ConnectionStrings__DefaultConnection` environment variable in the shell running that command.
6. Open the App Service URL. The frontend should load at `/` and the API is available under `/api`.

## Azure VM with a public IP

An Azure public IP must be attached to a VM or another Azure resource. On a Linux VM with Docker installed:

1. Clone this repository on the VM.
2. Build from the repository root: `docker build -f BMBAssessment/Dockerfile -t bmbassessment-api .`
3. Run the container with the production settings: `docker run -d --name bmbassessment-api --restart unless-stopped -p 80:8080 --env-file .env bmbassessment-api`
4. Add an inbound NSG rule allowing TCP 80. For production, put HTTPS in front of the VM with a reverse proxy or Azure Application Gateway and allow TCP 443.
5. Use Azure SQL for the database. Do not expose SQL Server's port 1433 publicly and do not use the development `Trusted_Connection` string on Linux.

## Windows Server VM with IIS

For a Windows VM with SQL Server installed locally, IIS is the simplest deployment option:

1. On the VM, install the **ASP.NET Core Hosting Bundle for .NET 6**. The project currently targets .NET 6; upgrade the project to a supported .NET version before production.
2. Install the IIS role and the IIS Management Console. Create a folder such as `C:\Sites\BMBAssessment` and an IIS application pool named `BMBAssessmentPool`. Set the pool to **No Managed Code**.
3. From the repository root on your development computer, create the deployment files:

   `dotnet publish BMBAssessment/BMBAssessment.API.csproj -c Release -o .azure-publish`

4. Copy the contents of `.azure-publish` to `C:\Sites\BMBAssessment` on the VM. The folder must contain `BMBAssessment.API.dll`, `web.config`, and `wwwroot\index.html`.
5. Create an IIS website named `BMBAssessment`, set its physical path to `C:\Sites\BMBAssessment`, assign `BMBAssessmentPool`, and bind HTTP to port 80. Add an HTTPS binding after installing a certificate.
6. In SQL Server, create the database and a dedicated SQL login. Use a local connection string such as:

   `Server=localhost;Database=BMBAssessment;User Id=BMBAssessmentApp;Password=<strong-password>;Encrypt=True;TrustServerCertificate=True;`

   If SQL Server is a named instance, use `Server=localhost\<instance-name>` or configure a fixed TCP port. Do not use the development `Trusted_Connection=True` string for the IIS app pool.
7. On the VM, set machine environment variables from an elevated PowerShell window. Replace the placeholders with real values:

   `[Environment]::SetEnvironmentVariable('ConnectionStrings__DefaultConnection', 'Server=localhost;Database=BMBAssessment;User Id=BMBAssessmentApp;Password=<strong-password>;Encrypt=True;TrustServerCertificate=True;', 'Machine')`

   `[Environment]::SetEnvironmentVariable('Jwt__Issuer', 'BMBAssessment.API', 'Machine')`

   `[Environment]::SetEnvironmentVariable('Jwt__Audience', 'BMBAssessment.Client', 'Machine')`

   `[Environment]::SetEnvironmentVariable('Jwt__Key', '<random-secret-at-least-32-characters>', 'Machine')`

   `[Environment]::SetEnvironmentVariable('Jwt__ExpiryMinutes', '60', 'Machine')`

8. Apply the migrations from the VM while SQL Server is local. Copy the repository to the VM, open PowerShell in its root, and run:

   `dotnet tool restore`

   `$env:ConnectionStrings__DefaultConnection='Server=localhost;Database=BMBAssessment;User Id=BMBAssessmentApp;Password=<strong-password>;Encrypt=True;TrustServerCertificate=True;'`

   `dotnet ef database update --project BMBAssessment.Infrastructure --startup-project BMBAssessment`

9. Recycle the IIS application pool. Browse to `http://68.210.99.233/`. The frontend is served by the API and calls `/api` automatically.
10. For HTTPS, use a DNS name pointing to `68.210.99.233` and install a certificate for that name. IP-address HTTPS certificates are uncommon. Add the HTTPS binding in IIS and configure HTTP to HTTPS redirection.

Only expose inbound ports `80`, `443`, and restricted administrative access such as RDP. SQL Server port `1433` should not be open to the Internet when the API and database are on the same VM.

## Separate frontend hosting

If the frontend is deployed to Static Web Apps or Storage Static Website instead of being served by the API, change `EcommerceFrontEnd/js/api.js` to use the API's HTTPS URL and set `Frontend__Origins__0` to the exact frontend origin. Do not use `*` with authenticated requests.

## Important checks

- Never commit `.env`; it contains database credentials and the JWT signing key.
- Use HTTPS for the public deployment.
- The current project targets .NET 6, which is no longer supported. Retarget before production if the Azure runtime does not provide .NET 6.
