using Microsoft.Maui.Controls;

namespace ProjectName.MauiApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new MainPage());
    }
}
