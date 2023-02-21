
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
using System.IO;

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
                // program match-11 img1-path img2-path ans-path
                if (a.Length == 4)
                    MatchOneOne(a[1], a[2], a[3]);
                else
                    PrintHelp();
                break;

            case "match-1n":
                // program match-1n thread-cnt probe-path candidate-dir ans-path
                
                if (a.Length == 5) 
                {
                    int threads = 1;
                    try 
                    { 
                        threads = Convert.ToInt32(a[1]); 
                    } 
                    catch (Exception e) { PrintHelp(); break; }
                    MatchOneMany(threads, a[2], a[3], a[4]);
                }
                else
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
     * @ matching section - One to One
     * */
    static void MatchOneOne(string fProbe, string fCandi, string fAns)
    {
        // pre-process
        Processor proc = new();
        Fingerprint f = ProcFingerprint(proc, fProbe);
        Fingerprint g = ProcFingerprint(proc, fCandi);

        MatchOneOne(fProbe, fCandi, f, g, fAns);
    }

    static void MatchOneOne(string fProbe, string fCandi, Fingerprint probe, Fingerprint candi, string fAns)
    {
        // match
        List<Minutia> mf = new(), mg = new();
        Matcher matcher = new();
        int mCnt = matcher.Match(probe, candi, ref mf, ref mg);

        // return
        Image<Bgr, byte> ans = Visualization.VisualizeComparison(fProbe, fCandi, ImgHeight, ImgWidth, mf, mg);
        CvInvoke.Imwrite(fAns, ans);
    }

    /**
     * @ matching section - One to Many
     * */
    static void MatchOneMany(int threads, string fProbe, string dCandi, string fAns)
    {
        // process probe
        Processor proc = new();
        Fingerprint probe = ProcFingerprint(proc, fProbe);

        // get candidates
        string[] fCandidates = Directory.GetFiles(dCandi);
        int[] score = new int[fCandidates.Length];

        // allocate threads
        int threadSize = (fCandidates.Length + threads - 1) / threads;   // ceil(a / b)
        Parallel.For(0, threads, (t) =>
        {
            int l = t * threadSize, r = Math.Min((t + 1) * threadSize, fCandidates.Length);
            Processor proc = new();
            Matcher mtch = new();
            for (; l < r; l++) 
            {
                Fingerprint candidate = ProcFingerprint(proc, fCandidates[l]);
                score[l] = mtch.Match(probe, candidate);
            }
        });

        // get best match
        int ansInd = 0, ansScore = -1;
        for (int i = 0; i < score.Length; i++) if (score[i] >= ansScore) 
        {
            ansInd = i; ansScore = score[i];
        }

        // return
        Fingerprint ans = ProcFingerprint(proc, fCandidates[ansInd]);
        MatchOneOne(fProbe, fCandidates[ansInd], probe, ans, fAns);
    }

    /** 
     * @ tools 
     * */
    static Fingerprint ProcFingerprint(Processor proc, string f)
    {
        proc.Process(new Image<Gray, byte>(f));
        proc.PrepareForExtraction();
        List<Minutia> dat = MinutiaeExtractor.Extract(proc.SkeletonMat, proc.SegmentMsk);
        return new(dat);
    }

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
 