using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace final_aerialview.Controllers
{
    public class DashboardFolderController : BaseController
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _dashboardFolderPath;

        public DashboardFolderController(DataAccess dataAccess, IFileProvider fileProvider) : base(dataAccess)
        {
            _fileProvider = fileProvider;
            _dashboardFolderPath = "Dashboards";
        }

        //taking all the files in dashboards folder to set in dash configuration
        public IActionResult DashInfo()
        {
            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
            ViewBag.Contents = contents;
            return View(contents);
        }
    }
}
