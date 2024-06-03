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

        [HttpGet]
        public IActionResult ReportDesigner(int datagridRptid)
        {
            UpdateModel.DatagridRptid = datagridRptid;
            //to bring the data according to some specific rptid
            //var reportDesignerData = GetReportDesignerData(datagridRptid);
            //return View(reportDesignerData);
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
