using AADYA_HouseOfFragrance.Data;
using AADYA_HouseOfFragrance.Helpers;
using AADYA_HouseOfFragrance.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. DATABASE CONTEXT
// ============================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ============================================================
// 2. DEPENDENCY INJECTION (Services)
// ============================================================

builder.Services.AddScoped<JwtHelper>();       // JWT token generation and validation helper
builder.Services.AddScoped<EmailService>();    // SMTP email service for notifications
builder.Services.AddHttpContextAccessor();     // Access to HttpContext for dynamic URL generation in emails

// ============================================================
// 3. SESSION STATE
// ============================================================

builder.Services.AddDistributedMemoryCache();  // In-memory cache for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expires after 30 minutes of inactivity
    options.Cookie.HttpOnly = true;                 // Prevent client-side script access to the cookie
    options.Cookie.IsEssential = true;              // Allow session cookie even if user hasn't consented
});

// ============================================================
// 4. JWT AUTHENTICATION
// ============================================================

// Retrieve and validate the JWT signing key from configuration
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is missing from appsettings.json. Please add 'Jwt:Key' with a secure 32+ character value.");
}
var key = Encoding.UTF8.GetBytes(jwtKey);

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;   // Set to true in production with HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Tokens expire exactly at the expiry time, no grace period
    };
});

// ============================================================
// 5. CORS (Cross-Origin Resource Sharing)
// ============================================================

// Read allowed origins from configuration or use default localhost values.
// This allows the frontend to call the API from different origins.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5000", "https://localhost:5001" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Allow cookies/session to be sent with CORS requests
    });
});

// ============================================================
// 6. CONTROLLERS & JSON SERIALIZATION
// ============================================================

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// ============================================================
// BUILD THE APPLICATION
// ============================================================

var app = builder.Build();

// ============================================================
// 7. HTTP PIPELINE MIDDLEWARE
// ============================================================

// Configure error handling and HTTPS for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection();      // Redirect HTTP requests to HTTPS
app.UseStaticFiles();           // Serve static files (CSS, JS, images)
app.UseRouting();               // Route endpoint selection

// Session must come AFTER UseRouting and BEFORE UseAuthorization
app.UseSession();

app.UseCors("AllowFrontend");   // Enable CORS for cross-origin requests

// Authentication must come BEFORE Authorization
app.UseAuthentication();       // Identify users
app.UseAuthorization();        // Check permissions

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();