using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace MauiApp2
{
    public partial class FullScreenImagePage : ContentPage
    {
        private bool isSaved = false;
        private bool isHearted = false;
        private readonly string _uploaderName;

        public FullScreenImagePage(
            string imageUrl,
            string uploaderName,
            string category,
            string title,
            string profilePictureUrl)
        {
            InitializeComponent();

            _uploaderName = uploaderName;

            // Full‐screen image
            FullImageView.Source = imageUrl;

            // Metadata
            UploaderNameLabel.Text = uploaderName;
            CategoryLabel.Text = category;
            TitleLabel.Text = title;

            // Poser’s profile picture
            if (!string.IsNullOrEmpty(profilePictureUrl))
                UserPic.Source = profilePictureUrl;
        }

        // <TapGestureRecognizer Tapped="OnUserPicTapped"/>
        private async void OnUserPicTapped(object sender, TappedEventArgs e)
        {
            // Navigate to the tapped user’s profile by name:
            await Navigation.PushAsync(new ArtistProfilePage(_uploaderName));
        }

        // Back button tap
        private async void OnBackButtonTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PopAsync();
        }

        // Heart icon tap
        private void OnHeartButtonTapped(object sender, TappedEventArgs e)
        {
            isHearted = !isHearted;
            HeartButton.Source = isHearted ? "heartred.png" : "heart.png";
        }

        // Save icon tap
        private void OnSaveButtonTapped(object sender, TappedEventArgs e)
        {
            isSaved = !isSaved;
            SavedButton.Source = isSaved ? "savebold.png" : "saved.png";
        }
    }
}
