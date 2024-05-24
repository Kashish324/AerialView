using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class ReportController : BaseController
    {

        public ReportController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }
        public IActionResult ReportDesigner(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;

            return View();
        }

        public IActionResult DocumentViewer(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;

            ViewData["DatagridRptid"] = datagridRptid;


            string reportName = _dataAccess.GetReportFromDatabase();
            ViewData["ReportName"] = reportName;

            return View();
        }

        
    }
}
