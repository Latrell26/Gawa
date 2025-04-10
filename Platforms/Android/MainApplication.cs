using Android.App;
using Android.Runtime;
using MauiApp2;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace CarListApp.Maui;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
#if ANDROID
        EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            handler.PlatformView.Background = null; 
        });
#endif
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
