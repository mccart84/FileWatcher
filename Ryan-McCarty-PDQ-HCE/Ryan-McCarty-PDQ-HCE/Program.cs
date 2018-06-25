using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.IO;

namespace Ryan_McCarty_PDQ_HCE
{
    class Program
    {
        public static FileSystemWatcher fileWatcher = new FileSystemWatcher();

        static void Main(string[] args)
        {
            Run();
        }

        [PermissionSet(SecurityAction.Demand, Name ="FullTrust")]
        public static void Run()
        {
            // Get the arguements from the command line
            string[] args = System.Environment.GetCommandLineArgs();

            // Check if the arguements provided meet the required standards
            if (CheckArgsValid(args) == false)
                return;

            BindFileWatcherAttributes(args);
            BindFileWatcherEventHandlers();

            // Begin watching.
            fileWatcher.EnableRaisingEvents = true;

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.ReadKey().Key != ConsoleKey.Q) ;
        }

        public static bool CheckArgsValid(string[] args)
        {
            if (!(args.Length >= 2))
            {
                Console.WriteLine("Usage: program.exe \"(File Directory)\" Optional: (File Pattern e.g. *.txt)");
                return false;
            }

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine("The directory \"" + args[1].ToString() + "\" does not exist.");
                return false;
            }            

            return true;
        }

        public static void BindFileWatcherAttributes(string[] args)
        {
            // Set the Directory for the FileSystemWatcher to watch
            fileWatcher.Path = args[1];

            // Events to listen for
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            // Removes all subdirectories from the watch list
            fileWatcher.IncludeSubdirectories = false;

            // The file or file type to watch for
            fileWatcher.Filter = args[2] != null ? args[2] : "*.*";
        }
        
        public static void BindFileWatcherEventHandlers()
        {
            // Event handlers
            fileWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileWatcher.Created += new FileSystemEventHandler(OnCreated);
            fileWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            fileWatcher.Renamed += new RenamedEventHandler(OnRenamed);

            
        }        

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                var lineCount = File.ReadLines(e.FullPath).Count();
                Console.WriteLine("File Created: " + e.Name + " - " + lineCount + " lines.");
            }
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                var lineCount = File.ReadLines(e.FullPath).Count();
                Console.WriteLine("File Deleted: " + e.Name + ".");
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed) {
                Console.WriteLine("File:" + e.FullPath + " " + e.ChangeType);
            }            
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File: {0} renamed to {1}", e.OldName, e.Name);
        }
    }
}
