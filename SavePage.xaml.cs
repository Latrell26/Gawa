namespace MauiApp2;

public partial class SavePage : ContentPage
{
	public SavePage()
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