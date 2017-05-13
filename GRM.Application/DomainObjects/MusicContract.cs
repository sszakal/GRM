using System;

namespace GRM.Application.DomainObjects
{
    public class MusicContract
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public EContractUsage Usages { get; set; }
        public DateTime StartDate { get; set; }
        public string DisplayStartDate => StartDate.ToString("d'st' MMM yyyy");
        public DateTime? EndDate { get; set; }
        public string DisplayEndDate => EndDate?.ToString("d'st' MMM yyyy") ?? "";

    }
}
