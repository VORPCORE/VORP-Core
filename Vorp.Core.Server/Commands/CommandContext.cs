using System.Collections.Generic;

namespace Vorp.Core.Server.Commands
{
    public abstract class CommandContext
    {
        public abstract string[] Aliases { get; set; }
        public abstract string Title { get; set; }
        public abstract bool IsRestricted { get; set; }
        public abstract List<string> RequiredRoles { get; set; }
    }
}