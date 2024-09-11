using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class PdfImgController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        public IActionResult Index()
        {
            var pdfImageData = _dataAccess.GetPdfImageData();

            return View(pdfImageData);
        }
    }
}
