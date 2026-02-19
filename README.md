## How to run the project

1. Create a MySQL database named `bookstore`
2. Update the connection string in `appsettings.json` with your own MySQL credentials
3. Start the project
4. The application will automatically create the `books` table on startup
5. Open `/swagger` to test the endpoints

Make sure you have these NuGet packages installed:

- dotnet add package MySql.Data
- dotnet add package Swashbuckle.AspNetCore

---

## Testing

Here are two JSON objects you can use to add a book:

```json
{
  "title": "The Hobbit",
  "author": "J.R.R. Tolkien",
  "publicationYear": 1937,
  "isbn": "978-0547928227",
  "inStock": 5
}

{
  "title": "1984",
  "author": "George Orwell",
  "publicationYear": 1949,
  "isbn": "978-0451524935",
  "inStock": 8
}
