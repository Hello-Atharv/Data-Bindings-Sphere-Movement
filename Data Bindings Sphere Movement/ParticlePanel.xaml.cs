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
using System.ComponentModel;

namespace DataBindingsSphereMovement
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public class ParticleGroupProperties : INotifyPropertyChanged
    {
        public Binding diameter;
        public Binding mass;

        public SolidColorBrush colour;
        public event PropertyChangedEventHandler PropertyChanged;

        public ParticleGroupProperties(Binding diameter, Binding mass, SolidColorBrush colour)
        {
            this.diameter = diameter;
            this.mass = mass;
            this.colour = colour;
        }

        public void ChangeColour(SolidColorBrush newColour)
        {
            colour = newColour;
            OnPropertyChanged("Colour");
        }

        public SolidColorBrush Colour
        {
            get { return colour; }
            set { colour = value; OnPropertyChanged("Colour"); }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public partial class ParticlePanel : Window
    {
        private SimBuild builder;
        private int groupDisplayed = 1;
        private int noGroups = 0;
        private Dictionary<int, ParticleGroupProperties> particleGroups;

        private const int red = 113; //113 is index for colour red (default starting colour for particles)

        public event NotifyClosing ClosePanel;


        private Random rand = new Random();


        public ParticlePanel(SimBuild simBuild) 
        {
            InitializeComponent();
            particleGroups = new Dictionary<int, ParticleGroupProperties>();
            
            builder = simBuild;
            DataContext = this;

            AddParticleGroup(null, null);

            ColourSelector.ItemsSource = typeof(Colors).GetProperties();
            ColourSelector.SelectedIndex = red;

            
            SetBindings();
            /*
            Binding radius = new Binding("Radius");
            radius.Source = builder.SimWorld.AttributesDictionary[1];
            RadiusSlider.SetBinding(Slider.ValueProperty, radius);


            Binding mass = new Binding("Mass");
            mass.Source = builder.SimWorld.AttributesDictionary[1];
            MassSlider.SetBinding(Slider.ValueProperty, mass);*/
        }

        

        private void AddParticleGroup(object sender, RoutedEventArgs e)
        {
            builder.ParticleGroupAdd();
            noGroups++;

            Binding diameter = new Binding("Diameter");
            diameter.Source = builder.SimWorld.AttributesDictionary[noGroups];

            Binding mass = new Binding("Mass");
            mass.Source = builder.SimWorld.AttributesDictionary[noGroups];

            ParticleGroupProperties newGroup = new ParticleGroupProperties(diameter, mass, new SolidColorBrush(Colors.Red));
            particleGroups.Add(noGroups, newGroup);
            
            CheckBoundaryValues();
        }

        private void ClickNextGroup(object sender, RoutedEventArgs e)
        {
            groupDisplayed++;
            GroupDisplayedChanged();
        }

        private void ClickPreviousGroup(object sender, RoutedEventArgs e)
        {
            
            groupDisplayed--;
            GroupDisplayedChanged();
        }

        private void SetBindings()
        {
            DiameterSlider.SetBinding(Slider.ValueProperty, particleGroups[groupDisplayed].diameter);
            MassSlider.SetBinding(Slider.ValueProperty, particleGroups[groupDisplayed].mass);
        }

        private void UpdateColour(object sender, SelectionChangedEventArgs e){
            string colour = ColourSelector.SelectedItem.ToString().Split(' ')[1]; //[1] to get second value in array
            SolidColorBrush selectedColour = new SolidColorBrush();
            selectedColour.Color = (Color)ColorConverter.ConvertFromString(colour);
            particleGroups[groupDisplayed].ChangeColour(selectedColour);
        }

        private void CheckBoundaryValues()
        {
            PreviousGroup.Visibility = Visibility.Visible;
            NextGroup.Visibility = Visibility.Visible;
            if (groupDisplayed == 1)
            {
                PreviousGroup.Visibility = Visibility.Hidden;
            }
            if (groupDisplayed == noGroups)
            {
                NextGroup.Visibility = Visibility.Hidden;
            }
        }

        public void UpdateGroupNumber()
        {
            GroupNumber.Content = groupDisplayed;
        }

        private void OnPanelClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePanel?.Invoke();
        }

        private void GroupDisplayedChanged()
        {
            CheckBoundaryValues();
            UpdateGroupNumber();
            SetBindings();
        }

        private void SpawnChange(object sender, RoutedEventArgs e)
        {
            builder.SimWorld.GroupSelected = groupDisplayed;
        }

        public int GroupDisplayed
        {
            get { return groupDisplayed; }
        }

        public Dictionary<int, ParticleGroupProperties> ParticleGroups
        {
            get { return particleGroups; }
            set { particleGroups = value; }
        }
    }
}
