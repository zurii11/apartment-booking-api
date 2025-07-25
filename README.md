# 🏢 Apartment Booking System

## 📌 Overview

This is a RESTful web API for managing apartment bookings with robust concurrency control. It is designed to demonstrate reliable transactional behavior under simultaneous access, with **all business logic implemented in SQL Server stored procedures**.

The application is built using:

- ASP.NET Core Web API
- SQL Server (via Docker)
- JWT Authentication
- Pessimistic concurrency control using SQL locking hints

---

## 🚀 Features

- ✅ List all available apartments within a date range
- ✅ Check availability for a specific apartment
- ✅ Book an apartment (with concurrency safety)
- ✅ Cancel an existing booking
- ✅ Simulate simultaneous booking attempts
- ✅ All domain logic is performed in stored procedures

---

## 🛠️ Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/products/docker-desktop)

### 1. Clone the Repository

```bash
git clone https://github.com/zurii11/apartment-booking-api.git
cd apartment-booking-api
```

### 2. Run the application
```bash
docker-compose up --build
```

### 3. Access the API
- Swagger URL: http://localhost:5000/swagger

### 4. Run the simulation
```bash
cd apartment-booking-api/ConcurrentSimulation
dotnet run
```
You will see that from 10 requests sent concurrently to create a booking only one will be satisfied.

### Project structure
```
api/                    -> .NET Project root
    Controllers/        -> API Endpoints
    Services/           -> Service logic calling stored procedures
    Models/             -> DTOs
    Database/           -> Setting up the initial connection to the DB
    Extensions/         -> Injecting services
    Helpers/            -> JWT helper
sql/                    -> TSQL scripts
    create_db.sql       -> Create the database if not already exists
    init.sql            -> Setting up tables and inserting inital test data
    procedures.sql      -> All the procedures
ConcurrentSimulation/   -> App to test concurrency control
```

### Authentication

JWT-based authentication at /api/auth/register and /api/auth/login. Right now all no endpoints require authorization for the ease of testing.

## 📡 API Endpoints

| Method | Endpoint                                                                 | Description                                        |
|--------|--------------------------------------------------------------------------|----------------------------------------------------|
| GET    | `/Apartments/available?start_date=yyyy-mm-dd&end_date=yyyy-mm-dd`         | List available apartments for a date range         |
| GET    | `/Apartments/{id}/availability?start-date=yyyy-mm-dd&end_date=yyyy-mm-dd` | Check if a specific apartment is available         |
| POST   | `/Bookings`                                                              | Create a new booking *(auth required)*             |
| DELETE | `/Bookings/{id}`                                                         | Cancel an existing booking *(auth required)*       |

---

## 🧾 SQL Stored Procedures

All business/domain logic is handled in stored procedures:

- `RegisterUser` – Registers a new user with hashed password
- `GetUserByUsername` – Returns user details by username
- `CreateBooking` – Creates a booking only if the apartment is available
- `CheckApartmentAvailability` – Checks availability of a specific apartment
- `GetAvailableApartments` – Returns list of apartments not booked in date range
- `CancelBooking` – Cancels a booking if it belongs to the current user

All stored procedures are located in `sql/procedures.sql` when the database is created.


## My reasoning

This project uses **Pessimistic Concurrency Control (PCC)** to ensure data integrity in high-conflict scenarios like apartment booking.

### Why Pessimistic Concurrency?

In the context of booking systems, concurrency conflicts are likely — multiple users may attempt to book the same apartment for overlapping dates at the same time. To prevent race conditions and ensure consistency, we use **explicit locking** inside SQL stored procedures.

### Locking Mechanisms

- **UPDLOCK**: Used when checking for overlapping bookings during the booking creation process. It prevents other transactions from acquiring conflicting locks and avoids deadlocks by ensuring only one transaction can proceed to write.
- **HOLDLOCK**: Combined with UPDLOCK to ensure the acquired lock is held for the duration of the transaction, simulating a `SERIALIZABLE` isolation level.
- **READ COMMITTED** (default): Used for simple reads (e.g., availability checks) where no immediate follow-up write action is expected.

### Benefits

- **Prevents double booking** reliably
- **Avoids lost updates or dirty reads**
- **Ensures correctness without requiring application-level logic for concurrency resolution**

## Resources

Here are some resources I used to learn about this stuff. Some of them were much more technical than I could understand at this level but helped me not only to get this task done, but understand what and why is happening at a deeper level.

- [The Fundamentals of Reliability in the Context of Distributed Databases](https://blogs.oracle.com/maa/post/fundamentals-of-reliability-distributed-databases-part-1)
- [From Chaos to Order: The Importance of Concurrency Control within the Database](https://blogs.oracle.com/maa/post/from-chaos-to-order-the-importance-of-concurrency-control-within-the-database-2-of-6)
- [Stored procedures (Database Engine)](https://learn.microsoft.com/en-us/sql/relational-databases/stored-procedures/stored-procedures-database-engine?view=sql-server-ver17)
- [Transact-SQL reference (Database Engine)](https://learn.microsoft.com/en-us/sql/t-sql/language-reference?view=sql-server-ver17)
- [Database Locking: What it is, Why it Matters and What to do About it](https://www.methodsandtools.com/archive/archive.php?id=83)
- [Table Hints (Transact-SQL) - SQL Server | Microsoft Learn](https://learn.microsoft.com/en-us/sql/t-sql/queries/hints-transact-sql-table?view=sql-server-ver17)
- [Transactions and Concurrency Control Patterns by Vlad Mihalcea](https://www.youtube.com/watch?v=onYjxRcToto&t=2022s)
