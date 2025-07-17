# RPSLS Game Service

A backend service for playing "Rock, Paper, Scissors, Lizard, Spock" (RPSLS) with user management, scoreboards, and RESTful API.  
Built with .NET 8 and C# 12 using Clean Architecture.

## Features

- Play RPSLS against a computer opponent
- User account CRUD (SQLite persistence)
- Per-user scoreboard (last 10 rounds)
- RESTful API endpoints
- OpenAPI/Swagger documentation
- Unit tests for game logic and user management

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) [SQLite CLI](https://www.sqlite.org/download.html) for DB inspection

### Setup

1. Clone the repository:
    ```sh
    git clone <your-repo-url>
    cd <repo-folder>
    ```

2. Restore dependencies:
    ```sh
    dotnet restore
    ```

3. Build the solution:
    ```sh
    dotnet build
    ```

4. Run the API:
    ```sh
    dotnet run --project RPSLS.API
    ```

5. Open [https://localhost:5001/swagger](https://localhost:5001/swagger) for API docs.

### Running Tests


## API Endpoints

### Game

- `GET /api/choices`  
  Returns all possible choices.

- `GET /api/choice`  
  Returns a random choice.

- `POST /api/play`  
  Play a round.  
  **Body:**
  
  
- `GET /api/scoreboard?userId={guid}`  
  Get last 10 results for a user.

- `POST /api/scoreboard/reset?userId={guid}`  
  Reset a user's scoreboard.

### Users

- `GET /api/users`  
  List all users.

- `GET /api/users/{id}`  
  Get user by Id.

- `POST /api/users`  
  Create user.  
  **Body:**
  
  
- `DELETE /api/users/{id}`  
  Delete user.

## Project Structure

## Docker

To build and run the project in Docker:
