# FaqAssistant - AI-Powered FAQ Management System

## Overview

FaqAssistant is a comprehensive FAQ (Frequently Asked Questions) management system built with .NET 10 and PostgreSQL. The application features AI-powered FAQ answering capabilities, user authentication with JWT, rate limiting, and a clean architecture pattern following CQRS with MediatR.

## Features

- ?? **AI-Powered Answers**: Integration with Groq AI for intelligent FAQ responses
- ?? **JWT Authentication**: Secure user authentication and authorization
- ?? **Rate Limiting**: Token bucket rate limiting for AI endpoints
- ??? **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- ?? **Comprehensive Unit Tests**: xUnit test suite with Moq
- ?? **PostgreSQL Database**: Entity Framework Core with migrations
- ?? **Docker Support**: Full containerization with Docker Compose
- ?? **Swagger/OpenAPI**: Interactive API documentation
- ? **CQRS Pattern**: Command Query Responsibility Segregation with MediatR
- ? **Validation**: FluentValidation for request validation

## Technology Stack

- **.NET 10** - Latest .NET framework
- **ASP.NET Core 10** - Web API framework
- **Entity Framework Core 10** - ORM for database operations
- **PostgreSQL 16** - Relational database
- **MediatR** - CQRS and mediator pattern implementation
- **FluentValidation** - Model validation
- **BCrypt.Net** - Password hashing
- **JWT** - JSON Web Tokens for authentication
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework for tests
- **Docker** - Containerization
- **Swagger/OpenAPI** - API documentation

## Project Structure

```
FaqAssistant/
??? FaqAssistant.Api/                    # Web API layer
?   ??? Controllers/                     # API controllers
?   ?   ??? AuthorizationController.cs
?   ?   ??? CategoryController.cs
?   ?   ??? FaqController.cs
?   ?   ??? TagController.cs
?   ?   ??? UserController.cs
?   ??? Program.cs                       # Application entry point
?   ??? appsettings.json                 # Configuration
?   ??? Dockerfile                       # Docker configuration
?
??? FaqAssistant.Application/            # Application layer (CQRS)
?   ??? Commands/                        # Command handlers
?   ??? Queries/                         # Query handlers
?   ??? FaqAssistant.Application.csproj
?
??? FaqAssistant.Domain/                 # Domain layer
?   ??? Entities/                        # Domain entities
?       ??? Category.cs
?       ??? EntityBase.cs
?       ??? Faq.cs
?       ??? FaqTag.cs
?       ??? Rating.cs
?       ??? Tag.cs
?       ??? User.cs
?
??? FaqAssistant.Infrastructure/         # Infrastructure layer
?   ??? Data/                           # Database context
?   ?   ??? AppDbContext.cs
?   ??? Migrations/                     # EF Core migrations
?   ??? Repositories/                   # Data access
?   ??? Services/                       # Infrastructure services
?
??? FaqAssistant.UnitTests/             # Unit tests
?   ??? CommandHandlers/                # Command handler tests
?   ??? QueryHandlers/                  # Query handler tests
?   ??? Services/                       # Service tests
?
??? docker-compose.yml                  # Docker Compose configuration
```

## Database Schema

The system includes the following entities:

- **Users**: User accounts with authentication
- **Categories**: FAQ categories for organization
- **Tags**: Tags for FAQ classification
- **Faqs**: FAQ questions and answers
- **FaqTag**: Many-to-many relationship between FAQs and Tags
- **Ratings**: User ratings (upvotes/downvotes) for FAQs

## Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Docker deployment)
- [PostgreSQL 16](https://www.postgresql.org/download/) (for local development without Docker)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (recommended)

## Getting Started

### Option 1: Running with Docker (Recommended)

This is the easiest way to get the application running with all dependencies.

#### 1. Download and Extract the Project

Download the project files and extract them to your desired location, then navigate to the project directory:

```bash
cd FaqAssistant
```

#### 2. Configure Environment

Update the `docker-compose.yml` file if you want to change default credentials or ports.

#### 3. Build and Run with Docker Compose

```bash
docker-compose up --build
```

This will start three services:
- **PostgreSQL Database** - Port 5432
- **pgAdmin** - Port 8888 (Database Management UI)
- **FaqAssistant API** - Port 8080

#### 4. Access the Application

**Note**: When running with Docker, the application uses different ports than local development.

- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **pgAdmin**: http://localhost:8888
  - Email: `admin@faq.com`
  - Password: `admin123`

> **Port Information**: Docker uses port 8080 (HTTP) and 8081 (HTTPS). When running locally without Docker, the application uses port 7052 (HTTPS) and 5088 (HTTP).

#### 5. Apply Database Migrations (First Time Only)

After the containers are running, apply the database migrations:

```bash
docker exec -it faqassistant-api dotnet ef database update
```

Or connect to the API container and run:

```bash
docker exec -it faqassistant-api bash
dotnet ef database update
```

### Option 2: Running Locally (Development)

#### 1. Download and Extract the Project

Download the project files and extract them to your desired location, then navigate to the project directory:

```bash
cd FaqAssistant
```

#### 2. Set Up PostgreSQL Database

Install PostgreSQL 16 and create a database:

```sql
CREATE DATABASE faq_db;
CREATE USER faq_user WITH PASSWORD 'faq_password';
GRANT ALL PRIVILEGES ON DATABASE faq_db TO faq_user;
```

#### 3. Configure Connection String

Update `appsettings.json` in the `FaqAssistant.Api` project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=faq_db;Username=faq_user;Password=faq_password"
  }
}
```

#### 4. Configure JWT Settings

Update JWT settings in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyMinimum32CharactersLong!!",
    "Issuer": "FaqAssistant",
    "Audience": "FaqAssistantUsers",
    "ExpirationMinutes": 60
  }
}
```

**Important**: Change the `SecretKey` to a secure random string in production!

#### 5. Configure AI Settings (Optional)

If you want to use AI-powered FAQ answering, configure the AI settings in `appsettings.json`:

```json
{
  "AISettings": {
    "Key": "your-groq-api-key",
    "BaseUrl": "https://api.groq.com/openai/v1/chat/completions"
  }
}
```

To get a Groq API key, sign up at [https://console.groq.com](https://console.groq.com)

#### 6. Restore NuGet Packages

```bash
dotnet restore
```

#### 7. Apply Database Migrations

Navigate to the API project directory and apply migrations:

```bash
cd FaqAssistant.Api
dotnet ef database update --project ../FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj
```

Or from the solution root:

```bash
dotnet ef database update --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

#### 8. Run the Application

```bash
cd FaqAssistant.Api
dotnet run
```

The API will be available at:
- **HTTPS**: https://localhost:7052
- **HTTP**: http://localhost:5088
- **Swagger (HTTPS)**: https://localhost:7052/swagger
- **Swagger (HTTP)**: http://localhost:5088/swagger

### Option 3: Using Visual Studio

1. Open `FaqAssistant.sln` in Visual Studio 2022
2. Set `FaqAssistant.Api` as the startup project
3. Update `appsettings.json` with your database configuration
4. Open **Package Manager Console** and run:
   ```powershell
   Update-Database
   ```
5. Press `F5` to run the application

## Database Migrations

### Creating a New Migration

When you make changes to the domain entities or DbContext:

```bash
dotnet ef migrations add MigrationName --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

### Applying Migrations

```bash
dotnet ef database update --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

### Removing Last Migration

```bash
dotnet ef migrations remove --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

### Viewing Migration SQL

```bash
dotnet ef migrations script --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

## Seeding Sample Data

Currently, the application does not include automatic data seeding. You can seed data in several ways:

### Option 1: Using Swagger UI

1. Navigate to your Swagger UI:
   - **Docker**: http://localhost:8080/swagger
   - **Local Development (HTTPS)**: https://localhost:7052/swagger
   - **Local Development (HTTP)**: http://localhost:5088/swagger
2. Register a new user via `/api/Authorization/register`
3. Login via `/api/Authorization/login` to get a JWT token
4. Click "Authorize" button and enter your token as `Bearer {your-token}`
5. Create categories, tags, and FAQs using the respective endpoints

### Option 2: Using SQL Scripts

Connect to your PostgreSQL database and run:

```sql
-- Insert sample categories
INSERT INTO "Categories" ("Id", "Name", "CreatedAt", "LastUpdatedAt", "IsDeleted")
VALUES 
    (gen_random_uuid(), 'Technical', NOW(), NOW(), false),
    (gen_random_uuid(), 'General', NOW(), NOW(), false),
    (gen_random_uuid(), 'Billing', NOW(), NOW(), false);

-- Insert sample tags
INSERT INTO "Tags" ("Id", "Name", "CreatedAt", "LastUpdatedAt", "IsDeleted")
VALUES 
    (gen_random_uuid(), 'C#', NOW(), NOW(), false),
    (gen_random_uuid(), '.NET', NOW(), NOW(), false),
    (gen_random_uuid(), 'API', NOW(), NOW(), false);
```

### Option 3: Create a Seed Service (Recommended for Production)

Add a seeding service in the `FaqAssistant.Infrastructure` project and call it during application startup in `Program.cs`.

## API Endpoints

### Authentication

- `POST /api/Authorization/register` - Register a new user
- `POST /api/Authorization/login` - Login and receive JWT token

### Users

- `GET /api/User` - Get all users
- `GET /api/User/{id}` - Get user by ID
- `PUT /api/User/{id}` - Update user
- `DELETE /api/User/{id}` - Delete user

### Categories

- `GET /api/Category` - Get all categories
- `GET /api/Category/{id}` - Get category by ID
- `POST /api/Category` - Create new category
- `PUT /api/Category/{id}` - Update category
- `DELETE /api/Category/{id}` - Delete category

### Tags

- `GET /api/Tag` - Get all tags
- `GET /api/Tag/{id}` - Get tag by ID
- `POST /api/Tag` - Create new tag
- `PUT /api/Tag/{id}` - Update tag
- `DELETE /api/Tag/{id}` - Delete tag

### FAQs

- `GET /api/Faq` - Get all FAQs
- `GET /api/Faq/{id}` - Get FAQ by ID
- `POST /api/Faq` - Create new FAQ
- `PUT /api/Faq/{id}` - Update FAQ
- `DELETE /api/Faq/{id}` - Delete FAQ
- `POST /api/Faq/ask` - Ask AI for FAQ answer (Rate Limited)

All endpoints except registration and login require authentication via JWT Bearer token.

## Running Tests

The solution includes comprehensive unit tests using xUnit and Moq.

### Run All Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test /p:CollectCoverage=true
```

### Run Tests in Visual Studio

1. Open Test Explorer (`Test > Test Explorer`)
2. Click "Run All Tests"

### Test Structure

Tests are organized by:
- **CommandHandlers**: Tests for CQRS command handlers
- **QueryHandlers**: Tests for CQRS query handlers
- **Services**: Tests for infrastructure services (JWT, Hashing, AI)

## Configuration

### appsettings.json

Key configuration sections:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=faq_db;Username=faq_user;Password=faq_password"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyMinimum32CharactersLong!!",
    "Issuer": "FaqAssistant",
    "Audience": "FaqAssistantUsers",
    "ExpirationMinutes": 60
  },
  "AISettings": {
    "Key": "your-api-key",
    "BaseUrl": "https://api.groq.com/openai/v1/chat/completions"
  },
  "Security": {
    "PasswordSalt": "MzY5ODUyMTQ3MzY1NDc4OQ=="
  }
}
```

### User Secrets (Development)

For development, it's recommended to use User Secrets for sensitive data:

```bash
cd FaqAssistant.Api
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "YourSecretKey"
dotnet user-secrets set "AISettings:Key" "YourAIKey"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionString"
```

### Environment Variables (Production)

In production, use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=faq_db;Username=faq_user;Password=faq_password"
export JwtSettings__SecretKey="YourProductionSecretKey"
export AISettings__Key="YourProductionAIKey"
```

## Rate Limiting

The application implements token bucket rate limiting for AI endpoints:

- **Token Limit**: 10 requests
- **Replenishment**: 10 tokens per minute
- **Queue Limit**: 2 requests

Rate limiting is applied per user (authenticated) or by IP (anonymous).

## Security Considerations

1. **JWT Secret Key**: Always use a strong, random secret key (minimum 32 characters) in production
2. **Password Hashing**: Passwords are hashed using BCrypt with a configurable salt
3. **HTTPS**: Always use HTTPS in production
4. **Connection Strings**: Never commit connection strings with production credentials
5. **API Keys**: Store API keys in secure vaults or environment variables
6. **CORS**: Configure CORS policies appropriately for your frontend application

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to PostgreSQL database

**Solution**:
- Verify PostgreSQL is running: `docker ps` (if using Docker) or check PostgreSQL service
- Check connection string in `appsettings.json`
- Ensure database user has proper permissions
- Check firewall settings

### Migration Issues

**Problem**: Migrations fail to apply

**Solution**:
```bash
# Drop and recreate database
dotnet ef database drop --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
dotnet ef database update --project FaqAssistant.Infrastructure/FaqAssistant.Infrastructure.csproj --startup-project FaqAssistant.Api/FaqAssistant.Api.csproj
```

### Docker Issues

**Problem**: Docker containers won't start

**Solution**:
```bash
# Clean up Docker
docker-compose down -v
docker system prune -a
docker-compose up --build
```

### JWT Authentication Issues

**Problem**: 401 Unauthorized errors

**Solution**:
- Verify JWT token is included in Authorization header as `Bearer {token}`
- Check token expiration
- Verify JWT secret key matches between token generation and validation
- Check Issuer and Audience configuration

## Performance Optimization

1. **Database Indexing**: Entity Framework creates indexes on foreign keys and unique constraints
2. **Rate Limiting**: Prevents API abuse and manages AI costs
3. **Async/Await**: All database operations are asynchronous
4. **Connection Pooling**: Configured in PostgreSQL connection string

## Deployment

### Docker Production Deployment

1. Build production image:
```bash
docker build -f FaqAssistant.Api/Dockerfile -t faqassistant-api:latest .
```

2. Run with production environment:
```bash
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e JwtSettings__SecretKey="your-secret-key" \
  faqassistant-api:latest
```

### Azure Deployment

1. Create Azure resources:
   - Azure App Service (Linux, .NET 10)
   - Azure Database for PostgreSQL

2. Configure connection strings and app settings in Azure Portal

3. Deploy using Azure CLI:
```bash
az webapp up --name faqassistant --resource-group your-rg --runtime "DOTNETCORE:10.0"
```

## Contributing

1. Download the latest version of the project
2. Make your changes and improvements
3. Test thoroughly to ensure everything works
4. Share your modifications with the project maintainer

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Contact the project maintainer
- Check the documentation thoroughly
- Review the troubleshooting section

## Acknowledgments

- Built with [.NET 10](https://dotnet.microsoft.com/)
- AI powered by [Groq](https://groq.com/)
- Database: [PostgreSQL](https://www.postgresql.org/)

---

**Happy Coding! ??**
