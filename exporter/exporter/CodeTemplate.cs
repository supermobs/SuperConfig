using System;
using System.Collections.Generic;
using System.Configuration;

namespace exporter
{
    public static class CodeTemplate
    {
        static List<string> allkeys = new List<string>();
        static CodeTemplate()
        {
            allkeys.AddRange(ConfigurationManager.AppSettings.AllKeys);
        }

        public enum Langue
        {
            Lua,
            Go,
            CS,
            TS
        }

        public static Langue curlang = Langue.Lua;

        public static string Get(string key)
        {
            switch (curlang)
            {
                case Langue.Lua: key = "[LUA]" + key; break;
                case Langue.Go: key = "[GO]" + key; break;
                case Langue.CS: key = "[CS]" + key; break;
                case Langue.TS: key = "[TS]" + key; break;
            }

            if (!allkeys.Contains(key))
                throw new Exception("there is no code template for " + key);
            return ConfigurationManager.AppSettings[key].ToString();
        }


    }
}
