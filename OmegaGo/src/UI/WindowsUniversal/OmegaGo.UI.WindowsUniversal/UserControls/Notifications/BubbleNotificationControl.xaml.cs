using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OmegaGo.Core.Annotations;
using OmegaGo.UI.Services.Notifications;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls.Notifications
{
    public sealed partial class BubbleNotificationControl : UserControl, INotifyPropertyChanged
    {
        public BubbleNotificationControl()
        {
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register(
            "Notification", typeof(BubbleNotification), typeof(BubbleNotificationControl), new PropertyMetadata(default(BubbleNotification), NotificationChanged));

        public BubbleNotification Notification
        {
            get { return (BubbleNotification)GetValue(NotificationProperty); }
            set { SetValue(NotificationProperty, value); }
        }

        /// <summary>
        /// Does the notification have a heading
        /// </summary>
        public bool HasHeading => Notification?.Heading != null;

        /// <summary>
        /// Icon of the notification
        /// </summary>
        public Uri Icon
        {
            get
            {
                if (Notification == null)
                {
                    return null;
                }
                return new Uri($"ms-appx:///Assets/Notifications/{Notification.Type}.png");
            }
        }

        /// <summary>
        /// Background of the icon
        /// </summary>
        public Brush IconBackground
        {
            get
            {
                if (Notification == null)
                {
                    return null;
                }
                Color color = Colors.White;
                switch (Notification.Type)
                {
                    case NotificationType.Info:
                        color = Colors.DodgerBlue;
                        break;
                    case NotificationType.Success:
                        color = Colors.LimeGreen;
                        break;
                    case NotificationType.Alert:
                        color = Colors.OrangeRed;
                        break;
                    case NotificationType.Error:
                        color = Colors.Red;
                        break;
                    case NotificationType.Achievement:
                        color = Colors.Gold;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return new SolidColorBrush(color) { Opacity = 0.8 };
            }
        }

        private static void NotificationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (BubbleNotificationControl)dependencyObject;
            control.OnPropertyChanged(nameof(IconBackground));
            control.OnPropertyChanged(nameof(Icon));
            control.OnPropertyChanged(nameof(HasHeading));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
