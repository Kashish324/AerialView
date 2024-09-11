using Microsoft.AspNetCore.Mvc;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;


namespace final_aerialview.Controllers
{
    public class DefaultDashboardController : DashboardController
    {
        public DefaultDashboardController(DashboardConfigurator configurator, IDataProtectionProvider? dataProtectionProvider = null)
             : base(configurator, dataProtectionProvider)
        {
        }
    }
}
