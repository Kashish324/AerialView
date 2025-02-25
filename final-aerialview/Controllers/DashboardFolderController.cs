using DevExpress.DashboardCommon;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

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

            //Dashboard d = new Dashboard();
            ////d.LoadFromXDocument(document);
            //var title = d.Title.Text;
            return View(contents);
        }
        #endregion
    }
}
