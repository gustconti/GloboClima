using GloboClimaSPA.Components;

var builder = WebApplication.CreateBuilder(args);

// Fetch environment-specific values
var environment = builder.Environment.EnvironmentName;
var certPassword = builder.Configuration["CERT_PASSWORD"];
string? certPath = environment == "Development" ? builder.Configuration["CERT_PATH"] : null;

// Add AWS ACM certificate configuration for Production
if (environment == "Production")
{
    // AWS Certificate Manager is typically used in a load balancer or via Application Load Balancer (ALB)
    // You do not need to manage certificates manually on the server when deploying to AWS
    // Configure SSL certificate through AWS services (ALB / CloudFront)
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            // You can optionally configure SSL in Kestrel or rely on ALB/CloudFront in AWS
            // For ALB/CloudFront, AWS manages the SSL certificate for you
        });
    });
}
else
{
    // Development: Use self-signed certificate
    if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(certPassword))
    {
        throw new InvalidOperationException("Certificate path or password is not configured.");
    }

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenLocalhost(8000, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    });
}

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
