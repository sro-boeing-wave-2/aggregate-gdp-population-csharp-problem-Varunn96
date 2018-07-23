using System;
using Xunit;
using AggregateGDPPopulation;
using System.IO;
using System.Threading.Tasks;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1Async()
        {
            await AggregateClass.AggregateCalculationAsync();
            Task<string> actualstring = ReadFileAsync("../../../../AggregateGDPPopulation/output/output.json");
            Task<string> expectedstring = ReadFileAsync("../../../expected-output.json");
            string actual = await actualstring;
            string expected = await expectedstring;
            Assert.True(actual == expected);
        }
        public static async Task<string> ReadFileAsync(string filepath)
        {
            StreamReader generatedOutput = new StreamReader(filepath);
            return await generatedOutput.ReadToEndAsync();
        }
    }
}
