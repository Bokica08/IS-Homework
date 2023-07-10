using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Service.Implementation;
using MovieShop.Domain.Identity;
using Stripe;
using MovieShop.Domain.Domain;
using System.Collections.Generic;

namespace MovieShop.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shopingCartService;
        private readonly UserManager<MovieApplicationUser> _userManager;
        private readonly IEmailService _emailService;


        public ShoppingCartController(IEmailService emailService,IShoppingCartService shopingCartService, UserManager<MovieApplicationUser> userManager)
        {
            _shopingCartService = shopingCartService;
            _userManager = userManager;
            _emailService = emailService;
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
        public IActionResult PayOrder(string stripeEmail, string stripeToken)
        {
            var customerService = new CustomerService();
            var chargeService = new ChargeService();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = this._shopingCartService.getShoppingCartInfo(userId);

            var customer = customerService.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = chargeService.Create(new ChargeCreateOptions
            {
                Amount = (Convert.ToInt32(order.Total) * 100),
                Description = "Tickets Application Payment",
                Currency = "usd",
                Customer = customer.Id
            });

            if (charge.Status == "succeeded")
            {
                var result = this.OrderNow();

                if (result)
                {
                    var emailMessage = new List<EmailMessage>
            {
                new EmailMessage
                {
                    MailTo = stripeEmail,
                    Subject = "Your order has been placed",
                    Content = "You have made succefull order. You have paid: $"+order.Total+"."
                }
            };

                    _emailService.SendEmailAsync(emailMessage);
                    return RedirectToAction("Index", "ShoppingCart");
                }
                else
                {
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            return RedirectToAction("Index", "ShoppingCart");
        }
        private Boolean OrderNow()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._shopingCartService.orderNow(userId);
            return result;
        }
    }
}
