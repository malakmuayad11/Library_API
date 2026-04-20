# 📚 Library Management System - API
A backend RESTful API built with ASP.NET Core for managing a complete library system, including authentication, authorization, and transaction handling.
> 💡 This project is a RESTful Web API of my Library Management System.  
> You can view it on my [GitHub Profile](https://github.com/malakmuayad11/Library_System).

## ✅ Features

### 📚 Library Operations
- Manage books, members, courses, users, loans, and fines  
- Borrow and return books with validation rules  
- Track library transactions and statuses  

### 🔐 Authentication & Authorization
- JWT-based authentication system  
- Role-based authorization  
- Ownership-based access policies  
- Refresh token support and secure logout  

### 🗄️ Data Management
- SQL Server database integration  
- ADO.NET-based data access layer  
- Structured CRUD operations for all entities  

### 🧩 System Architecture
- Three-tier architecture:
  - API Layer  
  - Business Logic Layer (BAL)  
  - Data Access Layer (DAL)
- Infrastructure for logging logic, and models (DTOs)
- Clean separation of concerns  

### 📊 Logging & Monitoring
- Custom logging system  
- Error logging to text files  
- Audit logging for system actions  

### 🌐 API Features
- RESTful API design  
- HTTPS support  
- CORS enabled  

## ⬇️ Installation
1. Clone the repository: git clone https://github.com/malakmuayad11/Library_API.git
2. Restore the database from the backup file, using SQL Server Management Studio.
3. Open the project in Visual Studio.
4. Configure the connection string from the appsettings.json file.
5. Press Start to run the application.

## ⚙️ Technologies:
- C# (ASP.NET Core / .NET 8.0)
- SQL Server
- ADO.NET
- Three-tier architecture (API Layer, BAL, DAL, and Infrastructure)
- Azure Key Vault integration

## 👩‍💻 Author
**Malak Muayad**  
📧 [malakmuayad15@gmail.com](mailto:malakmuayad15@gmail.com)  
🔗 [malakmuayad11](https://github.com/malakmuayad11)
