using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionsWeb.Models.TransactionViewModels
{
    public class TransactionObjectService
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public List<Transaction> Data { get; set; }
    }
}
