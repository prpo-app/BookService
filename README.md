# BookService
C# API microservice for BookWorm application.
## Features
- Manage books (add, delete, view)
- Search books by author, or genre
- User authentication and authorization
- RESTful API for external integrations
- Pagination and filtering support for book listings

## Tech Stack
- **Backend**: ASP.NET Core
- **Database**: PostgreSQL
- **Authentication**: JSON Web Tokens (JWT)
- **Documentation**: Swagger
- **Other**: Docker, Entity Framework Core

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Docker (optional, for containerized setup)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/BookService.git
   cd BookService
   ```

2. Set up the database:
   - Update the connection string in `appsettings.json` to match your PostgreSQL setup.

3. Run the application:
   ```bash
   dotnet run
   ```

4. Access the application:
   - API: `http://localhost:<port>`
   - Swagger UI: `http://localhost:<port>/swagger/index.html`

### Docker Setup (Optional)
1. Build the Docker image:
   ```bash
   docker build -t bookservice .
   ```

2. Run the container:
   ```bash
   docker run -p <port>:<port> bookservice
   ```

3. Access the application at `http://localhost:<port>`.

## API Endpoints

### Public Endpoints
- `GET /book/all`: Retrieve all books. (Testing purposes)
- `GET /book/{id}`: Retrieve a book by its ID.
- `GET /book`: Retrieve books with optional pagination and filtering.

### Admin Endpoints
- `POST /book`: Add a new book (Admin only).
- `DELETE /book/{id}`: Delete a book by its ID (Admin only).
