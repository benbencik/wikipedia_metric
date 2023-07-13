using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace wikipedia_metric
{
    public class Blob
    {
        public string Name;
        public List<string> Neighbours;

        public (double X, double Y) Coords;
        public double Radius;

        public int SelectAttempts;

        public Blob(string name, int size, List<string> neighbours)
        {
            Name = name;
            Radius = size / 2.0;
            Neighbours = neighbours;
        }

        public void Reset()
        {
            SelectAttempts = 0;
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
        private List<Blob> SelectedToDraw;

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
        }

        // Ensure we can fit all the blobs to the canvas by rescaling them
        private void RescaleBlobs()
        {
            // Calculate the scale so we can multiply sizes of each blob
            // so they all can fit on the canvas
            double scalingConstant =
                UsableCanvasSize / (AllBlobs.Sum(blob => blob.Radius * 2) + MinSize * AllBlobs.Count);

            foreach (var blob in AllBlobs)
            {
                blob.Radius *= scalingConstant;
                if (blob.Radius < MinSize)
                    blob.Radius = MinSize;
            }
        }

        // Check if the blob we want to select to draw
        // conflicts with any other already selected blobs
        private bool CheckIfConflicts(Blob blob)
        {
            foreach (var selected in SelectedToDraw)
            {
                var distance = Math.Sqrt(
                    Math.Pow(blob.Coords.X - selected.Coords.X, 2) +
                    Math.Pow(blob.Coords.Y - selected.Coords.Y, 2)
                );

                if (distance <= blob.Radius + selected.Radius)
                    return true;
            }

            return false;
        }

        // Go through all the paths and generate coordinates for each blob with respective radiuses so no blobs overlap.
        // Start in the centre of the canvas and draw a blob, than go thgough all of his Neighbours list and generate
        // coordinates for every neighbour. Than choose one of the neighbours and repeat recursively until all blobs
        // from path are selected do draw (have their coordinates and sizes correctly generated).
        // Each neighbouring blobs needs to be touching each other.
        // When some blobs overlap try 10 times to generate new coordinates for the blob and if it fails, backtrack
        //to the previous blob and regenerate its coordinates and repeat until all blobs are slected to draw.
        public void Print(List<Blob> paths)
        {
            AllBlobs = paths;
            RescaleBlobs();

            var rnd = new Random();
            var signs = new int[2] { -1, 1 };

            // Start in the centre of the canvas
            var x0 = CanvasSize / 2;
            var y0 = CanvasSize / 2;

            // Generate coordinates for the first blob
            var blob = AllBlobs[0];
            var r0 = blob.Radius;
            blob.Coords = (x0, y0);

            // Add the first blob to the list of selected blobs
            SelectedToDraw = new List<Blob> { blob };

            // Go through all the blobs and generate coordinates for each blob
            // with respective radiuses so no blobs overlap
            for (var i = 1; i < AllBlobs.Count; i++)
            {
                blob = AllBlobs[i];

                // Reset the number of attempts to generate coordinates for the blob
                blob.Reset();

                // Generate coordinates for the blob
                for (var attempt = 0; attempt < 10; attempt++)
                {
                    // Generate random angle
                    var phi = signs[rnd.Next(0, 2)] * (rnd.NextDouble() * Math.PI * 2);

                    // Generate random radius
                    var r = blob.Radius;

                    // Calculate coordinates
                    double x = x0 + Math.Sin(phi) * (r0 + r);
                    double y = y0 + Math.Cos(phi) * (r0 + r);

                    // Set coordinates
                    blob.Coords = (x, y);

                    // Check if the blob conflicts with any other already selected blobs
                    if (!CheckIfConflicts(blob))
                    {
                        // If it doesn't conflict, add it to the list of selected blobs
                        SelectedToDraw.Add(blob);

                        // Set the new centre of the canvas
                        x0 = x;
                        y0 = y;
                        r0 = r;

                        // Break out of the loop
                        break;
                    }
                }

                // If we tried 10 times to generate coordinates for the blob and failed, backtrack
                // to the previous blob and regenerate its coordinates and repeat until all blobs are slected to draw
                if (blob.SelectAttempts >= 10)
                {
                    // Remove the blob from the list of selected blobs
                    SelectedToDraw.Remove(blob);

                    // Remove alsoo all SelectedToDraw neighbours
                    foreach (var neighbour in blob.Neighbours)
                        SelectedToDraw.RemoveAll(b => b.Name == neighbour);

                    // Set the new centre of the canvas
                    x0 = SelectedToDraw.Last().Coords.X;
                    y0 = SelectedToDraw.Last().Coords.Y;
                    r0 = SelectedToDraw.Last().Radius;

                    // Go back to the previous blob and try again to generate coordinates for it
                    i -= 2;
                }
            }
            SaveImage(SelectedToDraw);
        }

        // A function that generates and saves a svg image from the SelectedToDraw array
        // with all the blobs drawn with black color on a white background
        public void SaveImage(List<Blob> selectedToDraw)
        {
            // Create a new image with a white background
            var image = new Bitmap((int)2000, (int)2000);

            // Get a graphics object from the image
            var graphics = Graphics.FromImage(image);
            graphics.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, (float)2000, (float)2000));

            // Set the color of the brush to black
            var brush = new Pen(Color.Black);

            // Draw the blobs on the image
            foreach (var blob in selectedToDraw)
            {
                // Create RectangleF object of the blob circle
                var rect = new RectangleF((float)(blob.Coords.X - blob.Radius + 500), (float)(blob.Coords.Y - blob.Radius + 500), (float)blob.Radius * 2, (float)blob.Radius * 2);

                // Draw the blob on the image
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