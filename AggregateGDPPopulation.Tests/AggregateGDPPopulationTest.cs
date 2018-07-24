using System;
using Xunit;
using AggregateGDPPopulation;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1Async()
        {
            AggregateGDPPopulation AGP = new AggregateGDPPopulation();
            await AGP.performAggregateOperation();
            Task<string> actualstring = IOOperations.ReadFileToEndAsync("../../../../AggregateGDPPopulation/output/output.json");
            Task<string> expectedstring = IOOperations.ReadFileToEndAsync("../../../expected-output.json");
            string actual = await actualstring;
            string expected = await expectedstring;
            JObject ExpectedJO = JSONOperations.JSONDeserialize(actual);
            JObject ActualJO = JSONOperations.JSONDeserialize(expected);
            Assert.Equal(ExpectedJO,ActualJO);
        }
    }
}
