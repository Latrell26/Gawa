using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp2
{
    public partial class ArtistProfilePage : ContentPage
    {
        readonly Supabase.Client _supabase;

        // Bound to your XAML
        public ObservableCollection<ImageData> Images { get; } = new();
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public string ProfilePictureUrl { get; private set; }

        public ArtistProfilePage(string tappedUserName)
        {
            InitializeComponent();
            BindingContext = this;

            UserName = tappedUserName;
            _supabase = new Supabase.Client(
                "https://xurgyelsilpzbkcrrjqo.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ"
            );

            // Start loading immediately
            _ = LoadProfileAsync();
            _ = LoadImagesAsync();
        }

        // Back button: pop all the way back to root (your main ProfilePage)
        private async void OnBackButtonTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        async Task LoadProfileAsync()
        {
            try
            {
                var resp = await _supabase
                    .From<User>()
                    .Filter("name", Supabase.Postgrest.Constants.Operator.Equals, UserName)
                    .Get();

                var u = resp.Models.FirstOrDefault();
                if (u != null)
                {
                    ProfilePictureUrl = u.ProfilePictureUrl;
                    OnPropertyChanged(nameof(ProfilePictureUrl));
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Profile load error: {ex}");
                await DisplayAlert("Error", "Unable to load profile.", "OK");
            }
        }

        async Task LoadImagesAsync()
        {
            try
            {
                var resp = await _supabase
                    .From<ImageData>()
                    .Filter("uploader_name", Supabase.Postgrest.Constants.Operator.Equals, UserName)
                    .Get();

                Images.Clear();
                foreach (var img in resp.Models)
                    Images.Add(img);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Images load error: {ex}");
                await DisplayAlert("Error", "Unable to load images.", "OK");
            }
        }

        async void OnImageTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is ImageData img)
            {
                await Navigation.PushAsync(new FullScreenImagePage(
                    img.ImageUrl,
                    img.UploaderName,
                    img.Category,
                    img.Title,
                    ProfilePictureUrl
                ));
            }
        }

        [Table("images")]
        public class ImageData : BaseModel
        {
            [PrimaryKey("id")] public int Id { get; set; }
            [Column("image_url")] public string ImageUrl { get; set; }
            [Column("uploader_name")] public string UploaderName { get; set; }
            [Column("category")] public string Category { get; set; }
            [Column("title")] public string Title { get; set; }
        }

        [Table("users")]
        public class User : BaseModel
        {
            [PrimaryKey("id")] public int Id { get; set; }
            [Column("name")] public string Name { get; set; }
            [Column("profile_picture_url")] public string ProfilePictureUrl { get; set; }
        }
    }
}
