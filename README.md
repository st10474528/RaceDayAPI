# RaceDay — Parts 1 & 2

A full-stack event management system for the South African road running, walking, and cycling community.

- **Part 1:** System planning — ERD, API endpoint plan, and SQL database script.
- **Part 2:** RESTful API built with ASP.NET Core 8, Entity Framework Core, and SQL Server.

---

## Table of Contents

1. [System Description](#system-description)
2. [Roles](#roles)
3. [Repository Structure](#repository-structure)
4. [Part 1 — Planning & Database](#part-1--planning--database)
5. [Part 2 — The API](#part-2--the-api)
6. [Getting Started](#getting-started)
7. [Running the Tests](#running-the-tests)
8. [API Endpoints](#api-endpoints)
9. [CI/CD](#cicd)
10. [Demo Videos](#demo-videos)
11. [Author](#author)
12. [AI Usage Note](#ai-usage-note)

---

## System Description

RaceDay allows **Event Organisers** to create and manage events, categories, and participant results, while **Participants** can browse events, enter events by category, track their personal enrolments, and view their race history.

The project is built in three parts:
- **Part 1** — System planning and database design (this repo's `/docs` folder).
- **Part 2** — The RESTful API (this repo's `RaceDayAPI` project).
- **Part 3** — The MVC front end + Azure Blob Storage + Docker (not in this repo yet).

---

## Roles

| Role | Capabilities |
|---|---|
| **Organiser** | Create, edit, and delete events; manage categories; view all enrolments for their events; capture participant results. |
| **Participant** | Register and log in; browse events; enrol in an event by selecting a category; view own enrolments; view own race results; update their profile. |

Role-based access is enforced at the API level via a custom `RoleFilter` that returns **403 Forbidden** when the logged-in user's role does not match the endpoint's requirement.

---

## Repository Structure
RaceDayAPI/
├── docs/ # Part 1 deliverables
│ ├── erd.png # Entity Relationship Diagram
│ ├── endpoint-plan.md # API endpoint plan
│ ├── raceday-schema.sql # SQL schema + seed script
│ └── ci-screenshot.png # Screenshot of green CI build
│
├── RaceDayAPI/ # Part 2 — Web API project
│ ├── Controllers/
│ ├── Data/
│ ├── DTOs/
│ ├── Middleware/
│ ├── Migrations/
│ ├── Models/
│ ├── Services/
│ ├── Program.cs
│ └── appsettings.json
│
├── RaceDayAPI.Tests/ # Part 2 — xUnit test project
│ ├── TestDbFactory.cs
│ ├── FakeSessionService.cs
│ ├── AuthControllerTests.cs
│ ├── EventsControllerTests.cs
│ └── EnrolmentsControllerTests.cs
│
├── .github/workflows/ci.yml # GitHub Actions CI
├── .gitignore
├── README.md
└── RaceDayAPI.sln

text

---

## Part 1 — Planning & Database

Before writing any API code, the full system was planned and documented in the `/docs` folder.

### Deliverables

| Document | Location | Description |
|---|---|---|
| Entity Relationship Diagram | `docs/erd.png` | All 6 entities, their attributes, primary keys, foreign keys, and cardinality. |
| API Endpoint Plan | `docs/endpoint-plan.md` | Full table of every endpoint with method, route, description, role, request body, and expected response. |
| SQL Database Script | `docs/raceday-schema.sql` | `CREATE TABLE` statements for every entity, all constraints, and seed data. |

### Database Entities

1. **User** — Organisers and Participants.
2. **EventType** — Run, Walk, or Cycle.
3. **Event** — Created and managed by an Organiser.
4. **Category** — Age or distance categories for each event.
5. **Enrolment** — A Participant's entry into an event category.
6. **Result** — Finish time and position recorded for an enrolment.

### Running the SQL Script

Open `docs/raceday-schema.sql` in SSMS, connect to your SQL Server instance, and execute. It will create `RaceDayDB`, all tables, all constraints, and seed data (2 Organisers, 2 Participants, 3 Events, categories, enrolments, results).

---

## Part 2 — The API

The API was built to match the Part 1 endpoint plan exactly, using:
- ASP.NET Core 8 Web API
- Entity Framework Core 8 (Code First)
- SQL Server / SQL Server LocalDB
- BCrypt.Net for password hashing
- Session-based authentication (`HttpOnly` cookie)
- Swagger / Swashbuckle for documentation
- xUnit for unit tests
- GitHub Actions for CI/CD

---

## Getting Started

### Prerequisites

- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET 8 SDK
- SQL Server LocalDB (comes with Visual Studio) or SQL Server Express
- SQL Server Management Studio (optional)

### Setup

1. **Clone the repository**

   ```bash
   git clone [your-repo-url]
   cd RaceDayAPI
Confirm the connection string

Open RaceDayAPI/appsettings.json and match it to the LocalDB instance where RaceDayDB lives:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\ProjectModels;Database=RaceDayDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
If your DB lives on MSSQLLocalDB instead of ProjectModels, change accordingly.

Apply the database migration

Open Tools → NuGet Package Manager → Package Manager Console

Set Default project to RaceDayAPI

Run:

powershell
Update-Database
Run the API

Press F5 in Visual Studio, or from a terminal:

bash
dotnet run --project RaceDayAPI
Swagger opens at https://localhost:7102/swagger.

Running the Tests
In Visual Studio: Test → Test Explorer → Run All Tests

Or from the terminal:

bash
dotnet test
Tests cover registration, login, event management, role enforcement, and enrolment scenarios. All must pass locally and in CI.

API Endpoints
All routes are prefixed with /api. Roles: Public = no auth, Any = any authenticated user, Organiser / Participant = role-restricted.

Authentication
Method	Route	Description	Role
POST	/api/auth/register	Register a new user	Public
POST	/api/auth/login	Log in and create a session	Public
POST	/api/auth/logout	End the current session	Any
GET	/api/auth/me	Get the currently authenticated user	Any
Users
Method	Route	Description	Role
GET	/api/users/profile	Get the current user's profile	Any
PUT	/api/users/profile	Update the current user's profile	Any
Events
Method	Route	Description	Role
GET	/api/events	List all events	Public
GET	/api/events/{id}	Get one event by ID	Public
POST	/api/events	Create a new event	Organiser
PUT	/api/events/{id}	Update an event (owner only)	Organiser
DELETE	/api/events/{id}	Delete an event (owner only)	Organiser
Categories
Method	Route	Description	Role
GET	/api/events/{eventId}/categories	List categories for an event	Public
POST	/api/events/{eventId}/categories	Add a category to an event	Organiser
DELETE	/api/events/{eventId}/categories/{categoryId}	Delete a category	Organiser
Enrolments
Method	Route	Description	Role
POST	/api/enrolments	Enrol the current participant in an event	Participant
GET	/api/enrolments/mine	List own enrolments	Participant
GET	/api/enrolments/event/{eventId}	List all enrolments for one of the organiser's events	Organiser
DELETE	/api/enrolments/{id}	Cancel own enrolment	Participant
Results
Method	Route	Description	Role
POST	/api/results	Capture a result for an enrolment	Organiser
GET	/api/results/mine	List own results	Participant
GET	/api/results/event/{eventId}	List all results for one of the organiser's events	Organiser
Full request/response schemas are visible in Swagger UI at https://localhost:7102/swagger.

CI/CD
The GitHub Actions workflow in .github/workflows/ci.yml runs on every push and pull request to main or part2. It:

Restores NuGet packages

Builds the solution in Release mode

Runs all unit tests

CI Status:

https://github.com/[your-username]/[your-repo]/actions/workflows/ci.yml/badge.svg

https://docs/ci-screenshot.png

The screenshot of the successful green build is saved at docs/ci-screenshot.png.

Demo Videos
Part	Video	Contents
Part 1	Watch on YouTube	Walk through the ERD, endpoint plan, and live SQL script run in SSMS.
Part 2	Watch on YouTube	Running API, Swagger, authentication, role-based access, and unit tests.
Part 3	Coming soon	MVC front end, Azure Blob Storage, and Docker.
Author
Student Name: [Your Full Name]

Student Number: [Your Student Number]

Module: PROG6212

Institution: The Independent Institute of Education (IIE)

AI Usage Note
AI assistance (GitHub Copilot / ChatGPT) was used for [describe briefly — e.g., "code scaffolding, debugging guidance, and documentation drafting"]. All code was reviewed, understood, tested, and adapted by the author before submission.

text

---

## Before you commit — replace these

| Placeholder | Replace with |
|---|---|
| `[your-repo-url]` | Your GitHub repo clone URL |
| `[your-username]/[your-repo]` | Your GitHub username + repo name |
| `[part1-youtube-link]` | Your unlisted Part 1 video URL |
| `[part2-youtube-link]` | Your unlisted Part 2 video URL |
| `[Your Full Name]` | Your full name |
| `[Your Student Number]` | Your student number |
| AI usage note | One honest sentence about how you used AI |

## Make sure these files exist before committing

- `docs/erd.png` — your ERD image
- `docs/endpoint-plan.md` — your endpoint plan (Markdown)
- `docs/raceday-schema.sql` — your SQL script (rename from `CREATE DATABASE RaceDayDB;.txt`)
- `docs/ci-screenshot.png` — screenshot of the green CI build

If any are missing, create them first (even a simple placeholder will do for the SQL — you already have it).

## Then commit

In VS → **Git Changes** → stage `README.md` + the entire `docs/` folder → message:
