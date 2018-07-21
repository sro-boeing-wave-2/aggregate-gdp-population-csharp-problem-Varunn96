using System;
using Xunit;
using AggregateGDPPopulation;
using System.IO;

namespace AggregateGDPPopulation.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            AggregateClass.AggregateCalculation();
            StreamReader generatedOutput = new StreamReader("../../../../AggregateGDPPopulation/output/output.json");
            StreamReader expectedOutput = new StreamReader("../../../expected-output.json");
            string actual = generatedOutput.ReadToEnd();
            string expected = expectedOutput.ReadToEnd();
            Assert.True(actual == expected);
        }
    }
}
