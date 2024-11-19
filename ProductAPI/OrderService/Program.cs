using Microsoft.EntityFrameworkCore;
using OrderService;
using OrderService.Profiles;
using OrderService.Repositories.Interfaces;
using OrderService.Repositories.Implementations;
using OrderService.Services;
using ProductDataAccess.Models;
using OrderService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ProductCategoryContext>(options =>
    options.UseSqlServer("Server=.;Database=Product_Category;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddHttpContextAccessor();
//Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//DI container
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<OrderConsumer>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHostedService<OrderConsumerBackgroundService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
