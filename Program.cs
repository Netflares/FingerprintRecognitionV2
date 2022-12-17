using FingerprintRecognitionV2.DataStructure;

NumPair<int, int> a = new(20, 20);
NumPair<int, int> b = new(10, 5);
a += b;
Console.WriteLine(a);
