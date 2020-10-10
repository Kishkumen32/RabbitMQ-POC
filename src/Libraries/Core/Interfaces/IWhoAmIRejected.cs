using System;

namespace Core.Interfaces
{
    public interface IWhoAmIRejected
    {
        string Reason { get; set; }
        DateTime Timestamp { get; set; }
    }
}
