using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO;

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

        public IActionResult DashInfo()
        {
            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
            return View(contents);
        }
    }
}

//using final_aerialview.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.FileProviders;
//using System.IO;

//namespace final_aerialview.Controllers
//{
//    public class DashboardFolderController : BaseController
//    {
//        private readonly IFileProvider _fileProvider;
//        private readonly string _dashboardFolderPath;

//        public DashboardFolderController(DataAccess dataAccess, IFileProvider fileProvider) : base(dataAccess)
//        {
//            _fileProvider = fileProvider;
//            _dashboardFolderPath = "Dashboards";
//        }

//        public IActionResult DashInfo()
//        {
//            var contents = _fileProvider.GetDirectoryContents(_dashboardFolderPath);
//            return View(contents);
//        }
//    }
//}
