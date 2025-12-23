# Vehicle Service

Vehicle catalog and management service for QuickWheels, built with Node.js and Express.

## 📋 Overview

This service handles:

- Vehicle listing creation and management
- Vehicle search and filtering
- Vehicle details retrieval
- Owner vehicle management
- Admin vehicle moderation

## 🛠️ Tech Stack

- **Runtime:** Node.js 20+
- **Framework:** Express.js
- **Language:** TypeScript
- **Database:** MongoDB
- **ODM:** Mongoose
- **Validation:** Joi / Zod

## 🚀 Getting Started

### Prerequisites

- Node.js 20+ and npm
- MongoDB (local or cloud)
- Running Auth Service (for token validation)

### Installation

1. **Navigate to the service directory:**

   ```bash
   cd vehicle-service
   ```

2. **Install dependencies:**

   ```bash
   npm install
   ```

3. **Configure environment:**

   Create `.env` file:

   ```env
   # Server
   PORT=5001
   NODE_ENV=development

   # Database
   MONGODB_URI=mongodb://localhost:27017/quickwheels_vehicles

   # Auth Service
   AUTH_SERVICE_URL=http://localhost:5000/api

   # CORS
   ALLOWED_ORIGINS=http://localhost:5173

   # JWT (for token verification)
   JWT_SECRET=your-super-secret-key-min-32-chars-long
   ```

4. **Run in development mode:**

   ```bash
   npm run dev
   ```

5. **Build for production:**
   ```bash
   npm run build
   npm start
   ```

The service will start at `http://localhost:5001`

## 🗄️ Database Schema

### Vehicle Collection

```javascript
{
  _id: ObjectId,
  ownerId: String,              // User ID from Auth Service
  make: String,                 // Toyota, Honda, etc.
  model: String,                // Corolla, Civic, etc.
  year: Number,                 // 2020
  category: String,             // 'CAR', 'VAN', 'SUV', 'BIKE'
  transmission: String,         // 'MANUAL', 'AUTOMATIC'
  fuelType: String,             // 'PETROL', 'DIESEL', 'ELECTRIC', 'HYBRID'
  seats: Number,                // 5
  pricePerDay: Number,          // 5000.00
  location: String,             // 'Colombo'
  district: String,             // 'Colombo'
  description: String,
  features: [String],           // ['AC', 'GPS', 'Bluetooth']
  images: [String],             // Array of image URLs
  status: String,               // 'AVAILABLE', 'RENTED', 'MAINTENANCE', 'REMOVED'
  isActive: Boolean,            // true (admin can deactivate)
  createdAt: Date,
  updatedAt: Date
}
```

### Indexes

```javascript
db.vehicles.createIndex({ ownerId: 1 });
db.vehicles.createIndex({ location: 1, district: 1 });
db.vehicles.createIndex({ status: 1, isActive: 1 });
db.vehicles.createIndex({ pricePerDay: 1 });
db.vehicles.createIndex({ category: 1 });
```

## 🔌 API Endpoints

**Base URL:** `http://localhost:5001/api`

### Public Endpoints

#### Search Vehicles

```http
GET /vehicles?location=Colombo&category=CAR&minPrice=2000&maxPrice=10000&page=1&limit=12

Response: 200 OK
{
  "vehicles": [...],
  "pagination": {
    "page": 1,
    "limit": 12,
    "total": 45,
    "totalPages": 4
  }
}
```

#### Get Vehicle Details

```http
GET /vehicles/{vehicleId}

Response: 200 OK
{
  "id": "vehicle-id-1",
  "make": "Toyota",
  "model": "Corolla",
  ...
}
```

### Protected Endpoints (User)

#### Create Vehicle Listing

```http
POST /vehicles
Authorization: Bearer {token}
Content-Type: application/json

{
  "make": "Toyota",
  "model": "Corolla",
  "year": 2020,
  "category": "CAR",
  "transmission": "AUTOMATIC",
  "fuelType": "PETROL",
  "seats": 5,
  "pricePerDay": 5000,
  "location": "Colombo",
  "district": "Colombo",
  "description": "Well-maintained car",
  "features": ["AC", "GPS", "Bluetooth"],
  "images": ["https://example.com/image1.jpg"]
}
```

#### Get My Listings

```http
GET /vehicles/my-listings?page=1&limit=10
Authorization: Bearer {token}
```

#### Update Vehicle

```http
PUT /vehicles/{vehicleId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "pricePerDay": 5500,
  "description": "Updated description"
}
```

#### Delete Vehicle

```http
DELETE /vehicles/{vehicleId}
Authorization: Bearer {token}
```

### Admin Endpoints

#### Get All Vehicles (Including Removed)

```http
GET /admin/vehicles?page=1&limit=20&status=all
Authorization: Bearer {admin-token}
```

#### Remove Vehicle

```http
PATCH /admin/vehicles/{vehicleId}/remove
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "reason": "Inappropriate content"
}
```

## 🧪 Testing

### Run all tests

```bash
npm test
```

### Run tests with coverage

```bash
npm run test:coverage
```

### Run tests in watch mode

```bash
npm run test:watch
```

## 📦 NPM Scripts

```json
{
  "dev": "nodemon src/app.ts",
  "build": "tsc",
  "start": "node dist/app.js",
  "test": "jest",
  "test:watch": "jest --watch",
  "test:coverage": "jest --coverage",
  "lint": "eslint src/**/*.ts",
  "lint:fix": "eslint src/**/*.ts --fix",
  "format": "prettier --write \"src/**/*.ts\""
}
```

## 📦 Key Dependencies

```json
{
  "express": "^4.18.0",
  "mongoose": "^8.0.0",
  "dotenv": "^16.0.0",
  "cors": "^2.8.5",
  "helmet": "^7.1.0",
  "express-rate-limit": "^7.1.0",
  "joi": "^17.11.0",
  "jsonwebtoken": "^9.0.0",
  "winston": "^3.11.0",
  "axios": "^1.6.0"
}
```

## 🔧 Configuration

### Environment Variables

- `PORT` - Service port (default: 5001)
- `MONGODB_URI` - MongoDB connection string
- `AUTH_SERVICE_URL` - Auth service URL for token validation
- `JWT_SECRET` - JWT secret for token verification
- `ALLOWED_ORIGINS` - CORS allowed origins
- `NODE_ENV` - Environment (development/production)

### CORS Configuration

```typescript
const corsOptions = {
  origin: process.env.ALLOWED_ORIGINS?.split(",") || ["http://localhost:5173"],
  credentials: true,
};
```

## 🔐 Authentication

This service validates JWT tokens from the Auth Service:

```typescript
import jwt from "jsonwebtoken";

export const authMiddleware = (req, res, next) => {
  const token = req.headers.authorization?.replace("Bearer ", "");

  if (!token) {
    return res.status(401).json({ error: "No token provided" });
  }

  try {
    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    req.user = decoded;
    next();
  } catch (error) {
    return res.status(401).json({ error: "Invalid token" });
  }
};
```

## 🐳 Docker

### Dockerfile

```dockerfile
FROM node:20-alpine

WORKDIR /app

COPY package*.json ./
RUN npm ci --only=production

COPY . .
RUN npm run build

EXPOSE 5001

CMD ["node", "dist/app.js"]
```

### Build and Run

```bash
docker build -t quickwheels-vehicle:latest .
docker run -d -p 5001:5001 --name vehicle-service quickwheels-vehicle:latest
```

## 📊 Logging

Using Winston for structured logging:

```typescript
import winston from "winston";

const logger = winston.createLogger({
  level: "info",
  format: winston.format.json(),
  transports: [
    new winston.transports.File({ filename: "error.log", level: "error" }),
    new winston.transports.File({ filename: "combined.log" }),
  ],
});
```

## 🚨 Error Handling

Standard error response:

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "pricePerDay",
        "message": "Must be a positive number"
      }
    ]
  }
}
```

## 🔄 Inter-Service Communication

### Calling Auth Service

```typescript
import axios from "axios";

const verifyUser = async (userId: string) => {
  try {
    const response = await axios.get(
      `${process.env.AUTH_SERVICE_URL}/users/${userId}`
    );
    return response.data;
  } catch (error) {
    throw new Error("User verification failed");
  }
};
```

## 📈 Performance Optimization

1. **Database Indexing:** Critical fields are indexed
2. **Pagination:** All list endpoints support pagination
3. **Caching:** Consider Redis for frequently accessed data
4. **Connection Pooling:** Mongoose connection pooling enabled

## 📞 Support

For issues specific to this service:

- Check MongoDB connection
- Verify Auth Service is running
- Review environment variables
- Check service logs

## 📄 License

© 2025 QuickWheels. All rights reserved.
