using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechnicalInterview_FrontEnd.Views.VoterDetails
{
    /// <summary>
    /// Interaction logic for VoterDetailsView.xaml
    /// </summary>
    public partial class VoterDetailsView : UserControl
    {
        Point CurrentPoint = new Point();

        public VoterDetailsView()
        {
            InitializeComponent();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            SignatureCapture.Children.Clear();
        }

        private void SignatureCapture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = e.Source as Canvas;

            if (e.ButtonState == MouseButtonState.Pressed)
                CurrentPoint = e.GetPosition(canvas);
        }

        private void SignatureCapture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = CurrentPoint.X;
                line.Y1 = CurrentPoint.Y;
                line.X2 = e.GetPosition(this.SignatureCapture).X;
                line.Y2 = e.GetPosition(this.SignatureCapture).Y;

                CurrentPoint = e.GetPosition(this.SignatureCapture);

                this.SignatureCapture.Children.Add(line);
            }
        }
    }
}
