
/**
 * the parameters used in this program is no longer
 * suitable for the dataset mentioned in README.md
 * 
 * it still returns good accuracy (still around 95%)
 * but it focuses on smaller dataset now
 * 
 * I feel like I really need to make an AI module
 * that decides these params automatically tbh
 * */

using Emgu.CV;
using Emgu.CV.Structure;
using FingerprintRecognitionV2.Util;
using FingerprintRecognitionV2.MatTool;
using FingerprintRecognitionV2.DataStructure;
using FingerprintRecognitionV2.Util.Comparator;
using FingerprintRecognitionV2.Util.FeaturesExtractor;
using FingerprintRecognitionV2.Util.Preprocessing;
using System.Diagnostics;

class Program
{
    /** 
     * @ const
     * */
    static readonly int 
        ImgHeight = FingerprintRecognitionV2.Util.Preprocessing.Param.Height,
        ImgWidth = FingerprintRecognitionV2.Util.Preprocessing.Param.Width;

    /** 
     * @ drivers
     * */
    static int Main(string[] a)
    {
        // implement the cli later
        if (a.Length > 0)
        {
            switch (a[0])
            {
            case "deb":
                Debug();
                break;

            case "match-11":
                // program match-11 img-1 img-2 ans-path
                if (a.Length == 4)
                    MatchOneOne(a[1], a[2], a[3]);
                // program match-11 img-1 img-2 dat-1 dat-2 ans-path
                else if (a.Length == 6)
                    MatchOneOne(a[1], a[2], a[3], a[4], a[5]);
                else
                    PrintHelp();
                break;

            case "match-1n":
                // program match-1n
                PrintHelp();
                break;

            case "match-nn":
                // program match-nn
                PrintHelp();
                break;

            default:
                PrintHelp();
                break;
            }
        }
        else
        {
            PrintHelp();
        }
        
        return 0;
    }

    static void PrintHelp()
    {
        Console.WriteLine("Help page not yet implemented");
    }

    /**
     * @ pre-processing section
     * */

    /**
     * @ minutiae extraction section
     * */

    /**
     * @ matching section
     * */
    static void MatchOneOne(string fProbe, string fCandi, string fAns)
    {
        // pre-process
        Processor proc = new();
        Fingerprint f = ProcFingerprint(proc, fProbe);
        Fingerprint g = ProcFingerprint(proc, fCandi);

        // match
        List<Minutia> mf = new(), mg = new();
        Matcher matcher = new();
        int mCnt = matcher.Match(f, g, ref mf, ref mg);

        // return
        Console.WriteLine("matches: " + mCnt);
        Image<Bgr, byte> ans = Visualization.VisualizeComparison(fProbe, fCandi, ImgHeight, ImgWidth, mf, mg);
        CvInvoke.Imwrite(fAns, ans);
    }

    static void MatchOneOne(string fProbe, string fCandi, string fProbeDat, string fCandiDat, string fAns)
    {
        Console.WriteLine("To be implemented");
    }

    static Fingerprint ProcFingerprint(Processor proc, string f)
    {
        proc.Process(new Image<Gray, byte>(f));
        proc.PrepareForExtraction();
        List<Minutia> dat = MinutiaeExtractor.Extract(proc.SkeletonMat, proc.SegmentMsk);
        return new(dat);
    }

    /** 
     * @ tools 
     * */
    static void Debug()
    {

    }

    static void PrintTime(Stopwatch timer, string m)
    {
        Console.WriteLine(string.Format(
            "{0}: {1}:{2}.{3}", m, 
            timer.Elapsed.Minutes, 
            timer.Elapsed.Seconds.ToString("D2"), 
            timer.Elapsed.Milliseconds.ToString("D3")
        ));
    }

    static string GetParam(string[] args, int idx, string def)
    {
        if (idx >= args.Length) return def;
        return args[idx];
    }
}
 