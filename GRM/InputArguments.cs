using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace GRM
{
    public class InputArguments
    {
        [Value(0, MetaName = "Music Contracts File Path", HelpText = "The system file path for the music contracts input file", Required = true)]
        public string MusicContractsFilePath { get; set; }

        [Value(1, MetaName = "Distribution Partners Contracts File Path", HelpText = "The system file path for the distribution partener contracts input file", Required = true)]
        public string DistributionPartnersContractsFilePath { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples => new[]
        {
            new Example("Example 1", new InputArguments
            {
                MusicContractsFilePath = @"C:\music.txt",
                DistributionPartnersContractsFilePath = @"C:\distribution.txt"
            })
        };
    }
}
