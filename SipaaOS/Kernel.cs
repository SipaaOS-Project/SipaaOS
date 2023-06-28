using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.System;
using Console = SipaaOS.VBEConsole;
using SipaaOS.Users;
using SipaaOS.Core;
using PrismAPI.Hardware.GPU;
using PrismAPI.Graphics;
using PrismAPI.Graphics.Fonts;
using Cosmos.Core;

namespace SipaaOS
{
    public class Pos
    {
        public int X, Y = 0;
    }
    public class Kernel: Cosmos.System.Kernel
    {
        public static Display Canvas;
        public static Shell sh;
        internal static User CurrentUser;
        public static string Version = "0.1";
        public static string KernelVer = "7";

        void LoadShell()
        {
            sh = new();
            sh.RegisterCommand("crash", "Stop SipaaKernel by crashing it", "crash", (args) => { Core.ExceptionHandler.SKBugCheck(new("User manually stopped the OS from running.")); }, true);
            sh.RegisterCommand("sha", "Encrypt a string using \"SHA256\"", "sha", (args) =>
            {
                string strtoencrypt = args[0];

                Cosmos.System.Global.Debugger.Send(Core.Sha256.Encrypt(strtoencrypt));
                Console.WriteLine(Core.Sha256.Encrypt(strtoencrypt));
            });
            sh.RegisterCommand("chres", "Change resolution", "chres [width] [height]", (args) =>
            {
                if (args.Count < 2)
                {
                    Console.WriteLine("You must have at least 2 arguments");
                }
                else
                {
                    try
                    {
                        ushort w = ushort.Parse(args[0]);
                        ushort h = ushort.Parse(args[1]);
                        Console.Canvas = Display.GetDisplay(w, h);
                    }
                    catch { Console.WriteLine("An error happened. Please make sure you inputted numbers."); }
                }
            });
            sh.RegisterCommand("scrsave", "Start a screensaver", "scrsave [screensaver]", (args) =>
            {
                if (args.Count == 0)
                {
                    Console.WriteLine("You must provide a screensaver!");
                    return;
                }
                else
                {
                    if (args[0] == "dvd")
                    {
                        int x = 0;
                        int y = 0;
                        bool ttb = true;
                        bool ltr = true;
                        bool exit = false;

                        while (!exit)
                        {
                            Cosmos.HAL.Global.PIT.Wait(20);
                            if (KeyboardManager.KeyAvailable)
                            {
                                var k = KeyboardManager.ReadKey();
                                bool isUpper = KeyboardManager.CapsLock || KeyboardManager.ShiftPressed;

                                if (KeyboardManager.ControlPressed && k.Key == ConsoleKeyEx.Escape)
                                {
                                    exit = true;
                                }
                            }

                            if (x == 0)
                                ltr = true;
                            if (x == Canvas.Width - 100)
                                ltr = false;
                            if (y == 0)
                                ttb = true;
                            if (y == Canvas.Height - 100)
                                ttb = false;

                            if (ltr)
                                x++;
                            else
                                x--;
                            if (ttb)
                                y++;
                            else
                                y--;

                            Canvas.Clear();
                            Canvas.DrawFilledRectangle(x, y, 100, 100, 0, Color.White);
                            Canvas.Update();
                        }
                    }
                    if (args[0] == "starfield")
                    {
                        int sCount = 200;
                        int sSpeed = 1;
                        
                        var stars = new Pos[sCount];
                        var random = new Random();

                        for (int i = 0; i < sCount; i++)
                        {
                            int x = random.Next(Convert.ToInt32(Canvas.Width));
                            int y = random.Next(Convert.ToInt32(Canvas.Height));
                            stars[i] = new Pos();
                            stars[i].X = x;
                            stars[i].Y = y;
                        }


                        bool exit = false;

                        while (!exit)
                        {
                            Cosmos.HAL.Global.PIT.Wait(20);
                            if (KeyboardManager.KeyAvailable)
                            {
                                var k = KeyboardManager.ReadKey();
                                bool isUpper = KeyboardManager.CapsLock || KeyboardManager.ShiftPressed;

                                if (KeyboardManager.ControlPressed && k.Key == ConsoleKeyEx.Escape)
                                {
                                    exit = true;
                                }
                            }

                            Canvas.Clear();

                            foreach (Pos star in stars)
                            {
                                int y = star.Y + sSpeed;

                                // If star goes off the screen, reset its position
                                if (y > Convert.ToInt32(Canvas.Height))
                                {
                                    star.X = random.Next(Convert.ToInt32(Canvas.Width));
                                    star.Y = 0;
                                }
                                else
                                {
                                    star.Y = y;
                                }

                                // Draw star
                                Kernel.Canvas[star.X, star.Y] = Color.White;
                            }

                            Canvas.Update();
                        }
                    }
                    if (args[0] == "rotatingcube")
                    {
                        PrismAPI.Graphics.Rasterizer.Engine e = new(Canvas.Width, Canvas.Height, 90);
                        PrismAPI.Graphics.Rasterizer.Mesh m = PrismAPI.Graphics.Rasterizer.Mesh.GetCube(60, 60, 60);
                        bool exit = false;

                        e.Objects.Add(m);

                        while (!exit)
                        {
                            if (KeyboardManager.KeyAvailable)
                            {
                                var k = KeyboardManager.ReadKey();
                                bool isUpper = KeyboardManager.CapsLock || KeyboardManager.ShiftPressed;

                                if (KeyboardManager.ControlPressed && k.Key == ConsoleKeyEx.Escape)
                                {
                                    exit = true;
                                }
                            }

                            m.TestLogic(0.01f);
                            e.Render();
                            Canvas.DrawImage(0, 0, e, true);
                            Canvas.Update();
                        }
                    }
                }
            });
            sh.RegisterCommand("sysfetch", "Fetch infos from SipaaOS", "sysfetch", (args) =>
            {
                Console.Write("          _____          ", true); Console.WriteLine($"\t{CurrentUser.Username}@sipaapc");
                Console.Write("         /\\    \\         ", true); Console.WriteLine($"\t-------------------------");
                Console.Write("        /::\\    \\        ", true); Console.WriteLine($"\tSipaaOS ver. {Version}");
                Console.Write("       /::::\\    \\       ", true); Console.WriteLine($"\tKernel : SipaaKernel V{KernelVer}");
                Console.Write("      /::::::\\    \\      ", true); Console.WriteLine($"\tDE : None");
                Console.Write("     /:::/\\:::\\    \\     ", true); Console.WriteLine($"\tShell : DefaultShell");
                Console.Write("    /:::/__\\:::\\    \\    ", true); Console.WriteLine($"\tGraphics : {Canvas.GetName()} ({Canvas.Width}x{Canvas.Height})");
                Console.Write("    \\:::\\   \\:::\\    \\   ", true); Console.WriteLine($"\tTotal Mem. : {Cosmos.Core.CPU.GetAmountOfRAM()}mb");
                Console.Write("  ___\\:::\\   \\:::\\    \\  ", true); Console.WriteLine($"\tUsed Mem. : {GCImplementation.GetUsedRAM() / 1024 / 1024}mb");
                Console.Write(" /\\   \\:::\\   \\:::\\    \\ ", true); Console.WriteLine($"\tGraphics API : PrismAPI");
                Console.Write("/::\\   \\:::\\   \\:::\\____\\", true); Console.WriteLine($"\tWM : Sipaa Window Manager");
                Console.Write(" \\:::\\   \\:::\\   \\/____/ " + '\n', true);
                Console.Write("  \\:::\\   \\:::\\    \\     " + '\n', true);
                Console.Write("   \\:::\\   \\:::\\____\\    " + '\n', true);
                Console.Write("    \\:::\\  /:::/    /    " + '\n', true);
                Console.Write("     \\:::\\/:::/    /     " + '\n', true);
                Console.Write("      \\::::::/    /      ", true); Console.WriteLine($"\tCredits :");
                Console.Write("       \\::::/    /       ", true); Console.WriteLine($"\tJspa2 : SphereOS's user management");
                Console.Write("        \\::/    /        ", true); Console.WriteLine($"\tTerminal.cs : PrismAPI developement");
                Console.Write("         \\/____/         ", true); Console.WriteLine($"\tCosmos team : Cosmos");
            });
        }

        protected override void BeforeRun()
        {
            // Initialize GUI-based console
            Console.Init();
            Canvas = Console.Canvas;

            // Initialize exception handler
            Core.ExceptionHandler.SKBugCheckRaised = (e) =>
            {
                var str1 = "SipaaOS messed up...";
                var str2 = "Try rebooting your PC, and if you see this error again,";
                var str3 = "report it to the SipaaOS Developement Team.";
                var bottomex = "Managed Exception (System.Exception) : " + e.Message;
                Canvas.Clear(Color.SunsetRed);
                Canvas.DrawString(Convert.ToInt32(Canvas.Width) / 2, 64 + 10, str1, default, Color.White, true);
                Canvas.DrawString(Convert.ToInt32(Canvas.Width) / 2, 64 + 10 + Convert.ToInt32(Font.Fallback.Size), str2, default, Color.White, true);
                Canvas.DrawString(Convert.ToInt32(Canvas.Width) / 2, 64 + 10 + (Convert.ToInt32(Font.Fallback.Size) * 2), str3, default, Color.White, true);
                Canvas.DrawString(Convert.ToInt32(Canvas.Width) / 2, Convert.ToInt32(Canvas.Height) - Font.Fallback.Size - 20, bottomex, default, Color.White, true);
                Canvas.Update();
                System.Console.ReadKey();
                Power.Reboot();
            };

            // Initialize shell
            sh = new();
            LoadShell();

            // Initialize file system
            var fsi = FileSystem.InitializeFS();

            // Initialize user manager
            if (fsi)
            {
                UserManager.Load();
                LoginPrompt.PromptLogin(true);

                while (CurrentUser == null)
                    LoginPrompt.PromptLogin();
            }
            else
            {
                CurrentUser = new User("guest", "0000", true);
                UserManager.Users.Add(CurrentUser);
                Console.Clear();
                Console.WriteLine("Welcome to SipaaOS, a UNIX like OS!");
                Console.WriteLine("WARNING : You are logged as guest because SipaaOS can't initialize file system");
            }
        }
        
        protected override void Run()
        {
            sh.GetInput();
        }
    }
}
