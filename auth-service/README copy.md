# sevaLK-service-auth

A clean architecture authentication service built with ASP.NET Core 9.0 featuring JWT authentication, email notifications, and secure password management.

## Architecture Overview

This service follows **Clean Architecture** principles with clear separation of concerns across four main layers:

```
src/
 ├── Auth/              → Feature Layer (API Use Cases)
 ├── Domain/            → Pure Business Logic & Services
 ├── Infra/             → External World (Database, Security)
 └── Shared/            → Cross-Cutting Concerns
```

---

## 🎯 Layer Responsibilities

### 1. Auth/ — Feature Layer

**The heart of the API.** Each folder represents one complete use case.

**Structure:**
```
Auth/
 ├── Login/             → User authentication
 ├── Register/          → User registration
 ├── RefreshToken/      → Token refresh
 ├── Logout/            → Session termination
 ├── ChangePassword/    → Password update for authenticated users
 └── ResetPassword/     → Password recovery via email (6-digit code)
```

**Each feature contains:**
- `Controller` — HTTP endpoint
- `Request` — Input DTOs
- `Handler` — Business logic orchestration

---

### 2. Domain/ — Pure Business Logic

**Core business entities, rules, and domain services.**

**Structure:**
```
Domain/
 ├── Entities/          → Business entities (User)
 ├── Enums/             → Fixed value sets (UserRole)
 └── Objects/           → Domain services (EmailService)
```

**Key Services:**
- **EmailService** — HTML email notifications with professional templates
  - Password reset emails (6-digit codes)
  - Welcome emails for new users

**Constraints:**
- ❌ No ASP.NET Core dependencies
- ❌ No Entity Framework Core
- ✔ Pure C# domain models and services

---

### 3. Infra/ — Infrastructure Layer

**External systems and technical implementations.**

**Structure:**
```
Infra/
 ├── Config/            → EF Core, AuthDbContext, PostgreSQL setup
 ├── Repositories/      → Data access layer (UserRepository)
 └── Security/          → JWT generation/validation, password hashing
```

**Characteristics:**
- ✔ Interface-based design (IJwtService, IPasswordHasher, IEmailService)
- ✔ Replaceable implementations
- ✔ Technology-specific code isolated here

---

### 4. Shared/ — Cross-Cutting Concerns

**Reusable components used across features.**

**Structure:**
```
Shared/
 ├── Exceptions/        → Custom exceptions
 └── Middlewares/       → ApiResponse wrapper, exception handler, JWT middleware
```

**Key Components:**
- Standardized API responses
- Centralized exception handling
- JWT token extraction middleware

---

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 13+
- Gmail account (or SMTP server) for email functionality

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd sevaLK-service-auth
   ```

2. **Configure environment variables**
   
   Copy `.env.example` to `.env`:
   ```bash
   cp .env.example .env
   ```

   Update `.env` with your actual values:
   ```env
   # Database
   ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=sevalK_auth_db;Username=sevalK_admin;Password=your-password

   # JWT
   JwtSettings__SecretKey=your-secret-key-min-32-chars
   JwtSettings__Issuer=sevalK-auth-service
   JwtSettings__Audience=sevalK-api
   JwtSettings__ExpirationMinutes=60
   JwtSettings__RefreshTokenExpirationDays=7

   # Email (Gmail example)
   EmailSettings__SmtpHost=smtp.gmail.com
   EmailSettings__SmtpPort=587
   EmailSettings__SmtpUsername=your-email@gmail.com
   EmailSettings__SmtpPassword=your-app-password
   EmailSettings__FromEmail=noreply@sevalK.com
   EmailSettings__FromName=SevaLK
   ```

   **⚠️ Gmail Setup:**
   - Enable 2FA in your Google account
   - Generate an [App Password](https://myaccount.google.com/apppasswords)
   - Use the 16-character app password (spaces optional)

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Run migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the service**
   ```bash
   dotnet run
   ```

   The service will be available at `http://localhost:3000`

---

## 📧 Email Features

### Password Reset Flow

1. User requests password reset via `/api/auth/request-password-reset`
2. System generates a **6-digit code** (valid for 1 hour)
3. **HTML email sent** with professional template containing the code
4. User submits code and new password to `/api/auth/reset-password`
5. System validates code and updates password

**Security Features:**
- Short-lived codes (1 hour expiration)
- Email enumeration prevention (always returns success)
- Secure token storage in database
- Professional HTML email templates

---

## 🔐 Authentication Flow

### Register → Login → Access Protected Resources

```
POST /api/auth/register          → Create account (sends welcome email)
POST /api/auth/login             → Get access + refresh tokens
GET  /api/protected              → Use Bearer token
POST /api/auth/refresh-token     → Get new access token
POST /api/auth/logout            → Invalidate refresh token
```

### Password Management

```
POST /api/auth/change-password          → Authenticated users
POST /api/auth/request-password-reset   → Forgot password (email sent)
POST /api/auth/reset-password           → Complete reset with 6-digit code
```

---

## 🔐 Security Features

- **JWT-based authentication** with access and refresh tokens
- **Secure password hashing** using BCrypt
- **Refresh token rotation** with database tracking
- **Token expiration handling** (60 min access, 7 day refresh by default)
- **Email enumeration prevention** on password reset
- **6-digit time-limited reset codes** (1 hour expiration)
- **HTTPS recommended** for production
- **Secret management** via environment variables (`.env` + DotNetEnv package)
- **Exception sanitization** (no sensitive data in API responses)

---

## 🛠️ Configuration

### Environment Variables

All secrets are managed via `.env` file (never committed):

| Variable | Description | Example |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | `Host=localhost;Port=5432;Database=...` |
| `JwtSettings__SecretKey` | JWT signing key (min 32 chars) | Generate with PowerShell/OpenSSL |
| `JwtSettings__Issuer` | Token issuer | `sevalK-auth-service` |
| `JwtSettings__Audience` | Token audience | `sevalK-api` |
| `JwtSettings__ExpirationMinutes` | Access token lifetime | `60` |
| `JwtSettings__RefreshTokenExpirationDays` | Refresh token lifetime | `7` |
| `EmailSettings__SmtpHost` | SMTP server | `smtp.gmail.com` |
| `EmailSettings__SmtpPort` | SMTP port | `587` |
| `EmailSettings__SmtpUsername` | Email account | `your-email@gmail.com` |
| `EmailSettings__SmtpPassword` | SMTP password/app password | Gmail app password |
| `EmailSettings__FromEmail` | Sender email | `noreply@sevalK.com` |
| `EmailSettings__FromName` | Sender display name | `SevaLK` |

**Note:** Double underscores (`__`) in environment variables represent nested JSON sections (e.g., `JwtSettings__SecretKey` → `JwtSettings:SecretKey` in code).

### Generate Secure JWT Secret

**PowerShell (Windows):**
```powershell
[Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

**OpenSSL (Linux/macOS/WSL):**
```bash
openssl rand -base64 48
```

---

## 🏗️ Architecture Benefits

### ✅ Clean Separation
- Business logic independent of frameworks
- Easy to test each layer in isolation
- Clear dependency flow (Auth → Domain ← Infra)

### ✅ Maintainability
- Each feature is self-contained
- Changes don't ripple across layers
- Easy to add new authentication features

### ✅ Testability
- Domain layer: Pure unit tests
- Handlers: Mock repositories and services
- Controllers: Integration tests

### ✅ Flexibility
- Easy to swap implementations (PostgreSQL → MySQL, SMTP → SendGrid)
- Technology-agnostic business logic
- Framework upgrades isolated to infrastructure layer

---

## 📐 Design Principles

1. **Dependency Inversion** — High-level modules depend on abstractions
2. **Single Responsibility** — Each component has one clear purpose
3. **Vertical Slicing** — Features organized by use case, not technical layer
4. **Interface Segregation** — Services expose minimal, focused interfaces
5. **DRY (Don't Repeat Yourself)** — Reusable components in Domain/Objects and Shared layers

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Watch mode
dotnet watch test
```

---

## 📦 Production Deployment

### Secrets Management

**Do NOT use `.env` in production.** Use:

- **Azure:** Azure Key Vault
- **AWS:** Secrets Manager or Parameter Store
- **Docker:** Environment variables in docker-compose/Kubernetes secrets
- **Linux:** System environment variables or systemd secrets

### Docker Example

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "sevaLK-service-auth.dll"]
```

```yaml
# docker-compose.yml
version: '3.8'
services:
  auth-service:
    build: .
    environment:
      ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=sevalK_auth_db;Username=postgres;Password=${DB_PASSWORD}"
      JwtSettings__SecretKey: "${JWT_SECRET}"
      EmailSettings__SmtpUsername: "${SMTP_USER}"
      EmailSettings__SmtpPassword: "${SMTP_PASS}"
    depends_on:
      - db
  db:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: "${DB_PASSWORD}"
```

---

## 🤝 Contributing

When adding new features:

1. Follow the established folder structure
2. Keep layers independent (Domain has no infrastructure dependencies)
3. Add new handlers for business logic
4. Write unit tests for handlers and services
5. Update this README with new endpoints/features
6. Never commit `.env` file

---

## 📝 License

[Your License Here]

---

## 👥 Team

sevaLK Development Team