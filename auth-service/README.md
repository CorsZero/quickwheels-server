# Auth Service

User authentication and authorization service for QuickWheels, built with .NET 9.0.

## 📋 Overview

This service handles:

- User registration and login
- JWT token generation and validation
- User profile management
- Admin user management
- Role-based access control (USER, ADMIN)

## 🛠️ Tech Stack

- **Framework:** .NET 9.0 (C#)
- **Database:** SQL Server / PostgreSQL
- **Authentication:** JWT Bearer tokens
- **Password Hashing:** BCrypt
- **ORM:** Entity Framework Core

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server or PostgreSQL
- Visual Studio 2022 / VS Code / Rider

### Installation

1. **Navigate to the service directory:**

   ```bash
   cd auth-service
   ```

2. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

3. **Configure database connection:**

   Create `appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=quickwheels_auth;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
     },
     "Jwt": {
       "Secret": "your-super-secret-key-min-32-chars-long",
       "Issuer": "QuickWheels",
       "Audience": "QuickWheelsApp",
       "ExpirationMinutes": 60,
       "RefreshTokenExpirationDays": 7
     },
     "Cors": {
       "AllowedOrigins": ["http://localhost:5173"]
     }
   }
   ```

4. **Run database migrations:**

   ```bash
   dotnet ef database update
   ```

5. **Run the service:**
   ```bash
   dotnet run
   ```

The service will start at `http://localhost:5000`

## 🗄️ Database Schema

### Users Table

```sql
CREATE TABLE Users (
    Id              VARCHAR(36) PRIMARY KEY,
    Email           VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash    VARCHAR(512) NOT NULL,
    FullName        VARCHAR(255) NOT NULL,
    Phone           VARCHAR(20) NOT NULL,
    NIC             VARCHAR(20) UNIQUE NOT NULL,
    Role            VARCHAR(20) DEFAULT 'USER',
    IsActive        BOOLEAN DEFAULT TRUE,
    CreatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### RefreshTokens Table

```sql
CREATE TABLE RefreshTokens (
    Id              VARCHAR(36) PRIMARY KEY,
    UserId          VARCHAR(36) NOT NULL,
    Token           VARCHAR(512) UNIQUE NOT NULL,
    ExpiresAt       TIMESTAMP NOT NULL,
    CreatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

## 🔌 API Endpoints

**Base URL:** `http://localhost:5000/api`

### Public Endpoints

#### Register User

```http
POST /auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "fullName": "John Doe",
  "phone": "+94771234567",
  "nic": "199812345678"
}
```

#### Login

```http
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

#### Refresh Token

```http
POST /auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Protected Endpoints

#### Get Profile

```http
GET /users/profile
Authorization: Bearer {token}
```

#### Update Profile

```http
PUT /users/profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "fullName": "John Updated",
  "phone": "+94779876543"
}
```

#### Change Password

```http
POST /auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}
```

### Admin Endpoints

#### Get All Users

```http
GET /admin/users?page=1&limit=20&search=john
Authorization: Bearer {admin-token}
```

#### Suspend/Activate User

```http
PATCH /admin/users/{userId}/status
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "isActive": false
}
```

## 🔐 Authentication Flow

1. User registers or logs in
2. Service validates credentials
3. JWT access token and refresh token are generated
4. Tokens are returned to client
5. Client includes access token in subsequent requests
6. Service validates token on each request
7. When access token expires, client uses refresh token to get new tokens

## 🧪 Testing

### Run all tests

```bash
dotnet test
```

### Run with coverage

```bash
dotnet test /p:CollectCoverage=true
```

### Test specific controller

```bash
dotnet test --filter FullyQualifiedName~AuthControllerTests
```

## 📦 NuGet Packages

- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `Microsoft.EntityFrameworkCore` - ORM
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `BCrypt.Net-Next` - Password hashing
- `Swashbuckle.AspNetCore` - API documentation
- `FluentValidation` - Input validation

## 🔧 Configuration

### JWT Settings

- **Secret:** Minimum 32 characters, store securely
- **Expiration:** 60 minutes for access token
- **Refresh Token:** 7 days

### CORS Settings

Add allowed origins in `appsettings.json`:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173", "https://yourdomain.com"]
  }
}
```

### Database Provider

Change provider in `Program.cs`:

```csharp
// SQL Server
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// PostgreSQL
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

## 🐳 Docker

### Build Image

```bash
docker build -t quickwheels-auth:latest .
```

### Run Container

```bash
docker run -d -p 5000:5000 --name auth-service quickwheels-auth:latest
```

## 📝 Environment Variables

Create `.env` file or set environment variables:

```bash
ConnectionStrings__DefaultConnection=Server=localhost;Database=quickwheels_auth;...
Jwt__Secret=your-secret-key
Jwt__ExpirationMinutes=60
ASPNETCORE_ENVIRONMENT=Development
```

## 🚨 Error Handling

Standard error response format:

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format"
      }
    ]
  }
}
```

## 🔒 Security Best Practices

1. **Password Requirements:**

   - Minimum 8 characters
   - At least 1 uppercase letter
   - At least 1 lowercase letter
   - At least 1 number
   - At least 1 special character

2. **Token Security:**

   - Store JWT secret in secure configuration
   - Use HTTPS in production
   - Implement token rotation
   - Set appropriate expiration times

3. **Database Security:**
   - Use parameterized queries
   - Enable SSL for database connections
   - Implement proper access controls

## 📊 Logging

Logs are written to:

- Console (Development)
- File: `logs/auth-service-{Date}.log` (Production)
- Application Insights (Production - optional)

Log levels:

- `Information` - General flow
- `Warning` - Unexpected events
- `Error` - Errors and exceptions

## 🔄 Deployment

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

### Run in Production

```bash
cd publish
dotnet AuthService.dll
```

## 📞 Support

For issues specific to this service:

- Check logs in `logs/` directory
- Review `appsettings.json` configuration
- Verify database connection
- Ensure JWT secret is properly configured

## 📄 License

© 2025 QuickWheels. All rights reserved.
