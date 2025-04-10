using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Supabase;
using Supabase.Storage;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MauiApp2
{

    public partial class CreatePage : ContentPage
    {
        private const string SupabaseUrl = "https://xurgyelsilpzbkcrrjqo.supabase.co";
        private const string SupabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ";
        private Supabase.Client supabase;

        public CreatePage()
        {
            InitializeComponent();
            InitializeSupabase();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object, bool>(this, "ToggleScrollView", false);
            MessagingCenter.Send<object, bool>(this, "ToggleSearchBar", false);
        }
        private async void InitializeSupabase()
        {
            supabase = new Supabase.Client(SupabaseUrl, SupabaseApiKey);
            await supabase.InitializeAsync();
        }


        private async void OnUploadImageClicked(object sender, EventArgs e)
        {
            try
            {
                if (categoryPicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(titleEntry.Text))
                {
                    await DisplayAlert("Error", "Please enter a title and select a category before uploading.", "OK");
                    return;
                }

                var result = await MediaPicker.PickPhotoAsync();
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    string fileName = $"{Guid.NewGuid()}.jpg";
                    string imageUrl = await UploadImageToSupabase(stream, fileName);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string selectedCategory = categoryPicker.SelectedItem.ToString();
                        string enteredTitle = titleEntry.Text; 

                        await InsertImageIntoTable(imageUrl, selectedCategory, enteredTitle);

                        await DisplayAlert("Success", "Image uploaded a", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to pick an image: " + ex.Message, "OK");
            }
        }
        private async Task InsertImageIntoTable(string imageUrl, string category, string title)
        {
            try
            {
                string uploaderName = Preferences.Get("UserName", "Unknown"); 

                var newImage = new ImageRecord
                {
                    ImageUrl = imageUrl,
                    Category = category,
                    UploaderName = uploaderName,
                    Title = title 
                };

                await supabase.From<ImageRecord>().Insert(newImage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to insert image URL into database: " + ex.Message, "OK");
            }
        }
        private async Task<string> UploadImageToSupabase(Stream fileStream, string fileName)
        {
            try
            {
                var storage = supabase.Storage.From("images");
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();

                await storage.Upload(fileBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
                return $"{SupabaseUrl}/storage/v1/object/public/images/{fileName}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Image upload failed: " + ex.Message, "OK");
                return null;
            }
        }
    }
    [Table("images")]
    public class ImageRecord : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("uploader_name")] 
        public string UploaderName { get; set; }

        [Column("title")] 
        public string Title { get; set; }
    }
}
