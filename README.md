# Health Hub

## Table of Contents

- [Overview](#overview)
- [Technologies](#technologies)
- [Folder Organization](#folder-organization)
- [Setup](#setup)
- [Notes](#notes)
- [Contributing](#contributing)
- [License](#license)

## Overview

HealthHub connects patients with doctors, enabling online consultations, appointment scheduling, and access to medical records. Features include medication reminders, health monitoring, health tips and much much more.

## Technologies

This particular repository holds the backend code used to provide services to the client. It is written in `C#` and makes use of `ASP.NET Web Api`. The code segregation used for this project is `MVCS` (Model-View-Controller-Service). This will be discussed more on the [Folder Organization](#folder-organization) section. Understanding these things will help you navigate through the project with ease.

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

## Folder Organization

```
📁ROOT
└── 📨Requests
    └── 📁Auth0
    └── 📁User
    └── ...
└── 💻Source
    └── 📃Attributes
    └── 📝Config
    └── 🗺Controllers
    └── 📁Data
    └── 🧮Enums
    └── ➕Extensions
    └── 🚗Models
        └── 📁Dtos
        └── 📁Entities
        └── 📁Responses
        └── 📁ViewModels
    └── ⚙Services
    └── 📟Views
└── 🧪 Tests
└── Program.cs
└── HealthHub.csproj
└── .gitignore
└── .env
└── .env.example
```

### Folder Structure: A Deep Dive

- **Requests**: This folder contains all api requests of the format `.http`. We use these to test our server endpoints. Download [RestClient](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) to get support for such file types.

- **Source**: This folder is where all of the good stuff happens. It contains all the source code that will be compiled to give our final product.
  - **Attributes**: This folder contains C# attribute classes that are responsible for validating the data that is transferred between client and server.
  - **Config**: This folder contains different configuration files. For example: It saves a lot of time to call properties `appConfig.DatabaseConnection` directly without any literal values like `configuration["DB_CONNECTION"]`. You could do it this way but believe me, dont! Maintenance will be a headache.
  - **Controllers**: Contains api-endpoint specific classes that are responsible to map client http-request to services.
  - **Data**: Any data related code goes here. Example: `AppContext.cs` is found in this folder. This class is used to perform database queries
  - **Enums**: These are enumerations we will use mostly when describing our database model.
  - **Extensions**: This folder contains Extension classes that allow custom defined method procedures to be invoked on instances of a class.
  - **Models**: This folder has definitions of different models like
    - `Dtos`: Data transfer objects
    - `Entities`: Models of database tables
    - `Response`: Interfaces for standardizing server-level communication
    - `ViewModel`: Just like Dtos but what makes it different is that these are used for performing model-binding with a view.
- **Tests**: This folder contains xUnit tests.
- **Program.cs**: This is the main entry point of our application which has different service and app configurations setup.
- **.env**: this is where you should store all sensitive data.

## Setup

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

## Notes

### Configuration

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

## Contributing

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

## Additional Information

- **Bug Reports**: If you find a bug, please open an issue in the repository with details about the problem.

- **Feature Requests**: If you have ideas for new features, feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License

### Summary

The MIT License is a permissive free software license originating at the Massachusetts Institute of Technology (MIT). It is a simple and easy-to-understand license that places very few restrictions on reuse, making it a popular choice for open source projects.

By using this project, you agree to include the original copyright notice and permission notice in any copies or substantial portions of the software.
