using DevExpress.DashboardCommon;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;

namespace final_aerialview.Controllers
{
    public class DashboardFolderController : BaseController
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _dashboardFolderPath;

        #region contains all the dashboard files which got saved in the dashboards folder of the project
        public DashboardFolderController(DataAccess dataAccess, IFileProvider fileProvider) : base(dataAccess)
        {
            _fileProvider = fileProvider;
            _dashboardFolderPath = "Dashboards";

            
        }
        #endregion


        #region taking all the files from dashboards folder to set in dash configuration
        public IActionResult DashInfo()
        {
            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
            ViewBag.Contents = contents;

            var dashboardList = new List<(string FileName, string Title)>();
            //to get the title of saved dashboards
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

                    // If title is empty, set default value
                    title = string.IsNullOrWhiteSpace(title) ? "Untitled Dashboard" : title;

                    // Add to list
                    dashboardList.Add((FileName: item.Name, Title: title));
                }
            }


            ViewBag.DashboardFiles = dashboardList.Select(file => new { FileName = file.FileName, Title = file.Title }).ToList();


            return View(contents);
        }
        #endregion
    }
}


//public IActionResult DashConfig(string parentMenu, string submenu)
//{

//    var dashboardListData = _dataAccess.GetDashboardMasterData();

//    ViewBag.ParentMenu = parentMenu;
//    ViewBag.Submenu = submenu;
//    ViewData["MenuName"] = submenu;

//    // Fetching the contents of the dashboard folder
//    var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
//    ViewBag.DashboardContents = contents; // Store the contents in ViewBag

//    // List to store file names and titles
//    var dashboardFiles = new List<(string FileName, string Title)>();

//    foreach (var item in contents)
//    {
//        if (!item.IsDirectory && item.Name.EndsWith(".xml"))
//        {
//            var filePath = Path.Combine(_dashboardFolderPath, item.Name);

//            // Read and parse the XML file to get the title text
//            var xmlDoc = new System.Xml.XmlDocument();
//            xmlDoc.Load(filePath);

//            var titleNode = xmlDoc.SelectSingleNode("//Title[@Text]");

//            string titleText = titleNode?.Attributes["Text"]?.Value ?? "Untitled";

//            dashboardFiles.Add((FileName: item.Name, Title: titleText));
//        }
//    }


//    //string filePath = "D:\\kashish\\Web_Development\\Dot Net Core Projects\\final-aerialview\\final-aerialview\\Dashboards\\dashboard1.xml";
//    //XmlDocument doc = new XmlDocument();
//    //doc.Load(filePath);


//    ViewBag.DashboardFiles = dashboardFiles;


//    return View(dashboardListData);
//}