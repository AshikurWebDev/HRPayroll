# EasyHR - Payroll & HR Management System

EasyHR is a robust, role-based Human Resource and Payroll Management System built on ASP.NET MVC 5 and Entity Framework. It streamlines HR operations, attendance tracking, leave management, and monthly payroll processing.

## 🚀 Features

* **Role-Based Access Control (RBAC):** Five distinct roles (Admin, HR Manager, HR Officer, Manager, Employee) with specific, secured access levels.
* **Employee Management:** Complete CRUD operations for employee profiles, designations, departments, and shifts.
* **Attendance System:** Track daily employee check-ins/check-outs with automated CSV uploads and department-filtered views for Managers.
* **Leave Management:** 
  * Configurable leave types (Paid/Unpaid, Gender-specific).
  * Auto-provisioning of leave balances for employees.
  * Leave overflow handler (Automatically shifts overflow requests into Unpaid Leave).
  * Manager and HR approval workflows.
* **Payroll Processing:**
  * Automated monthly payroll generation based on attendance and leave history.
  * PDF and Excel exports for salary slips and cycle reports.
  * Strict lock-down rules (Paid payroll cycles cannot be altered or deleted).

## 🛠️ Technology Stack

* **Framework:** ASP.NET MVC 5 (.NET Framework)
* **Language:** C#
* **ORM:** Entity Framework 6 (Code-First Approach)
* **Database:** SQL Server
* **Frontend:** HTML5, CSS3, Bootstrap 5, jQuery
* **Exporting/Reporting:** iTextSharp (PDF), EPPlus (Excel)
* **Authentication:** BCrypt password hashing & Cookie-based Forms Authentication

## ⚙️ Getting Started

### Prerequisites
* Visual Studio 2019 or later
* SQL Server (LocalDB or Express)
* .NET Framework 4.8

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone <your-repo-url>
   ```

2. **Open the Solution**
   Open `HR_PayRoll_Management.sln` in Visual Studio.

3. **Configure Database Connection**
   Open `Web.config` and modify the connection string if necessary:
   ```xml
   <connectionStrings>
     <add name="AppDbContext" connectionString="Data Source=.;Initial Catalog=HRPayrollDB;Integrated Security=True" providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Run Migrations (if applicable)**
   Open the Package Manager Console (`Tools` > `NuGet Package Manager` > `Package Manager Console`) and run:
   ```powershell
   Update-Database
   ```
   *Note: The system uses an `AppDbInitializer` to automatically seed default roles, departments, and users if the database is newly created.*

5. **Build and Run**
   * Press `Ctrl + Shift + B` to rebuild the solution.
   * Press `F5` to start the application.

## 👥 Default Login Credentials

By default, the database initializer creates the following accounts:

* **Admin:** Username: `admin` | Password: `admin`
* **HR Manager/Officer:** Username: `manager` | Password: `manager`
* **Employees:** Username: `rahim` or `karim` | Password: `12345`

*(Note: Change these passwords in a production environment!)*

## 📂 Project Structure

* `/Controllers` - Handles routing and HTTP request processing.
* `/Models` - Entity classes representing database tables.
* `/ViewModels` - Custom classes for transferring data between views and controllers.
* `/Services` - Core business logic (Payroll calculations, Leave processing).
* `/DAL` - Data Access Layer containing `AppDbContext`, Repositories, and UnitOfWork.
* `/Views` - Razor views (UI) organized by controller.
* `/Filters` - Custom Action Filters (e.g., `[HasPermission]`).

## 🛡️ License

This project is intended for educational purposes and internal corporate use.
