﻿using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using System.Linq;

namespace MauiApp2
{
    [QueryProperty(nameof(UserEmail), "email")]
    [QueryProperty(nameof(UserName), "name")]
    public partial class ProfilePage : ContentPage
    {
        private string _userEmail;
        private string _userName;
        private string _profilePictureUrl;

        private readonly Supabase.Client _supabaseClient;

        public ObservableCollection<ImageData> Images { get; set; } = new ObservableCollection<ImageData>();

        public string UserEmail
        {
            get => _userEmail;
            set
            {
                if (_userEmail != value)
                {
                    _userEmail = value;
                    OnPropertyChanged(nameof(UserEmail));
                }
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                    userNameLabel.Text = _userName;
                }
            }
        }

        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                if (_profilePictureUrl != value)
                {
                    _profilePictureUrl = value;
                    OnPropertyChanged(nameof(ProfilePictureUrl));
                }
            }
        }

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = this;

            _supabaseClient = new Supabase.Client(
                "https://xurgyelsilpzbkcrrjqo.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ"

            );

            LoadUserPreferences();
            LoadUserProfilePicture();
            LoadImages();
        }

        private void LoadUserPreferences()
        {
            if (Preferences.ContainsKey("UserEmail"))
                UserEmail = Preferences.Get("UserEmail", string.Empty);

            if (Preferences.ContainsKey("UserName"))
                UserName = Preferences.Get("UserName", string.Empty);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object, bool>(this, "ToggleScrollView", false);
            MessagingCenter.Send<object, bool>(this, "ToggleSearchBar", false);

            LoadImages();
            LoadUserProfilePicture();
        }

        private async void LoadUserProfilePicture()
        {
            try
            {
                var response = await _supabaseClient
                    .From<User>()
                    .Filter("email", Supabase.Postgrest.Constants.Operator.Equals, UserEmail)
                    .Get();

                var user = response.Models.FirstOrDefault();
                if (user != null)
                {
                    ProfilePictureUrl = user.ProfilePictureUrl;
                    Debug.WriteLine("Profile picture loaded: " + ProfilePictureUrl);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profile picture: {ex.Message}", "OK");
            }
        }

        private async void LoadImages()
        {
            try
            {
                if (string.IsNullOrEmpty(UserName))
                {
                    await DisplayAlert("Error", "User name is missing.", "OK");
                    return;
                }

                var response = await _supabaseClient
                    .From<ImageData>()
                    .Filter("uploader_name", Supabase.Postgrest.Constants.Operator.Equals, UserName)
                    .Get();

                Images.Clear();
                foreach (var image in response.Models)
                {
                    Images.Add(image);
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
                await Navigation.PushAsync(new FullScreenImagePage(
                    imageData.ImageUrl,
                    imageData.UploaderName,
                    imageData.Category,
                    imageData.Title,
                    ProfilePictureUrl
                ));
            }
            else
            {
                await DisplayAlert("Error", "No image data found.", "OK");
            }
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage());
        }

        [Table("images")]
        public class ImageData : BaseModel
        {
            [PrimaryKey("id")]
            public int Id { get; set; }

            [Column("image_url")]
            public string ImageUrl { get; set; }

            [Column("uploader_name")]
            [JsonPropertyName("uploader_name")]
            public string UploaderName { get; set; }

            [Column("category")]
            public string Category { get; set; }

            [Column("title")]
            public string Title { get; set; }
        }

        [Table("users")]
        public class User : BaseModel
        {
            [PrimaryKey("id")]
            public int Id { get; set; }

            [Column("email")]
            public string Email { get; set; }

            [Column("name")]
            public string Name { get; set; }

            [Column("profile_picture_url")]
            public string ProfilePictureUrl { get; set; }
        }
    }
}
