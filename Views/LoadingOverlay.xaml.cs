using System.Windows;
using OrgnTransplant.Models;
using OrgnTransplant.Utilities;
using System.Windows.Controls;

namespace OrgnTransplant.Views
{
    public partial class LoadingOverlay : UserControl
    {
        public LoadingOverlay()
        {
            InitializeComponent();
        }

        public string Message
        {
            get => LoadingText.Text;
            set => LoadingText.Text = value;
        }

        public static void Show(Grid parentGrid, string message = "Зареждане...")
        {
            var overlay = new LoadingOverlay { Message = message };
            Grid.SetRowSpan(overlay, int.MaxValue);
            Grid.SetColumnSpan(overlay, int.MaxValue);
            parentGrid.Children.Add(overlay);
        }

        public static void Hide(Grid parentGrid)
        {
            var overlay = parentGrid.Children.OfType<LoadingOverlay>().FirstOrDefault();
            if (overlay != null)
            {
                parentGrid.Children.Remove(overlay);
            }
        }
    }
}
