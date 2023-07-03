using Microsoft.Data.SqlClient;
using OrderProject;
using OrderProject.Extension;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.AddSingleton<Configuration>();
    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddSqlConnection(connString);
    builder.Services.AddRepositories();
    builder.Services.AddServices();
}

var app = builder.Build();
{
    app.MapControllers();
    app.Run();
}