using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace EnglishMonarchs.UnitTests
{
    public class MonarchProcessorTests
    {
        [Fact]
        public void GetMonarchs_Empty_MonarchSourceUrl()
        {
            //Arrange
            var errorMessage = string.Empty;
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            mockConsoleManager.Setup(c => c.ErrorWriteLine(It.IsAny<string>())).Callback((string message) =>
                {
                    errorMessage = message;
                }).Verifiable();

            mockConfigurationManager.Setup(c=>c.GetMonarchSourceUrl()).Returns(string.Empty).Verifiable();

            //Act
            var monarchProcessor = new MonarchProcessor(mockConsoleManager.Object, mockConfigurationManager.Object);
            monarchProcessor.GetMonarchs("");
            //Assert
            Assert.True(errorMessage != null && errorMessage.Contains("Cannot find 'MonarchSource' Url"));
        }

        [Fact]
        public void GetMonarchs_Null_MonarchSourceUrl()
        {
            //Arrange
            var errorMessage = string.Empty;
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            mockConsoleManager.Setup(c => c.ErrorWriteLine(It.IsAny<string>())).Callback((string message) =>
                {
                    errorMessage = message;
                }).Verifiable();
            mockConfigurationManager.Setup(c => c.GetMonarchSourceUrl()).Returns(string.Empty).Verifiable();

            //Act
            var monarchProcessor = new MonarchProcessor(mockConsoleManager.Object, mockConfigurationManager.Object);
            monarchProcessor.GetMonarchs(null);
            //Assert
            Assert.True(errorMessage != null && errorMessage.Contains("Cannot find 'MonarchSource' Url"));
        }

        [Fact]
        public void GetMonarchs_Invalid_MonarchSourceUrl()
        {
            //Arrange
            var errorMessage = string.Empty;
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            mockConsoleManager.Setup(c => c.ErrorWriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                errorMessage = message;
            }).Verifiable();
            mockConfigurationManager.Setup(c => c.GetMonarchSourceUrl()).Returns(string.Empty).Verifiable();

            //Act
            var monarchProcessor = new MonarchProcessor(mockConsoleManager.Object, mockConfigurationManager.Object);
            monarchProcessor.GetMonarchs("dfdfdfdfdfdfdfdfdfdfdfdfdf");
            //Assert
            Assert.True(errorMessage != null && errorMessage.Contains("Invalid 'MonarchSource' Url"));
        }

        [Fact]
        public void Run_NoMonarchsFound()
        {
            //Arrange
            var errorMessage = string.Empty;
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor =
                new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) {CallBase = true};
            mockMonarchProcessor.Setup(m => m.GetMonarchs(It.IsAny<string>())).Returns(new List<Monarch>()).Verifiable();

            //Act
            mockConsoleManager.Setup(c => c.ErrorWriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                errorMessage = message;
            }).Verifiable();
            mockMonarchProcessor.Object.Run();
            //Assert
            Assert.True(errorMessage != null && errorMessage.Contains("No monarchs found"));
        }

        [Fact]
        public void Monarch_EmptyFromRuledYears()
        {
            //Arrange
            var monarchJsonData = "[{\"id\":1,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-925\"},{\"id\":2,\"nm\":\"Athelstan\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-940\"},{\"id\":3,\"nm\":\"Edmund\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-946\"},{\"id\":4,\"nm\":\"Edred\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-955\"},{\"id\":5,\"nm\":\"Edwy\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"955-959\"},{\"id\":6,\"nm\":\"Edgar\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-975\"},{\"id\":7,\"nm\":\"Elizabeth II\",\"cty\":\"United Kingdom\",\"hse\":\"House of Windsor\",\"yrs\":\"-1952\"}]";
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor = new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) { CallBase = true };

            //Act
            //As per the Monarch.cs logic, if any of the ruled years from data is null or empty then the whole ruled years should be marked as 'N/A'
            var monarchs = mockMonarchProcessor.Object.DeserializeMonarchJson(monarchJsonData);
            //Assert
            Assert.True(monarchs.Any(m=>m.RuledYears.Contains("N/A")));
        }

        [Fact]
        public void Run_PrintNoOfMonarchs_Success()
        {
            //Arrange
            var monarchJsonData = "[{\"id\":1,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-925\"},{\"id\":2,\"nm\":\"Athelstan\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-940\"},{\"id\":3,\"nm\":\"Edmund\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-946\"},{\"id\":4,\"nm\":\"Edred\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-955\"},{\"id\":5,\"nm\":\"Edwy\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"955-959\"},{\"id\":6,\"nm\":\"Edgar\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-975\"},{\"id\":7,\"nm\":\"Elizabeth II\",\"cty\":\"United Kingdom\",\"hse\":\"House of Windsor\",\"yrs\":\"-1952\"}]";
            var printedOutPut = string.Empty;
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor = new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) { CallBase = true };
            var monarchs = mockMonarchProcessor.Object.DeserializeMonarchJson(monarchJsonData);
            mockMonarchProcessor.Object.AllMonarchs = monarchs;
            mockConsoleManager.Setup(c => c.WriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                printedOutPut = message;
            }).Verifiable();

            //Act
            mockMonarchProcessor.Object.PrintNoOfMonarchs();

            //Assert
            Assert.Contains(monarchs.Count().ToString(), printedOutPut);
        }

        [Fact]
        public void Run_PrintLongestRuledMonarch_Success()
        {
            //Arrange
            var monarchJsonData = "[{\"id\":1,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"899-925\"},{\"id\":2,\"nm\":\"Athelstan\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"925-940\"},{\"id\":3,\"nm\":\"Edmund\",\"cty\":\"United Kingdom\",\"hse\":\"HouseofWessex\",\"yrs\":\"940-946\"},{\"id\":4,\"nm\":\"Edred\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"946-955\"},{\"id\":56,\"nm\":\"George VI\",\"cty\":\"United Kingdom\",\"hse\":\"House of Windsor\",\"yrs\":\"1936-1952\"},{\"id\":57,\"nm\":\"Elizabeth II\",\"cty\":\"United Kingdom\",\"hse\":\"House of Windsor\",\"yrs\":\"1952-\"}, {\"id\":58,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"-925\"}]";
            var printedOutPut = string.Empty;
            var expectedMonarchName = "Elizabeth II";
            var expectedMonarchRuledYears = "70";
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor = new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) { CallBase = true };
            var monarchs = mockMonarchProcessor.Object.DeserializeMonarchJson(monarchJsonData);
            mockMonarchProcessor.Object.AllMonarchs = monarchs;
            mockConsoleManager.Setup(c => c.WriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                printedOutPut = message;
            }).Verifiable();

            //Act
            mockMonarchProcessor.Object.PrintLongestRuledMonarch();

            //Assert
            Assert.Contains(expectedMonarchRuledYears, printedOutPut);
            Assert.Contains(expectedMonarchName, printedOutPut);
        }

        [Fact]
        public void Run_PrintLongestRuledHouseWithYears_Success()
        {
            //Arrange
            var monarchJsonData = "[{\"id\":1,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"899-925\"},{\"id\":2,\"nm\":\"Athelstan\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"925-940\"},{\"id\":3,\"nm\":\"Edmund\",\"cty\":\"UnitedKingdom\",\"hse\":\"HouseofWessex\",\"yrs\":\"940-946\"},{\"id\":4,\"nm\":\"Edred\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"946-955\"},{\"id\":5,\"nm\":\"Edwy\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"955-959\"},{\"id\":6,\"nm\":\"Edgar\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"959-975\"}]";
            var printedOutPut = string.Empty;
            var expectedRuledHouse = "House of Wessex";
            var expectedMonarchRuledYears = "76";
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor = new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) { CallBase = true };
            var monarchs = mockMonarchProcessor.Object.DeserializeMonarchJson(monarchJsonData);
            mockMonarchProcessor.Object.AllMonarchs = monarchs;
            mockConsoleManager.Setup(c => c.WriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                printedOutPut = message;
            }).Verifiable();

            //Act
            mockMonarchProcessor.Object.PrintLongestRuledHouseWithYears();

            //Assert
            Assert.Contains(expectedMonarchRuledYears, printedOutPut);
            Assert.Contains(expectedRuledHouse, printedOutPut);
        }

        [Fact]
        public void Run_PrintCommonMonarchFirstName_Success()
        {
            //Arrange
            var monarchJsonData = "[{\"id\":1,\"nm\":\"Edward the Elder\",\"cty\":\"United Kingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"899-925\"},{\"id\":2,\"nm\":\"Athelstan\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"925-940\"},{\"id\":3,\"nm\":\"Edmund\",\"cty\":\"UnitedKingdom\",\"hse\":\"HouseofWessex\",\"yrs\":\"940-946\"},{\"id\":4,\"nm\":\"Edred\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"946-955\"},{\"id\":5,\"nm\":\"Edwy\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"955-959\"},{\"id\":6,\"nm\":\"Edgar\",\"cty\":\"UnitedKingdom\",\"hse\":\"House of Wessex\",\"yrs\":\"959-975\"}]";
            var printedOutPut = string.Empty;
            var expectedCommonFirstName = "Edward";
            var mockConsoleManager = new Mock<IConsoleManager>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMonarchProcessor = new Mock<MonarchProcessor>(mockConsoleManager.Object, mockConfigurationManager.Object) { CallBase = true };
            var monarchs = mockMonarchProcessor.Object.DeserializeMonarchJson(monarchJsonData);
            mockMonarchProcessor.Object.AllMonarchs = monarchs;
            mockConsoleManager.Setup(c => c.WriteLine(It.IsAny<string>())).Callback((string message) =>
            {
                printedOutPut = message;
            }).Verifiable();

            //Act
            mockMonarchProcessor.Object.PrintCommonMonarchFirstName();

            //Assert
            Assert.Contains(expectedCommonFirstName, printedOutPut);
        }
    }
}