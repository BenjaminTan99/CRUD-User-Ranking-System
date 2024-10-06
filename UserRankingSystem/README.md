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
   dotnet build
   dotnet run

5. **Testing the system**
   Now open a terminal or command prompt and firstly ensure that the port is visible (you can try curl 
   "http://{IP}:5043/api/users" where IP is your IP address) to test your connection to the port.

   You can now go ahead and test the system!

   The terminal or command prompt should ideally be used with Ubuntu (WSL) or Windows Powershell. See
   documentation for example commands to test the system below for more information on what commands you can use.
   Note that the commands are meant for Ubuntu (WSL) and that Windows Powershell may require a slightly different
   input.

### Example commands to test the system:
Notes: {IP} should be filled in by your personal IP address. You can find this using ipconfig or similar commands. You must follow the exact rules below for each command and only change parts where xxx appears. For better information in the terminal, you may use --verbose for better clarity. The terminal running dotnet run should automatically input information everytime a RESTful API is called.

1. **Add an entry to the database**
   curl -X POST "http://{IP}:5043/api/users" -H "Content-Type: application/json" -d '{"name": "xxx", "email": "xxx@example.com", "score": xxx}'

   e.g. curl -X POST "http://0.0.0.0:5043/api/users" -H "Content-Type: application/json" -d '{"name": "Alice", "email": "alice@example.com", "score": 120}'

2. **Get all users in the database**
   curl -X GET "http://{IP}:5043/api/users"

3. **Get user by specified ID**
   curl -X GET "http://{IP}:5043/api/users/xxx"
   e.g. curl -X GET "http://0.0.0.0:5043/api/users/1"

4. **Update user by specified ID**
   curl -X POST "http://{IP}:5043/api/users/1" -H "Content-Type: application/json" -d '{"name": "xxx", "email": "xxx@example.com", "score": xxx}'

   e.g. curl -X POST "http://0.0.0.0:5043/api/users/1" -H "Content-Type: application/json" -d '{"name": "Alice", "email": "alice_updated@example.com", "score": 150}'

5. **Delete user by specified ID**
   curl -X DELETE "http://{IP}:5043/api/users/xxx"

   e.g. curl -X DELETE "http://0.0.0.0/api/users/1"

6. **Get all users by rank**
   Note: Remember to add multiple users of different scores to see the rank!
   curl -X GET "http://{IP}:5043/api/users/rank"

7. **Get rank of user by ID**
   Note: Remember to add multiple users of different scores to see the rank!
   curl -X GET "http://{IP}:5043/api/users/rank/xxx"

   e.g. curl -X GET "http://0.0.0.0:5043/api/users/rank/1"

**Known bugs**
Currently there is an issue where the ID is not resetted to the minimum value after that ID has been deleted.
However, this bug should not affect the APIs on their own.
e.g. Adding user 1 and 2 then deleting user 1 and adding the another user will result in the new user having ID 3.

There is also a possibility of the original database being filled with some data. Please reset or create a new database before testing. 

**File structure**
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
├── README.md
└── Tests/
    └── UserControllerTests.cs
