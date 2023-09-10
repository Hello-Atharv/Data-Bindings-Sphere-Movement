using System;
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
        SimBuild Builder;

        private bool mouseDown = false;
        private bool showQuadtree = false;

        private const double sphereSize = 16;

        private const double mouseSafetyMargin = sphereSize/2;
        private const double sphereSpawnDelay = 3;
        private const double sphereSpawnMultiplier = 0.02;

        private bool panelDragState = false;

        private double startXPos;
        private double startYPos;
        private object gettingDragged;

        public MainWindow()
        {
            InitializeComponent();

            Builder = new SimBuild();
            DataContext = Builder.SimWorld;

            Builder.TickNotify += TickTimedFunctions;

            SetUpBorder();
            

        }

        private void UpdateGameTime()
        {
            double currentGameTime = Convert.ToDouble(gameTimeDisplay.Text);
            currentGameTime = currentGameTime + Builder.DeltaT;
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
            if (Builder.Ticks % Math.Round(sphereSpawnDelay-(Math.Log10(1+Builder.SimWorld.ParticleCount*sphereSpawnMultiplier))) == 0)
            {
                SphereCreation();
            }
        }

        private void BeginPanelDrag(object sender, RoutedEventArgs e)
        {
            IInputElement panel = Mouse.DirectlyOver;

            startXPos = Mouse.GetPosition(ParticlePanel).X;
            startYPos = Mouse.GetPosition(ParticlePanel).Y;

            gettingDragged = sender;
        }

        private void PanelDrag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && sender == gettingDragged)
            {
                double newLeft = Mouse.GetPosition(canvas).X - startXPos - canvas.Margin.Left;

                ParticlePanel.SetValue(Canvas.LeftProperty, newLeft);

                double newTop = Mouse.GetPosition(canvas).Y - startYPos - canvas.Margin.Top;

                ParticlePanel.SetValue(Canvas.TopProperty, newTop);
            }
        }

        private void EndPanelDrag(object sender, RoutedEventArgs e)
        {
            gettingDragged = null;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Ellipse ellipse = new Ellipse()
            {
                Width = 50,
                Height = 50,
                Stroke = Brushes.Black
            };

            int x = 60;
            int y = 70;

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);

            canvas.Children.Add(ellipse);

            Builder.ParticleAdd(x, y);

            Window win = new Window();
            win.Show();

        }

        private void SphereCreation()
        {
            const double mouseOffset = sphereSize / 2;
            double xPos;
            double yPos;
            if (mouseDown)
            {
                
                xPos = Mouse.GetPosition(canvas).X - mouseOffset;
                yPos = Mouse.GetPosition(canvas).Y - mouseOffset;

                if (CheckWithinBorder(xPos, yPos) && Mouse.DirectlyOver != ParticlePanel)
                {
                    AddSphere(xPos, yPos);
                }
                
            }
        }

        private void AddSphere(double mouseXPos, double mouseYPos)
        {

            PosConv posConv = new PosConv();

            Binding xPos = new Binding("XValue");
            Binding yPos = new Binding("YValue");

            xPos.Converter = posConv;
            yPos.Converter = posConv;

            Ellipse ellipse = new Ellipse()
            {
                Width = sphereSize,
                Height = sphereSize,
                Stroke = Brushes.Black,
                Fill = Brushes.Blue
            };

            Canvas.SetLeft(ellipse, mouseXPos);
            Canvas.SetTop(ellipse, mouseYPos);

            Panel.SetZIndex(ellipse, 1);

            canvas.Children.Add(ellipse);

            Builder.ParticleAdd(mouseXPos, mouseYPos);

            xPos.Source = Builder.SimWorld.AllParticles[Builder.SimWorld.ParticleCount - 1].Position;
            yPos.Source = Builder.SimWorld.AllParticles[Builder.SimWorld.ParticleCount - 1].Position;

            ellipse.SetBinding(Canvas.LeftProperty, xPos);
            ellipse.SetBinding(Canvas.TopProperty, yPos);

        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
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
            BoxBorder.Width = Builder.SimWorld.PerimeterWidth;
            BoxBorder.Height = Builder.SimWorld.PerimeterHeight;
            Canvas.SetLeft(BoxBorder, (Window.Width - BoxBorder.Width) / 2);
            Canvas.SetTop(BoxBorder, (Window.Height - BoxBorder.Height) / 2);
        }

        private void TempValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tempDisplay.Text = Convert.ToString(tempSlider.Value);
        }

        private void DisplayQuadtree()
        {
            Quadtree quadtree = Builder.SimWorld.GetQuadtree;

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

        private void ToggleParticlesPanel(object sender, RoutedEventArgs e)
        {
            
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
    }
}
