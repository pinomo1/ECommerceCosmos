using Azure.Storage.Blobs;
using ECommerce1.Extensions;
using ECommerce1.Models;
using ECommerce1.Models.Validators;
using ECommerce1.Models.ViewModels;
using ECommerce1.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using SendGrid.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

#region Services
services.AddCors();

services.AddDbContextPool<ResourceDbContext>(options =>
    options.UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "ResourceDb"));

services.AddDbContextPool<AccountDbContext>(options =>
    options.UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", "AccountDb"));

services.AddIdentity<AuthUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = true;

    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AccountDbContext>()
.AddTokenProvider<DataProtectorTokenProvider<AuthUser>>(TokenOptions.DefaultProvider)
.AddTokenProvider<DataProtectorTokenProvider<AuthUser>>(TokenOptions.DefaultPhoneProvider);

services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
services.AddEndpointsApiExplorer();
services.AddSwagger();
// services.AddJwtAuthentication(config["Secret"], new List<string>() { "User", "Seller", "Admin" });
services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.AddSendGrid(options =>
    options.ApiKey = builder.Configuration.GetValue<string>("SendGridApiKey")
                     ?? throw new Exception("The 'SendGridApiKey' is not configured")
);
services.AddJwtAuthentication("3c66ae61-d405-4d24-8622-096087df7d22", new List<string>() { "User", "Seller", "Admin" });
services.AddScoped<IValidator<AddProductViewModel>, ProductValidator>();
services.AddScoped<IValidator<UserCredentials>, UserRegistrationValidator>();
services.AddScoped<IValidator<StaffCredentials>, StaffRegistrationValidator>();
services.AddScoped<IValidator<SellerCredentials>, SellerRegistrationValidator>();
services.AddScoped<IValidator<LoginCredentials>, LoginValidator>();
services.AddTransient<BlobServiceClient>(x =>
{
    return new BlobServiceClient(config.GetConnectionString("BlobStorage"));
});
services.AddTransient<BlobWorker>();

services.AddAzureClients(builder =>
{
    builder.AddBlobServiceClient(config["ConnectionStrings:BlobStorage:blob"], preferMsi: true);
    builder.AddQueueServiceClient(config["ConnectionStrings:BlobStorage:queue"], preferMsi: true);
});
#endregion

#region Configure
services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

app.UseCors((options) =>
{
    options.WithOrigins()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed((x) => true)
    .AllowCredentials();
});

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Site API");
});

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion