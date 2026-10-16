# Library Management System – Group Project
Prepared by Abhi Adhikari

An ASP.NET Core MVC application for managing a library across multiple branches. This project builds on the individual **Coding Assignment 2** Library Management System and adds new ways for patrons and staff to use it, including a self-service kiosk, reservations, notifications, bulk import and reporting.

**Original project baseline:** Abhi Adhikari (S375342)  
**Group project repository:** https://github.com/Abhi56790/LibraryManagementSystem-GroupProject

## Technologies

- C# and .NET 10
- ASP.NET Core MVC and Razor views
- Entity Framework Core and SQL Server LocalDB
- ASP.NET Core Identity with Admin, Reception and Manager roles
- Bootstrap, HTML and CSS

## Main features

| Area | What it does | Access |
|---|---|---|
| Catalogue | Search books, music and toys; see availability and branch location | Public |
| Self-Service Kiosk | Find an item by library code, view loans using a registered email, and check out available items | Public |
| Item Management | Create, edit and manage books, music and toys | Admin |
| Multi-Branch Inventory | Assign items to a library branch and manage branch details | Admin |
| Branch Transfers | Request and complete transfers between branches | Admin |
| CSV Import | Add multiple books, music and toys from a CSV file | Admin |
| Loans and Borrowers | Register borrowers; check out, return and track items and fines | Reception |
| Reservations and Waitlists | Record holds on borrowed/damaged items and track queue order | Reception |
| Notifications | Log simulated loan, due-date, overdue, fine and reservation notifications | Reception |
| Manager Reports | Review loans, recorded fines, inventory status and branch totals; export CSV or print to PDF | Manager |
| Read-Only API | Provide item, category and service-status information using an API key | External API clients |

## How to install and run

### 1. Requirements

- Windows with **Visual Studio 2026**, or another Visual Studio version that supports targeting **.NET 10**
- **.NET 10 SDK**
- Visual Studio's **ASP.NET and web development** workload
- **SQL Server Express LocalDB**
- Git (for cloning the repository)

### 2. Download the project

```bash
git clone https://github.com/Abhi56790/LibraryManagementSystem-GroupProject.git
```

Open the cloned folder and load the `.sln` solution in Visual Studio. Allow NuGet packages to restore.

### 3. Check the database connection

Open `appsettings.json` and check the `DefaultConnection` connection string. It must point to a SQL Server instance available on your computer (normally LocalDB for this project).

The application uses Entity Framework Core migrations and runs `Database.MigrateAsync()` at startup. On a fresh LocalDB instance, this should create/update the database automatically.

If migration does not run successfully, check LocalDB and the connection string first. You can also apply the existing migrations from Visual Studio's **Package Manager Console**:

```powershell
Update-Database
```

### 4. Build and launch

1. Select **Build → Rebuild Solution**.
2. Run the application using **HTTPS** (`Ctrl + F5` or the Visual Studio Run button).
3. Open the local URL displayed by Visual Studio.
4. Sign in using one of the demonstration accounts below.

> These instructions should be checked on a fresh computer or virtual machine before submission. A successful run on the development computer does not replace that test.

## Demonstration accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@library.com` | `Admin123!` |
| Reception | `reception@library.com` | `Reception123!` |
| Manager | `manager@library.com` | `Manager123!` |

The application creates these demonstration accounts and roles during startup through `RoleSeeder`. They are **for local assessment and testing only**; change or remove them before any real deployment.

## Set up multi-branch demonstration data

The application automatically seeds the **demo user accounts**, but it does **not** automatically create catalogue items or library branches. For a new database, create the branches first, then upload the sample CSV.

1. Sign in as **Admin** and open **Library Branches**.
2. Create three example branches (the codes below must match your CSV):

   | Branch name | Branch code | Example address |
   |---|---|---|
   | Darwin City Library | `DAR01` | Darwin City, NT |
   | Casuarina Library | `CAS01` | Casuarina, NT |
   | Palmerston Library | `PAL01` | Palmerston, NT |

3. Open **CSV Import** and upload a `.csv` file with this exact column order:

```csv
Type,Name,Description,LibraryCode,BranchCode,Author,Genre,Artist,Year,ToyType,MinimumAge
Book,The Hobbit,Fantasy adventure novel,BK-DAR-002,DAR01,J.R.R. Tolkien,Fantasy,,,,
Music,Thriller,Pop music album,MU-DAR-001,DAR01,,,Michael Jackson,1982,,
Toy,Chess Set,Classic strategy board game,TY-CAS-001,CAS01,,,,,Board Game,8
```

4. Check the relevant **Books**, **Music**, **Toys** or public **Catalogue** page to confirm the items and branches appear.

The importer supports `Book`, `Music` and `Toy`, rejects duplicate library codes, and skips rows with unknown branch codes or other basic validation errors. Use **unique library codes** if you repeat this test. The current CSV parser is intended for simple comma-separated values; avoid commas inside field values.

## How to use and test the new modules

### Multi-Branch Inventory and Transfers

As Admin, assign an item to a branch while creating/editing it. Open **Branch Transfers**, choose an item and a different destination branch, and submit the request. The transfer starts as **Requested**. Complete it and check that the item's branch changes and the transfer shows **Completed**.

### Reservations, Waitlists and Notifications

As Reception, borrow an available item for a registered borrower. Open **Reservations** and add one or more reservations for that borrowed item. Queue positions are shown for waiting borrowers. Return the item; the first waiting reservation should become **ReadyForCollection**, and a corresponding notification should be added to the **Notification Log**.

Borrowing and fine events also create simulated notifications. On the Notification Log page, use **Check Loan Notifications** to generate nearly-due or overdue notices for eligible active loans. These are **application records**, not emails or SMS messages sent to real devices.

### Self-Service Kiosk

Open **Self-Service Kiosk** from the navigation bar or visit `/Kiosk`. Use an item's library code to find it. **My Account** shows the registered borrower's active loans and due dates after entering their email. **Check Out Item** uses a registered email and an available item's library code to create a loan. This is a demonstration kiosk, not a production patron authentication system.

### Manager Reports and Exports

Sign in as Manager, select **View Detailed Reports**, and review borrowing totals, overdue loans, inventory health, branch inventory and the loan/fine audit table.

- **Export Loan Report to CSV** downloads a CSV file.
- **Export / Save as PDF** opens a print-friendly report; select **Print → Save as PDF** in the browser. This is browser-based PDF saving, not server-side PDF generation.

The fine figures represent **fines recorded on loans**, not verified cash payments or collected revenue.

## Read-only REST API

The API exposes the following **GET-only** endpoints:

| Endpoint | Purpose |
|---|---|
| `/api/library/items` | Catalogue items |
| `/api/library/items/{id}` | One item by ID |
| `/api/library/categories` | Supported item categories |
| `/api/library/status` | API/service status |

Every request requires an `X-API-Key` header. The development key is configured under `LibraryApi:ApiKey` in `appsettings.json`. To test it in PowerShell, substitute the HTTPS port shown by Visual Studio and the configured key:

```powershell
$baseUrl = "https://localhost:PORT"
$apiKey = "YOUR_CONFIGURED_API_KEY"
Invoke-RestMethod -Uri "$baseUrl/api/library/items" -Headers @{ "X-API-Key" = $apiKey }
```

Repeat the test with `/api/library/categories` and `/api/library/status`. A request with no key or the wrong key should receive **401 Unauthorized**. The API does not expose write endpoints. Do not use a hard-coded demonstration key in production; move it to a secure configuration source.

## Project structure

| Folder/file | Purpose |
|---|---|
| `Controllers/` | MVC actions for catalogue, staff modules, kiosk, reports and API |
| `Models/` | Items, borrowers, loans, branches, transfers, reservations, notifications and view models |
| `Views/` | Razor pages for the website and kiosk |
| `Data/` | `ApplicationDbContext` and Identity role/user seeding |
| `Services/` | Fine-calculation service and related business logic |
| `Migrations/` | Entity Framework Core database migrations |
| `wwwroot/` | CSS and other static website files |
| `Program.cs` | Service registration, database migration, authentication and application routing |
| `appsettings.json` | Development configuration, including database connection and API settings |

## Design and security notes

`Item` is the shared base class for `Book`, `Music` and `Toy`. Entity Framework Core manages storage and relationships. ASP.NET Core Identity restricts the Admin, Reception and Manager areas by role, while the catalogue and kiosk are public. The fine calculator is registered through dependency injection.

This is a university demonstration application. Simulated notifications do not send real messages; API-key authentication and the kiosk's email-based account lookup would need stronger controls for production; and reporting does not include payment processing. The current installation instructions are designed around Windows and SQL Server LocalDB.

## Group portfolio and supporting evidence

The accompanying Group Portfolio contains the installation guide, user guide, requirements review, feature and user-journey diagrams, team plan, task allocation, meeting records and test evidence. Team members should add **actual** hours, meeting details, screenshots and results of fresh-machine/virtual-machine installation testing before submission.
