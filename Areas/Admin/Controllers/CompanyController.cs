using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulkyBook.Model;
using BulkyBook.DataAccess.Data;
using BulkyBook.Model.Models;
using BulkyBook.DataAccess.Reprository.IReprository;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.DataAccess.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       

        public CompanyController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        // GET: Category
        public IActionResult Index()
        {
            return View();
        }


       
        // GET: Category/Edit/5
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            

			if (id == null || id == 0)
            {
                //create product
              //  ViewBag.CategoryList = CategoryList;
               // ViewData["CoverTypeList"] = CoverTypeList;

				return View(company);
            }
            else
            {
                //Update product
                company = _unitOfWork.Company.GetFirstOrDefault(u=>u.Id== id);
                return View(company);
            }
            
            
           
        }

        // POST: Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {

            if (ModelState.IsValid)
            {
             
                if(obj.Id == 0) {
                    
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Category Created Successfully";

                }

                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Category Updated Successfully";

                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // GET: Category/Delete/5
      

        


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
		}


		// POST: Category/Delete/5
		
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			
			var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}


			_unitOfWork.Company.Remove(obj);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Delete Successful" });
			
		}


		#endregion


	}


}
