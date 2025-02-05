using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace FractalFern
{
    public partial class MainWindow : Window
    {
        // Random number generator for creating variations in the fractal   
        private Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent(); // Initializes the WPF components

            // Set initial slider values
            BranchLengthSlider.Value = 70; // Default length
            AngleSlider.Value = 20; // Default angle variance
            RandomnessSlider.Value = 0; // Default randomness

            this.Loaded += MainWindow_Loaded;

            


            // Redraw the fern when sliders are adjusted
            BranchLengthSlider.ValueChanged += (s, e) => DrawFern();
            AngleSlider.ValueChanged += (s, e) => DrawFern();
            RandomnessSlider.ValueChanged += (s, e) => DrawFern();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DrawFern(); // Draw the fern once the window is loaded
        }
        private void DrawFern()
        {
            fernCanvas.Children.Clear();
            double initialLength = BranchLengthSlider.Value;
            double angleVariance = AngleSlider.Value;
            double randomness = RandomnessSlider.Value;

            // Start drawing the fractal from the bottom center
            DrawBranch(fernCanvas.ActualWidth / 2, fernCanvas.ActualHeight, initialLength, -90, angleVariance, randomness, 100);
        }

        private void DrawBranch(double x, double y, double length, double angle, double angleVariance, double randomness, int depth)
        {
            if (length < 5 || depth <= 0) return;

            // Random variations for branch length and angle
            length *= 0.75 + (rand.NextDouble() - 0.5) * randomness / 20.0;
            angle += (rand.NextDouble() - 0.5) * randomness;

            // Calculate endpoint of the branch
            double xEnd = x + length * Math.Cos(angle * Math.PI / 180);
            double yEnd = y + length * Math.Sin(angle * Math.PI / 180);

            // Check if the endpoint is within the canvas boundaries
            if (xEnd < 0 || xEnd > fernCanvas.ActualWidth || yEnd < 0 || yEnd > fernCanvas.ActualHeight)
            {
                return; // Do not draw if outside the canvas
            }


            // Draw the main branch line
            Line line = new Line
            {
                X1 = x,
                Y1 = y,
                X2 = xEnd,
                Y2 = yEnd,
                Stroke = Brushes.LightGreen,
                StrokeThickness = 2
            };
            fernCanvas.Children.Add(line);

            // Optionally draw an ellipse or polygon as a leaf
            if (depth % 2 == 0)
            {
                Shape leaf = CreateRandomLeaf(xEnd, yEnd);
                fernCanvas.Children.Add(leaf);
            }

            // Recursive calls for branches
            DrawBranch(xEnd, yEnd, length, angle + angleVariance, angleVariance, randomness, depth - 1);
            DrawBranch(xEnd, yEnd, length, angle - angleVariance, angleVariance, randomness, depth - 1);
        }

        private Shape CreateRandomLeaf(double x, double y)
        {
            Shape leaf;
            if (rand.Next(2) == 0)
            {
                // Create an ellipse leaf
                leaf = new Ellipse
                {
                    Width = 5 + rand.Next(5),
                    Height = 10 + rand.Next(10),
                    Fill = Brushes.DarkGreen
                };
            }
            else
            {
                // Create a triangular polygon as a leaf
                Polygon polygon = new Polygon
                {
                    Fill = Brushes.ForestGreen,
                    Points = new PointCollection // Define the triangle's vertices
                    {
                        new Point(x, y),
                        new Point(x - 5, y + 10),
                        new Point(x + 5, y + 10)
                    }
                };
                leaf = polygon; // Assign the polygon to leaf variable
            }
            // Center the leaf on its position
            Canvas.SetLeft(leaf, x - leaf.Width / 2);
            Canvas.SetTop(leaf, y - leaf.Height / 2);
            return leaf;
        }
    }
}


