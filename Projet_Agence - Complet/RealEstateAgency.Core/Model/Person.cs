using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.Model
{
    [Table("persons")]
    public class Person : ViewModels.BaseNotifyPropertyChanged
    {
        #region Propriétés

        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("last_name"), NotNull]
        public string LastName { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("first_name")]
        public string FirstName { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("phone")]
        public string Phone { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("cellphone")]
        public string CellPhone { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("mail")]
        public string Mail { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("address"), NotNull]
        public string Address { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("zip"), NotNull]
        public string Zip { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("city"), NotNull]
        public string City { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("longitude")]
        public double? Longitude { get { return (double?)GetProperty(); } set { SetProperty(value); } }

        [Column("latitude")]
        public double? Latitude { get { return (double?)GetProperty(); } set { SetProperty(value); } }

        [Column("altitude")]
        public double? Altitude { get { return (double?)GetProperty(); } set { SetProperty(value); } }

        #endregion

        public Person() : this(false) { }
        public Person(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
