using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Model;
using final_aerialview.Data;
using final_aerialview.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace final_aerialview.Controllers
{
    public class CustomProjectController(DataAccess dataAccess) : BaseController(dataAccess)
    {
        #region Load custom project page
        [HttpGet]
        public async Task<IActionResult> CustomProject()

        {
            var subModel = new SubMenuModel();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // Ensure role is not null
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }

            var savedLinks = _dataAccess.GetCustomLinks();

            var userMasterList = await _dataAccess.GetAccessibleControlsForUserAsync(role);


            // Filter the saved links
            var filteredLinks = savedLinks.Where(link =>
                userMasterList.Any(control =>
                    control.ControlName == link.SubMenuName && control.Status == true
                )
            ).ToList();

            // Pass the filtered list to the view
            ViewBag.SavedLinks = filteredLinks;


            return View(subModel);
        }
        #endregion

        #region Add custom project link
        [HttpPost]
        public IActionResult CustomProject(SubMenuModel model)
        {
            if (ModelState.IsValid)
            {
                // Ensure the URL starts with http:// or https://
                if (!model.Link_FormName.StartsWith("http://") && !model.Link_FormName.StartsWith("https://"))
                {
                    model.Link_FormName = "https://" + model.Link_FormName;
                }

                var result = _dataAccess.InsertCustomLink(model);

                if (result)
                {
                    TempData["NotificationMessage"] = "Custom link added successfully!";
                    TempData["NotificationType"] = "success";
                    // Redirect to the GET method to refresh the list
                    return RedirectToAction("CustomProject");
                }
                else
                {
                    TempData["NotificationMessage"] = "Failed to add custom link.";
                    TempData["NotificationType"] = "error";
                    //ViewBag.ErrorMessage = "Failed to add custom link.";
                }
            }
            else
            {
                //ViewBag.ErrorMessage = "Please fill in all the required fields.";

                TempData["NotificationMessage"] = "Please fill in all the required fields.";
                TempData["NotificationType"] = "error";
            }

            // If there are errors, return the view with the same model
            return View(model);
        }
        #endregion

        #region get custom link details by id
        public IActionResult GetLinkDetails(int subMenuCode)
        {
            var link = _dataAccess.GetCustomLinksById(subMenuCode);

            return Json(new { success = true, data = link });
            
        }
        #endregion
    }
}