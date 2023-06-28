using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SipaaOS.Core
{
    public abstract class Process
    {
        public virtual string Name { get; set; } = "Process";
        public virtual string Description { get; set; } = "A process";
        public virtual ProcessType Type { get; set; } = ProcessType.UserApplication;
        public virtual bool IsCritical { get; set; } = false;

        public abstract bool Start();
        public abstract bool Update();
        public abstract bool Stop();
    }
}
