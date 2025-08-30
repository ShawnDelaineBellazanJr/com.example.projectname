using Microsoft.Maui.Controls;
using System.Text.Json.Nodes;

namespace ProjectName.MauiApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "site.json");
            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path);
                var node = JsonNode.Parse(json)?.AsObject();
                if (node is not null)
                {
                    HeroTitle.Text = node["hero"]?["title"]?.ToString();
                    HeroSubtitle.Text = node["hero"]?["subtitle"]?.ToString();
                }
            }
        }
        catch
        {
            // ignore for now
        }
    }
}
