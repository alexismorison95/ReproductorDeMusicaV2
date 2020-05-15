using ReproductorDeMusicaV2.DataModels;
using ReproductorDeMusicaV2.Services;
using ReproductorDeMusicaV2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace ReproductorDeMusicaV2.Pages
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CurrentPlayingBar : Page
    {
        MediaPlayer Player => PlaybackService.Instance.Player;

        MediaPlaybackList PlaybackList
        {
            get { return Player.Source as MediaPlaybackList; }
            set { Player.Source = value; }
        }
        MediaList MediaList
        {
            get { return PlaybackService.Instance.CurrentPlaylist; }
            set { PlaybackService.Instance.CurrentPlaylist = value; }
        }

        public PlayerViewModel PlayerViewModel { get; set; }

        public CurrentPlayingBar()
        {
            this.InitializeComponent();

            // Never reuse the cached page because the model is designed to be unloaded and disposed
            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            // Setup MediaPlayer view model
            PlayerViewModel = new PlayerViewModel(Player, Dispatcher);

            this.Loaded += CurrentPlayingBar_Loaded;
            this.Unloaded += CurrentPlayingBar_Unloaded;
        }

        private void CurrentPlayingBar_Unloaded(object sender, RoutedEventArgs e)
        {
            PlaybackList.ItemFailed -= PlaybackList_ItemFailed;
            PlayerViewModel.Dispose();
            PlayerViewModel = null;

            GC.Collect();
        }

        private async void CurrentPlayingBar_Loaded(object sender, RoutedEventArgs e)
        {
            // Bind player to element
            mediaPlayerElement.SetMediaPlayer(Player);

            // Load the playlist data model if needed
            if (MediaList == null)
            {
                // Create the playlist data model
                MediaList = new MediaList();

                try
                {
                    await MediaList.LoadFromApplicationUriAsync("ms-appx:///Assets/Data/playlist.json");
                }
                catch (Exception ex)
                {
                    var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

                    var toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode("Exception"));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode(ex.Message));

                    var toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
                
            }

            // Create a new playback list matching the data model if one does not exist
            if (PlaybackList == null)
            {
                PlaybackList = MediaList.ToPlaybackList();
            }

            // Subscribe to playback list item failure events
            PlaybackList.ItemFailed += PlaybackList_ItemFailed;

            // Create the view model list from the data model and playback model
            // and assign it to the player
            PlayerViewModel.MediaList = new MediaListViewModel(MediaList, PlaybackList, Dispatcher);
        }

        private async void PlaybackList_ItemFailed(MediaPlaybackList sender, MediaPlaybackItemFailedEventArgs args)
        {
            // Media callbacks use a worker thread so dispatch to UI as needed
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var error = string.Format("Item failed to play: {0} | 0x{1:x}",
                    args.Error.ErrorCode, args.Error.ExtendedError.HResult);
                //MainPage.Current.NotifyUser(error, NotifyType.ErrorMessage);
            });
        }
    }
}
