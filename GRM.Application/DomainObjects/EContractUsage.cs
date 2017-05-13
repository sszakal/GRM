using System;

namespace GRM.Application.DomainObjects
{
    [Flags]
    public enum EContractUsage
    {
        Default = 0,
        DigitalDownload = 1,
        Streaming = 2
    }
}
