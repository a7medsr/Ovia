using Microsoft.EntityFrameworkCore;
using Ovia.Helpers;
using Ovia.Models;
using Ovia.Services;
using Ovia.Services.SendEmails;
using Ovia.Services.StorageFiles;
using System.Text;
using Telegram.Bot;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hangfire;
using Ovia.Services.SendWhatsApp360Live;
using Microsoft.AspNetCore.Http.Features;
using System.Text.Json.Serialization;
using Ovia.Services.ChatHub;
using PdfSharp.Fonts;
using PdfSharp.Charting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Ovia.Models;

var builder = WebApplication.CreateBuilder(args);







//whatsapp 360 live
builder.Services.AddScoped<Whats360Client>();

//builder.Services.AddSingleton<BunnyNet>(new BunnyNet("3ab754c3 - f053 - 4cc0 - 9b69 - 92ca0c0307d1",
//    "vz-76f04906-368.b-cdn.net", 234264));


//3ab754c3 - f053 - 4cc0 - 9b69 - 92ca0c0307d1
//348f9ec2-7fa0-46ce-be2110c6bb5f-9123-407e

//builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

GlobalFontSettings.FontResolver = new CustomFontResolver();
builder.Services.AddTransient<FilesServices>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ovia", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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


builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
    options.KeyLengthLimit = int.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 50000000; // Set the maximum request body size here (30 MB in this example)
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
       });

////builder.Services.AddScoped<CountryServices>();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MomEntity>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings"));
    // options.UseSqlServer(builder.Configuration.GetConnectionString("MomentumDbConnection"));
});



builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("ConnectionStrings"));
    //  config.UseSqlServerStorage(builder.Configuration.GetConnectionString("MomentumDbConnection"));
});


builder.Services.AddHangfireServer();




builder.Services.AddTransient<IMailingServices, MailingServices>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//builder.Services.AddScoped<CustomerInfo, CustomerInfoController>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSingleton<TelegramBotClient>(provider => {
    var botToken = "6935466790:AAHBegNUuZw8DK2bvNYVfruK4MUGl626l9E";
    return new TelegramBotClient(botToken);
});



builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var appSettingsSection = builder.Configuration.GetSection("AppSettings"); // Use builder.Configuration

// configure jwt authentication
var appSettings = appSettingsSection.Get<AppSettings>(); // Use appSettingsSection instead of Configuration
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Use JwtBearerDefaults.AuthenticationScheme
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Use JwtBearerDefaults.AuthenticationScheme
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});












builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register your services here
// Register your services here
builder.Services.AddScoped<ProfitToCustomerAccountConverter>();
//builder.Services.AddScoped<ScheduledTask>(); // Change this to scoped as well


builder.Services.AddScoped<TelegramService>();

builder.Services.AddScoped<JWTService>(); // Register JWTService as a scoped service


builder.Services.AddSingleton<IStorageService, StorageService>();










var app = builder.Build();





app.UseCors("CorsPolicy"); // Apply CORS policy


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MomEntity>();

    if (!db.Country.Any())
    {
        var json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Countries.json"));
        var countries = JsonConvert.DeserializeObject<List<Countries>>(json);

        //foreach (var country in countries)
        //{
        //    // Remove the line: country.Id = id++;
        //}

        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Country ON");
        db.Country.AddRange(countries);
        db.SaveChanges();
        db.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Country OFF");
    }
}





app.UseRouting();

// Other app configurations...

app.UseEndpoints(endpoints =>
{
    // Map SignalR hub
    endpoints.MapHub<ChatHub>("/ChatHub");

    // Other endpoints...
});


app.UseHttpsRedirection();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "wwwroot", "Uploads")),
//    RequestPath = "/Uploads"
//});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();



app.UseHangfireDashboard("/hangfire");
app.UseHangfireServer();

// Schedule Hangfire job
//to hangfireconvert profit to customer account balance
//RecurringJob.AddOrUpdate<ProfitToCustomerAccountConverter>(
//    x => x.ConvertProfitToCustomerAccount(),
//    Cron.Weekly(dayOfWeek: DayOfWeek.Friday, hour: 0, minute: 00));

//RecurringJob.AddOrUpdate<ProfitToCustomerAccountConverter>(
//    x => x.ConvertProfitToCustomerAccount(),
//    "00 00 * * 5");


RecurringJob.AddOrUpdate<ProfitToCustomerAccountConverter>(
    x => x.ConvertProfitToCustomerAccount(),
    "0 1 * * MON");


//RecurringJob.AddOrUpdate<BVServices>(
//    x => x.ProcessPointProcesses(),
//    "0 1 * * *"); // Runs daily at 1:00 AM

//RecurringJob.AddOrUpdate<BV>(
//           x => x.Distribute_Comission_LeFt_and_Right_Points(),
//           "*/10 * * * 0"); // Runs every 10 minutes on Sundays

//RecurringJob.AddOrUpdate<BV>(
//    x => x.Distribute_BV_Comission(),
//    "0 1 * * *"); // Runs daily at 1:00 AM


//RecurringJob.AddOrUpdate<ProfitToCustomerAccountConverter>(
//       x => x.UpdateUplineHistoryId(),
//       Cron.Daily(4, 0)
//   );
RecurringJob.AddOrUpdate<ProfitToCustomerAccountConverter>(
    x => x.SendNewCheckMailToCustomr(),
    "0 3 * * MON");




app.MapControllers();
app.UseStaticFiles(); // Enable serving static files (including videos)

app.Run();



