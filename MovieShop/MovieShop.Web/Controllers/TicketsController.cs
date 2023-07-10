using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.Domain;
using MovieShop.Domain.DTO;
using MovieShop.Service.Implementation;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace MovieShop.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: Tickets
        public IActionResult Index(DateTime? dateFilter)
        {
            var allTickets = _ticketService.GetAllTickets(dateFilter);
            return View(allTickets);
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Date,Place,Url,Genre,Price")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                this._ticketService.CreateNewTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
        public IActionResult AddTicketToCard(Guid? id)
        {
            var model = this._ticketService.GetShoppingCartInfo(id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTicketToCard([Bind("TicketId", "Quantity")] AddedToShoppingCartDTO item)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = this._ticketService.AddToShoppingCart(item, userId);
            if (result)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            return View(item);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,Name,Date,Place,Url,Genre,Price")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.UpdeteExistingTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsForTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsForTicket(id) != null;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public FileContentResult ExportAllTickets()
        {
            string fileName = "Tickets.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Tickets");
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Date";
                worksheet.Cell(1, 4).Value = "Place";
                worksheet.Cell(1, 5).Value = "Picture URL";
                worksheet.Cell(1, 6).Value = "Genre";
                worksheet.Cell(1, 7).Value = "Price";

                var result = _ticketService.GetAllTickets();
                for (int i = 1; i <= result.Count; i++)
                {
                    var item = result[i - 1];
                    worksheet.Cell(i + 1, 2).Value = item.Name;
                    worksheet.Cell(i + 1, 3).Value = item.Date.ToString("MM/dd/yyyy");
                    worksheet.Cell(i + 1, 4).Value = item.Place;
                    worksheet.Cell(i + 1, 5).Value = item.Url;
                    worksheet.Cell(i + 1, 6).Value = item.Genre;
                    worksheet.Cell(i + 1, 7).Value = "$"+item.Price.ToString();
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }
    }
}
