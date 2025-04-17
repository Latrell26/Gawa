using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace MauiApp2
{
    public partial class NotificationView : ContentView, INotifyPropertyChanged
    {
        private string _searchText;
        private bool _isSearchVisible = true;
        private bool _isScrollViewVisible = true;


        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    OnPropertyChanged(nameof(IsSearchEmpty));
                    OnPropertyChanged(nameof(IsSearchNotEmpty));
                }
            }
        }
        public bool IsSearchEmpty => string.IsNullOrEmpty(SearchText);
        public bool IsSearchNotEmpty => !string.IsNullOrEmpty(SearchText);

        public bool IsSearchVisible
        {
            get => _isSearchVisible;
            set
            {
                if (_isSearchVisible != value)
                {
                    _isSearchVisible = value;
                    OnPropertyChanged(nameof(IsSearchVisible));
                }
            }
        }

        public bool IsScrollViewVisible
        {
            get => _isScrollViewVisible;
            set
            {
                if (_isScrollViewVisible != value)
                {
                    _isScrollViewVisible = value;
                    OnPropertyChanged(nameof(IsScrollViewVisible));
                }
            }
        }
        public NotificationView()
        {
            InitializeComponent();
            BindingContext = this;

            MessagingCenter.Subscribe<object, bool>(this, "ToggleSearchBar", (sender, isVisible) =>
            {
                IsSearchVisible = isVisible;
            });

            MessagingCenter.Subscribe<object, bool>(this, "ToggleScrollView", (sender, isVisible) =>
            {
                IsScrollViewVisible = isVisible;
            });
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText = e.NewTextValue;
        }

        private void OnClearSearch(object sender, EventArgs e)
        {
            SearchText = string.Empty;
            SearchEntry.Text = string.Empty; 
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnFilterClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string category)
            {
                MessagingCenter.Send<object, string>(this, "FilterImages", category);

                foreach (var child in ButtonStackLayout.Children)
                {
                    if (child is Button btn)
                    {
                        btn.Background = null;
                        btn.BackgroundColor = Color.FromArgb("#CCCCCC");
                        btn.TextColor = Colors.Black;
                    }
                }

                button.BackgroundColor = Color.FromArgb("#1B56FD");
                button.TextColor = Colors.White;

                if (category != "All")
                {
                    AllButton.IsVisible = true;
                }
                else
                {
                    AllButton.IsVisible = false;
                }
            }
        }

    }

}

