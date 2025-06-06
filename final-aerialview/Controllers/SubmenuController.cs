
using DevExpress.DashboardCommon;
using final_aerialview.Controllers;
using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.Storages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Xml;
using System.Xml.Linq;

public class SubmenuController : BaseController
{
    private readonly IFileProvider _fileProvider;
    private readonly string _dashboardFolderPath;

    public SubmenuController(DataAccess dataAccess, IFileProvider fileProvider) : base(dataAccess)
    {
        _fileProvider = fileProvider;
        _dashboardFolderPath = "Dashboards";
    }


        #region shows us the list of table names for user to select to redirect to report designer
        public IActionResult ReportDesigner(string parentMenu, string submenu)
    {
        var reportData = _dataAccess.GetReportData();
        var uniqueReportTypes = reportData
            .GroupBy(r => r.ReportType)
            .Select(g => g.First())
            .ToList();

        ViewData["ReportData"] = reportData;
        ViewData["ReportType"] = uniqueReportTypes;

        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View();
    }
    #endregion

    #region shows dashboard designer
    public IActionResult DashDesigner(string parentMenu, string submenu)
    {
        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;
        var dashboardListData = _dataAccess.GetDashboardMasterData();

        // Get all XML files from the dashboard folder
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

        // Pass the mapping for loading the correct file
        ViewBag.TitleToFileMap = titleToFileMap;


        return View(dashboardListData);
    }
    #endregion

    #region delete dashboard from dashboard designer and from project too
    public ActionResult DeleteDashboard(string DashboardID)
    {
        string dashboardPath = Path.Combine(Directory.GetCurrentDirectory(), "Dashboards");
        CustomDashboardFileStorage newDashboardStorage = new CustomDashboardFileStorage(dashboardPath);
        newDashboardStorage.DeleteDashboard(DashboardID);
        return new EmptyResult();
    }
    #endregion

    #region Dashboard configuration table actions start

    #region dashboard configuration table
    public IActionResult DashConfig(string parentMenu, string submenu)
    {

        var dashboardListData = _dataAccess.GetDashboardMasterData();

        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        // Fetching the contents of the dashboard folder
        var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
        ViewBag.DashboardContents = contents; // Store the contents in ViewBag


        //// Convert to a list for better LINQ support
        //var dashboardFiles = ((IEnumerable<dynamic>)ViewBag.DashboardFiles).ToList();


        //foreach (var item in dashboardListData)
        //{
        //    item.IsPathExists = dashboardFiles
        //        .Any(i => dashboardFiles
        //            .Any(d => d.Name.Equals(i.FileName, StringComparison.OrdinalIgnoreCase)
        //                   && i.Title.Equals(item.DashPath, StringComparison.OrdinalIgnoreCase)));
        //}



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

        return View(dashboardListData);
    }
    #endregion

    #region update new or existing row in dashboard configuration table 
    [HttpPost]
    public IActionResult UpdateData([FromBody] List<DashboardDataModel> data)
    {
        if (data == null || data.Count == 0)
        {
            return BadRequest("No data to update.");
        }

        //var existingRows = data.Where(d => d.DashId <= 80).ToList();
        //var newRows = data.Where(d => d.DashId > 80 && d.DashId <= 100).ToList();

        var existingRows = data.Where(d => !d.IsNew).ToList();
        var newRows = data.Where(d => d.IsNew).ToList();

        var errorMessages = new List<string>();

        if (existingRows.Any())
        {
            errorMessages.AddRange(_dataAccess.UpdateDashboardData(existingRows));
            //_dataAccess.UpdateDashboardData(existingRows);
        }

        if (newRows.Any())
        {
            //_dataAccess.InsertDashboardData(newRows);
            errorMessages.AddRange(_dataAccess.InsertDashboardData(newRows));
        }

        if (errorMessages.Any())
        {
            return Conflict(new { message = "Data not updated.", errors = errorMessages });
        }

        return Ok("Data updated successfully.");
    }
    #endregion

    #region delete dashboard item according to dashId
    [HttpPost]
    public IActionResult DeleteDashboardByDashId(int dashId)
    {

        Console.WriteLine($"Received dashId: {dashId}");

        _dataAccess.DeleteDashById(dashId);

        return Ok("Dashboard deleted successfully");
    }
    #endregion

    #endregion 

    #region actions as needed for each submenu view
    public IActionResult SubMenuConfiguration(string parentMenu, string submenu)
    {
        // Common logic or redirect to default view based on submenu
        switch (submenu.ToLower())
        {
            case "report configuration":
                return RedirectToAction("Index", "ReportConfiguration", new { parentMenu, submenu });
            case "report designer":
                return RedirectToAction(nameof(ReportDesigner), new { parentMenu, submenu });
            case "dash designer":
                return RedirectToAction(nameof(DashDesigner), new { parentMenu, submenu });
            case "dash configuration":
                return RedirectToAction(nameof(DashConfig), new { parentMenu, submenu });
            default:
                return RedirectToAction("Error", "Home");
        }
    }
    #endregion
}

