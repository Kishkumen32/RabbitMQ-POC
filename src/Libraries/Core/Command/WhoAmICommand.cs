using Core.Interfaces;

namespace Core.Command
{
    public class WhoAmICommand : IWhoAmICommand
    {
        public string Username { get; set; }
    }
}
