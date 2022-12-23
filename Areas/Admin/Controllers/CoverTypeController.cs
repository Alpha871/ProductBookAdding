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
	public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        // GET: Category
        public IActionResult Index()
        {
            IEnumerable<CoverType> listOfObjet = _unitOfWork.CoverType.GetAll();
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
        public IActionResult Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "CoverType created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _unitOfWork.CoverType == null)
            {
                return NotFound();
            }

            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        // POST: Category/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CoverType coverType)
        {
            if (id != coverType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.CoverType.Update(coverType);
                    TempData["success"] = "CoverType Updated successfully";
                    _unitOfWork.Save();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    Console.WriteLine(e);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _unitOfWork.CoverType == null)
            {
                return NotFound();
            }

            var coverType = _unitOfWork.CoverType
                .GetFirstOrDefault(m => m.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_unitOfWork.CoverType == null)
            {
                return Problem("Entity set 'DataProduct.CoverType'  is null.");
            }
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (coverType != null)
            {
                _unitOfWork.CoverType.Remove(coverType);
                TempData["success"] = "CoverType deleted successfully";
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


    }
}
