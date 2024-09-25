using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class PdfImgController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        #region get us all the project settings like module key, logo, client/project name for the view 
        public IActionResult Index()
        {
            var pdfImageData = _dataAccess.GetPdfImageData();

            return View(pdfImageData);
        }
        #endregion
    }
}
