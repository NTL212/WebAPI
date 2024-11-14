using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductAPI.Email;
using ProductAPI.Filters;
using ProductAPI.Profiles;
using ProductAPI.Repositories;
using ProductAPI.Repositories.Implementations;
using ProductAPI.Repositories.Interfaces;
using ProductAPI.Services;
using ProductDataAccess.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Http Client
builder.Services.AddHttpClient();

// MVC Controller
builder.Services.AddControllersWithViews();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ProductCategoryContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//DI Repository, Service
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepoisitory, UserRepository>();
builder.Services.AddScoped<IUserGroupRepository, UserGroupRepository>();
builder.Services.AddScoped<PasswordHasher<User>>();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ValidateTokenAttribute>();

//Mail
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

//Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product_Cateogry API", Version = "v1" });

    //JWT Authentication cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter the token in the format: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//Add CORS Policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin",
		policy => policy.WithOrigins("https://localhost:7291")
						.AllowAnyHeader()
						.AllowAnyMethod());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseStaticFiles();
app.MapControllers();

app.Run();
