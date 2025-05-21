using System;
using System.Windows;
using Mqtt_Explorer.ViewModels;

namespace Mqtt_Explorer
{
    /// <summary>
    /// Hauptfenster der Anwendung
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Setze das DataContext auf das ViewModel
            this.DataContext = new MqttViewModel();
        }

        // Zugriff auf das ViewModel über das DataContext
        private MqttViewModel Vm => (MqttViewModel)this.DataContext;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
