namespace MauiApp2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(RankingPage), typeof(RankingPage));
            Routing.RegisterRoute(nameof(CreatePage), typeof(CreatePage));
            Routing.RegisterRoute(nameof(SavePage), typeof(SavePage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

            CurrentItem = new ShellContent
            {
                ContentTemplate = new DataTemplate(typeof(MainPage))
            };

        }
    }}
