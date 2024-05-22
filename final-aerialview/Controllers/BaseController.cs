using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace final_aerialview.Controllers
{
    public class BaseController : Controller
    {
        protected readonly DataAccess _dataAccess;

        public BaseController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["MenuParentData"] = _dataAccess.GetMenuParentData();
            ViewData["SubMenuData"] = _dataAccess.GetSubMenuData();
            ViewData["ChildMenuData"] = _dataAccess.GetChildMenuData();
            ViewData["PdfImageData"] = _dataAccess.GetPdfImageData();
            ViewData["ReportData"] = _dataAccess.GetReportData();

            base.OnActionExecuting(context);
        }
    }
}
