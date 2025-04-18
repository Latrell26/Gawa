using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Supabase;
using MauiApp2.Models;

namespace MauiApp2
{
    public partial class CreatePage : ContentPage
    {
        private const string SupabaseUrl = "https://xurgyelsilpzbkcrrjqo.supabase.co";
        private const string SupabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ";
        private Supabase.Client supabase;
        private FileResult pickedImageFile;

        public CreatePage()
        {
            InitializeComponent();
            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            supabase = new Supabase.Client(SupabaseUrl, SupabaseApiKey);
            await supabase.InitializeAsync();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetForm();

            base.OnAppearing();
            MessagingCenter.Send<object, bool>(this, "ToggleScrollView", false);
            MessagingCenter.Send<object, bool>(this, "ToggleSearchBar", false);
        }
        private async void ResetForm()
        {
            pickedImageFile = null;

            previewImage.Source = null;
            previewImage.IsVisible = false;
            plusLabel.IsVisible = true;

            titleEntry.Text = string.Empty;
            categoryPicker.SelectedIndex = -1;

            formLayout.IsVisible = false;
        }


        private async void OnImageBoxTapped(object sender, EventArgs e)
        {
            try
            {
                pickedImageFile = await MediaPicker.PickPhotoAsync();

                if (pickedImageFile != null)
                {
                    // Set image preview
                    previewImage.Source = ImageSource.FromFile(pickedImageFile.FullPath);
                    previewImage.IsVisible = true;
                    plusLabel.IsVisible = false;
                    formLayout.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }



        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (pickedImageFile == null)
            {
                await DisplayAlert("Error", "Please select an image.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(titleEntry.Text) || categoryPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please enter title and select category.", "OK");
                return;
            }

            try
            {
                using var stream = await pickedImageFile.OpenReadAsync();
                string fileName = $"{Guid.NewGuid()}.jpg";
                string imageUrl = await UploadImageToSupabase(stream, fileName);

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    string uploaderName = Preferences.Get("UserName", "Unknown");
                    var imageData = new ImageRecord
                    {
                        ImageUrl = imageUrl,
                        Title = titleEntry.Text,
                        Category = categoryPicker.SelectedItem.ToString(),
                        UploaderName = uploaderName
                    };

                    await supabase.From<ImageRecord>().Insert(imageData);
                    await DisplayAlert("Success", "Image uploaded successfully!", "OK");

                    // Clear form
                    pickedImageFile = null;
                    titleEntry.Text = "";
                    categoryPicker.SelectedIndex = -1;
                    previewImage.Source = null;
                    previewImage.IsVisible = false;
                    plusLabel.IsVisible = true;
                    formLayout.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Upload Failed", ex.Message, "OK");
            }
        }

        private async Task<string> UploadImageToSupabase(Stream stream, string fileName)
        {
            var storage = supabase.Storage.From("images");

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            await storage.Upload(bytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });

            return $"{SupabaseUrl}/storage/v1/object/public/images/{fileName}";
        }
    }
}
