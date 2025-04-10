namespace MauiApp2;

public partial class RankingPage : ContentPage
{
	public RankingPage()
	{
        InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        MessagingCenter.Send<object, bool>(this, "ToggleScrollView", false);
        MessagingCenter.Send<object, bool>(this, "ToggleSearchBar", false);

    }

}