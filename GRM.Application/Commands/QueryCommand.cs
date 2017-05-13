using System;
using System.Collections.Generic;
using GRM.Application.DomainObjects;
using MediatR;

namespace GRM.Application.Commands
{
    public class QueryCommand : IRequest<Tuple<DistributionContract, IEnumerable<MusicContract>>>
    {
        public string Query { get; set; }
    }
}
