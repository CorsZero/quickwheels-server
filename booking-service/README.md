# Booking Service

Rental booking and reservation management service for QuickWheels, built with Node.js and Express.

## 📋 Overview

This service handles:

- Booking request creation
- Booking approval/rejection workflow
- Rental status management (Pending → Approved → Active → Completed)
- Availability checking
- Booking history and analytics
- Admin booking oversight

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
- Running Vehicle Service (for vehicle data)

### Installation

1. **Navigate to the service directory:**

   ```bash
   cd booking-service
   ```

2. **Install dependencies:**

   ```bash
   npm install
   ```

3. **Configure environment:**

   Create `.env` file:

   ```env
   # Server
   PORT=5002
   NODE_ENV=development

   # Database
   MONGODB_URI=mongodb://localhost:27017/quickwheels_bookings

   # External Services
   AUTH_SERVICE_URL=http://localhost:5000/api
   VEHICLE_SERVICE_URL=http://localhost:5001/api

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

The service will start at `http://localhost:5002`

## 🗄️ Database Schema

### Booking Collection

```javascript
{
  _id: ObjectId,
  renterId: String,             // User ID who is renting
  ownerId: String,              // Vehicle owner ID
  vehicleId: String,            // Vehicle ID

  // Vehicle snapshot (for historical reference)
  vehicleSnapshot: {
    make: String,
    model: String,
    year: Number,
    pricePerDay: Number,
    images: [String]
  },

  startDate: Date,              // 2025-01-01
  endDate: Date,                // 2025-01-05
  days: Number,                 // 5
  totalPrice: Number,           // 25000 (5 days * 5000)

  status: String,               // 'PENDING', 'APPROVED', 'REJECTED',
                                // 'ACTIVE', 'COMPLETED', 'CANCELLED'

  notes: String,                // Renter's notes
  rejectionReason: String,      // Owner's rejection reason

  createdAt: Date,
  updatedAt: Date
}
```

### Indexes

```javascript
db.bookings.createIndex({ renterId: 1 });
db.bookings.createIndex({ ownerId: 1 });
db.bookings.createIndex({ vehicleId: 1 });
db.bookings.createIndex({ status: 1 });
db.bookings.createIndex({ startDate: 1, endDate: 1 });
```

## 🔌 API Endpoints

**Base URL:** `http://localhost:5002/api`

### Protected Endpoints (User)

#### Create Booking Request

```http
POST /bookings
Authorization: Bearer {token}
Content-Type: application/json

{
  "vehicleId": "vehicle-id-1",
  "startDate": "2025-01-01",
  "endDate": "2025-01-05",
  "notes": "Need the car for a family trip"
}

Response: 201 Created
{
  "id": "booking-id-1",
  "message": "Booking request created",
  "booking": {
    "id": "booking-id-1",
    "vehicleId": "vehicle-id-1",
    "vehicleSnapshot": {...},
    "startDate": "2025-01-01",
    "endDate": "2025-01-05",
    "days": 5,
    "totalPrice": 25000,
    "status": "PENDING"
  }
}
```

#### Get My Rentals (As Renter)

```http
GET /bookings/my-rentals?status=PENDING&page=1&limit=10
Authorization: Bearer {token}
```

#### Get Booking Requests for My Vehicles (As Owner)

```http
GET /bookings/my-requests?status=PENDING&page=1&limit=10
Authorization: Bearer {token}
```

#### Get Booking Details

```http
GET /bookings/{bookingId}
Authorization: Bearer {token}
```

#### Approve Booking (Owner Only)

```http
PATCH /bookings/{bookingId}/approve
Authorization: Bearer {token}
```

#### Reject Booking (Owner Only)

```http
PATCH /bookings/{bookingId}/reject
Authorization: Bearer {token}
Content-Type: application/json

{
  "reason": "Vehicle not available for those dates"
}
```

#### Start Rental (Owner Only)

```http
PATCH /bookings/{bookingId}/start
Authorization: Bearer {token}
```

#### Complete Rental (Owner Only)

```http
PATCH /bookings/{bookingId}/complete
Authorization: Bearer {token}
```

#### Cancel Booking (Renter Only)

```http
PATCH /bookings/{bookingId}/cancel
Authorization: Bearer {token}
```

#### Check Vehicle Availability

```http
GET /bookings/availability/{vehicleId}?startDate=2025-01-01&endDate=2025-01-05

Response: 200 OK
{
  "available": true,
  "conflictingBookings": []
}
```

### Admin Endpoints

#### Get All Bookings

```http
GET /admin/bookings?status=all&page=1&limit=20
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "bookings": [...],
  "pagination": {...},
  "statistics": {
    "total": 500,
    "pending": 25,
    "approved": 50,
    "active": 30,
    "completed": 380
  }
}
```

#### Override Booking Status

```http
PATCH /admin/bookings/{bookingId}/status
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "status": "CANCELLED",
  "reason": "Policy violation"
}
```

#### Get Booking Analytics

```http
GET /admin/bookings/analytics?startDate=2025-01-01&endDate=2025-12-31
Authorization: Bearer {admin-token}
```

## 🔄 Booking Status Flow

```
PENDING (Renter creates booking)
   ├─> APPROVED (Owner approves) ──> ACTIVE (Rental starts) ──> COMPLETED (Rental ends)
   ├─> REJECTED (Owner rejects)
   └─> CANCELLED (Renter cancels)
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
  "axios": "^1.6.0",
  "date-fns": "^3.0.0"
}
```

## 🔧 Configuration

### Environment Variables

- `PORT` - Service port (default: 5002)
- `MONGODB_URI` - MongoDB connection string
- `AUTH_SERVICE_URL` - Auth service URL
- `VEHICLE_SERVICE_URL` - Vehicle service URL
- `JWT_SECRET` - JWT secret for token verification
- `ALLOWED_ORIGINS` - CORS allowed origins

## 🔐 Authentication & Authorization

### Authentication Middleware

```typescript
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

### Authorization Checks

```typescript
// Owner can approve/reject/start/complete
const isOwner = booking.ownerId === req.user.id;

// Renter can cancel
const isRenter = booking.renterId === req.user.id;

// Admin can override
const isAdmin = req.user.role === "ADMIN";
```

## 🔄 Inter-Service Communication

### Fetching Vehicle Data

```typescript
import axios from "axios";

const getVehicleDetails = async (vehicleId: string) => {
  try {
    const response = await axios.get(
      `${process.env.VEHICLE_SERVICE_URL}/vehicles/${vehicleId}`
    );
    return response.data;
  } catch (error) {
    throw new Error("Failed to fetch vehicle details");
  }
};
```

### Fetching User Data

```typescript
const getUserDetails = async (userId: string) => {
  try {
    const response = await axios.get(
      `${process.env.AUTH_SERVICE_URL}/users/${userId}`
    );
    return response.data;
  } catch (error) {
    throw new Error("Failed to fetch user details");
  }
};
```

### Updating Vehicle Status

```typescript
const updateVehicleStatus = async (vehicleId: string, status: string) => {
  try {
    await axios.patch(
      `${process.env.VEHICLE_SERVICE_URL}/vehicles/${vehicleId}/status`,
      { status }
    );
  } catch (error) {
    console.error("Failed to update vehicle status:", error);
  }
};
```

## 📊 Business Logic

### Availability Checking

```typescript
const checkAvailability = async (
  vehicleId: string,
  startDate: Date,
  endDate: Date
) => {
  const conflictingBookings = await Booking.find({
    vehicleId,
    status: { $in: ["APPROVED", "ACTIVE"] },
    $or: [{ startDate: { $lte: endDate }, endDate: { $gte: startDate } }],
  });

  return conflictingBookings.length === 0;
};
```

### Total Price Calculation

```typescript
const calculateTotalPrice = (
  pricePerDay: number,
  startDate: Date,
  endDate: Date
) => {
  const days = Math.ceil(
    (endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)
  );
  return pricePerDay * days;
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

EXPOSE 5002

CMD ["node", "dist/app.js"]
```

### Build and Run

```bash
docker build -t quickwheels-booking:latest .
docker run -d -p 5002:5002 --name booking-service quickwheels-booking:latest
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
    "code": "BOOKING_CONFLICT",
    "message": "Vehicle not available for selected dates",
    "details": {
      "conflictingBookings": [...]
    }
  }
}
```

## 📈 Analytics

The service provides analytics for admins:

- Total bookings by status
- Revenue over time
- Most booked vehicles
- Booking trends by month

## 📞 Support

For issues specific to this service:

- Check MongoDB connection
- Verify Auth and Vehicle services are running
- Review environment variables
- Check service logs
- Ensure date formats are correct

## 📄 License

© 2025 QuickWheels. All rights reserved.
