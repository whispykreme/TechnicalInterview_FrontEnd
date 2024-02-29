using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TechnicalInterview_FrontEnd.Converters
{
    public class StatusColorConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int statusId = (int)value;

            if (statusId == 1 || statusId == 2) { return Brushes.Green; }
            else if ((int)value == 3) { return Brushes.OrangeRed; }
            else return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
