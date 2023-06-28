using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = SipaaOS.VBEConsole;

namespace SipaaOS
{
    public class Shell
    {
        class Command 
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Usage { get; set; }
            public Action<List<string>> OnExecute { get; set; }
            public bool ExitMain { get; internal set; }

            public void Execute(List<string> args)
            {
                OnExecute(args);
            }

            public void Help()
            {
                Console.WriteLine(Description);
                Console.WriteLine("Usage : " + Usage);
            }
        }

        private List<Command> cmds;

        public Shell()
        {
            cmds = new List<Command>();
        }

        public void RegisterCommand(string name, string description, string usage, Action<List<string>> onRun, bool exit = false)
        {
            cmds.Add(new Command { Name = name, Description = description, Usage = usage, OnExecute = onRun, ExitMain = exit });
        }

        public bool GetInput()
        {
            Console.Write("> ");
            var inp = Console.ReadLine();
            var spl = inp.Split(' ');
            var args = new List<string>();

            for (int i = 1; i < spl.Length; i++)
            {
                args.Add(spl[i]);
            }

            foreach (Command c in cmds)
            {
                if (c.Name == spl[0])
                {
                    c.Execute(args);
                    if (c.ExitMain)
                        return true;
                }
            }

            return false;
        }
    }
}
