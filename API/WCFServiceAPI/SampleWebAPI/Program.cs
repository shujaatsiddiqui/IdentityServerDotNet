using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options => { options.AddPolicy("AllowAngularApp", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:44313/";  // Your authority URL
        options.Audience = "vcmsAPI";  // Your API client ID (audience)
        options.RequireHttpsMetadata = false;  // If testing locally, otherwise set it to true
    });

// Add services to your Web API
builder.Services.AddControllers();
builder.Services.AddControllers();
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
app.UseRouting();
app.UseCors("AllowAngularApp");

app.UseAuthentication();  // Enable authentication
app.UseAuthorization();   // Enable authorization

app.MapControllers();

app.Run();
