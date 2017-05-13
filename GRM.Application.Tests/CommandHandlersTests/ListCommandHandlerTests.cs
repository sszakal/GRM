using System;
using System.Linq;
using GRM.Application.CommandHandlers;
using GRM.Application.Commands;
using GRM.Application.Data;
using GRM.Application.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GRM.Application.Tests.CommandHandlersTests
{
    [TestClass]
    public class ListCommandHandlerTests
    {
        private ListCommandHandler _handler;
        private Mock<IRepository> _repositoryMock;
        private QueryCommand _command;
        private MusicContract _musicContract1;
        private MusicContract _musicContract2;
        private MusicContract _musicContract3;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository>();
            _musicContract1 = new MusicContract
            {
                Artist = "Artist 1",
                Title = "Title 1",
                Usages = EContractUsage.DigitalDownload,
                StartDate = new DateTime(2012, 6, 1),
                EndDate = new DateTime(2012, 7, 1)
            };
            _musicContract2 = new MusicContract
            {
                Artist = "Artist 2",
                Title = "Title 2",
                Usages = EContractUsage.DigitalDownload,
                StartDate = new DateTime(2012, 6, 1)
            };
            _musicContract3 = new MusicContract
            {
                Artist = "Artist 3",
                Title = "Title 3",
                Usages = EContractUsage.Streaming,
                StartDate = new DateTime(2013, 6, 1)
            };
            _repositoryMock.Setup(m => m.GetMusicContracts()).Returns(new[]
            {
                _musicContract1,
                _musicContract2,
                _musicContract3
            });

            _repositoryMock.Setup(m => m.GetDistributionContracts()).Returns(new[]
            {
                new DistributionContract { Partner = "Partener1", Usage = EContractUsage.Streaming},
                new DistributionContract { Partner = "Partener2", Usage = EContractUsage.DigitalDownload}
            });
            _handler = new ListCommandHandler(_repositoryMock.Object);
            _command = new QueryCommand();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Error: command was null")]
        public void Should_Check_Command_For_Null()
        {
            _handler.Handle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: invalid query")]
        public void Should_Check_If_Query_Is_Null()
        {
            _handler.Handle(_command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: invalid query")]
        public void Should_Check_If_Query_Is_WhiteSpace()
        {
            _command.Query = "                  ";
            _handler.Handle(_command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: invalid query")]
        public void Should_Check_If_Query_Has_Correct_Number_Of_Parameters()
        {
            _command.Query = "YouTube";
            _handler.Handle(_command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: invalid query - date format")]
        public void Should_Check_If_Query_Date_Is_Valid()
        {
            _command.Query = "YouTube INVALID";
            _handler.Handle(_command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Error: Partener not found YouTube")]
        public void Should_Check_If_Partener_Exists()
        {
            _command.Query = "YouTube 12st June 2012";
            _handler.Handle(_command);
        }

        [TestMethod]
        public void Should_Return_The_Correct_Number_Of_Contracts()
        {
            _command.Query = "Partener2 12st June 2012";
            var response = _handler.Handle(_command);
            Assert.AreEqual(2, response.Item2.Count(), "Query return incorrect number of items");
        }

        [TestMethod]
        public void Should_Display_Results()
        {
            _command.Query = "Partener2 12st June 2012";

            var response = _handler.Handle(_command);

            Assert.AreEqual(_musicContract1, response.Item2.ElementAt(0));
            Assert.AreEqual(_musicContract2, response.Item2.ElementAt(1));
        }
    }
}
