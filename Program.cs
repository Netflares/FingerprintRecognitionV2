
/**
 * the parameters used in this program is no longer
 * suitable for the dataset mentioned in README.md
 * 
 * it still returns good accuracy (still around 95%)
 * but it focuses on another dataset now:
 * http://bias.csr.unibo.it/fvc2002/download.asp
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
        if (a.Length == 0) 
        {
            PrintHelp();
            return 0;
        }

        Stopwatch timer = new();
        timer.Start();

        int threads = 1;
        switch (a[0])
        {
        case "deb":
            Debug(a);
            break;

        case "preproc":
            try { threads = Convert.ToInt32(a[1]); } 
            catch (Exception e) { PrintHelp(); break; }

            // program preproc threads img-dir ans-dir
            if (a.Length == 4)
                PreProcess(threads, a[2], a[3]);
            else 
                PrintHelp();
            break;

        case "skel":
            try { threads = Convert.ToInt32(a[1]); } 
            catch (Exception e) { PrintHelp(); break; }

            // program skel threads minu img-dir ans-dir
            if (a.Length == 5)
                Skel(threads, a[2] == "true", a[3], a[4]);
            else 
                PrintHelp();
            break;

        case "extract":
            try { threads = Convert.ToInt32(a[1]); } 
            catch (Exception e) { PrintHelp(); break; }

            // program extract threads img-dir ans-dir
            if (a.Length == 4)
                ExtractMinutiae(threads, a[2], a[3]);
            else 
                PrintHelp();
            break;

        case "match-11":
            // program match-11 img1-path img2-path ans-path
            if (a.Length == 4)
                MatchOneOne(a[1], a[2], a[3]);
            else
                PrintHelp();
            break;

        case "match-1n":
            try { threads = Convert.ToInt32(a[1]); } 
            catch (Exception e) { PrintHelp(); break; }

            // program match-1n threads probe-path candidate-dir candidate-data-dir ans-path
            if (a.Length == 6)
                MatchOneMany(threads, a[2], a[3], a[4], a[5]);
            else
                PrintHelp();
            break;

        case "match-nn":
            try { threads = Convert.ToInt32(a[1]); } 
            catch (Exception e) { PrintHelp(); break; }

            // program match-nn threads img-dir dat-dir ans-dir
            if (a.Length == 5)
                MatchManyMany(threads, a[2], a[3], a[4]);
            else
                PrintHelp();
            break;

        default:
            PrintHelp();
            break;
        }
        
        PrintTime(timer, "program ended in");
        return 0;
    }

    static void PrintHelp()
    {
        Console.WriteLine("Help page not yet implemented");
    }

    /**
     * @ pre-processing section
     * */
    static void PreProcess(int threads, string dImg, string dAns)
    {
        string[] imgs = Directory.GetFiles(dImg);
        Directory.CreateDirectory(dAns);

        // allocate threads
        Threading<Processor>(imgs.Length, threads, (proc, i) =>
        {
            string ansPath = Path.Join(dAns, Path.GetFileName(imgs[i]));
            if ( proc.Process(new Image<Gray, byte>(imgs[i])) )
                CvInvoke.Imwrite(ansPath, MatConverter.Bool2Img(proc.SkeletonMat));
        });
    }

    static void Skel(int threads, bool minu, string dImg, string dAns)
    {
        string[] imgs = Directory.GetFiles(dImg);
        Directory.CreateDirectory(dAns);

        // allocate threads
        Threading<Processor>(imgs.Length, threads, (proc, i) =>
        {
            string ansPath = Path.Join(dAns, Path.GetFileName(imgs[i]));
            if ( !proc.Process(new Image<Gray, byte>(imgs[i])) ) return;
            proc.PrepareForExtraction();
            Image<Bgr, byte> img;

            if (minu)
                img = Visualization.VisualizeImage(
                    proc.SkeletonMat, MinutiaeExtractor.Extract(proc.SkeletonMat, proc.SegmentMsk)
                );
            else
                img = Visualization.Bool2Bgr(proc.SkeletonMat);
            CvInvoke.Imwrite(ansPath, img);
        });
    }

    /**
     * @ minutiae extraction section
     * */
    static void ExtractMinutiae(int threads, string dImg, string dAns)
    {
        string[] imgs = Directory.GetFiles(dImg);
        Directory.CreateDirectory(dAns);

        // allocate threads
        Threading<Processor>(imgs.Length, threads, (proc, i) => 
        {
            string ansPath = Path.Join(dAns, Path.GetFileName(imgs[i]) + ".inp");
            if ( !proc.Process(new Image<Gray, byte>(imgs[i])) ) return;
            proc.PrepareForExtraction();
            MinutiaeExtractor.ExtractAndExport(ansPath, proc.SkeletonMat, proc.SegmentMsk);
        });
    }

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
    static void MatchOneMany(int threads, string fProbe, string dCandi, string dData, string fAns)
    {
        // process probe
        Processor proc = new();
        Fingerprint probe = ProcFingerprint(proc, fProbe);

        // get candidates
        string[] fCandidates = Directory.GetFiles(dCandi);
        int[] score = new int[fCandidates.Length];

        // allocate threads
        Threading<Matcher>(fCandidates.Length, threads, (mtch, i) => 
        {
            string data = Path.Join(dData, Path.GetFileName(fCandidates[i]) + ".inp");
            Fingerprint candidate = new(data);
            score[i] = mtch.Match(probe, candidate);
        });

        // get best match
        int ansInd = 0, ansScore = 0;
        for (int i = 0; i < score.Length; i++) if (score[i] >= ansScore) 
        {
            ansInd = i; 
            ansScore = score[i];
        }

        // return
        Fingerprint ans = ProcFingerprint(proc, fCandidates[ansInd]);
        MatchOneOne(fProbe, fCandidates[ansInd], probe, ans, fAns);
    }

    /**
     * @ matching section - Many to Many
     * */
    static void MatchManyMany(int threads, string dImg, string dDat, string dAns)
    {
        // directories handling
        string[] fnames = Directory.GetFiles(dImg);
        int len = fnames.Length;
        for (int i = 0; i < len; i++)
            fnames[i] = Path.GetFileName(fnames[i]);
        Directory.CreateDirectory(dAns);

        // load fingerprints
        Fingerprint[] fps = new Fingerprint[len];
        for (int i = 0; i < len; i++)
            fps[i] = new(Path.Join(dDat, fnames[i] + ".inp"));

        // prep
        int[,] ans = new int[len, 2];   // { index, value }

        // allocate threads
        Threading<Matcher>(len, threads, (mtch, i) =>
        {
            for (int j = 0; j < len; j++) if (i != j)
            {
                int score = mtch.Match(fps[i], fps[j]);
                if (score >= ans[i, 1])
                {
                    ans[i, 0] = j;
                    ans[i, 1] = score;
                }
            }
        });

        // writing results
        for (int i = 0; i < len; i++)
        {
            int j = ans[i, 0];
            string fProbe = Path.Join(dImg, fnames[i]),
                   fCandi = Path.Join(dImg, fnames[j]);
            string fAns = Path.Join(dAns, string.Format(
                "{0}-{1}.png", 
                Path.GetFileNameWithoutExtension(fnames[i]),
                Path.GetFileNameWithoutExtension(fnames[j])
            ));
            MatchOneOne(fProbe, fCandi, fps[i], fps[j], fAns);
        }
    }

    /** 
     * @ tools 
     * */
    static Fingerprint ProcFingerprint(Processor proc, string f)
    {
        if (!proc.Process(new Image<Gray, byte>(f))) 
            return new(new List<Minutia>());
        proc.PrepareForExtraction();
        List<Minutia> dat = MinutiaeExtractor.Extract(proc.SkeletonMat, proc.SegmentMsk);
        return new(dat);
    }

    static void Threading<T>(int size, int threads, Action<T, int> f) where T: new()
    {
        int threadSize = (size + threads - 1) / threads;   // ceil(a / b)
        Parallel.For(0, threads, (t) => 
        {
            int l = t * threadSize, r = Math.Min((t + 1) * threadSize, size);
            T obj = new();
            for (; l < r; l++) f(obj, l);
        });
    }

    /**
     * @ debug tools
     * */
    static void Debug(string[] a)
    {
        DebugObj.Exc(a);
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
 