using ClassLibrary;
using ClassLibrary.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);





builder.Services.AddControllers();
//add database
builder.Services.AddDbContext<DbHorse>(o =>
{
    o.UseNpgsql("Server=localhost;Port=5432;Database=Horse;User Id=postgres;Password=Acab1532;Timeout=15;");
});
var serviceCollection = builder.Services;

builder.Services.AddEndpointsApiExplorer();
serviceCollection.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Horse.Api", Version = "v1" });
    var securitySchema = new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
 
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
 
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(x => { x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
await using (var asyncServiceScope = app.Services.CreateAsyncScope())
{
    var dbContext = asyncServiceScope.ServiceProvider.GetService<DbHorse>()!;
    
    await dbContext.Database.MigrateAsync();
}
app.Run();