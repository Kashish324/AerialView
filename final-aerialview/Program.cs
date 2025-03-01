using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using final_aerialview.Data;
using final_aerialview.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddRazorPages();

//method to add the connection strings dynamically in appsettings.json
// Add the necessary services like DataAccess
builder.Services.AddScoped<DataAccess>();

// Add authentication and authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)

    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

//old
//builder.Services.AddScoped<DataAccess>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDevExpressControls();

builder.Services.AddTransient<ReportStorageWebExtension>(serviceProvider =>
{
    return new CustomReportStorageWebExtension(
        Path.Combine(Directory.GetCurrentDirectory(), "Reports"),
        serviceProvider.GetRequiredService<DataAccess>()
    );
});


// Configure DevExpress Dashboard storage
IFileProvider? fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration? configuration = builder.Configuration;

string dashboardFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Dashboards");

if (!Directory.Exists(dashboardFolderPath))
{
    Directory.CreateDirectory(dashboardFolderPath);
}

builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
{
    DashboardConfigurator configurator = new DashboardConfigurator();

    var connectionStringsProvider = new DashboardConnectionStringsProvider(builder.Configuration);

    configurator.SetDashboardStorage(new DashboardFileStorage(dashboardFolderPath));
    //configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
    configurator.SetConnectionStringsProvider(connectionStringsProvider);



    DashboardConfigurator.PassCredentials = true;
    return configurator;
});
// Add this in ConfigureServices method
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));



#region configure reporting services for report designer & report viewer
builder.Services.ConfigureReportingServices(configurator =>
{
    if (builder.Environment.IsDevelopment())
    {
        configurator.UseDevelopmentMode();
    }

    //Asynchronous Mode
    configurator.UseAsyncEngine();

    configurator.ConfigureReportDesigner(designerConfigurator =>
    {
        //designerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<MyDataSourceWizardConnectionStringsProvider>();
        // Register the SQL data source
        //designerConfigurator.RegisterDataSource(sqlDataSource);

        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();

        designerConfigurator.EnableCustomSql();
    });


    configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
    {
        // Use cache for document generation and export.
        // This setting is necessary in asynchronous mode and when a report has interactive or drill down features.
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
#endregion

builder.Services.AddMvc();

var app = builder.Build();

app.UseDevExpressControls();


System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
app.MapDashboardRoute("api/dashboard", "DefaultDashboard");

#region adding connection strings to runtime configuration file from dataaccess, fetch connection data
using (var scope = app.Services.CreateScope())
{
    var dataAccess = scope.ServiceProvider.GetRequiredService<DataAccess>();
    var connections = dataAccess.GetReportConnectionData();

    // Add connection strings dynamically to configuration
    foreach (var connection in connections)
    {
        app.Configuration[$"ConnectionStrings:{connection.ConName}"] = connection.stringName;
    }
}
#endregion


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Use session middleware
app.UseSession();

#region Add middleware to check authentication
app.Use(async (context, next) =>
{
    if (context.Request.Path != "/Account/Login" && !context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/Account/Login");
    }
    else
    {
        await next();
    }
});
#endregion

#region custom project route
//app.UseEndpoints(endPoints =>
//{
//    endPoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=CustomProject}/{action=CustomProject}/{id?}"
//        );
//});

app.MapControllerRoute(
    name: "Custom Link Route",
    pattern: "custom-link",
     defaults: new { controller = "CustomProject", action = "CustomProject" }
);
#endregion


#region submenu routing 
app.MapControllerRoute(
    name: "Submenu",
    pattern: "{parentMenu}/{submenu}",
    defaults: new { controller = "Submenu", action = "HandleSubmenu" }
);
#endregion


#region Dashboard folder, where all the saved dashboards are contained
app.MapControllerRoute(
    name: "DashboardFolder",
    pattern: "DashboardFolder",
    defaults: new { controller = "DashboardFolder", action = "DashInfo" }
);
#endregion

#region Default Dashboard - open in a new window at application startup
app.MapControllerRoute(
    name: "DefaultDashboard",
    pattern: "DefaultDashboardViewer/{page?}",
    defaults: new { controller = "DefaultDashViewer", action = "DefaultDashboard" }
);
#endregion

#region Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
#endregion

app.Run();

