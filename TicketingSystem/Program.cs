using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var endPoint = builder.Configuration["CosmosDB:AccountEndpoint"];
    var key = builder.Configuration["CosmosDB:AccountKey"];
    var database = builder.Configuration["CosmosDB:Database"];
    var cosmosClient = new CosmosClient(endPoint, key);
    Task.WaitAll(cosmosClient.CreateDatabaseIfNotExistsAsync(database));
    var db = cosmosClient.GetDatabase(database);

    List<Task> tasks = new List<Task>();
    tasks.Add(db.DefineContainer("Venues", "/Name").CreateIfNotExistsAsync());
    tasks.Add(db.DefineContainer("Events", "/VenueID").CreateIfNotExistsAsync());
    tasks.Add(db.DefineContainer("Tickets", "/EventId, /SectionId").CreateIfNotExistsAsync());
    Task.WaitAll(tasks.ToArray());
    return cosmosClient;

});
builder.Services.AddSingleton<Database>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var database = builder.Configuration["CosmosDB:Database"];
    return cosmosClient.GetDatabase(database);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
