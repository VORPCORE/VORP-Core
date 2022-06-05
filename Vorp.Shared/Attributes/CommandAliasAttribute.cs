/*
 * Credit where credit is due, Xd_Golden_Tiger#0001 in FiveM C# Scripters Discord
 * */

namespace Vorp.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CommandAliasAttribute : Attribute
    {
        public string Description { get; }
        public string[] Commands { get; }
        public bool Restricted = false;

        public CommandAliasAttribute(string discription, params string[] commands)
        {
            Description = discription;
            Commands = commands;
        }
    }
}
