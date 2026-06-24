using GoWebApplication.Db.Data;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using GoWeb.Mapping;
using GoWeb.Interfaces;
using GoWeb.Repositories;
using GoWeb.Service;
using Microsoft.AspNetCore.Authorization;

using GoWeb.Сonstants;
using GoWeb.Filters.Authorization;
using Serilog.Formatting.Compact;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// --- ✅ БЛОК РЕГИСТРАЦИИ СЕРВИСОВ ---

// Получаем строку подключения (должна быть доступна здесь)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. Регистрация DbContext (с MySql)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
    )
);

// 2. Регистрация Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>(); ;



builder.Services.Configure<IdentityOptions>(options =>
{
    // === ОБЩИЕ НАСТРОЙКИ СИГНАЛИЗАЦИИ ===
    options.SignIn.RequireConfirmedAccount = false; // ❌ ОТКЛЮЧАЕМ: Не требует подтвержденной учетной записи для входа.
    options.SignIn.RequireConfirmedEmail = false;    // ❌ ОТКЛЮЧАЕМ: Не требует подтвержденного Email.
    options.SignIn.RequireConfirmedPhoneNumber = false; // ❌ ОТКЛЮЧАЕМ: Не требует подтвержденного телефона.

    // === Настройки пароля (можно оставить по умолчанию, но полезно знать) ===
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // === Настройки пользователя ===
    options.User.RequireUniqueEmail = true;




    // === Настройки блокировки ===
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

// 3. Регистрация MVC/Контроллеров
builder.Services.AddControllersWithViews();

//для api
builder.Services.AddControllers();
// НАСТРОЙКА АВТОРИЗАЦИИ (КУКИ + JWT)

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login"; // Куда перенаправлять обычного пользователя для входа
})
.AddJwtBearer(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});


builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped <ICityService,CityService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventTypeRepository, EventTypeRepository>();
builder.Services.AddScoped<IEventTypeService, EventTypeService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserEventService, UserEventService>();
builder.Services.AddScoped<IStatusEvent,StatusEventRepository>();
builder.Services.AddScoped <IStatusEventService,StatusEventService>();
builder.Services.AddScoped<IUserEvent, UserEventRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<UserEventRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAutoMapper(typeof(CityProfile));       
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddSingleton<ICommandQueue,CommandQueue>();
builder.Services.AddHostedService<TimedBackgroundService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAuthorizationHandler, CheckAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CheckOrganizerHandler>();

// --- НАСТРОЙКА ЛОКАЛИЗАЦИИ ---
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var defaultCulture = "ru-RU"; // Указываем русскую локаль как основную
    var supportedCultures = new[]
    {
        new CultureInfo(defaultCulture),
        new CultureInfo("en-US") // Можете добавить другие, если нужно поддерживать английский ввод (с точкой)
    };

    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    // Добавление провайдера для определения культуры из заголовка Accept-Language
    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
// --- КОНЕЦ НАСТРОЙКИ ЛОКАЛИЗАЦИИ ---


// --- 🛑 СБОРКА И ЗАМОРОЗКА КОНТЕЙНЕРА СЕРВИСОВ ---
var app = builder.Build();


// --- ✅ БЛОК КОНФИГУРАЦИИ HTTP-КОНВЕЙЕРА ---

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


//var defaultCulture = "ru-RU";
//var localizationOptions = new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture(defaultCulture),
//    SupportedCultures = new List<CultureInfo> { new CultureInfo(defaultCulture) },
//    SupportedUICultures = new List<CultureInfo> { new CultureInfo(defaultCulture) }
//};


app.UseRequestLocalization();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


//для api
app.MapControllers();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}" 
);

app.MapControllerRoute(
    name: "manage",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "user",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();