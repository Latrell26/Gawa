    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Maui.Controls;


    namespace MauiApp2
    {
        public partial class MainPage : ContentPage
        {
            private const string SupabaseUrl = "https://xurgyelsilpzbkcrrjqo.supabase.co";
            private const string SupabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ";

            public MainPage()
            {
                InitializeComponent();
            }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(usernameEntry.Text) &&
                !string.IsNullOrWhiteSpace(emailEntry.Text) &&
                !string.IsNullOrWhiteSpace(passwordEntry.Text))
            {
                var userData = new
                {
                    name = usernameEntry.Text,
                    email = emailEntry.Text,
                    password = passwordEntry.Text  
                };

                string jsonData = JsonSerializer.Serialize(userData);

                try
                {
                    using var client = new HttpClient();
                    client.BaseAddress = new Uri(SupabaseUrl);
                    client.DefaultRequestHeaders.Add("apikey", SupabaseApiKey);
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", SupabaseApiKey);

                    var endpoint = "/rest/v1/users";

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string registeredName = usernameEntry.Text; 
                        Preferences.Set("UserName", registeredName);

                        await DisplayAlert("Success", "User created successfully.", "OK");
                        await Shell.Current.GoToAsync("///NewPage1");
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Error", $"Failed to create user: {error}", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Exception", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please fill in all fields before signing up.", "OK");
            }
        }


        private void OnTextChanged(object sender, TextChangedEventArgs e)
            {
                if (sender == usernameEntry)
                {
                    clearUsernameBtn.IsVisible = !string.IsNullOrEmpty(usernameEntry.Text);
                }
                else if (sender == emailEntry)
                {
                    clearEmailBtn.IsVisible = !string.IsNullOrEmpty(emailEntry.Text);
                }
            }

            private void OnClearUsername(object sender, EventArgs e)
            {
                usernameEntry.Text = "";
            }

            private void OnClearEmail(object sender, EventArgs e)
            {
                emailEntry.Text = "";
            }

            private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
            {
                togglePasswordBtn.IsVisible = !string.IsNullOrEmpty(passwordEntry.Text);
            }

            private void OnTogglePassword(object sender, EventArgs e)
            {
                if (passwordEntry.IsPassword)
                {
                    passwordEntry.IsPassword = false;
                    togglePasswordBtn.Source = "view.png"; 
                }
                else
                {
                    passwordEntry.IsPassword = true;
                    togglePasswordBtn.Source = "hide.png"; 
                }
            }

            private async void OnLoginTapped(object sender, EventArgs e)
            {
                await Navigation.PushAsync(new NewPage1());
            }
        }
    }
