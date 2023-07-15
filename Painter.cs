using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Xml;

namespace wikipedia_metric
{
    public class Blob : IComparable<Blob>
    {
        public string Name;
        public List<string> Neighbours;

        public (double X, double Y) Coords;
        public double Radius;
        public double Phi;

        public int SelectAttempts;

        public Blob(string name, int size, List<string> neighbours)
        {
            Name = name;
            Radius = size / 2.0;
            Neighbours = neighbours;
        }

        public int CompareTo(Blob other)
        {
            return Name.CompareTo(other.Name);
        }

        public void TrySelect((double X, double Y) coords, double phi)
        {
            Coords = coords;
            Phi = phi;
            SelectAttempts++;
        }
        public void ResetAttempts()
        {
            SelectAttempts = 0;
        }
        public void Reset()
        {
            Coords = (0, 0);
            Phi = 0;
        }
    }

    internal class Painter
    {
        private static readonly Logger Logger;
        private readonly double CanvasSize; // px
        private readonly double UsableCanvasSize; // px

        private readonly int BorderSize; // px

        // Minimal size of a blob, eg. blob of size 1 will have 10 px
        private readonly int MinSize; // px

        private List<Blob> AllBlobs;
        private HashSet<Blob> SelectedToDraw = new HashSet<Blob>();

        private Random Rnd;
        private readonly int[] Signs = new int[2] { -1, 1 };

        static Painter()
        {
            Logger = new Logger(nameof(Painter));
        }

        public Painter(int canvas, int border, int minimalBlobSize)
        {
            BorderSize = border;
            CanvasSize = canvas;
            UsableCanvasSize = CanvasSize - 2 * BorderSize;
            MinSize = minimalBlobSize;

            Rnd = new Random();
        }

        // Ensure we can fit all the blobs to the canvas by rescaling them
        private void RescaleBlobs()
        {
            // Calculate the scale so we can multiply sizes of each blob
            // so they all can fit on the canvas
            double scalingConstant =
                (UsableCanvasSize + 1000) / (AllBlobs.Sum(blob => blob.Radius * 2) + MinSize * AllBlobs.Count);

            foreach (var blob in AllBlobs)
            {
                blob.Radius *= scalingConstant;
                if (blob.Radius < MinSize)
                    blob.Radius = MinSize;
            }
        }

        // Check if the blob we want to select to draw
        // conflicts with any other already selected blobs
        private bool Conflicts(Blob origin, Blob blob, List<Blob> list)
        {
            if (blob.SelectAttempts == 0)
                return true;

            foreach (var selected in SelectedToDraw)
            {
                var distance = Math.Sqrt(
                    Math.Pow(blob.Coords.X - selected.Coords.X, 2) +
                    Math.Pow(blob.Coords.Y - selected.Coords.Y, 2)
                );

                if ((distance == blob.Radius + selected.Radius) && (selected.CompareTo(origin) == 0))
                    continue;

                if (distance <= blob.Radius + selected.Radius)
                    return true;

            }

            foreach (var selected in list)
            {
                var distance = Math.Sqrt(
                    Math.Pow(blob.Coords.X - selected.Coords.X, 2) +
                    Math.Pow(blob.Coords.Y - selected.Coords.Y, 2)
                );

                if ((distance == blob.Radius + selected.Radius) && (selected.CompareTo(origin) == 0))
                    continue;

                if (distance <= blob.Radius + selected.Radius)
                    return true;

            }

            return false;
        }

        private List<Blob> PrintPath(Blob node)
        {
            var list = new List<Blob>() { node };
            SelectedToDraw.Add(node);
            if (node.Neighbours.Count == 0)
            {
                return list;
            }

            Blob[] selectedTest = new Blob[SelectedToDraw.Count];
            SelectedToDraw.CopyTo(selectedTest);

            foreach (var neigh_name in node.Neighbours)
            {
                var neigh_node = AllBlobs.Find(x => x.Name == neigh_name);

                // This should not be needed
                if (list.Contains(neigh_node))
                    Logger.Warning($"Node {neigh_name} already selected to draw");

                List<Blob> test = null;
                while (neigh_node.SelectAttempts <= 10 && test == null)
                {
                    var phi = node.Phi + (Signs[Rnd.Next(0, 2)] * (Rnd.NextDouble() * Math.PI));
                    var x = node.Coords.X + Math.Sin(phi) * (node.Radius + neigh_node.Radius);
                    var y = node.Coords.Y + Math.Cos(phi) * (node.Radius + neigh_node.Radius);
                    neigh_node.TrySelect((x, y), phi);

                    if (!Conflicts(node, neigh_node, list))
                    {
                        test = PrintPath(neigh_node);
                        if (test == null)
                        {
                            foreach (var n in SelectedToDraw)
                            {
                                if (selectedTest.Contains(n))
                                    continue;
                                if (n == neigh_node)
                                {
                                    n.Reset();
                                }
                                else
                                {
                                    n.ResetAttempts();
                                }
                                SelectedToDraw.Remove(n);

                            }
                        }
                    }
                }

                if (neigh_node.SelectAttempts > 10)
                {
                    neigh_node.Reset();
                    neigh_node.ResetAttempts();
                    return null;
                }
                else
                {
                    list.AddRange(test);
                }
            }
            // list.AddRange(test);
            return list;
        }

        // Set coordinates for each blob center so no blob intersects and each blob is connected only with its neighbours
        public List<Blob> PrepareToDraw(List<Blob> tree)
        {
            AllBlobs = tree;
            RescaleBlobs();

            double phi0 = 0;
            double x0 = 0;
            double y0 = 0;
            var start = tree[0];
            start.TrySelect((x0, y0), phi0);


            var test = PrintPath(start);
            SaveImage(test);
            return test;
        }

        // A function that generates and saves a svg image from the SelectedToDraw array
        // with all the blobs drawn with black color on a white background
        public void SaveImage(List<Blob> selectedToDraw)
        {
            // Create a new image with a white background
            var image = new Bitmap((int)1000, (int)1000);

            // Get a graphics object from the image
            var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, (float)1000, (float)1000));

            // Set the color of the brush to black
            var brush = new Pen(Color.Black);

            // Draw the blobs on the image
            foreach (var blob in selectedToDraw)
            {
                // Create RectangleF object of the blob circle
                var rect = new RectangleF((float)(blob.Coords.X - blob.Radius + 500), (float)(blob.Coords.Y - blob.Radius + 500), (float)blob.Radius * 2, (float)blob.Radius * 2);

                // Draw the blob on the image
                graphics.DrawString(blob.Name, new Font("Calibri", 8), Brushes.Black, rect);
                graphics.DrawEllipse(brush, rect);
            }

            // Save the image to a file
            image.Save("image.png");
        }

        // 1000 / (250 + 566 + 1 + (3 * 10)) =~ 1.18
        // public void Paint(List<Blob> paths)
        // {
        //     // Calculate the scale so we can multiply sizes of each blob
        //     // so they all can fit on the canvas
        //     // double scalingConstant = CanvasSize / (totalSize + MinSize * paths.Count);
        //     double scalingConstant = 1;

        //     var rnd = new Random();
        //     var signs = new int[2] { -1, 1 };


        //     // var r0 =
        //     var pathsEnumerator = paths.GetEnumerator();
        //     pathsEnumerator.MoveNext();

        //     double r0 = pathsEnumerator.Current.Radius * scalingConstant;
        //     double x0 = 0;
        //     double y0 = 0;
        //     var phi0 = rnd.NextDouble() * 2 * Math.PI;

        //     foreach (var blob in paths)
        //     {
        //         var r = blob.Radius;
        //         var neighbours = blob.Neighbours;
        //     }

        //     double r1 = paths[1].Radius * scalingConstant;
        //     var phi1 = phi0 + (signs[rnd.Next(0, 2)] * (rnd.NextDouble() * Math.PI / 2));
        //     double x1 = x0 + Math.Sin(phi1) * (r0 + r1);
        //     double y1 = y0 + Math.Cos(phi1) * (r0 + r1);

        //     // double r2 = paths[2].Radius * scalingConstant;
        //     // var phi2 = phi1 + (signs[rnd.Next(0, 2)] * (rnd.NextDouble() * Math.PI / 2));
        //     // double x2 = x1 + Math.Sin(phi2) * (r1 + r2);
        //     // double y2 = y1 + Math.Cos(phi2) * (r1 + r2);

        //     // double r3 = 5;
        //     // // double r3 = paths["Transport"].Radius * scalingConstant;
        //     // var phi3 = phi2 + (signs[rnd.Next(0, 2)] * (rnd.NextDouble() * Math.PI / 2));
        //     // double x3 = x2 + Math.Sin(phi3) * (r2 + r3);
        //     // double y3 = y2 + Math.Cos(phi3) * (r2 + r3);

        //     Logger.Info(signs[rnd.Next(0, 2)]);
        // }
    }
}