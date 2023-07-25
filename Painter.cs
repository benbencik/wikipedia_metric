using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace wikipedia_metric
{
    public class Node : IComparable<Node>
    {
        public readonly string Name;
        public readonly List<string> Neighbours;

        public (double X, double Y) Coords;
        public double Radius;
        public double Phi;

        public int SelectAttempts;

        public Node(string name, int size, List<string> neighbours)
        {
            Name = name;
            Radius = size / 2.0;
            Neighbours = neighbours;
        }

        public int CompareTo(Node other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public void SelectCoords((double X, double Y) coords, double phi)
        {
            Coords = coords;
            Phi = phi;
            SelectAttempts++;
        }

        public void ResetSelectAttempts()
        {
            SelectAttempts = 0;
        }

        public void ResetCoords()
        {
            Coords = (0, 0);
            Phi = 0;
        }
    }

    internal class Painter
    {
        private static readonly Logger Logger;
        private readonly int PrintableCanvasSize; // px

        // Minimal size of a blob, eg. blob of size 1 will have 10 px
        private readonly int MinSize; // px

        private List<Node> AllNodes;
        private readonly HashSet<Node> SelectedToDraw = new();

        private readonly Random RandomGenerator;
        private const int MaxSelectAttempts = 100;
        private const double TOLERANCE = 0.001;
        private const double MinNeighbourDistance = 2;
        private readonly int[] Signs = new int[2] { -1, 1 };
        private double RotationFactor;

        static Painter()
        {
            Logger = new Logger(nameof(Painter));
        }

        public Painter(int canvas, int border, int minimalBlobSize)
        {
            PrintableCanvasSize = canvas - 2 * border;
            MinSize = minimalBlobSize;

            RandomGenerator = new Random();
        }

        // Ensure we can fit all the blobs to the canvas by rescaling them in place
        private List<Node> RescaleNodes(List<Node> nodes)
        {
            // Calculate the scale so we can multiply sizes of each blob
            // so they all can fit on the canvas
            double scalingConstant =
                PrintableCanvasSize / (nodes.Sum(blob => blob.Radius * 2) + MinSize * nodes.Count);

            foreach (var blob in nodes)
            {
                blob.Radius *= scalingConstant;
                // We assume some minimal node size when calculating the scaling constant
                // and sometimes the node has lower size as the minimal size so we clip it
                if (blob.Radius < MinSize)
                    blob.Radius = MinSize;
            }

            return nodes;
        }

        // Check if the blob we want to select to draw
        // conflicts with any other already selected nodes
        private bool CanDraw(Node origin, Node node, List<Node> currentlySelectedToDraw)
        {
            if (node.SelectAttempts == 0)
                return false;

            // Check if a node given some coordinates can be drawn without any
            // conflicts - it does not intersect with any other node and it
            //  touches only it's origin node
            foreach (var selected in SelectedToDraw)
            {
                var distance = Math.Sqrt(
                    Math.Pow(node.Coords.X - selected.Coords.X, 2) +
                    Math.Pow(node.Coords.Y - selected.Coords.Y, 2)
                );

                if ((Math.Abs(distance - (node.Radius + selected.Radius)) < TOLERANCE) && (selected == origin))
                    continue;

                if (distance <= node.Radius + selected.Radius + MinNeighbourDistance)
                    return false;
            }

            foreach (var selected in currentlySelectedToDraw)
            {
                var distance = Math.Sqrt(
                    Math.Pow(node.Coords.X - selected.Coords.X, 2) +
                    Math.Pow(node.Coords.Y - selected.Coords.Y, 2)
                );

                if ((Math.Abs(distance - (node.Radius + selected.Radius)) < TOLERANCE) &&
                    (selected.CompareTo(origin) == 0))
                    continue;

                if (distance <= node.Radius + selected.Radius + MinNeighbourDistance)
                    return false;
            }

            return true;
        }

        // For each neighbour of a given node try selecting non conflicting
        // coordinates and repeat this process for the neighbour neighbous etc.
        // When no non-conflicting coordinates can be selected, backtrack
        // and try selecting other coordinates for the parent node.
        // Repeat until all nodes have properly selected coordinates or
        // until we tried every possibility and it is simply not possible to
        // draw given tree.
        private List<Node> PrepareNodesToDraw(Node node)
        {
            var currentlySelectedToDraw = new List<Node>() { node };
            SelectedToDraw.Add(node);
            if (node.Neighbours.Count == 0)
            {
                return currentlySelectedToDraw;
            }

            Node[] selectedToDrawCheckpoint = new Node[SelectedToDraw.Count];
            SelectedToDraw.CopyTo(selectedToDrawCheckpoint);

            foreach (var neighNodeName in node.Neighbours)
            {
                var neighNode = AllNodes.Find(x => x.Name == neighNodeName);

                List<Node> progress = null;
                // Check if we can still select some coordinates or if we selected too many times
                // -> the likelihood of finding some non-conflicting coordinates is low for this
                // node so we need to change something else
                while (neighNode.SelectAttempts <= MaxSelectAttempts && progress == null)
                {
                    var phi = node.Phi + (Signs[RandomGenerator.Next(0, 2)] *
                                          (RandomGenerator.NextDouble() * Math.PI / RotationFactor));
                    var x = node.Coords.X + Math.Sin(phi) * (node.Radius + neighNode.Radius);
                    var y = node.Coords.Y + Math.Cos(phi) * (node.Radius + neighNode.Radius);

                    neighNode.SelectCoords((x, y), phi);

                    if (!CanDraw(node, neighNode, currentlySelectedToDraw)) continue;

                    progress = PrepareNodesToDraw(neighNode);

                    if (progress != null) continue;

                    foreach (var wronglySelectedNode in SelectedToDraw)
                    {
                        if (selectedToDrawCheckpoint.Contains(wronglySelectedNode))
                            continue;

                        if (wronglySelectedNode == neighNode)
                            wronglySelectedNode.ResetCoords();
                        else
                            wronglySelectedNode.ResetSelectAttempts();

                        SelectedToDraw.Remove(wronglySelectedNode);
                    }
                }

                if (neighNode.SelectAttempts > MaxSelectAttempts)
                {
                    neighNode.ResetCoords();
                    neighNode.ResetSelectAttempts();
                    return null;
                }

                currentlySelectedToDraw.AddRange(progress!);
            }

            return currentlySelectedToDraw;
        }

        // Set coordinates for each blob center so no blob intersects and each blob is connected only with its neighbours
        private List<Node> PrepareNodes(List<Node> tree, double rotationFactor = 4, double phi0 = 0, double x0 = 0,
            double y0 = 0)
        {
            RotationFactor = rotationFactor;
            AllNodes = RescaleNodes(tree);

            // Start the algorithm from a arbitrary node at arbitrary coordinates
            var startingNode = AllNodes[0];
            startingNode.SelectCoords((x0, y0), phi0);

            var drawableNodes = PrepareNodesToDraw(startingNode);
            return FitToCanvas(drawableNodes);
        }

        // Calculate the rectangular boundary that fits exactly all nodes and
        // move the boundary so it all fits into the canvas coordinates
        // (translate the final image so it can be drawn to the canvas)
        private List<Node> FitToCanvas(List<Node> nodes)
        {
            double top = 0;
            double right = 0;
            double bottom = 0;
            double left = 0;

            foreach (var node in nodes)
            {
                if (node.Coords.Y - node.Radius < top)
                    top = node.Coords.Y - node.Radius;
                if (node.Coords.X + node.Radius > right)
                    right = node.Coords.X + node.Radius;
                if (node.Coords.Y + node.Radius > bottom)
                    bottom = node.Coords.Y + node.Radius;
                if (node.Coords.X - node.Radius < left)
                    left = node.Coords.X - node.Radius;
            }

            Logger.Info(top);
            Logger.Info(right);
            Logger.Info(bottom);
            Logger.Info(left);

            return nodes;
        }

        // A function that generates and saves a svg image from the SelectedToDraw array
        // with all the nodes drawn with black color on a white background.
        // This is a temporary solution. Most likely will be replaced by Ben's own implementation.
        public void PaintToImage(List<Node> paths, string imgPath)
        {
            // Create a new image with a white background
            var image = new Bitmap(PrintableCanvasSize, PrintableCanvasSize);

            // Get a graphics object from the image
            var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, (float)1000, (float)1000));

            // Set the color of the brush to black
            var brush = new Pen(Color.Black);

            // Draw the blobs on the image
            foreach (var node in PrepareNodes(paths))
            {
                // Create RectangleF object of the blob circle
                var rect = new RectangleF((float)(node.Coords.X - node.Radius),
                    (float)(node.Coords.Y - node.Radius), (float)node.Radius * 2, (float)node.Radius * 2);

                // Draw the blob on the image
                graphics.DrawString(node.Name, new Font("Calibri", 8), Brushes.Black, rect);
                graphics.DrawEllipse(brush, rect);
            }

            // Save the image to a file
            image.Save(imgPath);
        }
    }
}