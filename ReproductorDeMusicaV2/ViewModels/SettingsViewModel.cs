using ReproductorDeMusicaV2.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReproductorDeMusicaV2.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool ToastOnAppEvents
        {
            get
            {
                return SettingsService.Instance.ToastOnAppEvents;
            }
            set
            {
                if (SettingsService.Instance.ToastOnAppEvents != value)
                {
                    SettingsService.Instance.ToastOnAppEvents = value;

                    RaisePropertyChanged("ToastOnAppEvents");
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
