using BulkyBook.DataAccess.Reprository.IReprository;
using BulkyBook.Model.Models;
using BulkyBook.Model.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
	{
		

		private readonly IUnitOfWork _unitOfWork;
        public OrderVM orderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
            _unitOfWork=unitOfWork;

        }
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Index(int orderId)
        {
            orderVM = new OrderVM
            {
                orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                orderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product")
            };
            return View(orderVM);
        }
        #region API CALLS
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(Commun.Role_Admin) || User.IsInRole(Commun.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUser.Id == claim.Value,includeProperties: "ApplicationUser");

            }



            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == Commun.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Commun.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Commun.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Commun.StatusApproved);
                    break;
                default:
                    break;

            }





            return Json(new { data = orderHeaders });
		}
		#endregion
	}
}
