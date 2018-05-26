using System;

namespace TransactionsApi.Domain
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public int TransactionTypeId { get; set; }
        public double Ammount{get;set;}
        public string NameOrig {get;set;}
        public double OldBalanceOrig {get;set;}
        public double NewBalanceOrig {get;set;}
        public string NameDest {get;set;}
        public double OldBalanceDest {get;set;}
        public double NewBalanceDest {get;set;}
        public bool IsFraud{get;set;} 
        public DateTime TransactionDate {get;set;}       
        public TransactionType TransactionType { get; set; }
    }
}