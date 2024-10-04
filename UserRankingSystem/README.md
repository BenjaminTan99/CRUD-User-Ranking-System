This is a simple ASP.NET Core web API project for managing users with scores and providing ranking based on those scores. The API supports full CRUD operations (Create, Read, Update, Delete) and includes ranking functionalities.

To run the project, follow the below steps:

## Prerequisites
Ensure you have the following installed:
1. [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
2. [SQLite](https://www.sqlite.org/download.html) (optional, as SQLite DB will be created automatically)

### Steps to Run the Project

1. **Clone the Repository:**
   Clone or download the `UserRankingSystem` folder to your local machine.

2. **Navigate to the Project Folder:**
   Open a terminal or command prompt, and navigate to the root of the project:
   cd UserRankingSystem

3. **Restore dependencies**
   dotnet restore

4. **Run Database Migrations & Application**
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run


Below is the overall file structure of the user ranking system.

UserRankingSystem/
│
├── Controllers/
│   └── UsersController.cs
│
├── Models/
│   └── User.cs
│
├── Data/
│   └── UserRankingContext.cs
│
├── appsettings.json
├── Program.cs
├── Startup.cs
├── UserRankingSystem.csproj
├── README.md
└── Tests/
    └── UserControllerTests.cs
