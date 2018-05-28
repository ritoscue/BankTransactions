using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionsWeb.Models.TransactionViewModels;

namespace TransactionsWeb.Services
{
    public interface ITransactionService
    {
        Task<int> CreateTransactionAsync(Transaction order, string token);
        Task<TransactionObjectService> GetTransactionsByFilter(int page, int take, bool? fraud, string destino, DateTime? TransactionDate, string token);
        Task<IEnumerable<SelectListItem>> GetTypes();
    }
}
