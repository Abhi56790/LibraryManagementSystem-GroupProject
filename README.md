# Library Management System
by Abhi Adhikari(s375342)

## Overview

This project is an ASP.NET Core MVC Library Management System developed for Coding Assignment 2.

The system manages books, music, toys, borrowers, borrowing, returns, fines and management statistics.

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQL Server LocalDB
- ASP.NET Core Identity
- Bootstrap

## Main Features

### Admin

- Add, view, edit and delete books
- Add, view, edit and delete music
- Add, view, edit and delete toys
- Manage item status
- Prevent duplicate library codes

### Reception

- Manage borrower details
- Borrow items using library code
- Prevent unavailable items from being borrowed
- Return borrowed items
- Automatically calculate late fines
- View active loans
- View loan history

### Manager

- View total loans
- View active and returned loans
- View overdue loans
- View item status statistics
- View fine statistics

### Public Catalogue

- Search library items without logging in
- Search by item name, description and library code
- Search books by author and genre
- Search music by artist
- Search toys by type
- Responsive layout for desktop and mobile

## Demo Accounts

### Admin

Email:

`admin@library.com`

Password:

`Admin123!`

### Reception

Email:

`reception@library.com`

Password:

`Reception123!`

### Manager

Email:

`manager@library.com`

Password:

`Manager123!`

These accounts are demonstration accounts created for the assignment.

## How to Run

1. Open `LibraryManagementSystem.sln` in Visual Studio.
2. Make sure .NET 10 and SQL Server LocalDB are installed.
3. Build the solution.
4. Run the project using HTTPS.
5. The database and required roles are created automatically when the application starts.

No manual database migration should normally be required.

## Project Structure

- `Models` contains the library data models.
- `Controllers` handles application requests and business workflow.
- `Views` contains the Razor user interface.
- `Data` contains the Entity Framework database context and role seeding.
- `Services` contains reusable business logic such as fine calculation.

## Important Design Decisions

`Item` is the superclass for `Book`, `Music` and `Toy`. This allows common properties such as name, description, library code and status to be shared.

Entity Framework Core is used to store and retrieve application data.

ASP.NET Core Identity and roles are used to restrict Admin, Reception and Manager functionality.

Dependency Injection is used for the fine calculation service so that the calculation logic is separated from the controller.

## Security

Role-based authorization protects staff areas:

- Admin pages require the Admin role.
- Reception pages require the Reception role.
- Manager pages require the Manager role.
- The Catalogue is available publicly.

The passwords included in this README are demo credentials only and are not intended for a production system.