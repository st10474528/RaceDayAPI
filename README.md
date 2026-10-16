RaceDay Event Management System - Part 1
📋 Table of Contents
System Overview

Features

User Roles

Documentation

Database Schema

Setup Instructions

CI/CD Pipeline

Video Presentation

GitHub Commit History

Author Information

🏃 System Overview
RaceDay is a comprehensive full-stack web-based event management system designed specifically for the South African road running, walking, and cycling community. The platform bridges the gap between event organisers and participants, digitizing the entire event lifecycle from creation to results tracking.

Key Statistics
8 Database Entities supporting a robust data model

27 API Endpoints planned for complete functionality

2 User Roles with distinct responsibilities

3 Sample Events pre-populated for testing

✨ Features
For Event Organisers
✅ Create, edit, and delete events

✅ Define event categories (age groups, distances)

✅ Capture participant results

✅ View all event enrolments

✅ Manage event weather information

✅ Generate event leaderboards

✅ Track event statistics

For Participants
✅ Create account and manage profile

✅ Browse upcoming events with filters

✅ Enrol in events by selecting categories

✅ View personal enrolment history

✅ Track personal results and performance

✅ Cancel enrolments if needed

👥 User Roles
🎯 Organiser
Capability	Description
Event Management	Full CRUD operations on events
Category Management	Define age groups, distances, and fees
Enrolment Viewing	See all participants registered for their events
Results Capture	Record finish times and positions
Weather Updates	Manage weather information for events
Statistics	View event analytics and reports
🏅 Participant
Capability	Description
Profile Management	Update personal information
Event Discovery	Search and filter upcoming events
Event Enrolment	Register for events and select categories
History Tracking	View past and current enrolments
Results Access	View personal race results and performance
Enrolment Management	Cancel enrolments if necessary
📚 Documentation
All planning documents are located in the /docs folder of the repository:

Document	Description	Format
ERD Diagram	Entity Relationship Diagram showing 8 entities with relationships	PNG
API Endpoint Plan	Complete API specification with 27 endpoints	Markdown/PDF
Database Script	SQL Server script with schema and sample data	SQL
Database Entity Breakdown
Users - Authentication and base user information

Organisers - Event organiser details (extends Users)

Participants - Participant details (extends Users)

Events - Core event management

Categories - Event categories (age/distances)

EventEnrolments - Participant registration linking

Results - Race performance data

Weather - Event weather conditions

🗄️ Database Schema
Entity Relationship Diagram
text
Users (1) ─── (1) Organisers
   │
   └─── (1) Participants
         │
Events (1) ─── (M) Categories (1) ─── (M) EventEnrolments
   │                                          │
   └─── (1) Weather                           └─── (1) Results
Key Relationships
One-to-One: Users → Organisers, Users → Participants, Events → Weather, EventEnrolments → Results

One-to-Many: Organisers → Events, Events → Categories, Categories → EventEnrolments, Participants → EventEnrolments

Sample Data Overview
2 Organisers: Cape Town Marathon Events, Soweto Sports Association

2 Participants: Michael Brown, Lisa Davis

3 Events: Cape Town Marathon, Table Mountain Challenge, Soweto Community Walk

10 Categories: Various distances and age groups

3 Enrolments: Participants registered for events

2 Results: Completed race results

2 Weather Records: Weather conditions for events

🚀 Setup Instructions
Prerequisites
SQL Server (2019 or later)

SQL Server Management Studio (SSMS)

Git (for version control)

GitHub Account

Step 1: Clone the Repository
bash
git clone https://github.com/yourusername/RaceDay.git
cd RaceDay
Step 2: Review Documentation
Navigate to the /docs folder to review:

ERD.png - Database design visualization

API-Endpoint-Plan.md - Complete endpoint specification

RaceDay-Database.sql - Database creation script

Step 3: Setup Database
Open SQL Server Management Studio (SSMS)

Connect to your SQL Server instance

Open docs/RaceDay-Database.sql

Run the entire script (press F5)

Verify the database RaceDayDB is created with all tables

Step 4: Verify Installation
Run this query in SSMS to confirm setup:

sql
USE RaceDayDB;
GO
SELECT 
    (SELECT COUNT(*) FROM Users) AS Users,
    (SELECT COUNT(*) FROM Organisers) AS Organisers,
    (SELECT COUNT(*) FROM Participants) AS Participants,
    (SELECT COUNT(*) FROM Events) AS Events,
    (SELECT COUNT(*) FROM Categories) AS Categories,
    (SELECT COUNT(*) FROM EventEnrolments) AS Enrolments;
Expected Output:

text
Users | Organisers | Participants | Events | Categories | Enrolments
------|------------|--------------|--------|------------|-----------
4     | 2          | 2            | 3      | 10         | 3
🔄 CI/CD Pipeline
GitHub Actions Workflow
The project includes a GitHub Actions workflow that validates the documentation structure:

yaml
name: Validate Documentation

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  validate-docs:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Check /docs folder exists
        run: test -d docs
      - name: Check ERD exists
        run: test -f docs/ERD.png || test -f docs/ERD.pdf
      - name: Check API plan exists
        run: test -f docs/API-Endpoint-Plan.md || test -f docs/API-Endpoint-Plan.pdf
      - name: Check SQL script exists
        run: test -f docs/RaceDay-Database.sql
      - name: Validate SQL script
        run: grep -q "CREATE TABLE" docs/RaceDay-Database.sql
CI/CD Status
https://github.com/yourusername/RaceDay/actions/workflows/validate-docs.yml/badge.svg

Build Requirements:

✅ All documentation files present

✅ SQL script contains CREATE TABLE statements

✅ Repository structure validated

✅ Green build status achieved

🎥 Video Presentation
https://youtu.be/_S5EybEQP-M
Part 1 Video Walkthrough
Watch the complete walkthrough of Part 1, including:

📊 Explanation of the ERD design decisions

📝 API endpoint plan overview

💻 Live SQL script execution in SSMS

🗄️ Database verification and testing

Click here to watch the Part 1 Video Presentation

Note: Video is unlisted and only accessible via this link.

What to Expect in the Video
Introduction (0:00 - 2:00)

System overview and project goals

Explanation of South African events context

ERD Walkthrough (2:00 - 8:00)

Entity relationship explanation

Cardinality decisions

Data model design choices

API Plan Review (8:00 - 15:00)

Endpoint structure explanation

Role-based access control design

Request/Response patterns

Database Script Live (15:00 - 25:00)

Running the SQL script in SSMS

Verifying table creation

Viewing sample data

Testing constraints and relationships

Conclusion (25:00 - 27:00)

Summary of Part 1

Preview of Part 2

Q&A and closing remarks

📊 GitHub Commit History
Commit Requirements for Part 1
Minimum: 20 meaningful commits

Valid Commits:

✅ Adding ERD diagram

✅ Creating API endpoint plan

✅ Writing SQL script

✅ Creating README file

✅ Setting up CI/CD workflow

✅ Adding documentation

✅ Updating design decisions

Invalid Commits (don't count):

❌ "fixed typo"

❌ "updated file"

❌ "minor changes"

❌ Commits with no meaningful code changes

Recommended Commit Structure
bash
# Initial structure
git add .github/workflows/validate-docs.yml
git commit -m "Add GitHub Actions workflow for doc validation"

# Documentation
git add docs/ERD.png
git commit -m "Add Entity Relationship Diagram with 8 entities"

git add docs/API-Endpoint-Plan.md
git commit -m "Add API endpoint plan with 27 endpoints"

git add docs/RaceDay-Database.sql
git commit -m "Add SQL database script with schema and sample data"

# README
git add README.md
git commit -m "Add comprehensive README with setup instructions"

# Final verification
git commit -m "Finalize Part 1 documentation and structure"
📁 Repository Structure
text
RaceDay/
├── .github/
│   └── workflows/
│       └── validate-docs.yml      # CI/CD workflow
├── docs/
│   ├── ERD.png                    # Entity Relationship Diagram
│   ├── API-Endpoint-Plan.md       # API endpoint specification
│   └── RaceDay-Database.sql       # SQL Server database script
├── README.md                      # This file
└── .gitignore                     # Git ignore configuration
🛠️ Technologies Used
Technology	Purpose
SQL Server	Relational database management
SSMS	Database development and management
Git	Version control
GitHub	Repository hosting and CI/CD
GitHub Actions	Continuous Integration
Markdown	Documentation formatting
PNG/PDF	ERD diagram format
🔜 What's Next?
Part 2 - API Development
Build RESTful API in C#

Connect to the RaceDay database

Implement all 27 endpoints

Write unit tests

Deploy with GitHub CI/CD

Part 3 - MVC Web Application
Build MVC web application

Consume the API

Integrate Azure Blob Storage

Containerize with Docker

📝 Submission Requirements
What to Submit
✅ GitHub repository link on ARC

✅ /docs folder containing:

ERD image (PNG or PDF)

API endpoint plan (Markdown or PDF)

SQL script (SQL)

✅ README.md with:

System description

Role descriptions

CI/CD screenshot

<img width="1438" height="401" alt="Screenshot 2026-09-04 194151" src="https://github.com/user-attachments/assets/4870b3ac-f440-41bb-a4c1-a3ea1a8e69d1" />


YouTube video link


# RaceDay API — Part 2

A RESTful API for the RaceDay event management system, built with ASP.NET Core 8,
Entity Framework Core, and SQL Server. Designed for the South African road running,
walking, and cycling community.

---

## System Description

RaceDay allows Event Organisers to create and manage events, categories, and
participant results, while Participants can browse events, enter events by
category, track their personal enrolments, and view their race history.

This repository contains **Part 2** — the RESTful API that powers the entire
RaceDay platform. The MVC front end will be built in Part 3.

---

## Roles

| Role | Capabilities |
|---|---|
| **Organiser** | Create, edit, and delete events; manage categories; view all enrolments for their events; capture participant results |
| **Participant** | Register and log in; browse events; enrol in an event by selecting a category; view own enrolments; view own race results; update their profile |

Role-based access is enforced at the API level via a custom `RoleFilter` that
returns **403 Forbidden** when the logged-in user's role does not match the
endpoint's requirement.

---

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8 (Code First)
- SQL Server / SQL Server LocalDB
- BCrypt.Net for password hashing
- Session-based authentication (`HttpOnly` cookie)
- Swagger / Swashbuckle for API documentation
- xUnit for unit tests
- GitHub Actions for CI/CD

---

## Getting Started

### Prerequisites

- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET 8 SDK
- SQL Server LocalDB (comes with Visual Studio) or SQL Server Express
- SQL Server Management Studio (optional, for inspection)

### Setup

1. **Clone the repository**

   ```bash
   git clone <your-repo-url>
   cd RaceDayAPI

📄 License
This project is created for educational purposes as part of the [Course Name] Portfolio of Evidence.

🙏 Acknowledgments
South African road events community for inspiration

Lecturers and course instructors for guidance

SQL Server documentation resources

© 2026 RaceDay Event Management System - All Rights Reserved

Last Updated: September 2026

🎯 Quick Links
Repository

Video Presentation

CI/CD Status

Issue Tracker
# RaceDay — Parts  2

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
