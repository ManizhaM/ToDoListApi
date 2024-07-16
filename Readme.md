# ToDoListApi

## Description

ToDoListApi is a web api for task manager.

## Features

- **Task Management:** Create, update, and delete tasks with due dates and priorities.
- **Priority Management:** Assign priorities to tasks for better organization.
- **User Management:** Add users, and manage their tasks.
- **Filtering:** Filter tasks by completion status, priority, and date range.

## Technologies Used

- **Backend:** C#, ASP.NET Core Web API, Entity Framework Core, PostgreSQL
- **Database:** PostgreSQL
- **Tools:** Visual Studio, Postman

## Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ManizhaM/ToDoListApi.githttps://github.com/ManizhaM/ToDoListApi.git
   cd ToDoListApi

## Database Setup:

1. Install PostgreSQL and create a new database 
2. Update the connection string in appsettings.json with your database credentials

## Run Migrations:

Open Package Manager Console in Visual Studio
Run the following commands: Update-Database

## Run the Application:

Press F5 in Visual Studio to build and run the application.
Navigate to https://localhost:<port> in your web browser.

## API Documentation:

Swagger UI is available at https://localhost:<port>/swagger for exploring and testing API endpoints.

## Postman Collection:

Import the Postman collection ToDoList.postman_collection.json located in the Postman folder to test API endpoints

## Testing:

Use Postman or any API testing tool to test operations for tasks and user management

## Troubleshooting
If you encounter issues with migrations or database connectivity, 
ensure your PostgreSQL server is running and configured correctly.
