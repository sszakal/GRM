using Moq;
using TechTalk.SpecFlow;

namespace GRM.Application.Tests.EndToEnd
{
    [Binding]
    public static class SpecflowSteps
    {
        private static string[] cliArguments;
        private static Mock<IConsole> _consoleMock;

        [BeforeScenario]
        public static void BeforeScenario()
        {
            _consoleMock = new Mock<IConsole>();
            Program.ConsoleInstance = _consoleMock.Object;
        }

        [Given(@"the supplied above reference data")]
        public static void GivenTheSuppliedAboveReferenceData()
        {
            cliArguments = new[] { @"EndToEnd\music.txt", @"EndToEnd\distribution.txt" };
        }

        [When(@"user enters '(.*)'")]
        public static void WhenUserEnters(string text)
        {
            _consoleMock.SetupSequence(m => m.ReadLine()).Returns(text).Returns("\n");
            Program.Main(cliArguments);
        }

        [Then(@"the output is: (.*)")]
        public static void ThenTheOutputIs(string text)
        {
            _consoleMock.Verify(m => m.WriteLine(text), Times.Once);
        }
    }
}
