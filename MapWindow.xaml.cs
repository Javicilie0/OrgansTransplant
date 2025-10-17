using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Windows;

namespace OrgnTransplant
{
    public partial class MapWindow : Window
    {
        private double fromLat;
        private double fromLng;
        private string fromName;
        private double toLat;
        private double toLng;
        private string toName;
        private double organViabilityHours;

        public MapWindow(
            double fromLatitude,
            double fromLongitude,
            string fromHospitalName,
            double toLatitude,
            double toLongitude,
            string toHospitalName,
            double organViabilityInHours)
        {
            InitializeComponent();

            this.fromLat = fromLatitude;
            this.fromLng = fromLongitude;
            this.fromName = fromHospitalName;
            this.toLat = toLatitude;
            this.toLng = toLongitude;
            this.toName = toHospitalName;
            this.organViabilityHours = organViabilityInHours;

            // Update UI text
            FromHospitalText.Text = fromHospitalName;
            ToHospitalText.Text = toHospitalName;
            RouteDescriptionText.Text = $"Маршрут от {fromHospitalName} до {toHospitalName}";
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize WebView2
                await MapWebView.EnsureCoreWebView2Async(null);

                // Load the HTML map file
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "map.html");

                if (File.Exists(htmlPath))
                {
                    MapWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);

                    // Wait for page to load, then call JavaScript function
                    MapWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                }
                else
                {
                    MessageBox.Show(
                        $"Файлът с картата не е намерен на пътя:\n{htmlPath}",
                        "Грешка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Грешка при зареждане на картата:\n{ex.Message}",
                    "Грешка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                try
                {
                    // Call JavaScript function to show route
                    string script = $@"
                        if (typeof showRoute === 'function') {{
                            showRoute(
                                {fromLat.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                                {fromLng.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                                '{fromName.Replace("'", "\\'")}',
                                {toLat.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                                {toLng.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                                '{toName.Replace("'", "\\'")}',
                                {organViabilityHours.ToString(System.Globalization.CultureInfo.InvariantCulture)}
                            );
                        }}
                    ";

                    await MapWebView.CoreWebView2.ExecuteScriptAsync(script);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Грешка при показване на маршрута:\n{ex.Message}",
                        "Грешка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
