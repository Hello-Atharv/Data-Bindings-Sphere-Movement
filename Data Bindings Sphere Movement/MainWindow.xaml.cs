﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataBindingsSphereMovement
{
    
    public partial class MainWindow : Window
    {
        private SimBuild builder;
        private ParticlePanel partPanel;
        private TemperaturePanel tempPanel;
        private GravityPanel gravityPanel;
        private CollisionPanel collPanel;
        
        private bool mouseDown = false;
        private bool showQuadtree = false;

        private const double sphereSize = 30;

        private const double mouseSafetyMargin = sphereSize/2;
        private const double sphereSpawnDelay = 3;
        private const double sphereSpawnMultiplier = 0.02;

        public MainWindow()
        {
            InitializeComponent();

            builder = new SimBuild();
            partPanel = new ParticlePanel(builder);
            tempPanel = new TemperaturePanel(builder);
            gravityPanel = new GravityPanel(builder);
            collPanel = new CollisionPanel(builder);

            DataContext = builder.SimWorld;

            builder.TickNotify += TickTimedFunctions;

            SetUpBorder();

            tempPanel.ClosePanel += CloseTempPanel;
            partPanel.ClosePanel += ClosePartPanel;
            gravityPanel.ClosePanel += CloseGravityPanel;
            collPanel.ClosePanel += CloseCollisionPanel;
        }

        private void UpdateGameTime()
        {
            double currentGameTime = Convert.ToDouble(gameTimeDisplay.Text);
            currentGameTime = currentGameTime + builder.DeltaT;
            gameTimeDisplay.Text = Convert.ToString(Math.Round(currentGameTime, 2));

        }

        private void TickTimedFunctions(object sender, EventArgs e)
        {
            UpdateGameTime();
            if (showQuadtree)
            {
                DisplayQuadtree();
            }
            else
            {
                DeleteQuadtree();
            }

            if (builder.Ticks % Math.Round(sphereSpawnDelay - (Math.Log10(1 + builder.SimWorld.ParticleCount * sphereSpawnMultiplier))) == 0)
            {
                SphereCreation();
            }
            /*
            try
            {
                itemDirectlyOver.Text = Convert.ToString(Panel.GetZIndex((UIElement)Mouse.DirectlyOver));
                
                if(Mouse.DirectlyOver == slider)
                {
                    itemDirectlyOver.Text = itemDirectlyOver.Text + " ooga booba";
                }
            }
            catch(Exception ex)
            {
                 
            }*/
            
        }
        /*
        private void PreviewDown(object sender, MouseButtonEventArgs e)
        {
            firstXPos = e.GetPosition(ParticlePanel).X;
            firstYPos = e.GetPosition(ParticlePanel).Y;

            movingObject = sender;
        }

        private void PreviewUp(object sender, MouseButtonEventArgs e)
        {
            movingObject = null;
        }

        private void MoveMouse(object sender, MouseEventArgs e)
        {
                
                if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
                {
                    mouseDown = false;
                    double newLeft = e.GetPosition(canvas).X - firstXPos - canvas.Margin.Left;

                    ParticlePanel.SetValue(Canvas.LeftProperty, newLeft);

                    double newTop = e.GetPosition(canvas).Y - firstYPos - canvas.Margin.Top;

                    ParticlePanel.SetValue(Canvas.TopProperty, newTop);

                }
            
        }
        */
        private void SphereCreation()
        {
            const double mouseOffset = sphereSize / 2; //problem of collisions is due to offsets - fix sphereSize, mouseOffset and PosConv converter
            double xPos;
            double yPos;
            if (mouseDown)
            {
                
                xPos = Mouse.GetPosition(canvas).X- mouseOffset;
                yPos = Mouse.GetPosition(canvas).Y-mouseOffset;

                if (CheckWithinBorder(xPos, yPos) && Panel.GetZIndex((UIElement)Mouse.DirectlyOver)<1 && Panel.GetZIndex((UIElement)Mouse.DirectlyOver)<50)
                {
                    AddSphere(xPos, yPos);
                }
                
            }
        }

        private void AddSphere(double mouseXPos, double mouseYPos)
        {

            Attributes diameterGroupPath = builder.SimWorld.AttributesDictionary[builder.SimWorld.GroupSelected];
            double diameterPath = diameterGroupPath.Diameter;

            PosConv posConv = new PosConv(diameterPath);
            //RadiusConv radiusConv = new RadiusConv();

            Binding xPos = new Binding("XValue");
            Binding yPos = new Binding("YValue");

            Binding diameter = new Binding("Diameter");

            Binding colour = new Binding("Colour");

            //xPos.Converter = posConv;
            //yPos.Converter = posConv;

            //diameter.Converter = radiusConv;

            Ellipse ellipse = new Ellipse()
            {
                Width = diameterPath,
                Height = diameterPath,
                Stroke = Brushes.Black,
                Fill = Brushes.Red
            };

            Canvas.SetLeft(ellipse, mouseXPos);
            Canvas.SetTop(ellipse, mouseYPos);

            Panel.SetZIndex(ellipse, 1);

            canvas.Children.Add(ellipse);

            builder.ParticleAdd(mouseXPos, mouseYPos);

            xPos.Source = builder.SimWorld.AllParticles[builder.SimWorld.ParticleCount - 1].Position;
            yPos.Source = builder.SimWorld.AllParticles[builder.SimWorld.ParticleCount - 1].Position;

            diameter.Source = diameterGroupPath;
            colour.Source = partPanel.ParticleGroups[partPanel.GroupDisplayed];
            
            ellipse.SetBinding(Canvas.HeightProperty, diameter);
            ellipse.SetBinding(Canvas.WidthProperty, diameter);

            ellipse.SetBinding(Ellipse.FillProperty, colour);

            ellipse.SetBinding(Canvas.LeftProperty, xPos);
            ellipse.SetBinding(Canvas.TopProperty, yPos);

        }
        private void Close(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void MouseHeld(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            mouseCoords.Text = Convert.ToString(Mouse.GetPosition(canvas).X) + " , " + Convert.ToString(Mouse.GetPosition(canvas).Y);
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private bool CheckWithinBorder(double xPos, double yPos)
        {
            bool withinBorder = false;

            if ((xPos > Canvas.GetLeft(BoxBorder) + mouseSafetyMargin + sphereSize / 2) && (xPos < Canvas.GetLeft(BoxBorder) - sphereSize / 2 + BoxBorder.Width - mouseSafetyMargin) && (yPos > Canvas.GetTop(BoxBorder) + sphereSize / 2 + mouseSafetyMargin) && (yPos < Canvas.GetTop(BoxBorder) - sphereSize / 2 + BoxBorder.Height - mouseSafetyMargin))
            {
                withinBorder = true;
            }

            return withinBorder;
        }

        private void SetUpBorder()
        {
            BoxBorder.Width = builder.SimWorld.PerimeterWidth;
            BoxBorder.Height = builder.SimWorld.PerimeterHeight;
            Canvas.SetLeft(BoxBorder, (Window.Width - BoxBorder.Width) / 2);
            Canvas.SetTop(BoxBorder, (Window.Height - BoxBorder.Height) / 2);
        }

        private void DisplayQuadtree()
        {
            Quadtree quadtree = builder.SimWorld.GetQuadtree;

            DeleteQuadtree();

            foreach(Node node in quadtree.Nodes)
            {
                
                CreateRectangle(node.BottomRight.XValue-node.TopLeft.XValue,node.BottomRight.YValue-node.TopLeft.YValue,node.TopLeft.XValue, node.TopLeft.YValue);
            }

        }

        private void CreateRectangle(double width, double height, double leftCoordinate, double topCoordinate)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Black
            };

            Canvas.SetTop(rectangle, topCoordinate);
            Canvas.SetLeft(rectangle, leftCoordinate);

            canvas.Children.Add(rectangle);
        }

        private void ToggleQuadtree(object sender, RoutedEventArgs e)
        {
            showQuadtree = !showQuadtree;
        }

        private void OpenParticlePanel(object sender, RoutedEventArgs e)
        {
            ToggleUIElement(partPanel);
        }

        private void OpenTempPanel(object sender, RoutedEventArgs e)
        {
            ToggleUIElement(tempPanel);
        }

        private void OpenGravityPanel(object sender, RoutedEventArgs e)
        {
            ToggleUIElement(gravityPanel);
        }

        private void OpenCollisionPanel(object sender, RoutedEventArgs e)
        {
            ToggleUIElement(collPanel);
        }

        private void CloseTempPanel()
        {
            tempPanel.Hide();
            TemperaturePanelTrigger.IsChecked = false;
        }

        private void ClosePartPanel()
        {
            partPanel.Hide();
            ParticlePanelTrigger.IsChecked = false;
        }

        private void CloseGravityPanel()
        {
            gravityPanel.Hide();
            GravityPanelTrigger.IsChecked = false;
        }

        private void CloseCollisionPanel()
        {
            collPanel.Hide();
            CollisionPanelTrigger.IsChecked = false;
        }

        private void DeleteQuadtree()
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (canvas.Children[i].GetType() == typeof(Rectangle))
                {
                    canvas.Children.RemoveAt(i);
                }
            }
        }

        private void ToggleSimulationTime(object sender, RoutedEventArgs e)
        {
            
            ToggleUIElement(gameTimeDisplay);
            ToggleUIElement(gameTimeLabel);
        }
        private void ToggleParticleCount(object sender, RoutedEventArgs e)
        {
            ToggleUIElement(particleCountDisplay);
            ToggleUIElement(particleCountLabel);
        }

        private void ToggleUIElement(UIElement element)
        {
            if(element.Visibility == Visibility.Visible)
            {
                element.Visibility = Visibility.Collapsed;
            }
            else
            {
                element.Visibility = Visibility.Visible;
            }
        }

        private NotifyClosing ClosePanel(UIElement panel, MenuItem trigger)
        {
            panel.Visibility = Visibility.Collapsed;
            trigger.IsChecked = false;

            return null;
        }

        private void OpenHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Figure it out yourself!!");
        }
    }
}
