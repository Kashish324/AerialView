using DevExpress.DashboardCommon;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;

namespace final_aerialview.Controllers
{
    public class SubmenuDashViewerController : Controller
    {
        private readonly DataAccess _dataAccess;
        private readonly IFileProvider _fileProvider;
        private readonly string _dashboardFolderPath;

        public SubmenuDashViewerController(DataAccess dataAccess, IFileProvider fileProvider)
        {
            _dataAccess = dataAccess;
            _fileProvider = fileProvider;
            _dashboardFolderPath = "Dashboards";
        }

        #region shows us the saved dashboard from the menu under menu parent "Dashboards"
        public IActionResult ViewDashboard(int dashId)
        {
            var dashboardListData = _dataAccess.GetDashboardMasterData();

            var dashboard = dashboardListData.FirstOrDefault(d => d.DashId == dashId);

            if (dashboard == null || string.IsNullOrEmpty(dashboard.DashPath))
            {
                return NotFound("Dashboard not found.");
            }

            // Load all XML files from the dashboard folder
            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);

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

                    // Map Title -> FileName for loading later
                    titleToFileMap[title] = item.Name;
                }
            }


            // Check if DashPath is a title and get corresponding file name
            if (titleToFileMap.ContainsKey(dashboard.DashPath))
            {
                // Update DashPath with the mapped file name
                dashboard.DashPath = titleToFileMap[dashboard.DashPath];
            }
            else
            {
                return NotFound("Dashboard file not found.");
            }


            return View(dashboard);
        }
        #endregion
    }
}
