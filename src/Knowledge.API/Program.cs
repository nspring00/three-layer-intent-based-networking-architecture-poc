using Common.Web.AspNetCore;
using Knowledge.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.CheckEnforceHttp2();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registration of custom services
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGrpcReflectionService();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
app.MapGrpcServices();

app.Run();
