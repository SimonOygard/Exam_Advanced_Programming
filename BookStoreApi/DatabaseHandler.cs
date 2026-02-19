using MySql.Data.MySqlClient;

namespace BookStoreApi
{
    public class DatabaseHandler
    {
        private readonly string _connectionString;

        public DatabaseHandler(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
        
        public void EnsureDatabaseSetup()
        {
            const string sql = @"
        CREATE TABLE IF NOT EXISTS books (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Title VARCHAR(255) NOT NULL,
            Author VARCHAR(255) NOT NULL,
            PublicationYear INT,
            ISBN VARCHAR(20),
            InStock INT DEFAULT 0
        );";
            
            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand(sql, sqlConnection);
            sqlCommand.ExecuteNonQuery();
        }

        public int CreateBook(Book book)
        {
            const string sql = @"
                INSERT INTO books (Title, Author, PublicationYear, ISBN, InStock)
                VALUES (@Title, @Author, @PublicationYear, @ISBN, @InStock);";

            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Title", book.Title);
            sqlCommand.Parameters.AddWithValue("@Author", book.Author);
            sqlCommand.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
            sqlCommand.Parameters.AddWithValue("@ISBN", book.ISBN);
            sqlCommand.Parameters.AddWithValue("@InStock", book.InStock);

            sqlCommand.ExecuteNonQuery();
            return (int)sqlCommand.LastInsertedId;
        }
        
        public Book? GetBookById(int id)
        {
            const string sql = "SELECT Id, Title, Author, PublicationYear, ISBN, InStock FROM books WHERE Id = @Id;";

            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", id);

            using var reader = sqlCommand.ExecuteReader();
            if (!reader.Read()) return null;

            return new Book
            {
                Id = reader.GetInt32("Id"),
                Title = reader.GetString("Title"),
                Author = reader.GetString("Author"),
                PublicationYear = reader.IsDBNull(reader.GetOrdinal("PublicationYear")) ? 0 : reader.GetInt32("PublicationYear"),
                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? "" : reader.GetString("ISBN"),
                InStock = reader.IsDBNull(reader.GetOrdinal("InStock")) ? 0 : reader.GetInt32("InStock")
            };
        }
        
        public List<Book> GetBooksFiltered(string? title, string? author, int? publicationYear)
        {
            var sql = @"
        SELECT Id, Title, Author, PublicationYear, ISBN, InStock
        FROM books
        WHERE 1=1";

            var bookList = new List<Book>();

            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand();
            sqlCommand.Connection = sqlConnection;

            if (!string.IsNullOrWhiteSpace(title))
            {
                sql += " AND Title LIKE @Title";
                sqlCommand.Parameters.AddWithValue("@Title", $"%{title}%");
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                sql += " AND Author LIKE @Author";
                sqlCommand.Parameters.AddWithValue("@Author", $"%{author}%");
            }

            if (publicationYear.HasValue)
            {
                sql += " AND PublicationYear = @Year";
                sqlCommand.Parameters.AddWithValue("@Year", publicationYear.Value);
            }

            sql += " ORDER BY Id;";
            sqlCommand.CommandText = sql;

            using var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                bookList.Add(new Book
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Author = reader.GetString("Author"),
                    PublicationYear = reader.IsDBNull(reader.GetOrdinal("PublicationYear")) ? 0 : reader.GetInt32("PublicationYear"),
                    ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? "" : reader.GetString("ISBN"),
                    InStock = reader.IsDBNull(reader.GetOrdinal("InStock")) ? 0 : reader.GetInt32("InStock")
                });
            }

            return bookList;
        }
        
        public bool UpdateBook(Book book)
        {
            const string sql = @"
                UPDATE books
                SET Title=@Title, Author=@Author, PublicationYear=@PublicationYear, ISBN=@ISBN, InStock=@InStock
                WHERE Id=@Id;";

            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", book.Id);
            sqlCommand.Parameters.AddWithValue("@Title", book.Title);
            sqlCommand.Parameters.AddWithValue("@Author", book.Author);
            sqlCommand.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
            sqlCommand.Parameters.AddWithValue("@ISBN", book.ISBN);
            sqlCommand.Parameters.AddWithValue("@InStock", book.InStock);

            var affected = sqlCommand.ExecuteNonQuery();
            return affected == 1;
        }
        
        public bool DeleteBook(int id)
        {
            const string sql = "DELETE FROM books WHERE Id=@Id;";

            using var sqlConnection = GetConnection();
            sqlConnection.Open();

            using var sqlCommand = new MySqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Id", id);

            var affected = sqlCommand.ExecuteNonQuery();
            return affected == 1;
        }
    }
}