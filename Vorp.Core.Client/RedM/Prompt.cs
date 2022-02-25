using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorp.Core.Client.RedM.Enums;

namespace Vorp.Core.Client.RedM
{
    internal class Prompt
    {
        int _handle;
        public int Handle => _handle;

        public Prompt(int handle)
        {
            _handle = handle;
        }

        public static Prompt Create(eControl control)
    }
}
