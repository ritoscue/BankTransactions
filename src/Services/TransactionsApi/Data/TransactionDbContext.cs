using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionsApi.Domain;

namespace TransactionsApi.Data
{
    public class TransactionDbContext :DbContext
    {
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TransactionType>(ConfigureTransactionType);
            builder.Entity<TransactionItem>(ConfigureTransactionItem);
        }
        /// <summary>
        /// Custom configuration for Transaction Type
        /// </summary>
        /// <param name="builder"></param>
        private void ConfigureTransactionType(EntityTypeBuilder<TransactionType> builder)
        {
            builder.ToTable("TransactionType");

            builder.HasKey(ti => ti.Id);
            builder.Property(ti => ti.Id)
               .ForSqlServerUseSequenceHiLo("Transaction_type_hilo")
               .IsRequired();
            builder.Property(tb => tb.Type)
                .IsRequired()
                .HasMaxLength(50);
        }

        /// <summary>
        /// Custom configuration for transaction item
        /// </summary>
        /// <param name="builder"></param>
        private void ConfigureTransactionItem(EntityTypeBuilder<TransactionItem> builder)
        {
            builder.ToTable("Transaction");
            builder.Property(ti => ti.Id)
                .ForSqlServerUseSequenceHiLo("transaction_hilo")
                .IsRequired();

            builder.HasOne(ti => ti.TransactionType)
                .WithMany()
                .HasForeignKey(ti => ti.TransactionTypeId);

            builder.Property(ti => ti.NameOrig)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ti => ti.NameDest)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ti => ti.Ammount)
                .IsRequired(true);
            
        }

        

    }
}