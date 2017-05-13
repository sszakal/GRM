using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Features.Variance;
using CommandLine;
using GRM.Application.Commands;
using GRM.Application.Data;
using GRM.Application.DomainObjects;
using MediatR;

namespace GRM
{
    public static class Program
    {
        public static IConsole ConsoleInstance = new Console();

        public static void Main(string[] args)
        {
            var builder = BuildContainer();

            var parser = new Parser(with => {
                with.EnableDashDash = true;
                with.CaseSensitive = false;
                with.IgnoreUnknownArguments = true;
                with.HelpWriter = System.Console.Out;
            });

            var result = parser.ParseArguments<InputArguments>(args) as Parsed<InputArguments>;

            if (result?.Value != null)
            {
                var inputFiles = result.Value;
                ValidateInputArguments(inputFiles);
                builder.RegisterInstance(new Repository(inputFiles.MusicContractsFilePath,
                    inputFiles.DistributionPartnersContractsFilePath)).As<IRepository>();
                RunApplication(builder);
            }
            else
            {
                parser.FormatCommandLine(new InputArguments());
                ConsoleInstance.ReadLine();
            }
        }

        private static void Check<TException>(bool condition, string errorMessage) where TException : Exception
        {
            if (condition) return;
            throw Activator.CreateInstance(typeof(TException), errorMessage) as TException;
        }

        private static void ValidateInputArguments(InputArguments inputFiles)
        {
            Check<FileNotFoundException>(File.Exists(Path.GetFullPath(inputFiles.MusicContractsFilePath)), $"File doesn't exist or incorrect path: {inputFiles.MusicContractsFilePath}");
            Check<FileNotFoundException>(File.Exists(Path.GetFullPath(inputFiles.DistributionPartnersContractsFilePath)), $"File doesn't exist or incorrect path: {inputFiles.DistributionPartnersContractsFilePath}");
        }   

        private static void RunApplication(ContainerBuilder builder)
        {
            var mediator = builder.Build().Resolve<IMediator>();

            while (true)
            {
                ConsoleInstance.WriteLine("Please enter query or press enter to exit:");

                var userInput = ConsoleInstance.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput)) break;

                try
                {
                    var response = mediator.Send(new QueryCommand {Query = userInput});

                    var availableContracts = response.Item2 as MusicContract[] ?? response.Item2.ToArray();
                    ConsoleInstance.WriteLine($"{availableContracts.Count()} contract(s) match your query");

                    ConsoleInstance.WriteLine(@"Artist|Title|Usage|StartDate|EndDate");

                    foreach (var contract in availableContracts)
                    {
                        ConsoleInstance.WriteLine(
                            $"{contract.Artist}|{contract.Title}|{response.Item1.DisplayUsage}|{contract.DisplayStartDate}|{contract.DisplayEndDate}");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleInstance.WriteLine(ex.Message);
                }

                ConsoleInstance.WriteLine("");
            }
        }

        private static ContainerBuilder BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof (IMediator).Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof (QueryCommand).Assembly).AsImplementedInterfaces();
            builder.Register(context => System.Console.Out).As<TextWriter>();

            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>) c.Resolve(typeof (IEnumerable<>).MakeGenericType(t));
            });

            builder.RegisterInstance(ConsoleInstance).As<IConsole>();

            return builder;
        }
    }
}
