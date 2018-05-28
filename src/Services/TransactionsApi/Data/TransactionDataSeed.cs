using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using TransactionsApi.Domain;

namespace TransactionsApi.Data
{
    public class TransactionDataSeed
    {
        /// <summary>
        /// Save data for Transactions on database. Methods is called when there is not transaction data. 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task SeedAsync(TransactionDbContext context, IHostingEnvironment env)
        {
            var contentRootPath = env.ContentRootPath;
            if(!context.TransactionTypes.Any())
            {
                context.TransactionTypes.AddRange(GetPreconfiguredTransactionTypes());
                await context.SaveChangesAsync();
            }
            if(!context.TransactionItems.Any()){
                var transactionTypeIdLookup = context.TransactionTypes.ToDictionary(ct => ct.Type, ct => ct.Id);
                context.TransactionItems.AddRange(GetTransactionsFromFile(contentRootPath, transactionTypeIdLookup));
                await context.SaveChangesAsync();                
            }
        }
        /// <summary>
        /// Load Transactions from File
        /// </summary>
        /// <param name="contentRootPath"></param>
        /// <param name="transactionTypeIdLookup"></param>
        /// <returns></returns>
        private IEnumerable<TransactionItem> GetTransactionsFromFile(string contentRootPath, Dictionary<string, int> transactionTypeIdLookup)
        {
            string csvFileTransactions = Path.Combine(contentRootPath, "Files", "Transactions.csv");
            if (!File.Exists(csvFileTransactions))
            {
                return GetPreconfiguredTransactions();
            }
            using (StreamReader reader = File.OpenText(csvFileTransactions))
            {
                //IEnumerable<TransactionItem> listTransaction;
                List<TransactionItem> listTransaction = new List<TransactionItem>();
                var csv= new CsvReader(reader);
                csv.Read();
                csv.ReadHeader();               
                while(csv.Read()){
                    var step = (-1)*csv.GetField<int>("step");  
                    string type = csv.GetField<string>("type");                                      
                    var transactionItem=new TransactionItem(){
                            TransactionTypeId=transactionTypeIdLookup[type],
                            Ammount = csv.GetField<double>("amount"),
                            NameOrig = csv.GetField<string>("nameOrig"),
                            OldBalanceOrig = csv.GetField<double>("oldbalanceOrg"),
                            NewBalanceOrig = csv.GetField<double>("newbalanceOrig"),
                            NameDest = csv.GetField<string>("nameDest"),
                            OldBalanceDest = csv.GetField<double>("oldbalanceDest"),
                            NewBalanceDest = csv.GetField<double>("newbalanceDest"),
                            IsFraud = csv.GetField<bool>("isFraud"),
                            TransactionDate = DateTime.Now.AddDays(step)
                    };
                    listTransaction.Add(transactionItem);                  
                }
                return listTransaction;
            }
        }
        /// <summary>
        /// Load Transactions on database
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TransactionItem> GetPreconfiguredTransactions()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Load Transaction types on database
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TransactionType> GetPreconfiguredTransactionTypes()
        {
            return new List<TransactionType>()
            {
                new TransactionType() { Type = "PAYMENT"},
                new TransactionType() { Type = "TRANSFER" },
                new TransactionType() { Type = "CASH_IN" },
                new TransactionType() { Type = "CASH_OUT" },
                new TransactionType() { Type = "DEBIT" }
            };
        }
    }
}