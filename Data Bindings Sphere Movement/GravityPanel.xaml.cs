﻿using System;
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
    /// Interaction logic for GravityPanel.xaml
    /// </summary>
    /// 
    

    public partial class GravityPanel : Window
    {
        
        private SimBuild builder;

        public event NotifyClosing ClosePanel;

        public GravityPanel(SimBuild simBuild)
        {
            builder = simBuild;
            InitializeComponent();
            DataContext = builder.SimWorld;
        }

        private void OnPanelClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePanel?.Invoke();
        }

    }
}
