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
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u=>u.Id== id);
                return View(productVM);
            }
            
            
           
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

                    if(obj.Product.ImageUrl!= null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }

                    _unitOfWork.Save();
                    TempData["success"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // GET: Category/Delete/5
      

        


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var productList = _unitOfWork.Product.GetAll(includeProperties :"Category,CoverType");
            return Json(new { data = productList });
		}


		// POST: Category/Delete/5
		
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			
			var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitOfWork.Product.Remove(obj);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Delete Successful" });
			
		}


		#endregion


	}


}
