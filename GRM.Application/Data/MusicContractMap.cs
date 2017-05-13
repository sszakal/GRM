using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper.Configuration;
using GRM.Application.DomainObjects;

namespace GRM.Application.Data
{
    public sealed class MusicContractMap : CsvClassMap<MusicContract>
    {
        private string _dateFormat1 = "d'st' MMM yyyy";
        private string _dateFormat2 = "d'st' MMMM yyyy";

        public MusicContractMap()
        {
            Map(m => m.Artist);
            Map(m => m.Title);
            Map(m => m.StartDate).ConvertUsing(row =>
            {
                DateTime date;
                DateTime.TryParseExact(row.GetField<string>("StartDate"), _dateFormat1, CultureInfo.InvariantCulture,DateTimeStyles.None, out date);
                if(date == DateTime.MinValue) DateTime.TryParseExact(row.GetField<string>("StartDate"), _dateFormat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                return date;
            });

            Map(m => m.EndDate).ConvertUsing(row =>
            {
                DateTime date;
                if (DateTime.TryParseExact(row.GetField<string>("EndDate"), _dateFormat1, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) return date;
                if (date == DateTime.MinValue) DateTime.TryParseExact(row.GetField<string>("EndDate"), _dateFormat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                return (DateTime?)null;
            });

            Map(m => m.Usages).ConvertUsing(row =>
            {
                var usages = row.GetField<string>("Usages");
                if (string.IsNullOrWhiteSpace(usages)) return EContractUsage.Default;
                return Regex.Split(usages, @",").Select(u =>
                {
                    switch (u.ToUpper().Trim())
                    {
                        case "DIGITAL DOWNLOAD":
                            return EContractUsage.DigitalDownload;
                        case "STREAMING":
                            return EContractUsage.Streaming;
                        default:
                            return EContractUsage.Default;
                    }
                }).Aggregate((ac, val) => ac | val);
            });
        }
    }
}
