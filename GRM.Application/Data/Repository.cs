using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using GRM.Application.DomainObjects;

namespace GRM.Application.Data
{
    public interface IRepository
    {
        IEnumerable<MusicContract> GetMusicContracts();
        IEnumerable<DistributionContract> GetDistributionContracts();
    }

    public class Repository: IRepository
    {
        private readonly string _musicContractsPath;
        private readonly string _distributionContractsPath;
        private readonly CsvConfiguration _csvConfig;

        public Repository(string musicContractsPath, string distributionContractsPath)
        {
            Check<FileNotFoundException>(File.Exists(Path.GetFullPath(musicContractsPath)), $"File doesn't exist or incorrect path: {musicContractsPath}");
            Check<FileNotFoundException>(File.Exists(Path.GetFullPath(distributionContractsPath)), $"File doesn't exist or incorrect path: {distributionContractsPath}");

            _distributionContractsPath = distributionContractsPath;
            _musicContractsPath = musicContractsPath;

            _csvConfig = new CsvConfiguration
            {
                Delimiter = "|",
                HasHeaderRecord = true
            };
            _csvConfig.RegisterClassMap<MusicContractMap>();
            _csvConfig.RegisterClassMap<DistributionContractMap>();
        }

        private static void Check<TException>(bool condition, string errorMessage) where TException : Exception
        {
            if (condition) return;
            throw Activator.CreateInstance(typeof(TException), errorMessage) as TException;
        }

        public IEnumerable<MusicContract> GetMusicContracts()
        {
            var csv = new CsvReader(File.OpenText(_musicContractsPath), _csvConfig);
            return csv.GetRecords<MusicContract>();
        }

        public IEnumerable<DistributionContract> GetDistributionContracts()
        {
            var csv = new CsvReader(File.OpenText(_distributionContractsPath), _csvConfig);
            return csv.GetRecords<DistributionContract>();
        }
    }
}
