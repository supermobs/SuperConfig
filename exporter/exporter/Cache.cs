using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace exporter
{
    public static class Cache
    {
        public static bool enable { get; private set; }
        static Dictionary<string, List<string>> file2tables = new Dictionary<string, List<string>>();
        static Dictionary<string, DateTime> cacheTime = new Dictionary<string, DateTime>();

        static Dictionary<string, List<string>> table2files = new Dictionary<string, List<string>>();

        static string cacheFilePath;

        public static void Init(bool _enable)
        {
            cacheFilePath = new FileInfo(Application.ExecutablePath).Directory.FullName + Path.DirectorySeparatorChar + "cache";

            enable = _enable;
            file2tables.Clear();
            cacheTime.Clear();
            table2files.Clear();

            if (File.Exists(cacheFilePath))
            {
                using (BinaryReader br = new BinaryReader(new FileStream(cacheFilePath, FileMode.Open)))
                {
                    int fcount = br.ReadInt32();
                    for (int i = 0; i < fcount; i++)
                    {
                        string name = br.ReadString();
                        cacheTime.Add(name, new DateTime(br.ReadInt64()));

                        List<string> tabs = new List<string>();
                        int tcount = br.ReadInt32();
                        for (int j = 0; j < tcount; j++)
                            tabs.Add(br.ReadString());

                        file2tables.Add(name, tabs);
                    }
                }

                var e = file2tables.GetEnumerator();
                while (e.MoveNext())
                {
                    foreach (var s in e.Current.Value)
                    {
                        if (!table2files.ContainsKey(s))
                            table2files.Add(s, new List<string>());
                        table2files[s].Add(e.Current.Key);
                    }
                }
            }
        }

        public static List<string> GetNoCacheAbout(FileInfo file)
        {
            bool cached = cacheTime.ContainsKey(file.Name) && file.LastWriteTime <= cacheTime[file.Name];
            cacheTime[file.Name] = file.LastWriteTime;

            if (enable && cached)
                return new List<string>();

            var list = new List<string>() { file.Name };
            if (enable && file2tables.ContainsKey(file.Name))
            {
                var tabs = new List<string>();
                tabs.AddRange(file2tables[file.Name]);
                for (int i = 0; i < tabs.Count; i++)
                {
                    foreach (var f in table2files[tabs[i]])
                    {
                        if (list.Contains(f))
                            continue;
                        list.Add(f);
                        foreach (var t in file2tables[f])
                            if (!tabs.Contains(t))
                                tabs.Add(t);
                    }
                }
            }
            return list;
        }

        public static void PrepareToExport()
        {
            if (!enable)
                file2tables.Clear();
        }

        public static void MarkTableAbout(string table, List<string> files)
        {
            foreach (var fname in files)
            {
                if (!file2tables.ContainsKey(fname))
                    file2tables.Add(fname, new List<string>());
                if (!file2tables[fname].Contains(table))
                    file2tables[fname].Add(table);
            }
        }

        public static void SaveCache()
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(cacheFilePath, FileMode.Create)))
            {
                bw.Write(file2tables.Count);

                var e = file2tables.GetEnumerator();
                while (e.MoveNext())
                {
                    bw.Write(e.Current.Key);
                    bw.Write(cacheTime.ContainsKey(e.Current.Key) ? cacheTime[e.Current.Key].Ticks : 0);

                    bw.Write(e.Current.Value.Count);
                    foreach (var s in e.Current.Value)
                        bw.Write(s);
                }
            }
        }
    }
}
