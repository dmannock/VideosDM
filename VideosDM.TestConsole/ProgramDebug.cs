using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections;
using VideosDM.Core;

namespace VideosDM.TestConsole
{

    class ProgramDebug
    {
        public static void Main()
        {
            ProgramDebug pd = new ProgramDebug();

            Config.SetBasePath(@"C:\Users\Public\NPVR\Plugins\VideosDM\");
            string dir = Config.ReadConfig().Path;

            try
            {
                flv = new FileListView(dir);
            }
            catch (DirectoryNotFoundException ex)
            {
                string message = string.Format("{0}\r\nPath: {1}", ex.Message, dir);
                Log.Write(message);
                Console.WriteLine(message);
                Console.ReadLine();
            }

            pd.ShowNavigation();

            Console.ReadLine();
        }


        private static FileListView flv;
        private void TestMessage()
        {
            
            Console.WriteLine("working dir: {0}", flv.WorkingDirectory);
            Console.WriteLine("-hasPrevDir {0}", flv.HasPrevDirectory);
            Console.WriteLine("-isroot: {0}", flv.IsRoot);
            Console.WriteLine("-has processed: {0}", flv.HasProcessed);
        }

        //Console implementation for testing
        private void ShowNavigation()
        {
            flv.Process();
            Console.WriteLine("Current dir: {0}", flv.WorkingDirectory);
            List<string> options = flv.GetList();
            FileListView.FileListViewDisplay disp = new FileListView.FileListViewDisplay();

            int i = 0;
            foreach (string o in options)
            {
                Console.WriteLine("{0}. {1}", ++i,  
                    (disp.IsLink(o) ? disp.ToLink(o) : 
                        (disp.IsDirectory(o) ? disp.ToDirectory(o)  : o)
                    )
                );

            }

            int opt = -1;
            Console.Write("Enter an option: ");

            try
            {
                opt = int.Parse(Console.ReadLine());

                if (opt < 1 || opt > options.Count)
                    throw new FormatException();

                opt--;

                string selected = options[opt];

                if (selected == disp.PREVIOUS_DIRECTORY)
                {
                    flv.PreviousDirectory();
                } 
                else if (disp.IsLink(selected))
                {
                    Console.WriteLine("is link");
                    try
                    {
                        flv.OpenDirectory(disp.FromLink(selected));
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (disp.IsDirectory(selected))
                {
                    flv.OpenDirectory(disp.FromDirectory(selected));
                }
                else
                {
                    Console.WriteLine("File selected: {0}", flv.GetFullFileName(selected));
                }

            }
            catch (FormatException ex)
            {
                Console.WriteLine("Invalid option");

            }
            ShowNavigation();
        }

    }


}

