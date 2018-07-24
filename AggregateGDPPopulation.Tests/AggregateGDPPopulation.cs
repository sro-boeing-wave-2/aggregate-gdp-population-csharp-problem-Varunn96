using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AggregateGDPPopulation
{
    public class AggregateClass
    {
        public static async Task AggregateCalculationAsync()
        {
            // Reading Files asynchronously. Second file is read independently
            Task<List<string>> CsvData = ReadCSVFileAsync("../../../../AggregateGDPPopulation/data/datafile.csv");
            Task<string> MapDataString = ReadMapFileAsync("../../../../AggregateGDPPopulation/data/countrytocontinentmap.json");

            // Awaiting contents of first file to be read to proceed. List of strings returned
            List<string> CsvDataList = await CsvData;

            // Obtaining only headers from CsvDataList, and split by comma and add into string array
            string[] CsvDataHeaders = CsvDataList[0].Replace("\"", string.Empty).Split(',');

            // Obtaining indices of required fields
            int CountryIndex = Array.IndexOf(CsvDataHeaders, "Country Name");
            int PopulationIndex = Array.IndexOf(CsvDataHeaders, "Population (Millions) 2012");
            int GDPIndex = Array.IndexOf(CsvDataHeaders, "GDP Billions (USD) 2012");

            // Removing headers from CsvDataList
            CsvDataList.Remove(CsvDataList[0]);

            // Splitting each list entry by comma and storing in a list of string arrays
            List<string[]> CsvDataListSplitByComma = new List<string[]>();
            foreach (string item in CsvDataList)
            {
                CsvDataListSplitByComma.Add(item.Replace("\"", string.Empty).Split(','));
            }

            // Awaiting contents of second file to be read to proceed. Returns string that contains JSON
            // Preceding steps independent of second file
            string MapData = await MapDataString;

            // Converting string to a JObject
            var CountryContinetMap = JObject.Parse(MapData);

            // Creating Dictionary with key string(continent) and value(object with GDP2012 and Population2012)
            Dictionary<string, object> AggregateOutput = AddAggregate(CountryContinetMap, CsvDataListSplitByComma, CountryIndex, PopulationIndex, GDPIndex);

            string Output = JsonConvert.SerializeObject(AggregateOutput, Formatting.Indented);
            Console.WriteLine(Output);
            await WriteToFileAsync("../../../../AggregateGDPPopulation/output/output.json", Output);
        }

        public static async Task<List<string>> ReadCSVFileAsync(string filepath)
        {
            StreamReader CsvData = new StreamReader(filepath);
            List<string> CsvDataList = new List<string>();
            try
            {
                string temp = null;
                while ((temp = await CsvData.ReadLineAsync()) != null)
                {
                    CsvDataList.Add(temp);
                }
            }
            catch
            {
                return null;
            }
            return CsvDataList;
        }

        public static async Task<string> ReadMapFileAsync(string filepath)
        {
            StreamReader MapData = new StreamReader(filepath);
            return await MapData.ReadToEndAsync();
        }

        public static async Task WriteToFileAsync(string filepath, string output)
        {
            using (StreamWriter write = new StreamWriter(filepath))
            {
                await write.WriteAsync(output);
            }
        }

        public Dictionary<string, object> AddAggregate(JObject CountryContinentMap, List<string[]> CsvDataListSplitByComma, int CountryIndex, int PopulationIndex, int GDPIndex)
        {
            Dictionary<string, GDPPop> AggregateOutput = new Dictionary<string, GDPPop>();

            // Adding data to AggregateOutput
            CsvDataListSplitByComma.ForEach((row) =>
            {
                try
                {
                    string Continent = CountryContinetMap.GetValue(row[CountryIndex]).ToString();
                    AggregateOutput[Continent].Population_2012 += float.Parse(row[PopulationIndex]);
                    AggregateOutput[Continent].GDP_2012 += float.Parse(row[GDPIndex]);
                }
                catch (Exception)
                {
                    try
                    {
                        string Continent = CountryContinetMap.GetValue(row[CountryIndex]).ToString();
                        GDPPop GDPandPOP = new GDPPop();
                        GDPandPOP.Population_2012 = float.Parse(row[PopulationIndex]);
                        GDPandPOP.GDP_2012 = float.Parse(row[GDPIndex]);
                        AggregateOutput.Add(Continent, GDPandPOP);
                    }
                    catch (Exception) { }
                }
            });
            return AggregateOutput;
        }
    }

    public class GDPPop
    {
        public float Population_2012 { get; set; }
        public float GDP_2012 { get; set; }
    }
}
