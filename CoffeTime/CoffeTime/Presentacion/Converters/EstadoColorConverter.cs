using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CoffeTime.Presentacion.Converters
{
    public class EstadoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var estado = value as string ?? string.Empty;

            if (estado.Equals("Alerta", StringComparison.OrdinalIgnoreCase))
                return new SolidColorBrush(Color.FromRgb(0xFF, 0xD8, 0xD8)); // rojo suave

            return new SolidColorBrush(Color.FromRgb(0xE2, 0xF5, 0xE9)); // verde suave







        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
