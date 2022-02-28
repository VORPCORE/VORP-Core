using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorp.Core.Client.Interface
{
    internal class PromptHandler
    {
        public delegate void PromptEvent();

        Dictionary<string, Prompt> prompts = new Dictionary<string, Prompt>();
    }
}
