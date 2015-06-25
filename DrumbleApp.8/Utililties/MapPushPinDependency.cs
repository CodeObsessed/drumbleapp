using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Collections;
using System.Linq;
using System.Windows;

namespace Drumble.Utilities
{
    public static class MapPushPinDependency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static DependencyProperty ItemsSourceProperty =
                DependencyProperty.RegisterAttached(
                 "ItemsSource", typeof(IEnumerable), typeof(MapPushPinDependency),
                 new PropertyMetadata(OnPushPinPropertyChanged));

        private static void OnPushPinPropertyChanged(DependencyObject d,
                DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            var pushpin = MapExtensions.GetChildren((Map)uie).OfType<MapItemsControl>().FirstOrDefault();
            
            pushpin.ItemsSource = (IEnumerable)e.NewValue;
        }

        #region Getters and Setters

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static IEnumerable GetItemsSource(DependencyObject obj)
        {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void SetItemsSource(DependencyObject obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        #endregion
    }
}
