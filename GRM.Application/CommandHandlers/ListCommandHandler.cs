using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GRM.Application.Commands;
using GRM.Application.Data;
using GRM.Application.DomainObjects;
using MediatR;

namespace GRM.Application.CommandHandlers
{
    public class ListCommandHandler : IRequestHandler<QueryCommand, Tuple<DistributionContract, IEnumerable<MusicContract>>>
    {
        private readonly IRepository _repository;

        public ListCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Tuple<DistributionContract, IEnumerable<MusicContract>> Handle(QueryCommand command)
        {
            var queryParse = ValidateAndParseQuery(command);
            var partener = ExtractPartener(queryParse);
            var effectiveDate = ExtractDate(queryParse);

            var distributionContract = _repository.GetDistributionContracts().SingleOrDefault(p => p.Partner == partener);

            Check<ArgumentException>(distributionContract != null, $"Error: Partener not found {partener}");

            var availableContracts = _repository.GetMusicContracts().Where(c => c.Usages.HasFlag(distributionContract.Usage)
                                                                                && c.StartDate <= effectiveDate
                                                                                && (!c.EndDate.HasValue || c.EndDate >= effectiveDate)).ToArray();


            return new Tuple<DistributionContract, IEnumerable<MusicContract>>(distributionContract, availableContracts);
        }

        private static void Check<TException>(bool condition, string errorMessage) where TException : Exception
        {
            if (condition) return;
            throw Activator.CreateInstance(typeof(TException), errorMessage) as TException;
        }

        private static string ExtractPartener(string[] queryParse)
        {
            return queryParse[0];
        }

        private static DateTime ExtractDate(string[] queryParse)
        {
            DateTime effectiveDate;
            Check<ArgumentException>(DateTime.TryParseExact(queryParse[1], "d'st' MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out effectiveDate)
                    || DateTime.TryParseExact(queryParse[1], "d'st' MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out effectiveDate)
                    , "Error: invalid query - date format");
            return effectiveDate;
        }

        private static string[] ValidateAndParseQuery(QueryCommand command)
        {
            Check<ArgumentNullException>(command != null, "Error: command was null");
            Check<ArgumentException>(!string.IsNullOrWhiteSpace(command.Query), "Error: invalid query");
            var queryParse = Regex.Split(command.Query, @"(?<=^[^\s]*)\s");
            Check<ArgumentException>(queryParse.Length == 2, "Error: invalid query");
            return queryParse;
        }
    }
}
