using DevExpress.DashboardCommon;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Xml;
using System.Xml.Linq;

namespace final_aerialview.Controllers
{
    public class DefaultDashViewerController : Controller
    {
        private readonly DataAccess _dataAccess;
        private readonly IFileProvider _fileProvider;
        private readonly string _dashboardFolderPath;

        public DefaultDashViewerController(DataAccess dataAccess, IFileProvider fileProvider)
        {
            _dataAccess = dataAccess;
            _fileProvider = fileProvider;
            _dashboardFolderPath = "Dashboards";
        }

        #region show us the default dashboard screen on application startup
        public IActionResult DefaultDashboard()
        {
            // Set session flag to indicate new tab is opened
            HttpContext.Session.SetString("IsNewTabOpened", "True");

            var dashboardListData = _dataAccess.GetDashboardMasterData();

            // Get all XML files from the dashboard folder
            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);

            var dashboardList = new List<(string FileName, string Title)>();
            var titleToFileMap = new Dictionary<string, string>();

            // Get the title of saved dashboards and map to file names
            foreach (var item in contents)
            {
                if (item.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    // Get the file path
                    var filePath = item.PhysicalPath;

                    // Load the XML as XDocument
                    XDocument document = XDocument.Load(filePath);

                    // Initialize and load dashboard
                    Dashboard d = new Dashboard();
                    d.LoadFromXDocument(document);

                    // Get the title
                    var title = d.Title.Text;

                    // Add to list for displaying
                    dashboardList.Add((FileName: item.Name, Title: title));

                    //// Map Title -> FileName for loading later
                    //titleToFileMap[title] = item.Name;

                    // Map Title -> FileName for loading later, ensuring uniqueness
                    titleToFileMap[$"{title}_{item.Name}"] = item.Name;
                }
            }

            // Pass the list for displaying titles
            ViewBag.DashboardFiles = dashboardList.Select(file => new { FileName = file.FileName, Title = file.Title }).ToList();

            // Pass the mapping for loading the correct file
            ViewBag.TitleToFileMap = titleToFileMap;

            return View(dashboardListData);
        }
        #endregion
    }
}