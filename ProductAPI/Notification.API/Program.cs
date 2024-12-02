using Notification.Application;
using Notification.Infrastructure;
using Notification.Infrastructure.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI Infrastructure Layer
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGrpcService<NotificationServiceGRPC>();

app.MapGet("/", () => "This is a gRPC server.");
app.UseAuthorization();

app.MapControllers();

app.Run();
