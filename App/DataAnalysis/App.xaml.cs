using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace FluentChartApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level =
                System.Diagnostics.SourceLevels.All;

            Debug.WriteLine("OnStartup was called.");
            base.OnStartup(e);
        }


    }
}

    
