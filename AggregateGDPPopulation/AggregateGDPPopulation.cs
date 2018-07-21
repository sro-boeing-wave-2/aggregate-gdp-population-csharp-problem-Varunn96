using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AggregateGDPPopulation
{
    public class AggregateClass
    {
        public static void AggregateCalculation()
        {
            var CsvData = File.ReadLines(@"D:\workspace\aggregate-gdp-population-csharp-problem-Varunn96\AggregateGDPPopulation\data\datafile.csv");
            List<string> CsvDataList = CsvData.ToList();
            //Console.Write(CsvDataList[1]); ===> First Country Details

            string[] CsvDataHeaders = CsvDataList[0].Replace("\"", string.Empty).Split(',');
            //Console.WriteLine(CsvDataHeaders[0]); ===> Country Name

            int CountryIndex = Array.IndexOf(CsvDataHeaders, "Country Name");
            int PopulationIndex = Array.IndexOf(CsvDataHeaders, "Population (Millions) 2012");
            int GDPIndex = Array.IndexOf(CsvDataHeaders, "GDP Billions (USD) 2012");
            //Console.Write("{0} {1} {2}", CountryIndex, PopulationIndex, GDPIndex); ===> 0, 4 and 10

            CsvDataList.Remove(CsvDataList[0]);
            //Console.WriteLine(CsvDataList[0]); ===> First Country Details

            List<string[]> CsvDataListSplitByComma = new List<string[]>();
            foreach (string item in CsvDataList)
            {
                CsvDataListSplitByComma.Add(item.Replace("\"", string.Empty).Split(','));
            }
            //Console.WriteLine(CsvDataListSplitByComma[0][CountryIndex]); ===> Argentina

            Dictionary<string, string> MapDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(@"D:\workspace\aggregate-gdp-population-csharp-problem-Varunn96\AggregateGDPPopulation\data\countrytocontinentmap.json"));

            Dictionary<string, GDPPop> AggregateOutput = new Dictionary<string, GDPPop>();
            CsvDataListSplitByComma.ForEach((row) =>
            {
                try
                {
                    AggregateOutput[MapDataDictionary[row[CountryIndex]]].Population_2012 += float.Parse(row[PopulationIndex]);
                    AggregateOutput[MapDataDictionary[row[CountryIndex]]].GDP_2012 += float.Parse(row[GDPIndex]);
                }
                catch (Exception)
                {
                    try
                    {
                        GDPPop GDPandPOP = new GDPPop();
                        GDPandPOP.Population_2012 = float.Parse(row[PopulationIndex]);
                        GDPandPOP.GDP_2012 = float.Parse(row[GDPIndex]);
                        AggregateOutput.Add(MapDataDictionary[row[CountryIndex]], GDPandPOP);
                    }
                    catch (Exception) { }
                }
            });
            string Output = JsonConvert.SerializeObject(AggregateOutput, Formatting.Indented);
                File.WriteAllText(@"D:\workspace\aggregate-gdp-population-csharp-problem-Varunn96\AggregateGDPPopulation\output\output.json", Output);
            //Console.WriteLine(JSONOutput);
            //Console.ReadKey();
        }
    }
    public class GDPPop
    {
        public float Population_2012 { get; set; }
        public float GDP_2012 { get; set; }
    }
}
