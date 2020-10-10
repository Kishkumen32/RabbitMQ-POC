using Core.Entities;
using MediatR;

namespace Core.Command
{
    public class WhoAmICommand : IRequest<User>
    {
        public string Username { get; set; }
    }
}
