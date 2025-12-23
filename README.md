# QuickWheels - Backend Services

Vehicle rental platform backend built with microservice architecture.

## 📁 Project Structure

```
quickwheels-server/
├── auth-service/          # User authentication & management (.NET 9.0)
├── vehicle-service/       # Vehicle catalog & search (Node.js)
├── booking-service/       # Rental bookings & reservations (Node.js)
└── SYSTEM_ARCHITECTURE_PLAN.md
```

## 🚀 Services Overview

| Service             | Technology        | Port | Database              | Description                                                    |
| ------------------- | ----------------- | ---- | --------------------- | -------------------------------------------------------------- |
| **Auth Service**    | .NET 9.0          | 5000 | SQL Server/PostgreSQL | User management, JWT authentication, role-based access control |
| **Vehicle Service** | Node.js + Express | 5001 | MongoDB               | Vehicle listings, search, filtering, and management            |
| **Booking Service** | Node.js + Express | 5002 | MongoDB               | Rental requests, booking management, and analytics             |

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- **Node.js** 20+ and npm
- **.NET 9.0 SDK**
- **MongoDB** (for Vehicle & Booking services)
- **SQL Server** or **PostgreSQL** (for Auth service)
- **Git**

## 🛠️ Installation & Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd quickwheels-server
```

### 2. Setup Auth Service

```bash
cd auth-service
# Follow instructions in auth-service/README.md
```

### 3. Setup Vehicle Service

```bash
cd vehicle-service
# Follow instructions in vehicle-service/README.md
```

### 4. Setup Booking Service

```bash
cd booking-service
# Follow instructions in booking-service/README.md
```

## 🔧 Environment Configuration

Each service requires its own `.env` file. Refer to individual service README files for specific configuration details.

### Auth Service (.NET)

- Database connection string
- JWT secret and settings
- CORS origins

### Vehicle Service (Node.js)

- MongoDB connection string
- Port configuration
- Auth service URL for token validation

### Booking Service (Node.js)

- MongoDB connection string
- Port configuration
- Auth service & Vehicle service URLs

## 🏃 Running Services

### Development Mode

Run each service in a separate terminal:

```bash
# Terminal 1 - Auth Service
cd auth-service
dotnet run

# Terminal 2 - Vehicle Service
cd vehicle-service
npm run dev

# Terminal 3 - Booking Service
cd booking-service
npm run dev
```

### Production Mode

```bash
# Auth Service
cd auth-service
dotnet run --configuration Release

# Vehicle Service
cd vehicle-service
npm run build
npm start

# Booking Service
cd booking-service
npm run build
npm start
```

## 🔗 API Documentation

Full API documentation is available in [SYSTEM_ARCHITECTURE_PLAN.md](./SYSTEM_ARCHITECTURE_PLAN.md)

### API Base URLs (Development)

- Auth Service: `http://localhost:5000/api`
- Vehicle Service: `http://localhost:5001/api`
- Booking Service: `http://localhost:5002/api`

### Quick API Reference

#### Auth Service

- `POST /auth/register` - Register new user
- `POST /auth/login` - Login user
- `GET /users/profile` - Get user profile
- `GET /admin/users` - Get all users (Admin)

#### Vehicle Service

- `GET /vehicles` - Search & filter vehicles
- `POST /vehicles` - Create vehicle listing
- `GET /vehicles/:id` - Get vehicle details
- `GET /vehicles/my-listings` - Get my listings

#### Booking Service

- `POST /bookings` - Create booking request
- `GET /bookings/my-rentals` - Get my rentals
- `GET /bookings/my-requests` - Get requests for my vehicles
- `PATCH /bookings/:id/approve` - Approve booking

## 🧪 Testing

```bash
# Auth Service
cd auth-service
dotnet test

# Vehicle Service
cd vehicle-service
npm test

# Booking Service
cd booking-service
npm test
```

## 📊 Database Setup

### MongoDB (Vehicle & Booking Services)

1. Install MongoDB Community Edition
2. Start MongoDB service
3. Create databases:
   ```javascript
   use quickwheels_vehicles
   use quickwheels_bookings
   ```

### SQL Server / PostgreSQL (Auth Service)

1. Install SQL Server or PostgreSQL
2. Create database: `quickwheels_auth`
3. Run migrations (auto-generated on first run)

## 🔐 Authentication Flow

1. User registers or logs in via Auth Service
2. Auth Service returns JWT access token and refresh token
3. Client includes token in `Authorization: Bearer <token>` header
4. Each service validates the token independently
5. Services use user ID from token to authorize operations

## 🌐 Inter-Service Communication

Services communicate via HTTP REST APIs:

- **Booking Service → Vehicle Service**: Fetch vehicle details for booking
- **Booking Service → Auth Service**: Fetch user details for display
- **Vehicle Service → Auth Service**: Validate user tokens

## 🐳 Docker Support (Optional)

```bash
# Build and run all services
docker-compose up --build

# Run specific service
docker-compose up auth-service
```

## 📝 Development Guidelines

1. **Code Style**: Follow language-specific conventions

   - .NET: Microsoft C# coding conventions
   - Node.js: Airbnb JavaScript/TypeScript style guide

2. **Branching Strategy**:

   - `main` - Production-ready code
   - `develop` - Development branch
   - `feature/*` - Feature branches
   - `bugfix/*` - Bug fix branches

3. **Commit Messages**: Use conventional commits

   ```
   feat: add vehicle search endpoint
   fix: resolve booking date validation issue
   docs: update API documentation
   ```

4. **Testing**: Write unit and integration tests for all endpoints

## 🐛 Troubleshooting

### Port Already in Use

```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:5000 | xargs kill
```

### Database Connection Issues

- Verify connection strings in `.env` files
- Ensure database services are running
- Check firewall settings

### CORS Errors

- Add client URL to CORS configuration in each service
- Ensure proper headers are set

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

© 2025 QuickWheels. All rights reserved.

## 👥 Team

- Backend Lead: [Your Name]
- Database Admin: [Name]
- DevOps: [Name]

## 📞 Support

For issues and questions:

- Create an issue in the repository
- Email: support@quickwheels.com
- Documentation: See SYSTEM_ARCHITECTURE_PLAN.md
