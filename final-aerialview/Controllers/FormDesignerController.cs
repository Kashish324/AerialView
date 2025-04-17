using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class FormDesignerController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        #region not in use currently 
        public IActionResult FormDesigner()
        {
            return View();
        }
        #endregion
    }
}
