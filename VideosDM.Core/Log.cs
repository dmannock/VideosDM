using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace VideosDM.Core
{
    public static class Log
    {
        public static string LogFile = Path.Combine(AssemblyDirectory, AssemblyName + ".plugin.log");

        //needs rewriting to find the Loaded assembly i.e. VideosDm.dll path from assembly name?
        static public string AssemblyDirectory
        {
            get
            {
                //These don't work so hardcoded for now
                // "assembly codebase " + Assembly.GetAssembly(typeof(Log)).CodeBase
                // "assembly local path " + new Uri(Assembly.GetAssembly(typeof(Log)).CodeBase).LocalPath
                // "assembly name " + AssemblyName
                // "assembly fullname " + Assembly.GetExecutingAssembly().GetName().FullName
                // "assembly Location " + Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                // "assembly Location2 " + Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location
                // "assembly LocalPath" + Path.GetDirectoryName(new Uri(Assembly.GetAssembly(typeof(Log)).CodeBase).LocalPath
                return @"C:\Users\Public\NPVR\Plugins\VideosDM";
            }
        }

        static public string AssemblyName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Name;
            }
        }

        public static void Write(string str)
        {
            string formattedStr = string.Format("{0}\t{1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:fff"), str);
            using (StreamWriter fs = File.AppendText(LogFile))
            {
                fs.WriteLine(formattedStr);
            }
        }
    }
}
