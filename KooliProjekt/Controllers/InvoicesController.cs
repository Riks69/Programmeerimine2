using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Models;
using KooliProjekt.Search;

namespace KooliProjekt.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // GET: Invoices
        public async Task<IActionResult> Index(int page = 1, InvoiceIndexModel model = null)
        {
            model = model ?? new InvoiceIndexModel();
            model.Data = await _invoiceService.List(page, 5, model.Search);

            return View(model);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceService.Get(id.Value);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Invoice Invoice)
        {
            if (ModelState.IsValid)
            {
                await _invoiceService.Save(Invoice);

                return RedirectToAction(nameof(Index));
            }
            return View(Invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoList = await _invoiceService.Get(id.Value);
            if (todoList == null)
            {
                return NotFound();
            }
            return View(todoList);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] Invoice Invoice)
        {
            if (id != Invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _invoiceService.Save(Invoice);
                return RedirectToAction(nameof(Index));
            }
            return View(Invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceService.Get(id.Value);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceService.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
