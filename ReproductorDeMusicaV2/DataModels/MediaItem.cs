using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace ReproductorDeMusicaV2.DataModels
{
    public class MediaItem
    {
        public const string MediaItemIdKey = "mediaItemId";

        Uri previewImageUri;

        public string ItemId { get; set; }

        public string Title { get; set; }

        public Uri MediaUri { get; set; }

        public virtual Uri PreviewImageUri
        {
            get { return previewImageUri; }
            set { previewImageUri = value; }
        }

        public MediaItem() { }

        public MediaItem(JsonObject json) : this()
        {
            ItemId = json.GetNamedString("id", Guid.NewGuid().ToString());

            Title = json.GetNamedString("title", string.Empty);

            if (json.Keys.Contains("mediaUri"))
            {
                MediaUri = new Uri(json.GetNamedString("mediaUri"));
            }
        }

        public virtual MediaPlaybackItem ToPlaybackItem()
        {
            // Create the media source from the Uri
            var source = MediaSource.CreateFromUri(MediaUri);

            // Create a configurable playback item backed by the media source
            var playbackItem = new MediaPlaybackItem(source);

            var displayProperties = playbackItem.GetDisplayProperties();

            // Populate thumbnail
            if (PreviewImageUri != null)
            {
                displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(PreviewImageUri);
            }

            // Apply properties to the playback item
            playbackItem.ApplyDisplayProperties(displayProperties);

            source.CustomProperties[MediaItem.MediaItemIdKey] = ItemId;

            return playbackItem;
        }
    }
}
