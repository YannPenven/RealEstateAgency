using System;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.Model
{
    [Table("transactions")]
    public class Transaction : ViewModels.BaseNotifyPropertyChanged
    {
        public enum TransactionType
        {
            Sale,
            Rent
        }

        #region Propriétés

        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("estate_id"), NotNull]
        public int EstateId { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("owner_id")]
        public int? OwnerId { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("client_id")]
        public int? ClientId { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("type"), NotNull]
        public TransactionType Type { get { return (TransactionType)GetProperty(); } set { SetProperty(value); } }

        [Column("proposal_price"), NotNull]
        public decimal ProposalPrice { get { return (decimal)GetProperty(); } set { SetProperty(value); } }

        [Column("real_price")]
        public decimal? RealPrice { get { return (decimal?)GetProperty(); } set { SetProperty(value); } }

        [Column("proposal_fees"), NotNull]
        public decimal ProposalFees { get { return (decimal)GetProperty(); } set { SetProperty(value); } }

        [Column("real_fees")]
        public decimal? RealFees { get { return (decimal?)GetProperty(); } set { SetProperty(value); } }

        [Column("title")]
        public string Title { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("description")]
        public string Description { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("publication_date")]
        public DateTime? PublicationDate { get { return (DateTime?)GetProperty(); } set { SetProperty(value); } }

        [Column("transaction_done")]
        public bool TransactionDone { get { return (bool)GetProperty(); } set { SetProperty(value); } }

        [Column("transaction_date")]
        public DateTime? TransactionDate { get { return (DateTime?)GetProperty(); } set { SetProperty(value); } }

        #endregion

        public Transaction() : this(false) { }
        public Transaction(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
