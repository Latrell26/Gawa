using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiApp2;

public partial class NewPage1 : ContentPage
{
    private const string SupabaseUrl = "https://xurgyelsilpzbkcrrjqo.supabase.co";
    private const string SupabaseApiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Inh1cmd5ZWxzaWxwemJrY3JyanFvIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDEwOTc4NDcsImV4cCI6MjA1NjY3Mzg0N30.6kRX0B5MmDuXFxNcRtEjjmIRLFDw4uC2BTsv3myomEQ";

    public NewPage1()
    {
        InitializeComponent();
    }

    private async void OnSignupTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());

    }

    private async void OnHomePageClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(emailEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields before logging in.", "OK");
            return;
        }

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(SupabaseUrl);
            client.DefaultRequestHeaders.Add("apikey", SupabaseApiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SupabaseApiKey);

            string endpoint = $"/rest/v1/users?email=eq.{emailEntry.Text}&select=email,password,name";

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "Failed to connect to the server.", "OK");
                return;
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<User[]>(jsonResponse);

            if (users == null || users.Length == 0)
            {
                await DisplayAlert("Error", "User not found. Please check your email or sign up.", "OK");
                return;
            }

            var user = users[0];

            if (user.password != passwordEntry.Text) 
            {
                await DisplayAlert("Error", "Incorrect password. Please try again.", "OK");
                return;
            }

            Preferences.Set("UserEmail", user.email);
            Preferences.Set("UserName", user.name);

            await DisplayAlert("Success", "Login successful!", "OK");

            await Shell.Current.GoToAsync("///HomePage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exception", ex.Message, "OK");
        }
    }


    private class User
    {
        public string email { get; set; }
        public string password { get; set; } 
        public string name { get; set; }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender == emailEntry)
        {
            clearEmailBtn.IsVisible = !string.IsNullOrEmpty(emailEntry.Text);
        }
    }

    private void OnClearEmail(object sender, EventArgs e)
    {
        emailEntry.Text = string.Empty;
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        togglePasswordBtn.IsVisible = !string.IsNullOrEmpty(passwordEntry.Text);
    }

    private void OnTogglePassword(object sender, EventArgs e)
    {
        passwordEntry.IsPassword = !passwordEntry.IsPassword;
        togglePasswordBtn.Source = passwordEntry.IsPassword ? "hide.png" : "view.png";
    }
}
