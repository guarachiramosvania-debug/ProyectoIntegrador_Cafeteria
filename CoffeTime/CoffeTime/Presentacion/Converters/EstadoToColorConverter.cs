using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CoffeTime.Presentacion.Converters
{
    public class EstadoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string estado = value?.ToString()?.ToLower() ?? "";

            return estado switch
            {
                "pagado" => new SolidColorBrush(Color.FromRgb(76, 175, 80)),      // verde
                "pendiente" => new SolidColorBrush(Color.FromRgb(255, 167, 38)), // naranja
                "cancelado" => new SolidColorBrush(Color.FromRgb(244, 67, 54)),  // rojo
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
