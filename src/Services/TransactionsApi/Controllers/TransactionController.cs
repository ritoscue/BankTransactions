using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransactionsApi.Data;
using TransactionsApi.Domain;
using TransactionsApi.ViewModel;

namespace TransactionsApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Transaction")]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionController : Controller
    {
        private readonly TransactionDbContext _transactionContext;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DbContext is received for DI</param>
        public TransactionController(TransactionDbContext context)
        {
            _transactionContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <summary>
        /// Save transaction on database
        /// </summary>
        /// <param name="transaction">Object type transaction</param>
        /// <returns></returns>
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
        /// <summary>
        /// Get transaction by Id
        /// </summary>
        /// <param name="id">Id Transaction to get</param>
        /// <returns></returns>
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
        /// <summary>
        /// Get elements catalog types
        /// </summary>
        /// <returns></returns>
        // GET api/[controller]/TransactionTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> TransactionTypes()
        {
            var items = await _transactionContext.TransactionTypes.ToListAsync();
            return Ok(items);
        }

        /// <summary>
        /// Get transaction list by filters
        /// </summary>
        /// <param name="isFraud">Are transactions fraud or no?</param>
        /// <param name="nameDest">Destination</param>
        /// <param name="transactionDate">Transaction date</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
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
            return Ok(model);

        }
      

    }
}