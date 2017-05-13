using System.Globalization;
using CsvHelper.Configuration;
using GRM.Application.DomainObjects;

namespace GRM.Application.Data
{
    public sealed class DistributionContractMap : CsvClassMap<DistributionContract>
    {
        public DistributionContractMap()
        {
            Map(m => m.Partner).TypeConverterOption(DateTimeStyles.AdjustToUniversal);
            Map(m => m.Usage).ConvertUsing(row =>
            {
                var usage = row.GetField<string>("Usage");
                if (string.IsNullOrWhiteSpace(usage)) return EContractUsage.Default;
                switch (usage.ToUpper())
                {
                    case "DIGITAL DOWNLOAD":
                        return EContractUsage.DigitalDownload;
                    case "STREAMING":
                        return EContractUsage.Streaming;
                    default:
                        return EContractUsage.Default;
                }
            });
        }
    }
}
