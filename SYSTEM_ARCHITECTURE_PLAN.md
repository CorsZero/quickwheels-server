-- Active: 1766610457996@@localhost@5432@quickwheels_booking_db

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
4. [Database Schema Reference](#database-schema-reference) ⭐ **All Tables in One Place**
5. [Service 1: Auth Service](#service-1-auth-service)
6. [Service 2: Vehicle Service](#service-2-vehicle-service)
7. [Service 3: Booking Service](#service-3-booking-service)
8. [Inter-Service Communication](#inter-service-communication)
9. [Technology Stack](#technology-stack)
10. [Deployment Ports](#deployment-ports)
11. [Security Considerations](#security-considerations)
12. [Getting Started](#getting-started)

---

## System Overview

QuickWheels is a vehicle rental platform where users can list their vehicles and rent vehicles from others. The system follows a microservice architecture with three independent services.

### Key Features

- User registration and authentication
- Vehicle listing and management
- Vehicle search and filtering
- Booking/rental management
- Admin moderation and system management
- Real-time booking status tracking

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
    │ .NET   │   │ .NET   │  │   .NET    │
    │:5000   │   │ :5001  │  │   :5002   │
    └────┬───┘   └───┬────┘  └────┬──────┘
         │           │            │
    ┌────▼───┐  ┌───▼────┐  ┌────▼──────┐
    │  PG    │  │   PG   │  │    PG     │
    │Auth DB │  │Vehicle │  │ Booking   │
    │        │  │   DB   │  │    DB     │
    └────────┘  └────────┘  └───────────┘
```

---

## Database Schema Reference

This section consolidates all database tables from all three services in one place for easy reference and management.

### 📊 Complete Database Overview

| Service             | Database Type | Database Name            | Tables/Collections | Total Indexes |
| ------------------- | ------------- | ------------------------ | ------------------ | ------------- |
| **Auth Service**    | PostgreSQL    | `quickwheels_auth_db`    | Users              | 1 index       |
| **Vehicle Service** | PostgreSQL    | `quickwheels_vehicle_db` | Vehicles           | 7 indexes     |
| **Booking Service** | PostgreSQL    | `quickwheels_booking_db` | Bookings           | 4 indexes     |

---

### 1️⃣ Auth Service Database (PostgreSQL)

**Database:** `quickwheels_auth_db`

#### Table: Users

| Column                   | Type         | Constraints      | Description                   |
| ------------------------ | ------------ | ---------------- | ----------------------------- |
| Id                       | VARCHAR(36)  | PRIMARY KEY      | Unique user identifier (UUID) |
| Email                    | VARCHAR(255) | UNIQUE, NOT NULL | User email address            |
| PasswordHash             | VARCHAR(512) | NOT NULL         | Hashed password (BCrypt)      |
| FullName                 | VARCHAR(255) | NOT NULL         | User's full name              |
| Phone                    | VARCHAR(20)  | NOT NULL         | Contact phone number          |
| Role                     | VARCHAR(20)  | DEFAULT 'USER'   | User role: 'USER' or 'ADMIN'  |
| IsActive                 | BOOLEAN      | DEFAULT TRUE     | Account active status         |
| RefreshToken             | VARCHAR(500) | NULL             | Current refresh token         |
| RefreshTokenExpiry       | TIMESTAMP    | NULL             | Refresh token expiration      |
| PasswordResetToken       | VARCHAR(500) | NULL             | Password reset token          |
| PasswordResetTokenExpiry | TIMESTAMP    | NULL             | Reset token expiration        |
| CreatedAt                | TIMESTAMP    | DEFAULT NOW()    | Account creation timestamp    |
| UpdatedAt                | TIMESTAMP    | NULL             | Last update timestamp         |
| LastLoginAt              | TIMESTAMP    | NULL             | Last login timestamp          |

**Indexes:**

- `idx_users_email` on Email (login queries)

**SQL Schema:**

```sql
CREATE TABLE Users (
    Id                          VARCHAR(36) PRIMARY KEY,
    Email                       VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash                VARCHAR(512) NOT NULL,
    FullName                    VARCHAR(255) NOT NULL,
    Phone                       VARCHAR(20) NOT NULL,
    Role                        VARCHAR(20) DEFAULT 'USER',
    IsActive                    BOOLEAN DEFAULT TRUE,
    RefreshToken                VARCHAR(500),
    RefreshTokenExpiry          TIMESTAMP,
    PasswordResetToken          VARCHAR(500),
    PasswordResetTokenExpiry    TIMESTAMP,
    CreatedAt                   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt                   TIMESTAMP,
    LastLoginAt                 TIMESTAMP
);

CREATE INDEX idx_users_email ON Users(Email);
```

**Design Notes:**

- Refresh tokens stored directly in Users table (one active token per user)
- Password reset tokens also stored in Users table for simplicity
- LastLoginAt tracks user activity

---

### 2️⃣ Vehicle Service Database (PostgreSQL)

**Database:** `quickwheels_vehicle_db`

#### Table: Vehicles

| Column       | Type         | Constraints   | Description                                     |
| ------------ | ------------ | ------------- | ----------------------------------------------- |
| Id           | VARCHAR(36)  | PRIMARY KEY   | Vehicle identifier (UUID)                       |
| OwnerId      | VARCHAR(36)  | NOT NULL      | Reference to User ID (Extracted from JWT)       |
| Make         | VARCHAR(100) | NOT NULL      | Vehicle manufacturer (Toyota, Honda, etc.)      |
| Model        | VARCHAR(100) | NOT NULL      | Vehicle model (Corolla, Civic, etc.)            |
| Year         | INT          | NOT NULL      | Manufacturing year                              |
| Category     | VARCHAR(20)  | NOT NULL      | 'CAR', 'VAN', 'SUV', 'BIKE'                     |
| Transmission | VARCHAR(20)  | NOT NULL      | 'MANUAL', 'AUTOMATIC'                           |
| FuelType     | VARCHAR(20)  | NOT NULL      | 'PETROL', 'DIESEL', 'ELECTRIC', 'HYBRID'        |
| Seats        | INT          | NOT NULL      | Number of seats                                 |
| PricePerDay  | DECIMAL      | NOT NULL      | Daily rental price                              |
| Location     | VARCHAR(100) | NOT NULL      | City/Location                                   |
| District     | VARCHAR(100) | NOT NULL      | District name                                   |
| Description  | TEXT         | NULL          | Vehicle description                             |
| Features     | TEXT         | NULL          | JSON array: ['AC', 'GPS', 'Bluetooth', etc.]    |
| Images       | TEXT         | NULL          | JSON array of image URLs                        |
| Status       | VARCHAR(20)  | NOT NULL      | 'AVAILABLE', 'RENTED', 'MAINTENANCE', 'REMOVED' |
| IsActive     | BOOLEAN      | DEFAULT TRUE  | Admin can deactivate (default: true)            |
| CreatedAt    | TIMESTAMP    | DEFAULT NOW() | Creation timestamp                              |
| UpdatedAt    | TIMESTAMP    | NULL          | Last update timestamp                           |

**Status Values:**

- `AVAILABLE` - Vehicle is available for rent
- `RENTED` - Vehicle is currently rented out
- `MAINTENANCE` - Vehicle is under maintenance (owner set)
- `REMOVED` - Vehicle removed by admin (inappropriate content)

**Indexes:**

- `idx_vehicles_owner` on OwnerId (owner's listings)
- `idx_vehicles_location` on (Location, District) (location searches)
- `idx_vehicles_status` on (Status, IsActive) (filter available vehicles)
- `idx_vehicles_price` on PricePerDay (price range searches)
- `idx_vehicles_category` on Category (category filters)
- `idx_vehicles_created` on CreatedAt DESC (recent listings)
- Full-text search index on (Make, Model, Description) (text search)

**SQL Schema:**

```sql
CREATE TABLE Vehicles (
    Id              VARCHAR(36) PRIMARY KEY,
    OwnerId         VARCHAR(36) NOT NULL,
    Make            VARCHAR(100) NOT NULL,
    Model           VARCHAR(100) NOT NULL,
    Year            INT NOT NULL,
    Category        VARCHAR(20) NOT NULL,
    Transmission    VARCHAR(20) NOT NULL,
    FuelType        VARCHAR(20) NOT NULL,
    Seats           INT NOT NULL,
    PricePerDay     DECIMAL(10,2) NOT NULL,
    Location        VARCHAR(100) NOT NULL,
    District        VARCHAR(100) NOT NULL,
    Description     TEXT,
    Features        TEXT,
    Images          TEXT,
    Status          VARCHAR(20) NOT NULL DEFAULT 'AVAILABLE',
    IsActive        BOOLEAN DEFAULT TRUE,
    CreatedAt       TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt       TIMESTAMP
);

CREATE INDEX idx_vehicles_owner ON Vehicles(OwnerId);
CREATE INDEX idx_vehicles_location ON Vehicles(Location, District);
CREATE INDEX idx_vehicles_status ON Vehicles(Status, IsActive);
CREATE INDEX idx_vehicles_price ON Vehicles(PricePerDay);
CREATE INDEX idx_vehicles_category ON Vehicles(Category);
CREATE INDEX idx_vehicles_created ON Vehicles(CreatedAt DESC);
```

**Design Notes:**

- Features and Images stored as JSON text (can be parsed to arrays)
- Category, Transmission, FuelType, Status stored as string enums
- PricePerDay uses DECIMAL for precise currency values
- Status defaults to 'AVAILABLE' on creation

**Cross-Service References:**

- `OwnerId` → Auth Service `Users.Id`

---

### 3️⃣ Booking Service Database (PostgreSQL)

**Database:** `quickwheels_booking_db`

#### Table: Bookings

| Column          | Type        | Constraints   | Description                             |
| --------------- | ----------- | ------------- | --------------------------------------- |
| Id              | VARCHAR(36) | PRIMARY KEY   | Booking identifier (UUID)               |
| RenterId        | VARCHAR(36) | NOT NULL      | User who is renting (from JWT)          |
| VehicleId       | VARCHAR(36) | NOT NULL      | Reference to Vehicle (contains ownerId) |
| StartDate       | DATE        | NOT NULL      | Rental start date                       |
| EndDate         | DATE        | NOT NULL      | Rental end date                         |
| Days            | INT         | NOT NULL      | Number of rental days                   |
| Status          | VARCHAR(20) | NOT NULL      | Booking status (see below)              |
| Notes           | TEXT        | NULL          | Renter's notes/requests                 |
| RejectionReason | TEXT        | NULL          | Owner's rejection reason                |
| CreatedAt       | TIMESTAMP   | DEFAULT NOW() | Booking creation time                   |
| UpdatedAt       | TIMESTAMP   | DEFAULT NOW() | Last status update time                 |

**Status Values:**

- `PENDING` - Waiting for owner approval
- `APPROVED` - Owner approved the booking
- `REJECTED` - Owner rejected the booking
- `ACTIVE` - Rental in progress (vehicle handed over)
- `COMPLETED` - Rental completed (vehicle returned)
- `CANCELLED` - Renter cancelled before approval

**Indexes:**

- `idx_bookings_renter` on RenterId (renter's bookings)
- `idx_bookings_vehicle` on VehicleId (vehicle bookings/availability)
- `idx_bookings_status` on Status (filter by status)
- `idx_bookings_dates` on (StartDate, EndDate) (availability checks)

**SQL Schema:**

```sql
CREATE TABLE Bookings (
    Id                  VARCHAR(36) PRIMARY KEY,
    RenterId            VARCHAR(36) NOT NULL,
    VehicleId           VARCHAR(36) NOT NULL,
    StartDate           DATE NOT NULL,
    EndDate             DATE NOT NULL,
    Days                INT NOT NULL,
    Status              VARCHAR(20) NOT NULL,
    Notes               TEXT,
    RejectionReason     TEXT,
    CreatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt           TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_bookings_renter ON Bookings(RenterId);
CREATE INDEX idx_bookings_vehicle ON Bookings(VehicleId);
CREATE INDEX idx_bookings_status ON Bookings(Status);
CREATE INDEX idx_bookings_dates ON Bookings(StartDate, EndDate);
```

**Cross-Service References:**

- `RenterId` → Auth Service `Users.Id`
- `VehicleId` → Vehicle Service `Vehicles._id`
- **Note:** Owner information retrieved via Vehicle Service (vehicle.ownerId)

**Design Pattern:**

- VehicleId references Vehicle Service which contains ownerId
- To get owner bookings: Query Vehicle Service for owner's vehicles, then query bookings by vehicleId
- This maintains true microservice independence with no data duplication
- Vehicle details are fetched from Vehicle Service by client
- TotalPrice is calculated on-demand by client using current vehicle price

---

### 🔗 Cross-Service Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                      CLIENT APPLICATION                     │
└──────────┬──────────────────┬────────────────┬─────────────┘
           │                  │                │
           ▼                  ▼                ▼
    ┌───────────┐      ┌────────────┐   ┌──────────────┐
    │   AUTH    │      │  VEHICLE   │   │   BOOKING    │
    │  SERVICE  │      │  SERVICE   │   │   SERVICE    │
    └─────┬─────┘      └──────┬─────┘   └───────┬──────┘
          │                   │                  │
          ▼                   ▼                  ▼
    ┌───────────┐      ┌────────────┐   ┌──────────────┐
    │   Users   │      │  Vehicles  │   │   Bookings   │
    │ (includes │      │            │   │              │
    │  refresh  │      │  ownerId ──┼───┼─→ (via       │
    │  tokens)  │      │            │   │   VehicleId) │
    └───────────┘      └────────────┘   │              │
                       │                │  RenterId ────┼──┐
                       │                │  VehicleId ───┼┐ │
                       │                └──────────────┘│ │
                       │                                 │ │
                       │  References Auth Users ←────────┘ │
                       └───────────────────────────────────┘
                            OwnerId → Users.Id (from JWT)
```

**Data Retrieval Pattern:**

1. Client fetches booking → gets `renterId` and `vehicleId`
2. Client fetches vehicle using `vehicleId` → gets `ownerId` and vehicle details
3. Client fetches renter using `renterId` → gets renter details
4. Client fetches owner using `ownerId` (from vehicle) → gets owner details
5. Client calculates `totalPrice` = `booking.days` × `vehicle.pricePerDay`

**Benefits:**

- ✅ Zero data duplication across services
- ✅ Always fresh data from source
- ✅ True service independence
- ✅ No synchronization issues
- ✅ GDPR compliant (user data in one place)

---

## Service 1: Auth Service

**Technology:** .NET 9.0 (C#)  
**Port:** 5000  
**Database:** PostgreSQL (`quickwheels_auth_db`)  
**Responsibility:** User management, authentication, and authorization

**📊 Database Schema:** See [Database Schema Reference](#database-schema-reference) - Section 1️⃣

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
  "phone": "+94771234567"
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

**Technology:** .NET 9.0 (C#)  
**Port:** 5001  
**Database:** PostgreSQL (`quickwheels_vehicle_db`)  
**Responsibility:** Vehicle catalog, listings, and search

**📊 Database Schema:** See [Database Schema Reference](#database-schema-reference) - Section 2️⃣

**Authentication:**

- `ownerId` is extracted from JWT token (not sent in request body)
- All vehicle creation/modification operations use authenticated user's ID from JWT

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
  "ownerId": "user-id-1",  // Extracted from JWT token
  "message": "Vehicle listed successfully",
  "vehicle": { ...vehicle data... }
}

-- Note: ownerId is automatically set from authenticated user's ID in JWT token
-- Client does not send ownerId in request body
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

**Technology:** .NET 9.0 (C#)  
**Port:** 5002  
**Database:** PostgreSQL (`quickwheels_booking_db`)  
**Responsibility:** Rental bookings and reservations

**📊 Database Schema:** See [Database Schema Reference](#database-schema-reference) - Section 3️⃣

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
  "renterId": "user-id-2",              // From JWT token
  "vehicleId": "vehicle-id-1",
  "startDate": "2025-01-01",
  "endDate": "2025-01-05",
  "days": 5,
  "status": "PENDING",
  "notes": "Need the car for a family trip",
  "createdAt": "2025-12-23T10:00:00Z",
  "message": "Booking request created successfully"
}

-- Client fetches vehicle details from Vehicle Service (includes ownerId)
-- Client calculates totalPrice = days * vehicle.pricePerDay
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
      "vehicleId": "vehicle-id-1",
      "startDate": "2025-01-01",
      "endDate": "2025-01-05",
      "days": 5,
      "status": "PENDING",
      "notes": "Need the car for a family trip",
      "createdAt": "2025-12-23T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 5
  }
}

-- Client fetches vehicle details: GET /api/vehicles/{vehicleId} (includes ownerId)
-- Client calculates totalPrice = days * vehicle.pricePerDay
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
      "renterId": "user-id-2",
      "vehicleId": "vehicle-id-1",
      "startDate": "2025-01-01",
      "endDate": "2025-01-05",
      "days": 5,
      "status": "PENDING",
      "notes": "Need the car for a family trip",
      "createdAt": "2025-12-23T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "limit": 10,
    "total": 5
  }
}

-- Implementation:
-- 1. Client gets owner's vehicles: GET /api/vehicles/my-listings
-- 2. Client queries bookings: GET /api/bookings?vehicleIds={ids}&status=PENDING
-- 3. Client fetches renter details: GET /api/users/{renterId}
-- This maintains true service independence
```

##### 4. Get Booking Details

```http
GET /bookings/{bookingId}
Authorization: Bearer {token}

Response: 200 OK
{
  "id": "booking-id-1",
  "renterId": "user-id-2",
  "vehicleId": "vehicle-id-1",
  "startDate": "2025-01-01",
  "endDate": "2025-01-05",
  "days": 5,
  "status": "APPROVED",
  "notes": "Need the car for a family trip",
  "rejectionReason": null,
  "createdAt": "2025-12-23T10:00:00Z",
  "updatedAt": "2025-12-23T11:00:00Z"
}

-- Client fetches related data separately:
-- Vehicle (includes ownerId): GET /api/vehicles/{vehicleId}
-- Renter: GET /api/users/{renterId}
-- Owner: Extracted from vehicle.ownerId
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

- **Runtime:** .NET 9.0 (C#)
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core
- **Validation:** Data Annotations
- **Authentication:** JWT (Bearer tokens)

#### Booking Service

- **Runtime:** .NET 9.0 (C#)
- **Database:** SQL Server / PostgreSQL
- **ORM:** Entity Framework Core
- **Validation:** FluentValidation / Data Annotations
- **Authentication:** JWT (Bearer tokens)

### Common Tools

- **API Documentation:** Swagger/OpenAPI
- **Environment Management:** dotenv (.env files)
- **Logging:** Serilog (.NET), Console logging
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

### Microservice Independence - Strategic Denormalization Pattern

Services follow microservice best practices with **strategic denormalization** for query efficiency:

**What We Store:**

- ✅ **Stable Reference IDs**: IDs that enable efficient queries (RenterId, VehicleId, OwnerId)
- ✅ **Domain Data**: Data owned by this service (dates, status, notes)

**What We DON'T Store:**

- ❌ **Volatile Details**: Vehicle specs, prices, images (frequent changes)
- ❌ **User Details**: Full names, emails, phone numbers (privacy & GDPR)
- ❌ **Calculated Values**: Total prices (calculated on-demand with current rates)

**Benefits:**

- ✅ Services can query efficiently without cross-service calls
- ✅ No data synchronization issues (only stable IDs are denormalized)
- ✅ Services can run independently
- ✅ Each service owns its domain data

### Authentication Flow

**All Services → Auth Service (Token Validation Only)**

- All services validate JWT tokens independently using shared secret key
- No direct service-to-service HTTP calls for authentication
- User ID extracted from JWT token by middleware

### Client-Side Orchestration (Recommended Pattern)

The **frontend client** or **API Gateway** orchestrates calls to multiple services:

```typescript
// Example: Client creates a booking
// Step 1: Client fetches vehicle to verify details and get ownerId
const vehicle = await fetch(`/api/vehicles/${vehicleId}`);

// Step 2: Client checks availability
const availability = await fetch(
  `/api/bookings/availability/${vehicleId}?startDate=...&endDate=...`
);

if (availability.available) {
  // Step 3: Client creates booking (only vehicleId needed)
  const booking = await fetch("/api/bookings", {
    method: "POST",
    body: JSON.stringify({
      vehicleId: vehicle.id,
      startDate: "2025-01-01",
      endDate: "2025-01-05",
      notes: "Family trip",
    }),
  });

  // Step 4: Client calculates price and extracts owner
  const totalPrice = booking.days * vehicle.pricePerDay;
  const ownerId = vehicle.ownerId;
}
```

### Data Retrieval Pattern

When displaying booking details, the client makes parallel requests:

```typescript
// Fetch booking
const booking = await fetch("/api/bookings/123");
// Returns: { id, renterId, vehicleId, startDate, endDate, days, status }

// Fetch related data in parallel
const [vehicle, renter] = await Promise.all([
  fetch(`/api/vehicles/${booking.vehicleId}`), // Contains ownerId
  fetch(`/api/users/${booking.renterId}`),
]);

// Extract owner from vehicle
const owner = await fetch(`/api/users/${vehicle.ownerId}`);

// Compose full view
const fullBookingView = {
  ...booking,
  vehicleDetails: vehicle, // { make, model, year, pricePerDay, images, ownerId }
  renterDetails: renter, // { fullName, phone, email }
  ownerDetails: owner, // { fullName, phone, email }
  totalPrice: booking.days * vehicle.pricePerDay, // Current price
};
```

### Benefits of This Architecture

1. **True Service Independence**: Each service can be deployed, scaled, and updated independently
2. **No Service-to-Service Dependencies**: Services never directly call each other via HTTP
3. **Zero Data Duplication**: No denormalized fields, all data fetched from source
4. **Single Source of Truth**: All data always comes from the owning service
5. **Resilience**: Booking Service operates independently for core booking operations
6. **Flexibility**: Easy to add/remove services without modifying existing ones
7. **No Stale Data**: Always fresh data from source services

### Alternative: API Gateway Pattern (Future Enhancement)

For production systems, consider implementing an API Gateway:

```
Client → API Gateway → [Auth, Vehicle, Booking Services]
         (Orchestrates calls and aggregates responses)
```

### Alternative: Event-Driven Architecture (Future Enhancement)

For real-time updates and better scalability, consider event-driven communication:

- Use message brokers (RabbitMQ, Kafka, Azure Service Bus)
- Services publish events instead of direct calls
- Example: "BookingApproved" event triggers vehicle status update

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

- .NET 9.0 SDK (for all services)
- PostgreSQL 14+ (for all services)
- Visual Studio 2022 / VS Code (recommended IDE)

### Installation Steps

1. **Clone Repository**

   ```bash
   git clone <repository-url>
   cd QuickWheels
   ```

2. **Setup Auth Service**

   ```bash
   cd auth-service
   dotnet restore
   dotnet run
   ```

3. **Setup Vehicle Service**

   ```bash
   cd vehicle-service
   dotnet restore
   dotnet run
   ```

4. **Setup Booking Service**

   ```bash
   cd booking-service
   dotnet restore
   dotnet run
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
