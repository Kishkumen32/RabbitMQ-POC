using System;

namespace Core.Interfaces
{
    public interface IWhoAmIAccepted
    {
        int Id { get; set; }

        string Firstname { get; set; }

        string Lastname { get; set; }

        DateTime Timestamp { get; set; }
    }
}
