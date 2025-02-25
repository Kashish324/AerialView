﻿using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace final_aerialview.Controllers
{
    public class BaseController : Controller
    {
        protected readonly DataAccess _dataAccess;

        public BaseController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        #region base controller action so that whatever page we redirect to it shows the all the menu items in the sidebar as long as thier controller is extended from the base controller
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["MenuParentData"] = _dataAccess.GetMenuParentData();
            ViewData["SubMenuData"] = _dataAccess.GetSubMenuData();
            ViewData["ChildMenuData"] = _dataAccess.GetChildMenuData();
            ViewData["PdfImageData"] = _dataAccess.GetPdfImageData();
            ViewData["ReportData"] = _dataAccess.GetReportData();

            // to get the navigation menu according to user logged in
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            string role = roleClaim?.Value ?? string.Empty;

            ViewBag.UserMasterData = _dataAccess.GetAccessibleControlsForUserAsync(role).Result;

            base.OnActionExecuting(context);
        }
        #endregion
    }
}
