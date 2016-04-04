using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace clusters
{
    class Program
    {
        static void Main(string[] args)
        {

            FileStream fs = new FileStream("clustered", FileMode.Open);
            StreamReader sr = new StreamReader(fs);


            FileStream fs2 = new FileStream("save", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            double[] mapping = new double[] { 32, 12.9412, 44, 7.2110, 11, 6.7403, 14, 6.7047, 13, 6.6206, 8, 6.6136, 48, 6.5935, 26, 6.5810, 36, 6.5181, 27, 6.5171, 33, 6.4018, 10, 6.3843, 9, 6.3272, 7, 6.3253, 49, 6.3178, 12, 6.2172, 5, 6.1988, 3, 6.1125, 1, 5.9843, 42, 5.8187, 40, 5.7928, 6, 5.7797, 43, 5.7167, 4, 5.5851, 17, 5.4927, 19, 5.4417, 39, 5.4197, 30, 5.1564, 18, 5.1353, 31, 5.0705, 22, 5.0467, 15, 4.9631, 45, 4.9174, 47, 4.9004, 29, 4.7532, 38, 4.7450, 41, 4.7334, 37, 4.7074, 46, 4.7051, 2, 4.7043, 34, 4.6993, 23, 4.6412, 21, 4.6105, 0, 4.4549, 16, 4.4029, 20, 4.2553, 28, 4.2450, 25, 4.1949, 24, 3.6278, 35, 3.1137 };

            Dictionary<int,double> map = new Dictionary<int,double>();
            for (int i = 0; i < mapping.Length; i+=2)
            {
                map.Add((int)mapping[i],mapping[i+1]);
            }

            Task t = null;
            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();

                string[] ss = s.Split(new char[] { ',' });
                if (t != null)
                {
                    t.Wait();
                }
                t = sw.WriteLineAsync(String.Format("{0},{1}", ss[0], map[ Convert.ToInt32(ss[1]) ].ToString()));
            }

            t.Wait();

            sw.Close();
            fs2.Close();

            fs.Close();
            sr.Close();

            Console.WriteLine("Done");
            Console.ReadLine();

        }


        static void score()
        {
            int k = 50;
            HashSet<string>[] projects = new HashSet<string>[k];
            int[,] scores = new int[k, 2];
            for (int i = 0; i < k; i++)
            {
                projects[i] = new HashSet<string>();
                scores[i, 0] = 0;
                scores[i, 1] = 0;
            }


            FileStream fs = new FileStream(@"C:\Users\Malcolm\Documents\Visual Studio 2012\Projects\okaymeansno\okaymeansno\bin\Debug\clustered", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                string[] ss = s.Split(new char[] { ',' }, 2);
                projects[Convert.ToInt32(ss[1])].Add(ss[0]);
            }

            sr.Close();
            fs.Close();



            fs = new FileStream(@"C:\Users\Malcolm\Documents\HKUST\COMP4332\outcomes.csv", FileMode.Open);
            sr = new StreamReader(fs);



            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                string[] ss = s.Split(new char[] { ',' });

                int c = -1;
                for (int i = 0; i < k; i++)
                {
                    if (projects[i].Contains(ss[0]))
                    {
                        c = i;
                        break;
                    }
                }
                if (c != -1)
                {
                    if (ss[1] == "t")
                    {
                        scores[c, 0]++;
                    }
                    else
                    {
                        scores[c, 1]++;
                    }
                }
            }


            sr.Close();
            fs.Close();

            for (int i = 0; i < k; i++)
            {
                Console.WriteLine("{0},{1},{2},{3}", i, scores[i, 0], scores[i, 1], projects[i].Count, scores[i, 0] / scores[i, 1]);
            }
        }


        static void sample()
        {
            FileStream fs = new FileStream("clustered", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            int sample = 11;
            int sampleSize = 10;
            List<string> projectsInCluster = new List<string>() { };

            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                string[] ss = s.Split(new char[] { ',' }, 2);

                if (Convert.ToInt32(ss[1]) == sample)
                {
                    projectsInCluster.Add(ss[0]);
                }
            }

            sr.Close();
            fs.Close();


            fs = new FileStream(@"C:\ProgramData\MySQL\MySQL Server 5.7\Uploads\essays.csv", FileMode.Open);
            sr = new StreamReader(fs);


            while (projectsInCluster.Count > sampleSize)
            {
                projectsInCluster.RemoveAt(rng.Next(0, projectsInCluster.Count));
            }

            do
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder key = new StringBuilder();
                bool instring = false;
                bool inescape = false;
                bool sampleIt = false;
                int field = 1;

                while (sr.EndOfStream == false)
                {
                    char c = (char)sr.Read();
                    if (c == '"' && !inescape)
                    {
                        instring = !instring;
                    }

                    if (c == ',' && !inescape && !instring)
                    {
                        field++;
                        if (field == 2 && projectsInCluster.Contains(key.ToString()))
                        {
                            sampleIt = true;
                        }
                        continue;
                    }

                    if (field == 1)
                    {
                        key.Append(c);
                    }

                    if (c == '\\' && !inescape)
                    {
                        inescape = true;
                        continue;
                    }
                    if (c == '\n' && !inescape && !instring)
                    {
                        if (sampleIt)
                        {
                            Console.WriteLine("----------------");
                            Console.WriteLine(sb.ToString());
                            Console.WriteLine("----------------");
                        }

                        key.Clear();
                        sb.Clear();
                        break;
                    }
                    if (field == 6 && sampleIt)
                    {
                        sb.Append(c);
                    }


                    inescape = false;

                }
            } while (sr.EndOfStream == false && projectsInCluster.Count > 0);


            Console.WriteLine("Done!");
            Console.ReadLine();
        }


        static void kmeans(string saves, string idfsaves, int k, int iter, int samplesize, string output)
        {
            string[] allprojects = allProjectIds(saves);
            //548862 projects in total


            Random rand = new Random();

            Dictionary<string, double> idf = loadIDF(idfsaves, allprojects.Length);
            Dictionary<string, Dictionary<string, double>> docs = loadR(saves, samplesize, allprojects.Length, idf);
            string[] projectids = docs.Keys.ToArray();


            List<Tuple<string, double>>[] bestCentroids = new List<Tuple<string, double>>[k];
            double bestScore = 0;

            for (int getBest = 0; getBest < iter; getBest++)
            {

                List<Tuple<string, double>>[] clusterCentroids = new List<Tuple<string, double>>[k];
                double thisScore = 0;

                //initialise cluster centroids with random values:
                for (int i = 0; i < k; i++)
                {
                    //pick a random document to serve as the centroid starting point:
                    string s = projectids[rand.Next(0, projectids.Length)];
                    List<Tuple<string, double>> cc = new List<Tuple<string, double>>();

                    Dictionary<string, double> wordScores = new Dictionary<string, double>();
                    foreach (KeyValuePair<string, double> w in docs[s])
                    {
                        cc.Add(new Tuple<string, double>(w.Key, w.Value));
                    }

                    clusterCentroids[i] = cc;
                }

                int iters = 0;
                int maxiter = 100;


                List<string>[] oldMapping = new List<string>[k];
                for (int i = 0; i < k; i++)
                    oldMapping[i] = new List<string>();
                bool changed = false;

                do
                {

                    //now assign each document to one of the centroids:
                    double similaritySum = 0;
                    List<string>[] centroidMapping = new List<string>[k];
                    for (int i = 0; i < k; i++)
                        centroidMapping[i] = new List<string>();

                    double dc = 1;
                    foreach (string d in projectids)
                    {
                        int argmax = 0;
                        double best = cossim(clusterCentroids[0], docs[d]);
                        for (int i = 1; i < k; i++)
                        {
                            double contender = cossim(clusterCentroids[i], docs[d]);
                            if (contender > best)
                            {
                                argmax = i;
                                best = contender;
                            }
                        }
                        centroidMapping[argmax].Add(d);
                        similaritySum+=(best/dc);
                        dc++;
                    }
                    thisScore = similaritySum;

                    //now update the centroids to be the mean of their documents:
                    for (int i = 0; i < k; i++)
                    {
                        List<Tuple<string, double>> newcc = new List<Tuple<string, double>>();
                        HashSet<string> candidateWords = new HashSet<string>();
                        for (int w = 0; w < clusterCentroids[i].Count; w++)
                        {
                            candidateWords.Add(clusterCentroids[i][w].Item1);
                        }
                        foreach (string s in centroidMapping[i])
                        {
                            foreach (string w in docs[s].Keys)
                                candidateWords.Add(w);
                        }

                        foreach (string w in candidateWords)
                        {
                            double sum = 0;
                            foreach (string s in centroidMapping[i])
                            {
                                if (docs[s].ContainsKey(w))
                                    sum += docs[s][w];
                            }
                            double mean = sum / centroidMapping[i].Count;
                            if (mean > 0)
                                newcc.Add(new Tuple<string, double>(w, mean));
                        }
                    }

                    //check if the centroid mapping actually changed:
                    changed = false;
                    for (int i = 0; i < k - 1; i++)
                    {
                        bool a = new HashSet<string>(oldMapping[i]).SetEquals(centroidMapping[i]);
                        if (!a)
                        {
                            changed = true;
                            break;
                        }
                    }
                    iters++;

                    oldMapping = centroidMapping;

                    Console.WriteLine(iters);
                } while (!(iters > maxiter || !changed));

                if (double.IsNaN(thisScore))
                {
                    thisScore = 0;
                }

                Console.WriteLine("Done! This score: {0}", thisScore);

                if (bestScore == 0 || thisScore > bestScore)
                {
                    bestCentroids = (List<Tuple<string,double>>[])clusterCentroids.Clone();
                    bestScore = thisScore;
                    Console.WriteLine("New best! Cluster {0}", getBest);
                }

                FileStream bfs = new FileStream("cluster" + getBest, FileMode.Create);
                StreamWriter bsw = new StreamWriter(bfs);

                for (int i = 0; i < k; i++)
                {
                    foreach (Tuple<string, double> t in clusterCentroids[i])
                    {
                        bsw.WriteLine("{0},{1},{2}", i, t.Item1, t.Item2);
                    }
                }
                bsw.Close();
                bfs.Close();
            }

            FileStream fs = new FileStream(output, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            FileStream fs2 = new FileStream(saves, FileMode.Open);
            StreamReader sr = new StreamReader(fs2);
            sr.ReadLine();

            int progress = 0;
            double percent = -1;
            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                int norm = 0;
                string[] ss = s.Split(new char[] { ',' }, 2);

                Dictionary<string, double> h2 = new Dictionary<string, double>();

                string[] ss2 = ss[1].Split(new char[] { ',' });
                for (int i = 0; i + 1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    norm += c;
                }

                for (int i = 0; i + 1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    double id = idf[ss2[i]];
                    double w = (c * id) / norm;
                    h2.Add(ss2[i], w);
                }

                //which cluster does this project belong to?
                int argmax = 0;
                double best = cossim(bestCentroids[0], h2);
                for (int i = 1; i < k; i++)
                {
                    double contender = cossim(bestCentroids[i], h2);
                    if (contender > best)
                    {
                        argmax = i;
                        best = contender;
                    }
                }
                sw.Write(ss[0]);
                sw.Write(",");
                sw.Write(argmax);
                sw.WriteLine();

                progress++;

                double p = progress / (double)allprojects.Length;
                double p10 = Math.Floor(p * 100);
                if (p10 > percent)
                {
                    percent = p10;
                    Console.WriteLine("{0}%", percent);
                }
            }
            sr.Close();
            fs2.Close();

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        static string[] allProjectIds(string filename)
        {

            List<string> pids = new List<string>() { };
            FileStream fs = new FileStream(filename, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            sr.ReadLine();

            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                string[] ss = s.Split(new char[] { ',' }, 2);

                pids.Add(ss[0]);
            }

            sr.Close();
            fs.Close();

            return pids.ToArray();
        }

        static double cossim(List<Tuple<string, double>> a, Dictionary<string, double> b)
        {
            double dotp=0, ma=0, mb=0;

            for (int i = 0; i < a.Count; i++)
            {
                double bv = 0;
                if (b.ContainsKey(a[i].Item1))
                {
                    bv = b[a[i].Item1];
                }

                dotp += a[i].Item2 * bv;
                ma += Math.Pow(a[i].Item2, 2);
                mb += Math.Pow(bv, 2);
            }

            ma = Math.Sqrt(ma);
            mb = Math.Sqrt(mb);
            double d = dotp / (ma * mb);
            return d == Double.NaN ? 0 : d;


        }


        static void sorted()
        {
            Dictionary<string, double> h = loadIDF("saveIDF",1000);
            List<KeyValuePair<string, double>> myList = h.ToList();

            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            myList.Reverse();

            int i = 0;
            foreach (KeyValuePair<string, double> s in myList)
            {
                Console.WriteLine("{0}, {1}", s.Key, s.Value);
                i++;
                if (i == 10)
                {
                    Console.ReadLine();
                    i = 0;
                }
            }
        }


        static void build()
        {

            Dictionary<string, int> df = new System.Collections.Generic.Dictionary<string, int>();
            //Dictionary<string, Dictionary<string, int>> tf = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, int>>();

            FileStream FS = new FileStream(@"C:\ProgramData\MySQL\MySQL Server 5.7\Uploads\essays.csv", FileMode.Open);
            StreamReader sr = new StreamReader(FS);

            FileStream fs2 = new FileStream("saves", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs2);

            
            



            double i = 0;
            double percent = -1;

            StringBuilder sb = new StringBuilder();
            StringBuilder key = new StringBuilder();
            Dictionary<string, int> myTFs = new System.Collections.Generic.Dictionary<string, int>();


            do
            {



                bool instring = false;
                bool inescape = false;
                int field = 1;

                sb.Clear();
                key.Clear();
                myTFs.Clear();


                while (sr.EndOfStream == false)
                {
                    char c = (char)sr.Read();
                    if (c == '"' && !inescape)
                    {
                        instring = !instring;
                    }

                    if (c == ',' && !inescape && !instring)
                    {
                        field++;
                        continue;
                    }
                    if (field == 1)
                    {
                        key.Append(c);
                    }
                    if (c == '\\' && !inescape)
                    {
                        inescape = true;
                        continue;
                    }
                    if (field == 6)
                    {
                        char cl = Char.ToLower(c);
                        if (cl >= 39 && cl <= 122 && !inescape)
                        {
                            if (cl >= 97)
                            {
                                sb.Append(cl);
                            }
                        }
                        else
                        {
                            if (sb.Length > 0)
                            {
                                string word = sb.ToString();
                                sb.Clear();

                                if (myTFs.ContainsKey(word))
                                {
                                    myTFs[word] = myTFs[word] + 1;
                                }
                                else
                                {
                                    myTFs.Add(word, 1);
                                    if (df.ContainsKey(word))
                                    {
                                        df[word] = df[word] + 1;
                                    }
                                    else
                                    {
                                        df.Add(word, 1);
                                    }
                                }

                            }
                        }
                    }
                    if (c == '\n' && !inescape && !instring)
                    {
                        i++;

                        double p = i / 664098;
                        double p10 = Math.Floor(p * 100);
                        if (p10 > percent)
                        {
                            percent = p10;
                            Console.WriteLine("{0}%", percent);

                            //if (percent > 0 && percent % 10 == 0 && percent < 100)
                            //{
                            //    save(tf, "save" + percent);
                            //    tf.Clear();
                            //}
                        }

                        StringBuilder fileline = new StringBuilder();
                        string k = key.ToString();
                        fileline.Append(k);
                        fileline.Append(',');
                        bool first = true;
                        foreach (string w in myTFs.Keys)
                        {
                            if (!first)
                                fileline.Append(',');
                            fileline.Append(w);
                            fileline.Append(',');
                            fileline.Append(myTFs[w]);
                            first = false;
                        }

                        sw.WriteLine(fileline.ToString());
                        //tf.Add(key.ToString(), myTFs);
                        //Console.WriteLine(i);


                        break;
                    }


                    inescape = false;

                }
            } while (sr.EndOfStream == false);

            sw.Flush();
            sw.Close();
            fs2.Close();

            save(df, "saveIDF");


            //Console.WriteLine(i);
            sr.Close();
            FS.Close();
        }

        static Dictionary<string, Dictionary<string,double>> load(string f, int n, int skip, Dictionary<string, double> idf)
        {
            FileStream fs = new FileStream(f, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            Dictionary<string, Dictionary<string, double>> h = new Dictionary<string, Dictionary<string, double>>();

            StringBuilder sb = new StringBuilder();
            while (sr.EndOfStream == false && (n == 0 || h.Keys.Count < n))
            {
                string s = sr.ReadLine();
                if (skip > 0)
                {
                    skip--;
                    continue;
                }
                int norm = 0;
                string[] ss = s.Split(new char[] { ',' }, 2);

                Dictionary<string, double> h2 = new Dictionary<string, double>();

                string[] ss2 = ss[1].Split(new char[] { ',' });
                for (int i = 0; i+1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    norm += c;
                }

                for (int i = 0; i + 1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    double id= idf[ss2[i]];
                    double w = (c*id)/norm;
                    h2.Add(ss2[i], w);
                }

                h.Add(ss[0], h2);
            }

            sr.Close();
            fs.Close();

            return h;
        }


        static Dictionary<string, Dictionary<string, double>> loadR(string f, int n, int max, Dictionary<string, double> idf)
        {
            //list of all possible lines
            int[] candidates = new int[max];
            for (int i = 0; i < max; i++)
                candidates[i] = i;
            int[] shuffled = Shuffle(candidates.ToList());
            int[] extract = new int[n];
            for (int i = 0; i < n; i++)
            {
                extract[i] = shuffled[i];
            }
            Array.Sort(extract);

            FileStream fs = new FileStream(f, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            sr.ReadLine();

            Dictionary<string, Dictionary<string, double>> h = new Dictionary<string, Dictionary<string, double>>();

            StringBuilder sb = new StringBuilder();
            int line = 0;
            int e = 0;
            while (sr.EndOfStream == false && (n == 0 || h.Keys.Count < n))
            {
                string s = sr.ReadLine();

                if (extract[e] != line)
                {
                    line++;
                    continue;
                }
                else
                {
                    line++;
                    e++;
                }

                int norm = 0;
                string[] ss = s.Split(new char[] { ',' }, 2);

                Dictionary<string, double> h2 = new Dictionary<string, double>();

                string[] ss2 = ss[1].Split(new char[] { ',' });
                for (int i = 0; i + 1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    norm += c;
                }

                for (int i = 0; i + 1 < ss2.Length; i += 2)
                {
                    int c = Convert.ToInt32(ss2[i + 1]);
                    double id = idf[ss2[i]];
                    double w = (c * id) / norm;
                    if (w > 0)
                        h2.Add(ss2[i], w);
                }

                h.Add(ss[0], h2);
            }

            sr.Close();
            fs.Close();

            return h;
        }

        static Dictionary<string, double> loadIDF(string f, int n)
        {
            FileStream fs = new FileStream(f, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            Dictionary<string, double> h = new Dictionary<string, double>();

            StringBuilder sb = new StringBuilder();
            while (sr.EndOfStream == false)
            {
                string s = sr.ReadLine();
                string[] ss = s.Split(new char[] {','} , 2);

                h.Add(ss[0], Math.Log(n / Convert.ToInt32(ss[1]), 2));
            }
            
            sr.Close();
            fs.Close();

            return h;
        }

        static void save(Dictionary<string, int> h, string f)
        {
            FileStream fs = new FileStream(f, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (string k in h.Keys)
            {
                sw.Write(k);
                sw.Write(',');
                sw.Write(h[k]);
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        static void save (Dictionary<string, Dictionary<string, int>> h, string f)
        {
            FileStream fs = new FileStream(f, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (string k in h.Keys)
            {
                sw.Write(k);
                sw.Write(',');
                bool first = true;
                foreach (string w in h[k].Keys)
                {
                    if (!first)
                        sw.Write(',');
                    sw.Write(w);
                    sw.Write(',');
                    sw.Write(h[k][w]);
                    first = false;
                }
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private static Random rng = new Random();  

        public static int[] Shuffle (List<int> l)  
        {
            int[] list = new int[l.Count];
            l.CopyTo(list);
            int n = list.Length;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                int value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }
            return list;
        }
    }





}
