using DevExpress.ReportServer.ServiceModel.DataContracts;
using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class EventConfigurationController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult EventConfig()
        {
            var connTableData = _dataAccess.GetChildMenuData();
            var reports = connTableData
                .GroupBy(r => r.stringName)
                .Select(g => g.First())
                .ToList();

            ViewData["Reports"] = reports;

            return View();
        }
    }
}
