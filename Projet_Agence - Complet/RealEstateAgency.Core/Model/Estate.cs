using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.Model
{
    [Table("estates")]
    public class Estate : ViewModels.BaseNotifyPropertyChanged
    {
        public enum EstateType
        {
            House,
            Flat,
            Field,
            Garage,
            CommercialLocal
        }

        #region Propriétés

        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("main_photo_id")]
        public int? MainPhotoId { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("commercial_id")]
        public int? CommercialId { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("estimated_price")]
        public decimal? EstimatedPrice { get { return (decimal?)GetProperty(); } set { SetProperty(value); } }

        [Column("annual_charges")]
        public decimal? AnnualCharges { get { return (decimal?)GetProperty(); } set { SetProperty(value); } }

        [Column("property_taxes")]
        public decimal? PropertyTaxes { get { return (decimal?)GetProperty(); } set { SetProperty(value); } }

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

        [Column("type"), NotNull]
        public EstateType Type { get { return (EstateType)GetProperty(); } set { SetProperty(value); } }

        [Column("surface")]
        public double? Surface { get { return (double?)GetProperty(); } set { SetProperty(value); } }

        [Column("rooms_count")]
        public int? RoomsCount { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("floor_number")]
        public int? FloorNumber { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("floors_count")]
        public int? FloorsCount { get { return (int?)GetProperty(); } set { SetProperty(value); } }

        [Column("elevator")]
        public bool? Elevator { get { return (bool?)GetProperty(); } set { SetProperty(value); } }


        #endregion

        public Estate() : this(false) { }
        public Estate(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
