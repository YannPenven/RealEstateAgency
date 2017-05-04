using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.Model
{
    [Table("photos")]
    public class Photo : ViewModels.BaseNotifyPropertyChanged
    {
        #region Propriétés

        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("estate_id"), NotNull]
        public int EstateId { get { return (int)GetProperty(); } set { SetProperty(value); } }

        [Column("base64_photo"), NotNull]
        public string Base64Photo { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("title")]
        public string Title { get { return (string)GetProperty(); } set { SetProperty(value); } }

        [Column("shooting_date"), NotNull]
        public DateTime ShootingDate { get { return (DateTime)GetProperty(); } set { SetProperty(value); } }

        #endregion

        public Photo() : this(false) { }
        public Photo(bool synchronizeWithContext = false) : base(synchronizeWithContext) { }
    }
}
