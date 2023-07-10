using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Domain.Identity;
using MovieShop.Repository;
using MovieShop.Service.Implementation;
using MovieShop.Domain.Domain;
using System.IO;
using System.Text;
using System;
using GemBox.Document;

namespace MovieShop.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<MovieApplicationUser> _userManager;
        private readonly IOrderService _orderService;

        public OrderController(UserManager<MovieApplicationUser> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        public async Task<IActionResult> UserOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _orderService.getOrdersByUserId(user.Id.ToString());
           if(orders.Count > 0) { 

            return View(orders);
            }
           return View();
        }

        public async Task<IActionResult> ExportOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = _orderService.getOrdersByUserId(user.Id.ToString());

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Orders.docx");

            var tempDocument = DocumentModel.Load(templatePath);
            foreach (var order in orders)
            {


                tempDocument.Content.Replace("{{OrderNumber}}", order.Id.ToString());
                tempDocument.Content.Replace("{{CustomerEmail}}", order.User.Email);
                tempDocument.Content.Replace("{{CostumerInfo}}", (order.User.FirstName + " " + order.User.LastName));

                StringBuilder sb = new StringBuilder();

                var total = 0.0;

                foreach (var orderTicket in order.OrderTicket)
                {
                    var ticket = orderTicket.SelectedTicket;
                    sb.AppendLine($"Ticket Name: {ticket.Name}, Venue: {ticket.Place}, Date: {ticket.Date.ToString("MM/dd/yyyy")}, Genre: {ticket.Genre}, Quantity: {orderTicket.Quantity}");
                    total += ticket.Price * orderTicket.Quantity;
                }

                tempDocument.Content.Replace("{{AllTickets}}", sb.ToString());
                tempDocument.Content.Replace("{{TotalPrice}}", "$"+total.ToString());

            }

            var stream = new MemoryStream();

            tempDocument.Save(stream, new PdfSaveOptions());


            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "MyOrders.pdf");
        }


    }
}
