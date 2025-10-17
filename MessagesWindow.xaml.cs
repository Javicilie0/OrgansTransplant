using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OrgnTransplant
{
    public partial class MessagesWindow : Window
    {
        private string currentHospital;

        public MessagesWindow(string hospitalName)
        {
            InitializeComponent();
            currentHospital = hospitalName;
            HospitalNameText.Text = hospitalName;
            LoadInboxMessages();
        }

        /// <summary>
        /// Показва входящи съобщения
        /// </summary>
        private void ShowInbox_Click(object sender, RoutedEventArgs e)
        {
            // Смяна на активния таб
            InboxTabButton.Style = (Style)FindResource("ActiveTabButton");
            OutboxTabButton.Style = (Style)FindResource("TabButton");

            // Смяна на видимостта
            InboxGrid.Visibility = Visibility.Visible;
            OutboxGrid.Visibility = Visibility.Collapsed;

            LoadInboxMessages();
        }

        /// <summary>
        /// Показва изходящи съобщения
        /// </summary>
        private void ShowOutbox_Click(object sender, RoutedEventArgs e)
        {
            // Смяна на активния таб
            InboxTabButton.Style = (Style)FindResource("TabButton");
            OutboxTabButton.Style = (Style)FindResource("ActiveTabButton");

            // Смяна на видимостта
            InboxGrid.Visibility = Visibility.Collapsed;
            OutboxGrid.Visibility = Visibility.Visible;

            LoadOutboxMessages();
        }

        /// <summary>
        /// Обновява съобщенията
        /// </summary>
        private void RefreshMessages_Click(object sender, RoutedEventArgs e)
        {
            if (InboxGrid.Visibility == Visibility.Visible)
            {
                LoadInboxMessages();
            }
            else
            {
                LoadOutboxMessages();
            }
        }

        /// <summary>
        /// Зарежда входящите съобщения
        /// </summary>
        private void LoadInboxMessages()
        {
            try
            {
                InboxMessagesPanel.Children.Clear();
                List<Message> messages = MessagesHelper.GetReceivedMessages(currentHospital);

                if (messages.Count == 0)
                {
                    InboxEmptyState.Visibility = Visibility.Visible;
                    return;
                }

                InboxEmptyState.Visibility = Visibility.Collapsed;

                foreach (var message in messages)
                {
                    InboxMessagesPanel.Children.Add(CreateInboxMessageCard(message));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане на входящи съобщения:\n\n{ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Зарежда изходящите съобщения
        /// </summary>
        private void LoadOutboxMessages()
        {
            try
            {
                OutboxMessagesPanel.Children.Clear();
                List<Message> messages = MessagesHelper.GetSentMessages(currentHospital);

                if (messages.Count == 0)
                {
                    OutboxEmptyState.Visibility = Visibility.Visible;
                    return;
                }

                OutboxEmptyState.Visibility = Visibility.Collapsed;

                foreach (var message in messages)
                {
                    OutboxMessagesPanel.Children.Add(CreateOutboxMessageCard(message));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане на изходящи съобщения:\n\n{ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Създава карта за входящо съобщение
        /// </summary>
        private Border CreateInboxMessageCard(Message message)
        {
            Border card = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Лява част - информация
            StackPanel leftPanel = new StackPanel();

            // Хедър
            StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

            TextBlock fromText = new TextBlock
            {
                Text = $"От: {message.FromHospital}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"))
            };
            headerPanel.Children.Add(fromText);

            // Статус бадж
            Border statusBadge = CreateStatusBadge(message.Status);
            statusBadge.Margin = new Thickness(15, 0, 0, 0);
            headerPanel.Children.Add(statusBadge);

            leftPanel.Children.Add(headerPanel);

            // Орган и донор
            TextBlock organText = new TextBlock
            {
                Text = $"🫀 Орган: {message.OrganName}",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                Margin = new Thickness(0, 8, 0, 0)
            };
            leftPanel.Children.Add(organText);

            if (!string.IsNullOrEmpty(message.DonorName))
            {
                TextBlock donorText = new TextBlock
                {
                    Text = $"👤 Донор: {message.DonorName}",
                    FontSize = 14,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                leftPanel.Children.Add(donorText);
            }

            // Съобщение
            if (!string.IsNullOrEmpty(message.MessageText))
            {
                TextBlock messageText = new TextBlock
                {
                    Text = $"💬 {message.MessageText}",
                    FontSize = 13,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    Margin = new Thickness(0, 10, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                };
                leftPanel.Children.Add(messageText);
            }

            // Дата
            TextBlock dateText = new TextBlock
            {
                Text = $"📅 {message.CreatedAt:dd.MM.yyyy HH:mm}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6")),
                Margin = new Thickness(0, 10, 0, 0)
            };
            leftPanel.Children.Add(dateText);

            // Бутон за карта
            Button mapButton = new Button
            {
                Content = "🗺️ Покажи маршрут",
                Background = Brushes.White,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FACFE")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FACFE")),
                BorderThickness = new Thickness(2),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(15, 8, 15, 8),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            mapButton.Click += (s, e) => ShowMapForMessage(message);
            leftPanel.Children.Add(mapButton);

            // Ако има отговор
            if (message.Status != MessageStatus.Pending)
            {
                Border responseBorder = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1")),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 10, 0, 0)
                };

                StackPanel responsePanel = new StackPanel();

                TextBlock responseLabel = new TextBlock
                {
                    Text = "✅ Вашият отговор:",
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"))
                };
                responsePanel.Children.Add(responseLabel);

                // Показваме текста само ако има такъв
                if (!string.IsNullOrEmpty(message.ResponseText))
                {
                    TextBlock responseText = new TextBlock
                    {
                        Text = message.ResponseText,
                        FontSize = 13,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                        Margin = new Thickness(0, 5, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    responsePanel.Children.Add(responseText);
                }

                // Опция за доставка - показваме винаги при приети заявки
                if (message.Status == MessageStatus.Accepted)
                {
                    string deliveryText = message.DeliveryOption == DeliveryOption.WithDriver ? "🚗 С шофьор" :
                                         message.DeliveryOption == DeliveryOption.WithHelicopter ? "🚁 С хеликоптер" :
                                         message.DeliveryOption == DeliveryOption.PickupRequired ? "🚶 Вземане лично" :
                                         "⚠️ Неуточнено";

                    TextBlock deliveryTextBlock = new TextBlock
                    {
                        Text = deliveryText,
                        FontSize = 13,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                        Margin = new Thickness(0, 5, 0, 0)
                    };
                    responsePanel.Children.Add(deliveryTextBlock);
                }

                responseBorder.Child = responsePanel;
                leftPanel.Children.Add(responseBorder);
            }

            Grid.SetColumn(leftPanel, 0);
            grid.Children.Add(leftPanel);

            // Дясна част - бутони
            if (message.Status == MessageStatus.Pending)
            {
                StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Vertical };

                Button acceptButton = new Button
                {
                    Content = "✅ Приеми",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60")),
                    Foreground = Brushes.White,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Padding = new Thickness(20, 8, 20, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Width = 150
                };
                acceptButton.Click += (s, e) => RespondToMessage(message, MessageStatus.Accepted);
                buttonPanel.Children.Add(acceptButton);

                Button rejectButton = new Button
                {
                    Content = "❌ Отхвърли",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")),
                    Foreground = Brushes.White,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Padding = new Thickness(20, 8, 20, 8),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Width = 150,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                rejectButton.Click += (s, e) => RespondToMessage(message, MessageStatus.Rejected);
                buttonPanel.Children.Add(rejectButton);

                Grid.SetColumn(buttonPanel, 1);
                grid.Children.Add(buttonPanel);
            }

            card.Child = grid;
            return card;
        }

        /// <summary>
        /// Създава карта за изходящо съобщение
        /// </summary>
        private Border CreateOutboxMessageCard(Message message)
        {
            Border card = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            StackPanel panel = new StackPanel();

            // Хедър
            StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

            TextBlock toText = new TextBlock
            {
                Text = $"До: {message.ToHospital}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3E50"))
            };
            headerPanel.Children.Add(toText);

            // Статус бадж
            Border statusBadge = CreateStatusBadge(message.Status);
            statusBadge.Margin = new Thickness(15, 0, 0, 0);
            headerPanel.Children.Add(statusBadge);

            panel.Children.Add(headerPanel);

            // Орган
            TextBlock organText = new TextBlock
            {
                Text = $"🫀 Орган: {message.OrganName}",
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                Margin = new Thickness(0, 8, 0, 0)
            };
            panel.Children.Add(organText);

            // Донор
            if (!string.IsNullOrEmpty(message.DonorName))
            {
                TextBlock donorText = new TextBlock
                {
                    Text = $"👤 Донор: {message.DonorName}",
                    FontSize = 14,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                    Margin = new Thickness(0, 5, 0, 0)
                };
                panel.Children.Add(donorText);
            }

            // Съобщение
            if (!string.IsNullOrEmpty(message.MessageText))
            {
                TextBlock messageText = new TextBlock
                {
                    Text = $"💬 {message.MessageText}",
                    FontSize = 13,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")),
                    Margin = new Thickness(0, 10, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                };
                panel.Children.Add(messageText);
            }

            // Дата
            TextBlock dateText = new TextBlock
            {
                Text = $"📅 {message.CreatedAt:dd.MM.yyyy HH:mm}",
                FontSize = 12,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#95A5A6")),
                Margin = new Thickness(0, 10, 0, 0)
            };
            panel.Children.Add(dateText);

            // Бутон за карта
            Button mapButton = new Button
            {
                Content = "🗺️ Покажи маршрут",
                Background = Brushes.White,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FACFE")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4FACFE")),
                BorderThickness = new Thickness(2),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Padding = new Thickness(15, 8, 15, 8),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            mapButton.Click += (s, e) => ShowMapForMessage(message);
            panel.Children.Add(mapButton);

            // Ако има отговор
            if (message.Status != MessageStatus.Pending)
            {
                Border responseBorder = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F8F5")),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 10, 0, 0)
                };

                StackPanel responsePanel = new StackPanel();

                TextBlock responseLabel = new TextBlock
                {
                    Text = "📨 Отговор от болницата:",
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A085"))
                };
                responsePanel.Children.Add(responseLabel);

                // Показваме текста само ако има такъв
                if (!string.IsNullOrEmpty(message.ResponseText))
                {
                    TextBlock responseText = new TextBlock
                    {
                        Text = message.ResponseText,
                        FontSize = 13,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E")),
                        Margin = new Thickness(0, 5, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    };
                    responsePanel.Children.Add(responseText);
                }

                // Опция за доставка
                if (message.Status == MessageStatus.Accepted)
                {
                    string deliveryText = message.DeliveryOption == DeliveryOption.WithDriver ? "🚗 С шофьор" :
                                         message.DeliveryOption == DeliveryOption.WithHelicopter ? "🚁 С хеликоптер" :
                                         message.DeliveryOption == DeliveryOption.PickupRequired ? "🚶 Вземане лично" :
                                         "⚠️ Неуточнено";

                    TextBlock deliveryTextBlock = new TextBlock
                    {
                        Text = deliveryText,
                        FontSize = 13,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                        Margin = new Thickness(0, 5, 0, 0)
                    };
                    responsePanel.Children.Add(deliveryTextBlock);
                }

                responseBorder.Child = responsePanel;
                panel.Children.Add(responseBorder);
            }

            card.Child = panel;
            return card;
        }

        /// <summary>
        /// Създава бадж за статус
        /// </summary>
        private Border CreateStatusBadge(MessageStatus status)
        {
            string text = "";
            string bgColor = "";

            switch (status)
            {
                case MessageStatus.Pending:
                    text = "⏳ Чака отговор";
                    bgColor = "#F39C12";
                    break;
                case MessageStatus.Accepted:
                    text = "✅ Приета";
                    bgColor = "#27AE60";
                    break;
                case MessageStatus.Rejected:
                    text = "❌ Отхвърлена";
                    bgColor = "#E74C3C";
                    break;
            }

            Border badge = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12, 4, 12, 4),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock badgeText = new TextBlock
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White
            };

            badge.Child = badgeText;
            return badge;
        }

        /// <summary>
        /// Отговаря на съобщение
        /// </summary>
        private void RespondToMessage(Message message, MessageStatus status)
        {
            try
            {
                // Показваме прозорец за отговор
                RespondWindow respondWindow = new RespondWindow(message, status);
                if (respondWindow.ShowDialog() == true)
                {
                    // Обновяваме списъка
                    LoadInboxMessages();

                    MessageBox.Show("Отговорът беше изпратен успешно!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при отговор на съобщението:\n\n{ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Показва карта с маршрут за съобщение
        /// </summary>
        private void ShowMapForMessage(Message message)
        {
            try
            {
                // Вземаме информация за болниците
                var fromHospital = HospitalLocation.GetByName(message.FromHospital);
                var toHospital = HospitalLocation.GetByName(message.ToHospital);

                if (fromHospital == null)
                {
                    MessageBox.Show($"Болницата \"{message.FromHospital}\" не е намерена в базата данни.",
                        "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (toHospital == null)
                {
                    MessageBox.Show($"Болницата \"{message.ToHospital}\" не е намерена в базата данни.",
                        "Грешка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Вземаме жизнеспособността на органа в часове
                double organViabilityHours = OrganViability.GetViabilityHours(message.OrganName);

                // Отваряме прозореца с картата
                MapWindow mapWindow = new MapWindow(
                    fromHospital.Latitude,
                    fromHospital.Longitude,
                    fromHospital.Name,
                    toHospital.Latitude,
                    toHospital.Longitude,
                    toHospital.Name,
                    organViabilityHours);

                mapWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при показване на картата:\n\n{ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
