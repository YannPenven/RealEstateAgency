using RealEstateAgency.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using RealEstateAgency.Core.Interfaces;

namespace RealEstateAgency.Core.ViewModels
{
    public class EstatesViewModel : NavigateViewModel
    {
        public class EstateWithPhoto : BaseNotifyPropertyChanged
        {
            private const string DEFAULT_PHOTO = "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";

            private long _currentPhotoId = 0;
            private string _currentPhotoBase64 = DEFAULT_PHOTO;
            private EstatesViewModel _parent = null;

            public Model.Estate Estate
            {
                get { return (Model.Estate)GetProperty(); }
                set { if (SetProperty(value)) OnPropertyChanged("Photo"); }
            }
            public string Photo
            {
                get
                {
                    if (Estate == null && _currentPhotoId != 0)
                    {
                        _currentPhotoId = 0;
                        _currentPhotoBase64 = DEFAULT_PHOTO;
                    }
                    else if (Estate != null && _currentPhotoId != Estate.MainPhotoId)
                    {
                        if (Estate.MainPhotoId == null || Estate.MainPhotoId == 0)
                        {
                            _currentPhotoId = 0;
                            _currentPhotoBase64 = DEFAULT_PHOTO;
                        }
                        else
                        {
                            Model.Photo photo = _parent.DBConn.SelectItem<Model.Photo>((p) => p.EstateId == Estate.Id
                                                                                           && p.Id == Estate.MainPhotoId);
                            _parent.Errors.AddRange(_parent.DBConn.Errors);

                            if (_parent.DBConn.Errors.Count == 0)
                            {
                                _currentPhotoId = photo.Id;
                                _currentPhotoBase64 = photo.Base64Photo;
                            }
                            else
                            {
                                _currentPhotoId = 0;
                                _currentPhotoBase64 = DEFAULT_PHOTO;
                            }

                        }
                    }

                    return _currentPhotoBase64;
                }
            }

            internal EstateWithPhoto(EstatesViewModel parent) : base()
            {
                _parent = parent;
            }
        }
        protected DataAccess.Connection DBConn
        {
            get
            {
                return DataAccess.Connection.
                  GetCurrentAsync().ExecuteSynchronously();
            }
        }

        public string Message
        {
            get { return (string)GetProperty(); }
            set { SetProperty(value); }
        }

        public ObservableCollection<EstateWithPhoto> EstatesList
        {
            get { return (ObservableCollection <EstateWithPhoto>) GetProperty(); }
            set { SetProperty(value); }
        }

        public EstatesViewModel(INavigationService navigationService, params INavigationPage[] pages) : this(false, navigationService, pages) { }
        public EstatesViewModel(bool synchronizeWithContext, INavigationService navigationService, params INavigationPage[] pages) : base(synchronizeWithContext, navigationService, pages)
        {
            _navParameterChanged = () => Message = (string)_navParameter;
        }


        public override async Task Initialize()
        {
            ObservableCollection<Model.Estate> list = new ObservableCollection<Model.Estate>();
            list = await DBConn.SelectItemsAsync<Model.Estate>();
            this.EstatesList = await DBConn.SelectItemsAsync<EstateWithPhoto>();
       
            if(this.EstatesList == null)
            {
                EstatesList = new ObservableCollection<EstateWithPhoto>();
            }
            if (!ErrorsExists)
            {
                foreach (Model.Estate e in list)
                {
                    EstatesList.Add(new EstateWithPhoto(this) { Estate = e });
                }
            }
        }

    }


}
