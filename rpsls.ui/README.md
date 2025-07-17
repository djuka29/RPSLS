# RPSLS UI – Rock Paper Scissors Lizard Spock

This project is a React-based UI for playing Rock Paper Scissors Lizard Spock against a computer, with user authentication and a scoreboard.  
It is designed to work seamlessly with Docker and Docker Compose for easy setup.

---

## 🚀 Quick Start (with Docker)

### 1. **Clone the Repository**

```sh
git clone https://github.com/yourusername/rpsls.git
cd rpsls
```

### 2. **Project Structure**

```
/RPSLS.API        # ASP.NET Core API project folder
/rpsls.ui         # React UI project folder
/Dockerfile       # API Dockerfile (in root)
/docker-compose.yml
```

### 3. **Configure Environment Variables**

- The UI uses a `.env` file for the API base URL.
- Make sure `.env` is in the `rpsls.ui` folder (not in `src/`):

  ```
  rpsls.ui/.env
  ```

  Example content:
  ```
  REACT_APP_API_BASE=http://api:80/api
  ```

  > **Note:** When running locally (not in Docker), set `REACT_APP_API_BASE=http://localhost:5001/api`.

### 4. **Build and Run with Docker Compose**

From the project root, run:

```sh
docker-compose up --build
```

- This will build and start both the API and UI containers.
- The first build may take a few minutes.

### 5. **Access the Application**

- **UI:** [http://localhost:3000](http://localhost:3000)
- **API:** [http://localhost:5001/api](http://localhost:5001/api) (for testing API endpoints)

---

## 🐳 Docker Compose Overview

Your `docker-compose.yml` should look like:

```yaml
version: '3.8'
services:
  api:
    build:
      context: .      # Root folder, where API Dockerfile is
      dockerfile: Dockerfile
    ports:
      - "5001:80"
    environment:
      - ASPNETCORE_URLS=http://+:80
  ui:
    build:
      context: ./rpsls.ui
    ports:
      - "3000:80"
    depends_on:
      - api
    environment:
      - REACT_APP_API_BASE=http://api:80/api
```

---

## 🛠️ Useful Commands

- **Start containers:**  
  `docker-compose up --build`

- **Stop containers:**  
  Press `Ctrl+C` in the terminal, or run  
  `docker-compose down`

- **Rebuild after code changes:**  
  `docker-compose up --build`

---

## 📝 Notes

- The UI will not work if the `.env` file is missing or in the wrong place.
- The API and UI communicate over a Docker network using service names (`api`).
- For local development (without Docker), set `REACT_APP_API_BASE` to `http://localhost:5001/api` in `rpsls.ui/.env` and run the API separately.

---

## 📦 Requirements

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) or Docker Engine (Linux)
- (Optional) [Node.js](https://nodejs.org/) and [.NET SDK](https://dotnet.microsoft.com/download) for local development

---

## 🙋‍♂️ Troubleshooting

- **API calls go to `/undefined/...` or `/localhost:3000/undefined/...`**  
  → Your `.env` is missing or in the wrong place, or you did not rebuild after changing it.

- **UI or API not reachable?**  
  → Make sure Docker is running and ports 3000/5001 are not blocked.

---

## 📚 More

- UI: React + Material UI (`rpsls.ui`)
- API: ASP.NET Core (`RPSLS.API`)
- Docker Compose for orchestration

---

**Enjoy playing Rock Paper Scissors Lizard Spock