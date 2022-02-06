using System;

namespace Vorp.Core.Client
{
    public class TickHandler : Attribute
    {
        public bool SessionWait { get; set; }
    }
}
