using CompuCare.Platform.Companies.Application.Acl;
using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Application.Internal.CommandServices;
using CompuCare.Platform.Companies.Application.Internal.QueryServices;
using CompuCare.Platform.Companies.Application.QueryServices;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Companies.Interfaces.Acl;
using CompuCare.Platform.Iam.Application.Acl;
using CompuCare.Platform.Iam.Application.CommandServices;
using CompuCare.Platform.Iam.Application.Internal.CommandServices;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Application.Internal.QueryServices;
using CompuCare.Platform.Iam.Application.QueryServices;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Iam.Infrastructure.Email.Smtp.Configuration;
using CompuCare.Platform.Iam.Infrastructure.Email.Smtp.Services;
using CompuCare.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using CompuCare.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using CompuCare.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using CompuCare.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using CompuCare.Platform.Iam.Interfaces.Acl;
using CompuCare.Platform.Notifications.Application.Acl;
using CompuCare.Platform.Notifications.Application.CommandServices;
using CompuCare.Platform.Notifications.Application.Internal.CommandServices;
using CompuCare.Platform.Notifications.Application.Internal.QueryServices;
using CompuCare.Platform.Notifications.Application.QueryServices;
using CompuCare.Platform.Notifications.Domain.Repositories;
using CompuCare.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Notifications.Interfaces.Acl;
using CompuCare.Platform.Quotations.Application.Acl;
using CompuCare.Platform.Quotations.Application.CommandServices;
using CompuCare.Platform.Quotations.Application.Internal.CommandServices;
using CompuCare.Platform.Quotations.Application.Internal.QueryServices;
using CompuCare.Platform.Quotations.Application.QueryServices;
using CompuCare.Platform.Quotations.Domain.Repositories;
using CompuCare.Platform.Quotations.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Quotations.Interfaces.Acl;
using CompuCare.Platform.Resources.Errors;
using CompuCare.Platform.Resources.Shared;
using CompuCare.Platform.ServiceRequests.Application.Acl;
using CompuCare.Platform.ServiceRequests.Application.CommandServices;
using CompuCare.Platform.ServiceRequests.Application.Internal.CommandServices;
using CompuCare.Platform.ServiceRequests.Application.Internal.QueryServices;
using CompuCare.Platform.ServiceRequests.Application.QueryServices;
using CompuCare.Platform.ServiceRequests.Domain.Repositories;
using CompuCare.Platform.ServiceRequests.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.ServiceRequests.Interfaces.Acl;
using CompuCare.Platform.Shared.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;
using CompuCare.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Seeding;
using CompuCare.Platform.Shared.Infrastructure.Pipeline.Middleware.Components;
using CompuCare.Platform.Subscriptions.Application.Acl;
using CompuCare.Platform.Subscriptions.Application.CommandServices;
using CompuCare.Platform.Subscriptions.Application.Internal.CommandServices;
using CompuCare.Platform.Subscriptions.Application.Internal.QueryServices;
using CompuCare.Platform.Subscriptions.Application.QueryServices;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using CompuCare.Platform.Subscriptions.Interfaces.Acl;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Scalar.AspNetCore;
using CompaniesAcl = CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;
using IamAcl = CompuCare.Platform.Iam.Application.Internal.OutboundServices.Acl;
using ProblemDetailsFactory = CompuCare.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;
using QuotationsAcl = CompuCare.Platform.Quotations.Application.Internal.OutboundServices.Acl;
using ServiceRequestsAcl = CompuCare.Platform.ServiceRequests.Application.Internal.OutboundServices.Acl;
using SubscriptionsAcl = CompuCare.Platform.Subscriptions.Application.Internal.OutboundServices.Acl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// Add ProblemDetails and ExceptionHandler services
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .AddInterceptors(serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

builder.Services.AddLocalization();

// Explicitly register IStringLocalizer for ErrorMessages and Commons
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services.AddSingleton<IStringLocalizer<CommonMessages>, StringLocalizer<CommonMessages>>();

// Register the custom ProblemDetailsFactory
builder.Services.AddSingleton<ProblemDetailsFactory>();

// OpenAPI / Scalar documentation
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<DispatchDomainEventsInterceptor>();

// IAM Bounded Context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IPasswordResetCommandService, PasswordResetCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();
builder.Services.AddScoped<IamAcl.ExternalCompanyService>();
builder.Services.AddScoped<IamAcl.ExternalSubscriptionService>();

// Companies Bounded Context
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<ICompanyCommandService, CompanyCommandService>();
builder.Services.AddScoped<ILocationCommandService, LocationCommandService>();
builder.Services.AddScoped<IEquipmentCommandService, EquipmentCommandService>();
builder.Services.AddScoped<ILocationQueryService, LocationQueryService>();
builder.Services.AddScoped<IEquipmentQueryService, EquipmentQueryService>();
builder.Services.AddScoped<ICompaniesContextFacade, CompaniesContextFacade>();
builder.Services.AddScoped<CompaniesAcl.ExternalEmployeeService>();
builder.Services.AddScoped<CompaniesAcl.ExternalServiceRequestService>();
builder.Services.AddScoped<CompaniesAcl.ExternalMaintenanceService>();

// Subscriptions Bounded Context
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IMaintenanceVisitRepository, MaintenanceVisitRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<IMaintenanceVisitCommandService, MaintenanceVisitCommandService>();
builder.Services.AddScoped<IPlanQueryService, PlanQueryService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<IMaintenanceVisitQueryService, MaintenanceVisitQueryService>();
builder.Services.AddScoped<ISubscriptionsContextFacade, SubscriptionsContextFacade>();
builder.Services.AddScoped<SubscriptionsAcl.ExternalEquipmentService>();

// Service Requests Bounded Context
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IServiceRequestCommandService, ServiceRequestCommandService>();
builder.Services.AddScoped<IServiceRequestQueryService, ServiceRequestQueryService>();
builder.Services.AddScoped<ServiceRequestDetailsBuilder>();
builder.Services.AddScoped<IServiceRequestsContextFacade, ServiceRequestsContextFacade>();
builder.Services.AddScoped<ServiceRequestsAcl.ExternalEquipmentService>();
builder.Services.AddScoped<ServiceRequestsAcl.ExternalUserService>();
builder.Services.AddScoped<ServiceRequestsAcl.ExternalQuotationService>();
builder.Services.AddScoped<ServiceRequestsAcl.ExternalSubscriptionService>();
builder.Services.AddScoped<ServiceRequestsAcl.ExternalNotificationService>();

// Quotations Bounded Context
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();
builder.Services.AddScoped<IQuotationCommandService, QuotationCommandService>();
builder.Services.AddScoped<IQuotationQueryService, QuotationQueryService>();
builder.Services.AddScoped<IQuotationsContextFacade, QuotationsContextFacade>();
builder.Services.AddScoped<QuotationsAcl.ExternalServiceRequestService>();
builder.Services.AddScoped<QuotationsAcl.ExternalUserService>();
builder.Services.AddScoped<QuotationsAcl.ExternalNotificationService>();

// Notifications Bounded Context
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationCommandService, NotificationCommandService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();
builder.Services.AddScoped<INotificationsContextFacade, NotificationsContextFacade>();

// Mediator Configuration
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(ExceptionHandlingCommandBehavior<>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(TransactionCommandBehavior<>));

// Add Cortex Mediator for Command and Event Handling
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    // Optional fictitious accounts for local demonstrations only.
    if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("DemoSeed:Enabled"))
        await DemoDataSeeder.SeedAsync(context, services.GetRequiredService<IHashingService>(),
            app.Configuration["DemoSeed:Password"] ?? string.Empty);
}

// Trust the reverse proxy's X-Forwarded-* headers (Azure App Service terminates
// HTTPS and forwards to the container over plain HTTP) so the app sees the real
// external scheme/host instead of defaulting to http. Must run before any
// middleware that reasons about the request's scheme or host.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Scalar's API explorer is mapped regardless of environment, so every feature can be
// verified with a real HTTP call after deployment.
app.MapOpenApi();
app.MapScalarApiReference();

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

// Add Authorization Middleware to Pipeline
app.UseRequestAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
