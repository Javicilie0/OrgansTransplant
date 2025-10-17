using System;
using System.Windows;
using System.Windows.Controls;

namespace OrgnTransplant
{
    public partial class RespondWindow : Window
    {
        private Message currentMessage;
        private MessageStatus responseStatus;

        public RespondWindow(Message message, MessageStatus status)
        {
            InitializeComponent();
            currentMessage = message;
            responseStatus = status;

            // Настройка на UI според статуса
            if (status == MessageStatus.Accepted)
            {
                TitleText.Text = "✅ Приемане на заявка";
                TitleText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                TitleText.Text = "❌ Отхвърляне на заявка";
                TitleText.Foreground = System.Windows.Media.Brushes.Red;

                // Скриваме опциите за доставка при отхвърляне
                WithDriverRadio.Visibility = Visibility.Collapsed;
                WithHelicopterRadio.Visibility = Visibility.Collapsed;
                PickupRequiredRadio.Visibility = Visibility.Collapsed;

                // Променяме текста на лейбъла
                TextBlock deliveryLabel = (TextBlock)((StackPanel)((ScrollViewer)((Border)this.Content).Child).Content).Children[0];
                deliveryLabel.Text = "Причина за отхвърляне:";
            }

            // Показваме информация за заявката
            OrganInfoText.Text = $"Заявка за {message.OrganName} от {message.FromHospital}";
        }

        /// <summary>
        /// Изпраща отговора
        /// </summary>
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string responseText = ResponseTextBox.Text.Trim();

                // Валидация
                if (responseStatus == MessageStatus.Rejected && string.IsNullOrEmpty(responseText))
                {
                    MessageBox.Show("Моля, въведете причина за отхвърляне на заявката.",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Определяме опцията за доставка
                DeliveryOption deliveryOption = DeliveryOption.NotSpecified;

                if (responseStatus == MessageStatus.Accepted)
                {
                    if (WithDriverRadio.IsChecked == true)
                    {
                        deliveryOption = DeliveryOption.WithDriver;
                    }
                    else if (WithHelicopterRadio.IsChecked == true)
                    {
                        deliveryOption = DeliveryOption.WithHelicopter;
                    }
                    else if (PickupRequiredRadio.IsChecked == true)
                    {
                        deliveryOption = DeliveryOption.PickupRequired;
                    }
                }

                // Изпращаме отговора
                MessagesHelper.RespondToRequest(
                    currentMessage.Id,
                    responseStatus,
                    deliveryOption,
                    responseText
                );

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при изпращане на отговор:\n\n{ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Затваря прозореца без да изпраща отговор
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
