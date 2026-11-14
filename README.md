# 1. Project Overview

RaceDay is a web-based Race Event Management System developed to make managing race events and participant enrolments easier.

The system is designed to support two main user roles:

- **Organiser:** Manages race events, categories, participant enrolments and race results.
- **Participant:** Registers, logs in, views available events, manages enrolments and views race results.

The project consists of a Web API and an ASP.NET Core MVC web application.

## 2. Project Objectives

The main objectives of RaceDay are to:

- Provide user registration and login.
- Support role-based access for Organisers and Participants.
- Allow Organisers to create and manage race events.
- Allow Participants to browse and enrol in events.
- Allow Organisers to manage event categories and enrolments.
- Allow Organisers to capture and publish race results.
- Provide a user-friendly interface for managing race activities.
- Support cloud image storage for event banner images.

## 3. Technologies Used

The project uses the following technologies:

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API | Backend services |
| ASP.NET Core MVC | Website and user interface |
| .NET 10 | Development framework |
| Entity Framework Core | Database access |
| SQL Server | Database management |
| HTML and CSS | Website structure and styling |
| Bootstrap | Responsive user interface |
| Swagger | API testing and documentation |
| Azure Blob Storage | Planned event image storage |
| Git and GitHub | Version control and project hosting |
| Docker | Containerisation support |

## 4. Project Structure

The solution contains two main projects.

### Part 2 – Web API

`P2_RaceDay.API`

The API handles backend operations, including:

- User authentication.
- Event management.
- Category management.
- Participant enrolments.
- Race results.
- Database operations.

### Part 3 – MVC Web Application

`P3_RaceDay.MVC`

The MVC application provides the website interface.

Its main folders include:

- `Controllers` – handles requests from the website.
- `Models` – contains application models and data objects.
- `Services` – contains services used by the application.
- `Views` – contains the website pages.
- `wwwroot` – contains CSS, JavaScript and other static files.

## 5. Main System Features

### Organiser Interface

The Organiser interface is designed to provide:

- An organiser dashboard.
- Event creation and management.
- Event editing and deletion.
- Category management.
- Participant enrolment management.
- Race result management.

### Participant Interface

The Participant interface is designed to provide:

- Participant dashboard.
- Browse available events.
- View enrolled events.
- View race results.
- View participant profile information.

### Authentication

The system includes:

- User registration.
- User login.
- JWT-based authentication through the API.
- Role-based access for Organisers and Participants.

### Event Image Storage

Azure Blob Storage is intended to store event banner images. Storage configuration and upload functionality must be completed before this feature can be used.

## 6. Installation and Setup

### Requirements

Before running the project, install:

- Visual Studio with ASP.NET and web development support.
- .NET 10 SDK.
- SQL Server.
- Git.
- A GitHub account.

### Step 1: Clone the Repository

Open a terminal and run:

```bash
git clone YOUR-GITHUB-REPOSITORY-URL
```

Move into the downloaded project folder:

```bash
cd YOUR-PROJECT-FOLDER
```

### Step 2: Restore Dependencies

Restore the NuGet packages for both projects:

```bash
dotnet restore P2_RaceDay.API
dotnet restore P3_RaceDay.MVC
```

### Step 3: Configure the API

Check the API configuration and provide the appropriate database connection string and JWT settings.

Do not upload passwords, JWT keys or database credentials to GitHub.

### Step 4: Configure the MVC Application

In the MVC project's `appsettings.json`, configure the API address:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7059/"
  }
}
```

Use the actual API address configured in your launch settings if it differs.

### Step 5: Run the API

Open a terminal in the API project folder and run:

```bash
dotnet run
```

The API Swagger page can be accessed at:

```text
https://localhost:7059/swagger
```

### Step 6: Run the MVC Application

Open another terminal in the MVC project folder and run:

```bash
dotnet run
```

Open the MVC address shown in the terminal or configured in its launch settings.

The API must be running while the MVC application is communicating with it.

## 7. Database

The application uses Entity Framework Core to communicate with the database.

The database contains entities for:

- Users
- Participants
- Categories
- Events
- Races
- Race Entries
- Race Results

A valid database connection must be configured before using SQL Server.

## 8. Current Development Status

The project is being developed in stages.

- [x] Part 2 Web API project created.
- [x] Part 3 MVC project created.
- [x] Organiser and Participant dashboard pages created.
- [x] Initial event management pages created.
- [x] Initial enrolment, category and results pages created.
- [x] API service created.
- [x] Azure Blob Storage service code added.
- [ ] Complete and test registration and login.
- [ ] Complete and test all event management operations.
- [ ] Complete and test participant enrolments.
- [ ] Complete and test results management.
- [ ] Configure and test Azure Blob Storage.
- [ ] Complete Docker configuration.
- [ ] Perform final system testing.

## 9. Future Improvements

Possible future improvements include:

- Improved dashboard statistics.
- Event banner image previews.
- Email notifications.
- Advanced event search and filtering.
- Improved reporting.
- Enhanced security and validation.

## 10. Author

**Project:** RaceDay – Race Event Management System

**Framework:** ASP.NET Core MVC and Web API

**Language:** C#

**Version:** Development Version

---

*RaceDay is an academic software development project.*

