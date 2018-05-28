using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransactionsWeb.Data;
using TransactionsWeb.Models;
using TransactionsWeb.Models.TransactionViewModels;
using TransactionsWeb.Services;

namespace TransactionsWeb.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ITransactionService _transactionService;

        public TransactionController(ApplicationDbContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        // GET: Transaction
        public async Task<IActionResult> Index(bool Fraud, DateTime TransactionDate,  string destination, int? page)
        {
            int itemsPage = 10;
            page = page ?? 1;
            if (TempData["token"] != null)
            {
                string token = TempData["token"].ToString();
                var transactionitems = await _transactionService.GetTransactionsByFilter((int)(page - 1), itemsPage, Fraud, destination, TransactionDate, token);
                return View(new PaginatedList<Transaction>(transactionitems.Data, transactionitems.Count, transactionitems.PageIndex, transactionitems.PageSize));
            }
            else
            {
                return NotFound();
            }            
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .SingleOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TransactionTypeId,Ammount,NameOrig,OldBalanceOrig,NewBalanceOrig,NameDest,OldBalanceDest,NewBalanceDest,IsFraud,TransactionDate")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (TempData["token"] != null)
                {
                    string token = TempData["token"].ToString();
                    await _transactionService.CreateTransactionAsync(transaction, token);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.SingleOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TransactionTypeId,Ammount,NameOrig,OldBalanceOrig,NewBalanceOrig,NameDest,OldBalanceDest,NewBalanceDest,IsFraud,TransactionDate")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .SingleOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

       

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
