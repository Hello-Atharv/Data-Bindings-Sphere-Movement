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
    /// Interaction logic for CollisionPanel.xaml
    /// </summary>
    public partial class CollisionPanel : Window
    {

        private SimBuild builder;

        public event NotifyClosing ClosePanel;
        public CollisionPanel(SimBuild simBuild)
        {
            InitializeComponent();

            builder = simBuild;
            DataContext = builder.SimWorld;
        }

        private void OnPanelClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePanel?.Invoke();
        }
    }
}
