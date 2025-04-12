using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class HomePage : ContentPage
    {
        private readonly Supabase.Client _supabaseClient;
        public ObservableCollection<ImageData> Images { get; set; }

        public HomePage()
        {
            InitializeComponent();

            _supabaseClient = new Supabase.Client(
                "https://xurgyelsilpzbkcrrjqo.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ"
            );

            Images = new ObservableCollection<ImageData>();
            LoadImages(); // Load all initially
            BindingContext = this;

            // ✅ Listen for messages from NotificationView
            MessagingCenter.Subscribe<object, string>(this, "FilterImages", (sender, category) =>
            {
                if (category == "All")
                    LoadImages();
                else
                    LoadImages(category);
            });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object, bool>(this, "ToggleScrollView", true);
            MessagingCenter.Send<object, bool>(this, "ToggleSearchBar", true);
        }
        public async void LoadImages(string category = null)
        {
            try
            {
                string currentUserName = Preferences.Get("UserName", string.Empty);

                if (string.IsNullOrEmpty(currentUserName))
                {
                    await DisplayAlert("Error", "User name is missing.", "OK");
                    return;
                }

                var query = _supabaseClient
                    .From<ImageData>()
                    .Filter("uploader_name", Supabase.Postgrest.Constants.Operator.NotEqual, currentUserName);

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Filter("category", Supabase.Postgrest.Constants.Operator.Equals, category);
                }

                var response = await query.Get();

                Debug.WriteLine(response.Content);

                Images.Clear();
                foreach (var image in response.Models)
                {
                    Images.Add(image);
                }

                if (Images.Count == 0)
                {
                    string catMessage = string.IsNullOrEmpty(category) ? "from other users" : $"in the '{category}' category";
                    await DisplayAlert("No Data", $"No images found {catMessage}.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load images: {ex.Message}", "OK");
            }
        }

        private async void OnImageTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is ImageData imageData)
            {
                await Navigation.PushAsync(new FullScreenImagePage(imageData.ImageUrl, imageData.UploaderName, imageData.Category, imageData.Title));
            }
            else
            {
                await DisplayAlert("Error", "No image data found.", "OK");
            }
        }

        [Table("images")]
        public class ImageData : BaseModel
        {
            [PrimaryKey("id")]
            public int Id { get; set; }

            [Column("image_url")]
            public string ImageUrl { get; set; }

            [Column("uploader_name")]
            public string UploaderName { get; set; }

            [Column("category")]
            public string Category { get; set; }

            [Column("title")]
            public string Title { get; set; }
        }
    }
}
