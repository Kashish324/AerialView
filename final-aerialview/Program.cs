using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using final_aerialview.Data;
using final_aerialview.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddScoped<DataAccess>();

builder.Services.AddDevExpressControls();

builder.Services.AddTransient<ReportStorageWebExtension>(serviceProvider =>
{
    return new CustomReportStorageWebExtension(
        Path.Combine(Directory.GetCurrentDirectory(), "Reports"),
        serviceProvider.GetRequiredService<DataAccess>()

    );
});


builder.Services.AddMvc();

builder.Services.ConfigureReportingServices(configurator =>
{
    if (builder.Environment.IsDevelopment())
    {
        configurator.UseDevelopmentMode();
    }
    configurator.ConfigureReportDesigner(designerConfigurator =>
    {
        //designerConfigurator.RegisterDataSourceWizardConnectionStringsProvider<MyDataSourceWizardConnectionStringsProvider>();
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

var app = builder.Build();

app.UseDevExpressControls();
System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDevExpressControls();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Submenu",
    pattern: "{parentMenu}/{submenu}",
    defaults: new { controller = "Submenu", action = "HandleSubmenu" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllers();

//#pragma warning disable ASP0014
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "Submenu",
//        pattern: "{parentMenu}/{submenu}",
//        defaults: new { controller = "Submenu", action = "ReportConfiguration" }
//    );

   

//    // Additional endpoints can be defined here 
//});
//#pragma warning restore ASP0014

app.Run();
