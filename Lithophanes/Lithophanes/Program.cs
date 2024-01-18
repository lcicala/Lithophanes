using SkiaSharp;
using STLVolumeCalculator;
using System;
using System.IO;
using System.Text;

namespace Lithophanes
{
    class Program
    {
        static void Main(string[] args)
        {
            int Width = int.Parse(args[0]);
            int Height = int.Parse(args[1]);
            int Downscale = int.Parse(args[2]);
            double Depth = double.Parse(args[3]);
            SKBitmap image = SKBitmap.FromImage(SKImage.FromEncodedData(args[4]));
            double[,] values = new double[image.Width / Downscale + 1, image.Height / Downscale + 1];

            for (int i = 0; i < image.Width; i += Downscale)
            {
                for (int j = 0; j < image.Height; j += Downscale)
                {
                    var pixel = image.GetPixel(i, j);
                    var value = (255 - pixel.Red + pixel.Green + pixel.Blue) / 3.0;
                    values[i / Downscale, j / Downscale] = value / 255.0 * Depth;
                }
            }
            Mesh mesh = new Mesh();
            for (int i = 0; i < image.Width / Downscale; i++)
            {
                for (int j = 0; j < image.Height / Downscale; j++)
                {
                    var X1 = (double)i / image.Width / Downscale * Width;
                    var Y1 = (double)j / image.Height / Downscale * Height;
                    var Z1 = values[i, j];
                    var X2 = (double)(i + 1) / image.Width / Downscale * Width;
                    var Y2 = (double)j / image.Height / Downscale * Height;
                    var Z2 = values[i + 1, j];
                    var X3 = (double)i / image.Width / Downscale * Width;
                    var Y3 = (double)(j + 1) / image.Height / Downscale * Height;
                    var Z3 = values[i, j + 1];
                    var V1 = new Vector3(X1, Y1, Z1);
                    var V2 = new Vector3(X2, Y2, Z2);
                    var V3 = new Vector3(X3, Y3, Z3);
                    var N = ((V2 - V1)|(V3 - V1));
                    var triangle = new Triangle(V1, V2, V3, N);
                    mesh.Triangles.Add(triangle);
                    X1 = (double)(i + 1) / image.Width / Downscale * Width;
                    Y1 = (double)(j + 1) / image.Height / Downscale * Height;
                    Z1 = values[i + 1, j + 1];
                    X2 = (double)(i + 1) / image.Width / Downscale * Width;
                    Y2 = (double)j / image.Height / Downscale * Height;
                    Z2 = values[i + 1, j];
                    X3 = (double)i / image.Width / Downscale * Width;
                    Y3 = (double)(j + 1) / image.Height / Downscale * Height;
                    Z3 = values[i, j + 1];
                    V1 = new Vector3(X1, Y1, Z1);
                    V2 = new Vector3(X2, Y2, Z2);
                    V3 = new Vector3(X3, Y3, Z3);
                    N = ((V2 - V1)|(V3 - V1));
                    triangle = new Triangle(V1, V2, V3, N);
                    mesh.Triangles.Add(triangle);
                }
            }
            FileStream fs = new FileStream(args[5], FileMode.Create);
            fs.Write(Encoding.ASCII.GetBytes(mesh.ExportSTLA()));
            fs.Flush();
            fs.Close();
        }
    }
}
