# QuickWheels - Vehicle Rental System

## Complete System Architecture & API Documentation

**Version:** 1.0  
**Date:** December 23, 2025  
**Architecture:** Microservices

---

## 📋 Table of Contents

1. [System Overview](#system-overview)
2. [User Roles](#user-roles)
3. [Architecture](#architecture)
4. [Service 1: Auth Service](#service-1-auth-service)
5. [Service 2: Vehicle Service](#service-2-vehicle-service)
6. [Service 3: Booking Service](#service-3-booking-service)
7. [Technology Stack](#technology-stack)
8. [Deployment Ports](#deployment-ports)

---

## System Overview

QuickWheels is a vehicle rental platform where users can list their vehicles and rent vehicles from others. The system follows a microservice architecture with three independent services.

### Key Features

- User registration and authentication
- Vehicle listing and management
- Vehicle search and filtering
- Booking/rental management
- Admin moderation and system management
- No payment integration (removed as per client request)

---

## User Roles

### 1. **User** (Default Role)

- Can register and login
- Can list vehicles for rent
- Can rent vehicles from other users
- Can manage their own listings
- Can view and manage their bookings

### 2. **Admin** (Privileged Role)

- All User permissions
- Can view all users
- Can suspend/activate users
- Can view and manage all vehicles
- Can remove inappropriate listings
- Can view all bookings
- Can override booking statuses
- Access to analytics and reports

---

## Architecture

```
┌─────────────────┐
│  React Client   │
│  (Port 5173)    │
└────────┬────────┘
         │
    ┌────┴─────────────────────────┐
    │                              │
    │         API Gateway          │
    │       (Optional/Future)      │
    │                              │
    └────┬────────────┬────────────┤
         │            │            │
    ┌────▼───┐   ┌───▼────┐  ┌────▼──────┐
    │  Auth  │   │Vehicle │  │  Booking  │
    │Service │   │Service │  │  Service  │
    │:5000   │   │ :5001  │  │   :5002   │
    └────┬───┘   └───┬────┘  └────┬──────┘
         │           │            │
    ┌────▼───┐  ┌───▼────┐  ┌────▼──────┐
    │ Auth   │  │Vehicle │  │  Booking  │
    │   DB   │  │   DB   │  │    DB     │
    └────────┘  └────────┘  └───────────┘
```

---

## Service 1: Auth Service

**Technology:** .NET 9.0 (C#)  
**Port:** 5000  
**Database:** SQL Server / PostgreSQL  
**Responsibility:** User management, authentication, and authorization

### Database Model

#### Users Table

```sql
CREATE TABLE Users (
    Id              VARCHAR(36) PRIMARY KEY,
    Email           VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash    VARCHAR(512) NOT NULL,
    FullName        VARCHAR(255) NOT NULL,
    Phone           VARCHAR(20) NOT NULL,//
    NIC             VARCHAR(20) UNIQUE NOT NULL,
    Role            VARCHAR(20) DEFAULT 'USER',  -- 'USER' or 'ADMIN'//
    IsActive        BOOLEAN DEFAULT TRUE,
    CreatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_users_role ON Users(Role);
```

#### RefreshTokens Table

```sql
CREATE TABLE RefreshTokens (
    Id              VARCHAR(36) PRIMARY KEY,
    UserId          VARCHAR(36) NOT NULL,
    Token           VARCHAR(512) UNIQUE NOT NULL,
    ExpiresAt       TIMESTAMP NOT NULL,
    CreatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX idx_refresh_tokens_user ON RefreshTokens(UserId);
```

### API Endpoints

**Base URL:** `http://localhost:5000/api`

#### Public Endpoints

##### 1. Register User

```http
POST /auth/register
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "fullName": "John Doe",
  "phone": "+94771234567",
  "nic": "199812345678"
}

Response: 201 Created
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "uuid-here",
    "email": "user@example.com",
    "fullName": "John Doe",
    "phone": "+94771234567",
    "role": "USER",
    "isActive": true
  }
}
```

##### 2. Login

```http
POST /auth/login
Content-Type: application/json

Request Body:
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "uuid-here",
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "USER"
  }
}
```

##### 3. Refresh Token

```http
POST /auth/refresh-token
Content-Type: application/json

Request Body:
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### Protected Endpoints (Require Authentication)

##### 4. Get Current User Profile

```http
GET /users/profile
Authorization: Bearer {token}

Response: 200 OK
{
  "id": "uuid-here",
  "email": "user@example.com",
  "fullName": "John Doe",
  "phone": "+94771234567",
  "nic": "199812345678",
  "role": "USER",
  "isActive": true,
  "createdAt": "2025-12-23T10:00:00Z"
}
```

##### 5. Update Profile

```http
PUT /users/profile
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "fullName": "John Updated Doe",
  "phone": "+94779876543"
}

Response: 200 OK
{
  "message": "Profile updated successfully",
  "user": {
    "id": "uuid-here",
    "fullName": "John Updated Doe",
    "phone": "+94779876543"
  }
}
```

##### 6. Change Password

```http
POST /auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "currentPassword": "OldPass123!",
  "newPassword": "NewPass456!"
}

Response: 200 OK
{
  "message": "Password changed successfully"
}
```

##### 7. Logout

```http
POST /auth/logout
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Logged out successfully"
}
```

#### Admin Only Endpoints

##### 8. Get All Users (Admin)

```http
GET /admin/users?page=1&limit=20&search=john&role=USER
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "users": [
    {
      "id": "uuid-1",
      "email": "user1@example.com",
      "fullName": "John Doe",
      "phone": "+94771234567",
      "role": "USER",
      "isActive": true,
      "createdAt": "2025-12-20T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 150,
    "totalPages": 8
  }
}
```

##### 9. Get User by ID (Admin)

```http
GET /admin/users/{userId}
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "id": "uuid-here",
  "email": "user@example.com",
  "fullName": "John Doe",
  "phone": "+94771234567",
  "nic": "199812345678",
  "role": "USER",
  "isActive": true,
  "createdAt": "2025-12-20T10:00:00Z"
}
```

##### 10. Suspend/Activate User (Admin)

```http
PATCH /admin/users/{userId}/status
Authorization: Bearer {admin-token}
Content-Type: application/json

Request Body:
{
  "isActive": false
}

Response: 200 OK
{
  "message": "User status updated",
  "user": {
    "id": "uuid-here",
    "isActive": false
  }
}
```

##### 11. Delete User (Admin)

```http
DELETE /admin/users/{userId}
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "message": "User deleted successfully"
}
```

---

## Service 2: Vehicle Service

**Technology:** Node.js + Express + TypeScript  
**Port:** 5001  
**Database:** MongoDB  
**Responsibility:** Vehicle catalog, listings, and search

### Database Model

#### Vehicles Collection

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

// Indexes
db.vehicles.createIndex({ ownerId: 1 })
db.vehicles.createIndex({ location: 1, district: 1 })
db.vehicles.createIndex({ status: 1, isActive: 1 })
db.vehicles.createIndex({ pricePerDay: 1 })
db.vehicles.createIndex({ category: 1 })
```

### API Endpoints

**Base URL:** `http://localhost:5001/api`

#### Public Endpoints

##### 1. Get All Vehicles (Search & Filter)

```http
GET /vehicles?
    location=Colombo
    &district=Colombo
    &category=CAR
    &minPrice=2000
    &maxPrice=10000
    &transmission=AUTOMATIC
    &seats=5
    &page=1
    &limit=12

Response: 200 OK
{
  "vehicles": [
    {
      "id": "vehicle-id-1",
      "ownerId": "user-id-1",
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
      "images": ["url1", "url2"],
      "features": ["AC", "GPS"],
      "status": "AVAILABLE"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 12,
    "total": 45,
    "totalPages": 4
  }
}
```

##### 2. Get Vehicle by ID

```http
GET /vehicles/{vehicleId}

Response: 200 OK
{
  "id": "vehicle-id-1",
  "ownerId": "user-id-1",
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
  "description": "Well-maintained car with full service history",
  "features": ["AC", "GPS", "Bluetooth"],
  "images": ["url1", "url2", "url3"],
  "status": "AVAILABLE",
  "createdAt": "2025-12-20T10:00:00Z"
}
```

#### Protected Endpoints (User)

##### 3. Create Vehicle Listing

```http
POST /vehicles
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
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

Response: 201 Created
{
  "id": "vehicle-id-1",
  "message": "Vehicle listed successfully",
  "vehicle": { ...vehicle data... }
}
```

##### 4. Get My Listings

```http
GET /vehicles/my-listings?page=1&limit=10
Authorization: Bearer {token}

Response: 200 OK
{
  "vehicles": [
    {
      "id": "vehicle-id-1",
      "make": "Toyota",
      "model": "Corolla",
      "status": "AVAILABLE",
      "pricePerDay": 5000,
      "createdAt": "2025-12-20T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 3
  }
}
```

##### 5. Update Vehicle

```http
PUT /vehicles/{vehicleId}
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "pricePerDay": 5500,
  "description": "Updated description",
  "features": ["AC", "GPS", "Bluetooth", "Parking Sensors"]
}

Response: 200 OK
{
  "message": "Vehicle updated successfully",
  "vehicle": { ...updated vehicle data... }
}
```

##### 6. Update Vehicle Status

```http
PATCH /vehicles/{vehicleId}/status
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "status": "MAINTENANCE"  // AVAILABLE, MAINTENANCE
}

Response: 200 OK
{
  "message": "Vehicle status updated",
  "status": "MAINTENANCE"
}
```

##### 7. Delete Vehicle

```http
DELETE /vehicles/{vehicleId}
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Vehicle deleted successfully"
}
```

#### Admin Endpoints

##### 8. Get All Vehicles (Admin - includes removed)

```http
GET /admin/vehicles?page=1&limit=20&status=all
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "vehicles": [ ...all vehicles including removed... ],
  "pagination": { ... }
}
```

##### 9. Remove Vehicle (Admin)

```http
PATCH /admin/vehicles/{vehicleId}/remove
Authorization: Bearer {admin-token}
Content-Type: application/json

Request Body:
{
  "reason": "Inappropriate content"
}

Response: 200 OK
{
  "message": "Vehicle removed successfully"
}
```

##### 10. Activate Vehicle (Admin)

```http
PATCH /admin/vehicles/{vehicleId}/activate
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "message": "Vehicle activated successfully"
}
```

---

## Service 3: Booking Service

**Technology:** Node.js + Express + TypeScript  
**Port:** 5002  
**Database:** MongoDB  
**Responsibility:** Rental bookings and reservations

### Database Model

#### Bookings Collection

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

// Indexes
db.bookings.createIndex({ renterId: 1 })
db.bookings.createIndex({ ownerId: 1 })
db.bookings.createIndex({ vehicleId: 1 })
db.bookings.createIndex({ status: 1 })
db.bookings.createIndex({ startDate: 1, endDate: 1 })
```

### API Endpoints

**Base URL:** `http://localhost:5002/api`

#### Protected Endpoints (User)

##### 1. Create Booking Request

```http
POST /bookings
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
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
    "vehicleSnapshot": {
      "make": "Toyota",
      "model": "Corolla",
      "year": 2020,
      "pricePerDay": 5000
    },
    "startDate": "2025-01-01",
    "endDate": "2025-01-05",
    "days": 5,
    "totalPrice": 25000,
    "status": "PENDING",
    "createdAt": "2025-12-23T10:00:00Z"
  }
}
```

##### 2. Get My Rentals (As Renter)

```http
GET /bookings/my-rentals?status=PENDING&page=1&limit=10
Authorization: Bearer {token}

Response: 200 OK
{
  "bookings": [
    {
      "id": "booking-id-1",
      "vehicleSnapshot": {
        "make": "Toyota",
        "model": "Corolla",
        "images": ["url1"]
      },
      "startDate": "2025-01-01",
      "endDate": "2025-01-05",
      "totalPrice": 25000,
      "status": "PENDING",
      "createdAt": "2025-12-23T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 5
  }
}
```

##### 3. Get Booking Requests for My Vehicles (As Owner)

```http
GET /bookings/my-requests?status=PENDING&page=1&limit=10
Authorization: Bearer {token}

Response: 200 OK
{
  "bookings": [
    {
      "id": "booking-id-1",
      "renterInfo": {
        "id": "user-id-2",
        "fullName": "Jane Smith",
        "phone": "+94771234567"
      },
      "vehicleSnapshot": {
        "make": "Toyota",
        "model": "Corolla"
      },
      "startDate": "2025-01-01",
      "endDate": "2025-01-05",
      "totalPrice": 25000,
      "status": "PENDING",
      "notes": "Need the car for a family trip",
      "createdAt": "2025-12-23T10:00:00Z"
    }
  ],
  "pagination": { ... }
}
```

##### 4. Get Booking Details

```http
GET /bookings/{bookingId}
Authorization: Bearer {token}

Response: 200 OK
{
  "id": "booking-id-1",
  "renterId": "user-id-2",
  "ownerId": "user-id-1",
  "vehicleId": "vehicle-id-1",
  "vehicleSnapshot": { ... },
  "startDate": "2025-01-01",
  "endDate": "2025-01-05",
  "days": 5,
  "totalPrice": 25000,
  "status": "APPROVED",
  "notes": "Need the car for a family trip",
  "createdAt": "2025-12-23T10:00:00Z"
}
```

##### 5. Approve Booking (Owner Only)

```http
PATCH /bookings/{bookingId}/approve
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Booking approved",
  "booking": {
    "id": "booking-id-1",
    "status": "APPROVED"
  }
}
```

##### 6. Reject Booking (Owner Only)

```http
PATCH /bookings/{bookingId}/reject
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "reason": "Vehicle not available for those dates"
}

Response: 200 OK
{
  "message": "Booking rejected",
  "booking": {
    "id": "booking-id-1",
    "status": "REJECTED",
    "rejectionReason": "Vehicle not available for those dates"
  }
}
```

##### 7. Start Rental (Owner Only - when renter picks up)

```http
PATCH /bookings/{bookingId}/start
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Rental started",
  "booking": {
    "id": "booking-id-1",
    "status": "ACTIVE"
  }
}
```

##### 8. Complete Rental (Owner Only - when renter returns)

```http
PATCH /bookings/{bookingId}/complete
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Rental completed",
  "booking": {
    "id": "booking-id-1",
    "status": "COMPLETED"
  }
}
```

##### 9. Cancel Booking (Renter Only - before approval)

```http
PATCH /bookings/{bookingId}/cancel
Authorization: Bearer {token}

Response: 200 OK
{
  "message": "Booking cancelled",
  "booking": {
    "id": "booking-id-1",
    "status": "CANCELLED"
  }
}
```

##### 10. Check Vehicle Availability

```http
GET /bookings/availability/{vehicleId}?startDate=2025-01-01&endDate=2025-01-05

Response: 200 OK
{
  "available": true,
  "conflictingBookings": []
}

OR

Response: 200 OK
{
  "available": false,
  "conflictingBookings": [
    {
      "startDate": "2025-01-02",
      "endDate": "2025-01-04",
      "status": "APPROVED"
    }
  ]
}
```

#### Admin Endpoints

##### 11. Get All Bookings (Admin)

```http
GET /admin/bookings?status=all&page=1&limit=20
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "bookings": [ ...all bookings... ],
  "pagination": { ... },
  "statistics": {
    "total": 500,
    "pending": 25,
    "approved": 50,
    "active": 30,
    "completed": 380,
    "rejected": 10,
    "cancelled": 5
  }
}
```

##### 12. Override Booking Status (Admin)

```http
PATCH /admin/bookings/{bookingId}/status
Authorization: Bearer {admin-token}
Content-Type: application/json

Request Body:
{
  "status": "CANCELLED",
  "reason": "Policy violation"
}

Response: 200 OK
{
  "message": "Booking status updated",
  "booking": { ... }
}
```

##### 13. Get Booking Analytics (Admin)

```http
GET /admin/bookings/analytics?startDate=2025-01-01&endDate=2025-12-31
Authorization: Bearer {admin-token}

Response: 200 OK
{
  "totalBookings": 500,
  "totalRevenue": 2500000,
  "averageBookingValue": 5000,
  "bookingsByStatus": {
    "PENDING": 25,
    "APPROVED": 50,
    "ACTIVE": 30,
    "COMPLETED": 380
  },
  "bookingsByMonth": [
    { "month": "2025-01", "count": 45, "revenue": 225000 },
    { "month": "2025-02", "count": 50, "revenue": 250000 }
  ],
  "topVehicles": [
    {
      "vehicleId": "vehicle-1",
      "make": "Toyota",
      "model": "Corolla",
      "bookings": 25
    }
  ]
}
```

---

## Technology Stack

### Frontend

- **Framework:** React 18+ with TypeScript
- **Build Tool:** Vite
- **Styling:** CSS Modules
- **HTTP Client:** Axios
- **State Management:** React Context API
- **Routing:** React Router v6

### Backend Services

#### Auth Service

- **Runtime:** .NET 9.0 (C#)
- **Database:** SQL Server / PostgreSQL
- **Authentication:** JWT (Bearer tokens)
- **Password Hashing:** BCrypt / PBKDF2

#### Vehicle Service

- **Runtime:** Node.js 20+ with TypeScript
- **Framework:** Express.js
- **Database:** MongoDB
- **ODM:** Mongoose
- **Validation:** Joi / Zod

#### Booking Service

- **Runtime:** Node.js 20+ with TypeScript
- **Framework:** Express.js
- **Database:** MongoDB
- **ODM:** Mongoose
- **Validation:** Joi / Zod

### Common Tools

- **API Documentation:** Swagger/OpenAPI
- **Environment Management:** dotenv
- **Logging:** Winston (Node.js), Serilog (.NET)
- **CORS:** Configured for cross-origin requests
- **Containerization:** Docker (optional)

---

## Deployment Ports

| Service         | Port | Protocol |
| --------------- | ---- | -------- |
| React Client    | 5173 | HTTP     |
| Auth Service    | 5000 | HTTP     |
| Vehicle Service | 5001 | HTTP     |
| Booking Service | 5002 | HTTP     |

---

## Booking Status Flow

```
PENDING (Renter creates)
   ├─> APPROVED (Owner approves) ──> ACTIVE (Rental starts) ──> COMPLETED (Rental ends)
   ├─> REJECTED (Owner rejects)
   └─> CANCELLED (Renter cancels)
```

---

## Inter-Service Communication

### Vehicle Service → Auth Service

- Validates JWT tokens for protected routes
- Can optionally call Auth Service to verify user details

### Booking Service → Auth Service

- Validates JWT tokens
- Fetches user details (renter/owner) for displaying in booking details

### Booking Service → Vehicle Service

- Fetches vehicle details and pricing when creating a booking
- Can update vehicle status when booking is approved/completed

### Implementation Pattern: HTTP REST Calls

```javascript
// Example: Booking Service calling Vehicle Service
const vehicleResponse = await axios.get(
  `http://localhost:5001/api/vehicles/${vehicleId}`
);
```

### Alternative: Message Queue (Future Enhancement)

For better decoupling, consider using RabbitMQ or Kafka for event-driven communication.

---

## Security Considerations

1. **Authentication:** All protected routes require valid JWT tokens
2. **Authorization:** Role-based access control (USER vs ADMIN)
3. **Data Validation:** Validate all inputs on both client and server
4. **Password Security:** Hash passwords using BCrypt (minimum 10 rounds)
5. **SQL Injection Prevention:** Use parameterized queries
6. **XSS Prevention:** Sanitize user inputs
7. **CORS:** Configure allowed origins
8. **Rate Limiting:** Implement rate limiting on API endpoints
9. **HTTPS:** Use HTTPS in production

---

## API Response Standards

### Success Response

```json
{
  "data": { ... },
  "message": "Operation successful"
}
```

### Error Response

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

### Common HTTP Status Codes

- `200 OK` - Successful GET, PUT, PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource conflict
- `500 Internal Server Error` - Server error

---

## Future Enhancements (Not in Scope)

1. Payment Integration (Stripe/PayPal)
2. Real-time Chat between Renter and Owner
3. Review and Rating System
4. Email/SMS Notifications
5. Vehicle Insurance Management
6. Mobile Application
7. API Gateway with Kong/NGINX
8. CI/CD Pipeline
9. Monitoring with Prometheus/Grafana
10. ElasticSearch for Advanced Search

---

## Getting Started

### Prerequisites

- Node.js 20+
- .NET 9.0 SDK
- MongoDB
- SQL Server / PostgreSQL

### Installation Steps

1. **Clone Repository**

   ```bash
   git clone <repository-url>
   cd QuickWheels
   ```

2. **Setup Auth Service**

   ```bash
   cd sevaLK-service-auth
   dotnet restore
   dotnet run
   ```

3. **Setup Vehicle Service**

   ```bash
   cd quickwheels-server
   npm install
   npm run dev
   ```

4. **Setup Booking Service**

   ```bash
   cd quickwheels-booking-service
   npm install
   npm run dev
   ```

5. **Setup Frontend**
   ```bash
   cd quickwheels-client
   npm install
   npm run dev
   ```

---

## License

© 2025 QuickWheels. All rights reserved.
