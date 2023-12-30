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
        private Binding diameter;
        private Binding mass;
        private string colour;
        private Binding groupCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public ParticleGroupProperties(Binding diameter, Binding mass, Binding groupCount, string colour)
        {
            this.diameter = diameter;
            this.mass = mass;
            this.colour = colour;
            this.groupCount = groupCount;
        }

        public string Colour
        {
            get { return colour; }
            set { colour = value; OnPropertyChanged("Colour"); }
        }

        public Binding Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }
        public Binding Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Binding GroupCount
        {
            get { return groupCount; }
            set { groupCount = value; }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

    public partial class ParticlePanel : Window, INotifyPropertyChanged
    {
        private SimBuild builder;
        private int groupDisplayed = 1;
        private int noGroups = 0;
        private Dictionary<int, ParticleGroupProperties> particleGroups;

        private const int red = 113; //113 is index for colour red (default starting colour for particles)

        public event NotifyClosing ClosePanel;

        public event PropertyChangedEventHandler PropertyChanged;

        private IntStringConverter valueConv;


        public ParticlePanel(SimBuild simBuild) 
        {
            InitializeComponent();
            particleGroups = new Dictionary<int, ParticleGroupProperties>();
            valueConv = new IntStringConverter();

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

            Binding groupCount = new Binding("GroupCount");
            groupCount.Source = builder.SimWorld.AttributesDictionary[noGroups];
            groupCount.Converter = valueConv;
            
            ParticleGroupProperties newGroup = new ParticleGroupProperties(diameter, mass, groupCount, "Red");
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
            DiameterSlider.SetBinding(Slider.ValueProperty, particleGroups[groupDisplayed].Diameter);
            MassSlider.SetBinding(Slider.ValueProperty, particleGroups[groupDisplayed].Mass);
            GroupCountLabel.SetBinding(Label.ContentProperty, particleGroups[groupDisplayed].GroupCount);
        }

        private void UpdateColour(object sender, SelectionChangedEventArgs e){
            string colour = GetColourStringFromSelector(ColourSelector.SelectedItem);
            particleGroups[groupDisplayed].Colour = colour;
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

        private void OnPanelClosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            ClosePanel?.Invoke();
        }

        private void GroupDisplayedChanged()
        {
            OnPropertyChanged("GroupDisplayed");
            CheckBoundaryValues();
            SetBindings();
            GroupChangedColourChange();
            ChangeSpawnButtonStatus();
        }

        private void GroupChangedColourChange()
        {
            string colour = particleGroups[groupDisplayed].Colour;

            bool found = false;
            int i = 0;
            while (!found && i<ColourSelector.Items.Count)
            {
                if(GetColourStringFromSelector(ColourSelector.Items.GetItemAt(i)) == colour)
                {
                    found = true;
                }
                i++;
            }

            ColourSelector.SelectedIndex = i-1;
        }

        private string GetColourStringFromSelector(object item)
        {
            string value = item.ToString().Split(' ')[1]; //[1] to get second value in array
            return value;
        }

        private void SpawnChange(object sender, RoutedEventArgs e)
        {
            builder.SimWorld.GroupSelected = groupDisplayed;
            ChangeSpawnButtonStatus();
        }

        private void ChangeSpawnButtonStatus()
        {
            if(groupDisplayed == builder.SimWorld.GroupSelected)
            {
                SpawnButton.Content = "—";
            }
            else
            {
                SpawnButton.Content = "Spawn";
            }
        }

        public int GroupDisplayed
        {
            get { return groupDisplayed;}
        }

        public Dictionary<int, ParticleGroupProperties> ParticleGroups
        {
            get { return particleGroups; }
            set { particleGroups = value; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
