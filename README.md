# CampusERP

CampusERP is a modern cloud-based Enterprise Resource Planning (ERP) platform designed for educational institutions such as universities, colleges, schools, and multi-campus organizations.

The platform provides a centralized system to manage academic, administrative, and operational activities across institutions while maintaining strict tenant isolation and role-based security.

---

## Key Features

### Multi-Tenant Architecture

* Institution Management
* Campus Management
* Department Management
* Tenant Isolation

### User & Access Management

* Authentication
* Authorization
* Role Management
* Permission Management
* JWT Security
* Refresh Tokens

### Academic Management

* Courses
* Semesters
* Subjects
* Academic Structure Management
* Teacher Assignments
* Student Management

### Faculty Management

* Teacher Profiles
* Department Assignment
* Subject Assignment
* Workload Management

### Student Management

* Admissions
* Enrollment
* Academic Records
* Course Tracking

### Attendance Management

* Attendance Recording
* Attendance Reports
* Subject-wise Attendance
* Faculty Attendance Tracking

### Examination & Results

* Exam Scheduling
* Marks Management
* Grade Calculation
* Result Processing
* Transcript Generation

### Fee Management

* Fee Structures
* Fee Collection
* Payment Tracking
* Financial Reporting

### Timetable Management

* Class Scheduling
* Faculty Scheduling
* Room Allocation

### Communication

* Notifications
* Email Alerts
* SMS Integration
* Announcements

### Reporting & Analytics

* Academic Reports
* Student Performance Analytics
* Attendance Analytics
* Administrative Dashboards

---

## Technology Stack

### Backend

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server

### Frontend

* Angular
* TypeScript
* Angular Material

### Security

* JWT Authentication
* Role-Based Access Control (RBAC)

### Cloud & DevOps

* Microsoft Azure
* Docker
* Kubernetes
* CI/CD Pipelines

---

## Architecture

```text
Frontend (Angular)
        │
        ▼
ASP.NET Core API
        │
        ▼
Application Layer
        │
        ▼
Domain Layer
        │
        ▼
Infrastructure Layer
        │
        ▼
SQL Server
```

---

## Project Structure

```text
CampusERP

├── CampusERP.API
├── CampusERP.Application
├── CampusERP.Contracts
├── CampusERP.Domain
├── CampusERP.Infrastructure
├── CampusERP.Shared

Frontend

├── campuserp-web
```

---

## Design Principles

* Clean Architecture
* SOLID Principles
* Domain Driven Design (DDD)
* Secure by Design
* Scalable Multi-Tenant Architecture
* Cloud-Native Development
* Enterprise Grade Development Practices

---

## Author

Chirag Goyal
