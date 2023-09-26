using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Service.Implementation;
using MovieShop.Domain.Identity;
using Stripe;
using MovieShop.Domain.Domain;
using System.Collections.Generic;
using DocumentFormat.OpenXml.InkML;
using MovieShop.Repository.Interface;

namespace MovieShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shopingCartService;
        private readonly UserManager<MovieApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
 


        public ShoppingCartController(IEmailService emailService,IShoppingCartService shopingCartService, 
            UserManager<MovieApplicationUser> userManager, IUserRepository userRepository)
        {
            _shopingCartService = shopingCartService;
            _userManager = userManager;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(this._shopingCartService.getShoppingCartInfo(userId));
        }
        public IActionResult DeleteFromShoppingCart(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            this._shopingCartService.deleteTicketFromShoppingCart(userId, id);
            return RedirectToAction("Index", "ShoppingCart");
        }
        public IActionResult PayOrder()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = _userRepository.Get(userId).Email;

            this.OrderNow();

            var order = this._shopingCartService.getShoppingCartInfo(userId);

   
                    var emailMessage = new List<EmailMessage>
            {
                new EmailMessage
                {
                    MailTo = userEmail,
                    Subject = "Your order has been placed", 
                    Content = "You have made succefull order. You have paid: $"+order.Total+"."
                }
            };

                    _emailService.SendEmailAsync(emailMessage);
                    return RedirectToAction("UserOrders", "Order");
                }
   
        private Boolean OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._shopingCartService.orderNow(userId);
            return result;
        }
    }
}
