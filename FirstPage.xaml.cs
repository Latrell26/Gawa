using Microsoft.Maui.Controls;

namespace MauiApp2
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage()); // Ensure SignUpPage exists
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewPage1()); // Ensure LoginPage exists
        }
    }
}
