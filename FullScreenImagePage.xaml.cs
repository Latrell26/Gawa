using Microsoft.Maui.Controls;
using System;

namespace MauiApp2
{
    public partial class FullScreenImagePage : ContentPage
    {
        private bool isSaved = false;
        private bool isHearted = false;

        public FullScreenImagePage(string imageUrl, string uploaderName, string category, string title, string profilePictureUrl)
        {
            InitializeComponent();

            // Set the image source for the full-screen image view
            FullImageView.Source = imageUrl;

            // Set the uploader's name and category
            UploaderNameLabel.Text = uploaderName;
            CategoryLabel.Text = category;

            // Set the title of the image
            TitleLabel.Text = title;

            // Set the uploader's profile picture
            if (!string.IsNullOrEmpty(profilePictureUrl))
            {
                UserPic.Source = profilePictureUrl;  // Display the uploader's profile picture
            }
        }
        private async void OnUserPicTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArtistProfilePage());
        }
        private void OnBackButtonTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync(); // Go back to the previous page
        }

        private void OnSaveButtonTapped(object sender, EventArgs e)
        {
            isSaved = !isSaved;
            SavedButton.Source = isSaved ? "savebold.png" : "saved.png"; // Change the button image based on state
        }

        private void OnHeartButtonTapped(object sender, EventArgs e)
        {
            isHearted = !isHearted;
            HeartButton.Source = isHearted ? "heartred.png" : "heart.png"; // Change the heart button image based on state
        }
    }
}
