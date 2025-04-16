using Microsoft.Maui.Controls;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using MauiApp2.Models;

namespace MauiApp2
{
    [Table("users")]
    public class UserRecord : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("profile_picture_url")]
        public string ProfilePictureUrl { get; set; }
    }

    public partial class EditProfilePage : ContentPage
    {
        private const string SupabaseUrl = "https://xurgyelsilpzbkcrrjqo.supabase.co";
        private const string SupabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ"
;

        private Supabase.Client supabase;
        private FileResult selectedImage;

        public string UserName { get; set; }

        public EditProfilePage()
        {
            InitializeComponent();
            InitializeSupabase();
            LoadUserProfile();
        }

        private async void InitializeSupabase()
        {
            supabase = new Supabase.Client(SupabaseUrl, SupabaseApiKey);
            await supabase.InitializeAsync();
        }

        private async void LoadUserProfile()
        {
            string email = Preferences.Get("UserEmail", string.Empty);
            var response = await supabase
                .From<UserRecord>()
                .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
                .Get();

            if (response.Models.Count > 0)
            {
                var user = response.Models[0] as UserRecord;
                UserName = user.Name;
                profileImagePreview.Source = user.ProfilePictureUrl;
                nameEntry.Text = user.Name;
            }
        }

        private async void OnChooseImageClicked(object sender, EventArgs e)
        {
            try
            {
                selectedImage = await MediaPicker.PickPhotoAsync();

                if (selectedImage != null)
                {
                    profileImagePreview.Source = ImageSource.FromFile(selectedImage.FullPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to pick an image: " + ex.Message, "OK");
            }
        }

       private async void OnDoneButtonClicked(object sender, EventArgs e)
{
    string newName = nameEntry.Text;

    // Validate the new name
    if (string.IsNullOrWhiteSpace(newName) || newName.Length > 10)
    {
        await DisplayAlert("Error", "Name must be less than 10 characters.", "OK");
        return;
    }

    try
    {
        string imageUrl = null;

        if (selectedImage != null)
        {
            using var stream = await selectedImage.OpenReadAsync();
            string fileName = $"{Guid.NewGuid()}.jpg";
            imageUrl = await UploadImageToSupabase(stream, fileName);
        }

        string email = Preferences.Get("UserEmail", string.Empty);
        var response = await supabase
            .From<UserRecord>()
            .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, email)
            .Get();

        if (response.Models.Count > 0)
        {
            var user = response.Models[0] as UserRecord;
            string oldName = user.Name;  // Store the old name for updating the images table
            user.Name = newName;

            // Update the profile picture URL if a new image was uploaded
            if (!string.IsNullOrEmpty(imageUrl))
            {
                user.ProfilePictureUrl = imageUrl;
            }

            // Update the user table
            var updateUserResponse = await supabase.From<UserRecord>().Update(user);

            // Check if there is an issue with updating the user record
            if (updateUserResponse.Models.Count == 0)
            {
                await DisplayAlert("Error", "Failed to update user profile.", "OK");
                return;
            }

            // Now update the uploader_name in the images table
            var updateImageResponse = await supabase
                .From<ImageRecord>()
                .Filter("uploader_name", Supabase.Postgrest.Constants.Operator.Equals, oldName)
                .Set(x => x.UploaderName, newName)
                .Update();

            // Check if there was an issue updating the image record
            if (updateImageResponse.Models.Count == 0)
            {
                await DisplayAlert("Error", "Failed to update image uploader name.", "OK");
                return;
            }

            await DisplayAlert("Success", "Profile updated successfully.", "OK");
            await Navigation.PushAsync(new ProfilePage());
                
            }
        else
        {
            await DisplayAlert("Error", "User not found in the database.", "OK");
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", "Failed to update profile: " + ex.Message, "OK");
    }
}



        private async Task<string> UploadImageToSupabase(Stream stream, string fileName)
        {
            var storage = supabase.Storage.From("images");
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            await storage.Upload(fileBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
            return $"{SupabaseUrl}/storage/v1/object/public/images/{fileName}";
        }
    }
}
