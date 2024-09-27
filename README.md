# 👨‍⚕️ Health Hub 👩‍⚕️

<br>

# Table of Contents

- [Overview](#overview)
- [Technologies](#technologies)
- [Folder Organization](#folder-organization)
- [Configurations](#configurations)
  - [Environment Variables](#environment-variables)
  - [Payments](#payments)
  - [Authentication](#authentication)
- [Setup](#setup)
- [Contributing](#contributing)
- [License](#license)

<br>

# Overview

HealthHub connects patients with doctors, enabling online consultations, appointment scheduling, and access to medical records. Features include medication reminders, health monitoring, health tips and much much more.

<br>

# Technologies

This particular repository holds the backend code used to provide services to the client. It is written in `C#` and makes use of `ASP.NET`. The code segregation used for this project is `MVCS` (Model-View-Controller-Service). This will be discussed more on the [Folder Organization](#folder-organization) section. Understanding these things will help you navigate through the project with ease.

The set of technologies we utilized in this project:

1. **Programming Language**: ![C#](https://img.shields.io/badge/C%23-8A2BE2?style=flat&logo=csharp&logoColor=white)
2. **Backend Framework**: ![ASP.NET](https://img.shields.io/badge/ASP.NET-purple?style=flat&logo=dotnet&logoColor=white)
3. **Authentication & Authorization**: ![Auth0](https://img.shields.io/badge/Auth0-7D7D7D?style=flat&logo=auth0&logoColor=white)
4. **Object Relational Mapping**: ![EF Core](https://img.shields.io/badge/EF%20Core-7D3F8C?style=flat&logo=efcore&logoColor=white)
5. **Database Engine**: ![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=&logoColor=white)
6. **Logging**: ![Serilog](https://img.shields.io/badge/Serilog-7d7d7d?style=flat&logo=serilog&logoColor=white) ![Seq](https://img.shields.io/badge/Seq-5C2D91?style=flat&logo=seq&logoColor=white)
7. **Testing Framework**: ![xUnit](https://img.shields.io/badge/xUnit-6e6e6e?style=flat&logo=xunit&logoColor=white)
8. **API Testing**: ![Postman](https://img.shields.io/badge/Postman-00C4CC?style=flat&logo=postman&logoColor=white)
9. **API Documentation**: ![OpenAPI Swagger](https://img.shields.io/badge/OpenAPI%20Swagger-85EA2D?style=flat&logo=swagger&logoColor=black)
10. **Templating**: ![Razor Pages](https://img.shields.io/badge/Razor%20Pages-61DAFB?style=flat&logo=blazor&logoColor=black)

<br>

# Folder Organization

## High Level Overview

```
└── 📁Migrations
└── 📁Requests
└── 📁Source
    └── 📁Attributes
    └── 📁Config
    └── 📁Controllers
    └── 📁Data
    └── 📁Filters
    └── 📁Helpers
        └── 📁Defaults
        └── 📁Extensions
    └── 📁Hubs
    └── 📁Middlewares
    └── 📁Models
        └── 📁Dtos
        └── 📁Entities
        └── 📁Enums
        └── 📁Interfaces
        └── 📁Responses
        └── 📁ViewModels
    └── 📁Services
    └── 📁Validation
    └── 📁Views
└── 📁Tests
Program.cs
```

## A Deep Dive

```
└── 📁Migrations
   > This folder automatically stores the database model migrations when you run the `dotnet ef migrations add` command.

└── 📁Requests
   > Contains REST client HTTP API tests for the completed endpoints.

└── 📁Source
   └── 📁Attributes
      > Contains custom attributes used to validate incoming request payloads.

   └── 📁Config
      > Stores application configuration settings, like app settings and environment variables.

   └── 📁Controllers
      > Contains API controllers that map client requests to the appropriate services.

   └── 📁Data
      > Contains data-related code, such as the `DbContext`, which manages database connections and operations.

   └── 📁Filters
      > Stores Action Filters that help intercept and modify requests/responses before they reach the controller or after processing.

   └── 📁Helpers
      └── 📁Defaults
         > Contains default values and constants used across the application.

      └── 📁Extensions
         > Stores extension methods for common functionality, such as mapping between DTOs and database models.

   └── 📁Hubs
      > Contains SignalR hubs that handle WebSocket requests. For example, hubs for chat, notifications, etc.

   └── 📁Middlewares
      > Contains middlewares that intercept the request/response life cycle to modify or handle data as needed.

   └── 📁Models
      └── 📁Dtos
         > Stores Data Transfer Objects (DTOs) that represent only the necessary fields for request/response payloads.

      └── 📁Entities
         > Contains database models, such as User, Doctor, and Patient.

      └── 📁Enums
         > Stores Enums used throughout the application.

      └── 📁Interfaces
         > Contains interface definitions for core services like Payment and Chat.

      └── 📁Responses
         > Stores response models, typically used as return types from controllers to ensure consistent response formats.

      └── 📁ViewModels
         > Contains view models that bind data to views. This may also be used for email templates or payment pages, as this is a backend-focused project.

   └── 📁Services
      > This is the core of the project. It contains service classes that handle business logic, interact with the database, and enforce rules.

   └── 📁Validation
      > Stores Fluent Validation classes for validating request payloads with fine-grained control.

   └── 📁Views
      > Stores view templates, if needed, though this is more backend-focused, so views may be limited to things like emails or invoices.
```

# Configurations

### Environment Variables

Application related configurations are found in `Source/Config` folder. One such configuration is `AppConfig.cs` - which is responsible for making environment variables easily accessible throughout our codebase.

Dont Forget to add appropriate mapping from your `.env` file to this configuration class.

Example:

- Lets say you have such environment variable. Inside `.env`:

```bash
DB_CONNECTION=SOME_CONNECTION_STRING_GOES_HERE
```

- To use this `DB_CONNECTION` you should add the following to `AppConfig.cs`:

```cs
public class AppConfig(IConfiguration configuration){
  // Add it here
  public string? DatabaseConnection { get; set; } = configuration["DB_CONNECTION"];
}
```

- You can make use of this `AppConfig` from other services through **DI**.

```cs
public class SomeService(AppConfig appconfig){
  public void DoSomething(){
    var dbConn = appConfig.DatabaseConnection; // You have access here!
  }
}
```

# Payments

### Chapa Configurations

### Webhooks

1. Create a [Chapa](https://chapa.co/) account
2. Goto **Profile > Settings > Webhooks**:
3. Enter the following input fields

```bash
Webhook Url = YOUR_WEBHOOK_URL
Secret Hash = YOUR_SECRET_HASH
```

4. Then enable the checkboxes for
   **Receive Webhook** and **Receive Webhook for failed Payments**.

<br>

> **Side Note**: Make sure you have configured the `WEBHOOK_SECRET` in the environment variables and its the same as the one you entered above for the field **Secret Hash**. Make sure your secret hash is at least 32 characters for security reasons. You can use this free tool [here](https://bitwarden.com/password-generator/#password-generator).

### Callbacks

1. Goto your chapa dashboard and on the **Profile > Settings > Account** Settings. Enter the input fields for **Callback Url** and **Success Url**

```bash
Callback Url = API_ORIGIN/api/payments/chapa/callback
Success Url =  API_ORIGIN/api/payments/chapa/success
```

> **Side Note**: `API_ORIGIN` is configured inside the `.env` file. Make sure you set it up in the project!

If you intend to change the endpoints shown above please make sure to edit the `PaymentController.cs` to match your preferred endpoint.

# Authentication

### Auth0 Configuration

1. Head to [Auth0](https://auth0.com/) and Create an application.
2. Click on the newly created app and navigate to **Settings** tab
3. Under the basic information section copy the **Client ID** and **Client Secret** and paste it into the `.env` file accordingly
4. Under Application URIs Section add the urls you want to allow. One such url you **must** add is `API_ORIGIN/api/auth/callback`, **API_ORIGIN** being the host url found in `env` as we've discussed before.
5. Under Cross-Origin Authentication enable **Allow Cross-Origin Authentication**
6. Navigate to **Connections** tab and enable **Username-Password-Authentication** and **google-oauth2**

<br>

# Setup

1. Clone the repo

```bash
git clone https://github.com/yordanos-bogale5/HealthHub-Backend.git
```

2. Install all dependencies

```bash
dotnet restore
```

3. Copy contents from `.env.example` to `.env` and configure accordingly.

```bash
.env.example > .env
```

4. Start the ASP Server (Development)

```bash
dotnet watch run
```

<br>

# Contributing

We welcome contributions to this project! To get started, please follow these guidelines:

### How to Contribute

1. **Fork the repository**: Click the "Fork" button at the top right of this page to create your own copy of the repository.
2. **Clone your fork**: Clone the forked repository to your local machine.
   ```bash
   git clone https://github.com/your-username/your-repository.git
   ```
3. **Create a new branch**: Create a new branch for your feature or bugfix.
   ```bash
   git checkout -b feature/your-feature
   ```
4. **Make your changes**: Implement your feature or fix the bug. Ensure your code adheres to the project's coding standards and style.
5. **Commit your changes**: Commit your changes with a descriptive message.
   ```bash
   git add .
   git commit -m 'Add new feature or fix bug'
   ```
6. **Push your branch**: Push your branch to your forked repository.
   ```bash
   git push origin feature/your-feature
   ```
7. **Create a Pull Request**: Go to the repository on GitHub, switch to your branch, and click the `New Pull Request` button. Provide a detailed description of your changes and submit the pull request.

<br>

# Additional Information

- **Bug Reports**: If you find a bug, please open an issue in the repository with details about the problem.

- **Feature Requests**: If you have ideas for new features, feel free to open an issue or submit a pull request.

<br>

# License

This project is licensed under the MIT License

<br>

# Summary

The MIT License is a permissive free software license originating at the Massachusetts Institute of Technology (MIT). It is a simple and easy-to-understand license that places very few restrictions on reuse, making it a popular choice for open source projects.

By using this project, you agree to include the original copyright notice and permission notice in any copies or substantial portions of the software.
