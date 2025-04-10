using Microsoft.Maui.Controls;
using System;

namespace MauiApp2
{
    public partial class FullScreenImagePage : ContentPage
    {
        private bool isSaved = false;
        private bool isHearted = false;

        public FullScreenImagePage(string imageUrl, string uploaderName, string category, string title)
        {
            InitializeComponent();

            FullImageView.Source = imageUrl;

            UploaderNameLabel.Text = uploaderName;
            CategoryLabel.Text = category;

            TitleLabel.Text = title;
        }

        private void OnBackButtonTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync(); 
        }

        private void OnSaveButtonTapped(object sender, EventArgs e)
        {
            isSaved = !isSaved;
            SavedButton.Source = isSaved ? "savebold.png" : "saved.png";
        }

        private void OnHeartButtonTapped(object sender, EventArgs e)
        {
            isHearted = !isHearted;
            HeartButton.Source = isHearted ? "heartred.png" : "heart.png";
        }
    }
}
