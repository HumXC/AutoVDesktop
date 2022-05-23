using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoVDesktop.VirtualDesktop
{
    internal class VirtualDesktop
    {

    }
    public class Desktop
    {
        public string Name { get; }
        public Guid Guid { get; }
        public Desktop(string name, string guid)
        {
            Name = name;
            Guid = new(guid);
        }
    }
}
