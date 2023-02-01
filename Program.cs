using DelaunatorSharp;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

Point[] arr = new Point[4] { new(2, 3), new(5, 1), new(3, 5), new(6, 5) };
List<IPoint> pts = new(4);

for (int i = 0; i < 4; i++) pts.Add(arr[i]);

Delaunator d = new(pts.ToArray());
for (int i = 0; i < d.Triangles.Length; i++)
    Console.Write(d.Triangles[i] + " ");
Console.Write("\n");
