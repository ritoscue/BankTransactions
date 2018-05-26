using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionsApi.Data;
using TransactionsApi.Domain;
using TransactionsApi.ViewModel;

namespace TransactionsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Transaction")]
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _transactionContext;

        public TransactionController(TransactionDbContext context)
        {
            _transactionContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        //POST api/[controller]/items
        [Route("items")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody]TransactionItem transaction)
        {
            var item =  new TransactionItem{
                TransactionTypeId = transaction.TransactionTypeId,
                Ammount=transaction.Ammount,
                NameOrig=transaction.NameOrig,
                NameDest=transaction.NameDest,
                TransactionDate=DateTime.Now
            };
            _transactionContext.TransactionItems.Add(item);
            await _transactionContext.SaveChangesAsync();
             return CreatedAtAction(nameof(GetTransactionById), new { id = item.Id }, null);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetTransactionById(int id){
            if(id<=0){
                return BadRequest();
            }
            var item =  await _transactionContext.TransactionItems.SingleOrDefaultAsync(ti => ti.Id==id);
            if(item!=null)
                return Ok(item);
            return NotFound();
        }

        // GET api/[controller]/TransactionTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _transactionContext.TransactionTypes.ToListAsync();
            return Ok(items);
        }

        // GET api/[controller]/items/isFraud/1/nameDest/null/transactionDate/null[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/isFraud/{isFraud}/nameDest/{nameDest}/transactionDate/{transactionDate}")]
        public async Task<IActionResult> Items(bool? isFraud, string nameDest, DateTime? transactionDate, [FromQuery]int pageSize = 6, [FromQuery]int pageIndex = 0)
        {
            var query =  (IQueryable<TransactionItem>)_transactionContext.TransactionItems;
            if(isFraud.HasValue){
                query = query.Where(ti => ti.IsFraud == isFraud);
            }
            if(!string.IsNullOrEmpty(nameDest))
            {                
                query = query.Where(ti => ti.NameDest.StartsWith(nameDest));
            }
            if(transactionDate.HasValue){
                query =  query.Where(ti => ti.TransactionDate==transactionDate);
            }
            var totalItems = await query.LongCountAsync();
            var itemsOnPage= await query
                .Skip(pageSize * pageIndex)
                .OrderBy(c => c.NameOrig)
                .Take(pageSize)
                .ToListAsync();
            var model = new PaginatedItemsViewModel<TransactionItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);
            return Ok(itemsOnPage);

        }
      

    }
}