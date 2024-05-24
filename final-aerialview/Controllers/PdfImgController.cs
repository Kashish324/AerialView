using final_aerialview.Data;
using Microsoft.AspNetCore.Mvc;

namespace final_aerialview.Controllers
{
    public class PdfImgController : BaseController
    {

        public PdfImgController(DataAccess dataAccess) : base(dataAccess)
        {
            //_dataAccess = dataAccess;
        }
        public IActionResult Index()
        {
            var pdfImageData = _dataAccess.GetPdfImageData();

            return View(pdfImageData);
        }
    }
}
