
using final_aerialview.Controllers;
using final_aerialview.Data;
using final_aerialview.Models;
using final_aerialview.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;

public class SubmenuController(DataAccess dataAccess) : BaseController(dataAccess)
{
    public IActionResult ReportConfiguration(string parentMenu, string submenu)
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

    public IActionResult DashDesigner(string parentMenu, string submenu)
    {
        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View();
    }

    public IActionResult DashConfig(string parentMenu, string submenu)
    {

        var dashboardListData = _dataAccess.GetDashboardMasterData();

        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View(dashboardListData);
    }


    [HttpPost]
    public IActionResult UpdateData([FromBody] List<DashboardDataModel> data)
    {
        if (data == null || data.Count == 0)
        {
            return BadRequest("No data to update.");
        }

        var existingRows = data.Where(d => d.DashId <= 60).ToList();
        var newRows = data.Where(d => d.DashId > 60 && d.DashId <= 100).ToList();
        //var newRows = data.Where(d => d.DashId > 50 && d.DashId <= 100 && !string.IsNullOrEmpty(d.DashPath)).ToList();


        if (existingRows.Any())
        {
            _dataAccess.UpdateDashboardData(existingRows);
        }

        if (newRows.Any())
        {
            _dataAccess.InsertDashboardData(newRows);
            
        }

        return Ok("Data updated successfully.");
    }


    [HttpPost]
    public IActionResult DeleteDashboardByDashId(int dashId)
    {
     
        Console.WriteLine($"Received dashId: {dashId}");

        _dataAccess.DeleteDashById(dashId);

        return Ok("Dashboard deleted successfully");
    }


    public IActionResult EmailSetting(string parentMenu, string submenu)
    {

        var profileData = _dataAccess.GetSettingProfileData().FirstOrDefault();
        var contactData = _dataAccess.GetSettingContactData().ToList();

        var viewModel = new EmailSettingViewModel
        {
            Contacts = contactData ?? new List<SettingContactModel>(),
            Profile = profileData ?? new SettingProfileModel()
        };

        ViewBag.ParentMenu = parentMenu;
        ViewBag.Submenu = submenu;
        ViewData["MenuName"] = submenu;

        return View(viewModel);
    }


    // Add more actions as needed for each submenu view
    public IActionResult SubMenuConfiguration(string parentMenu, string submenu)
    {
        // Common logic or redirect to default view based on submenu
        switch (submenu.ToLower())
        {
            case "report configuration":
                return RedirectToAction(nameof(ReportConfiguration), new { parentMenu, submenu });

            case "dash designer":
                return RedirectToAction(nameof(DashDesigner), new { parentMenu, submenu });

            case "dash configuration":
                return RedirectToAction(nameof(DashConfig), new { parentMenu, submenu });

            case "email setting":
                return RedirectToAction(nameof(EmailSetting), new { parentMenu, submenu });

            default:
                return View("DefaultView");
        }
    }
}
