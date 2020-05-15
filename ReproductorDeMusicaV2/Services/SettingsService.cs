using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace ReproductorDeMusicaV2.Services
{
    class SettingsService
    {
        static SettingsService instance;

        public static SettingsService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsService();
                }

                return instance;
            }
        }

        const string ToastOnAppEventsKey = "toast-on-app-events";

        IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;

        public bool ToastOnAppEvents
        {
            get
            {
                object setting;

                if (settings.TryGetValue(ToastOnAppEventsKey, out setting))
                {
                    return (bool)setting;
                }
                // TODO: Cuando este implementada la pagina, cambiar a true
                return true;
            }
            set
            {
                settings[ToastOnAppEventsKey] = value;
            }
        }
    }
}
