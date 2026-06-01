# CampusERP

CampusERP is a startup-grade ERP system for educational institutions built using .NET and Angular.

## Vision

The goal of this project is to build a modern, scalable, enterprise-level ERP platform while learning professional software engineering practices including:

- Clean Architecture
- SOLID Principles
- Domain Driven Design (DDD)
- CQRS (future)
- Microservices Architecture (future)
- Event Driven Architecture (future)
- Authentication & Authorization
- Payment Gateway Integration
- Notification System
- Email Services
- Examination Management
- Attendance Management
- Fee Management
- MCQ Assessment System
- Reporting & Analytics

---

## Technology Stack

### Backend

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server LocalDB
- JWT Authentication (Upcoming)

### Frontend

- Angular (Upcoming)

### Database

- SQL Server

### Future Integrations

- RabbitMQ
- Hangfire
- Redis
- Razorpay
- Stripe
- SendGrid
- Docker
- Kubernetes

---

## Solution Structure

```text
CampusERP.API
CampusERP.Application
CampusERP.BackgroundJobs
CampusERP.Contracts
CampusERP.Domain
CampusERP.Infrastructure
CampusERP.Shared
CampusERP.Tests
```

## Current Modules

### Core Domain

- Users
- Students
- Teachers
- Courses
- Departments

### Security

- Roles
- Permissions
- UserRoles
- RolePermissions
- RefreshTokens

### Infrastructure

- EF Core
- SQL Server
- Dependency Injection
- Migrations

---

## Current Progress

### Completed

- Clean Architecture setup
- Entity Framework Core setup
- SQL Server integration
- Initial database migration
- RBAC (Role-Based Access Control) foundation
- Refresh Token foundation
- Dependency Injection setup
- Swagger setup

### In Progress

- Authentication & Authorization

### Upcoming

- JWT Authentication
- Role Seeding
- Permission Seeding
- Student Management
- Teacher Management
- Attendance Module
- Examination Module
- Fee Module
- Payment Gateway Module
- Notification Module
- Email Module
- Angular Frontend

---

## Author

Chirag Goyal