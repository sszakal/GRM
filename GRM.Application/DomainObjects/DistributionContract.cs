namespace GRM.Application.DomainObjects
{
    public class DistributionContract
    {
        public string Partner { get; set; }
        public EContractUsage Usage { get; set; }
        public string DisplayUsage
        {
            get
            {
                switch (Usage)
                {
                    case EContractUsage.DigitalDownload:
                        return "digital download";
                    case EContractUsage.Streaming:
                        return "streaming";
                    default:
                        return "";
                }
            }
        }
    }
}
