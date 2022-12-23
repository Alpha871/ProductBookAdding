using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulkyBook.Model;
using BulkyBook.DataAccess.Data;
using BulkyBook.Model.Models;
using BulkyBook.DataAccess.Reprository;
using BulkyBook.DataAccess.Reprository.IReprository;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        // GET: Category
        public IActionResult Index()
        {
            IEnumerable<Category> listOfObjet = _unitOfWork.Category.GetAll();
            return View(listOfObjet);
        }


        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _unitOfWork.Category == null)
            {
                return NotFound();
            }

            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.Category.Update(category);
                    TempData["success"] = "Category Updated successfully";
                    _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    Console.WriteLine(e);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _unitOfWork.Category == null)
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


    }
}
