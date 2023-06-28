using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;
using Console = SipaaOS.VBEConsole;

namespace SipaaOS.Core
{
    public class FileSystem
    {
        internal static Sys.FileSystem.CosmosVFS Fs;

        private static void CreateIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                //Logging.Log.Info($"FsManager", $"Directory '{dir}' was created.");
            }
        }

        internal static bool InitializeFS()
        {
            Console.WriteLine("Registering file system...");
            Fs = new Sys.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(Fs);
            try
            {
                Directory.GetFiles(@"0:\");

                CreateIfNotExists(@"0:\etc");

                //SysCfg.Load();

                //if (SysCfg.BootLock)
                //{
                if (File.Exists(@"0:\etc\bootlock.tmp"))
                {
                    Console.WriteLine("SipaaOS shut down unexpectedly last time.");
                }
                else
                {
                    File.Create(@"0:\etc\bootlock.tmp");
                }
                //}
                return true;
            }
            catch (System.Exception e)
            {
                //ExceptionHandler.SKBugCheck(new($"Filesystem init failure! {e.ToString()}"));
                //Util.PrintSystem($"Failed to initialise filesystem. Ensure you have a valid FAT32 filesystem on the disk. {e.ToString()}");
                //Logging.Log.Error("FsManager", $"Failed to initialise filesystem: {e.ToString()}");
                return false;
            }
        }
    }
}
