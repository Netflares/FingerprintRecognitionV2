using DelaunatorSharp;
using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

const int n = 5;

Point[] arr = new Point[n] { new(2, 3), new(5, 1), new(3, 5), new(6, 5), new(6, 5) };
List<IPoint> pts = new(n);

for (int i = 0; i < n; i++) pts.Add(arr[i]);

Delaunator d = new(pts.ToArray());
for (int i = 0; i < d.Triangles.Length; i++)
    Console.Write(d.Triangles[i] + " ");
Console.Write("\n");
