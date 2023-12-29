using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataBindingsSphereMovement
{
    /// <summary>
    /// Interaction logic for TemperaturePanel.xaml
    /// </summary>
    

    public partial class TemperaturePanel : Window
    {
        private SimBuild builder;

        public event NotifyClosing ClosePanel;

        public TemperaturePanel(SimBuild simBuild)
        {
            InitializeComponent();

            builder = simBuild;
            DataContext = builder.SimWorld;
        }

        private void TempValueChanged(object sender, RoutedEventArgs e)
        {
            tempDisplay.Text = Convert.ToString(Math.Round(MapToCorrectUnit(tempSlider.Value)));
        }

        private void OnPanelClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePanel?.Invoke();
        }

        private double MapToCorrectUnit(double temp)
        {
            double correctedTemp = temp * 573 / 10;

            if (UnitSelector.SelectedItem == Celsius)
            {
                correctedTemp = correctedTemp - 273;
            }

            return correctedTemp;
        }

        private void UnitsChanged(object sender, RoutedEventArgs e)
        {
            TempValueChanged(sender, e);
        }
    }
}
