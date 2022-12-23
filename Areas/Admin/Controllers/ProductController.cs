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
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = db;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Category
        public IActionResult Index()
        {
            return View();
        }


       
        // GET: Category/Edit/5
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
				CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})

			};

			if (id == null || id == 0)
            {
                //create product
              //  ViewBag.CategoryList = CategoryList;
               // ViewData["CoverTypeList"] = CoverTypeList;

				return View(productVM);
            }
            else
            {
                //Update product
            }
            
            
            return View(productVM);
        }

        // POST: Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null) {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                _unitOfWork.Product.Add(obj.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Category Created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category
                .GetFirstOrDefault(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_unitOfWork.Category == null)
            {
                return Problem("Entity set 'DataProduct.Categories'  is null.");
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category != null)
            {
                _unitOfWork.Category.Remove(category);
                TempData["success"] = "Category deleted successfully";
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var productList = _unitOfWork.Product.GetAll();
            return Json(new { data = productList });
		}
		#endregion


	}


}
