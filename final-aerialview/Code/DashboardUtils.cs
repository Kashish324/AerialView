//using DevExpress.DashboardAspNetCore;
//using DevExpress.DashboardWeb;
//using final_aerialview.Storages;
//using Microsoft.Extensions.FileProviders;

//namespace final_aerialview.Code
//{
//    public class DashboardUtils
//    {
//        public static DashboardConfigurator CreateDashboardConfigurator(IConfiguration configuration, IFileProvider fileProvider)
//        {
//            DashboardConfigurator configurator = new DashboardConfigurator();
//            configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));

//            // Get the absolute path for the Dashboards folder
//            string dashboardPath = Path.Combine(AppContext.BaseDirectory, "Dashboards");

//            DashboardFileStorage dashboardFileStorage = new CustomDashboardFileStorage(dashboardPath);
//            configurator.SetDashboardStorage(dashboardFileStorage);

//            return configurator;

//        }
//    }
//}
