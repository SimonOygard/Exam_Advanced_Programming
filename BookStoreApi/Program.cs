using BookStoreApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("BookStoreDb")
         ?? throw new InvalidOperationException("Missing connection string 'BookStoreDb' in appsettings.json");

var handler = new DatabaseHandler(connectionString);
handler.EnsureDatabaseSetup();

var app = builder.Build();

app.MapGet("/", () => "BookStore API kjører. Se /swagger og bruk /books.");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/books", (string? title, string? author, int? publicationYear) =>
{
    var books = handler.GetBooksFiltered(title, author, publicationYear);
    return Results.Ok(books);
});

app.MapGet("/books/{id:int}", (int id) =>
    handler.GetBookById(id) is { } book ? Results.Ok(book) : Results.NotFound());

app.MapPost("/books", (Book book) =>
{
    if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
        return Results.BadRequest("Title og Author kan ikke være tomme.");

    var id = handler.CreateBook(book);
    return Results.Created($"/books/{id}", new { id });
});

app.MapPut("/books/{id:int}", (int id, Book book) =>
{
    if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
        return Results.BadRequest("Title og Author kan ikke være tomme.");

    book.Id = id;
    return handler.UpdateBook(book) ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/books/{id:int}", (int id) =>
    handler.DeleteBook(id) ? Results.NoContent() : Results.NotFound());

app.Run();