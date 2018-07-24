using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AggregateGDPPopulation
{
    public class AggregateGDPPopulation
    {
       
        public async Task performAggregateOperation()
        {
            Task<List<string>> CsvData = IOOperations.ReadFileByLineAsync("../../../../AggregateGDPPopulation/data/datafile.csv");
            Task<string> MapDataString = IOOperations.ReadFileToEndAsync("../../../../AggregateGDPPopulation/data/countrytocontinentmap.json");

            List<string> CsvDataList = await CsvData;
            string[] CsvDataHeaders = CsvDataList[0].Replace("\"", string.Empty).Split(',');
            int CountryIndex = Array.IndexOf(CsvDataHeaders, "Country Name");
            int PopulationIndex = Array.IndexOf(CsvDataHeaders, "Population (Millions) 2012");
            int GDPIndex = Array.IndexOf(CsvDataHeaders, "GDP Billions (USD) 2012");
            CsvDataList.Remove(CsvDataList[0]);
            List<string[]> CsvDataListSplitByComma = new List<string[]>();
            AddOrUpdateOutput output = new AddOrUpdateOutput();
            foreach (string item in CsvDataList)
            {
                CsvDataListSplitByComma.Add(item.Replace("\"", string.Empty).Split(','));
            }

            string MapData = await MapDataString;
            var CountryContinetMap = JSONOperations.JSONDeserialize(MapData);

            foreach (string[] row in CsvDataListSplitByComma)
            {
                float Population = float.Parse(row[PopulationIndex]);
                float GDP = float.Parse(row[GDPIndex]);
                try
                {
                    string Continent = CountryContinetMap.GetValue(row[CountryIndex]).ToString();
                    output.AddOrUpdate(Continent, Population, GDP);
                }
                catch (Exception) { }
                
            }

            string Output = JSONOperations.JSONSerialize(output.AggregateOutput);
            await IOOperations.WriteToFileAsync("../../../../AggregateGDPPopulation/output/output.json", Output);
        }
    }

    public class AddOrUpdateOutput
    {
        public Dictionary<string, GDPandPop> AggregateOutput;
        public AddOrUpdateOutput()
        {
            AggregateOutput = new Dictionary<string, GDPandPop>();
        }
        public void AddOrUpdate(string Continent, float Population, float GDP)
        {
            try
            {
                AggregateOutput[Continent].Population_2012 += Population;
                AggregateOutput[Continent].Gdp_2012 += GDP;
            }
            catch (Exception)
            {
                AggregateOutput.Add(Continent, new GDPandPop() { Population_2012 = Population ,Gdp_2012 = GDP });
            }
        }
    }

    public class GDPandPop
    {
        private float population_2012;
        private float gdp_2012;
        public float Population_2012
        {
            get
            {
                return this.population_2012;
            }
            set
            {
                this.population_2012 = value;
            }
        }
        public float Gdp_2012
        {
            get
            {
                return this.gdp_2012;
            }
            set
            {
                this.gdp_2012 = value;
            }
        }
    }

    public class IOOperations
    {
        // Reads a file line by line Asynchronously. Returns List of strings
        public static async Task<List<string>> ReadFileByLineAsync(string filepath)
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

        // Reads a file to the end Asynchronously. Returns string
        public static async Task<string> ReadFileToEndAsync(string filepath)
        {
            StreamReader MapData = new StreamReader(filepath);
            return await MapData.ReadToEndAsync();
        }

        // Writes data to a file Asynchronously
        public static async Task WriteToFileAsync(string filepath, string output)
        {
            using (StreamWriter write = new StreamWriter(filepath))
            {
                await write.WriteAsync(output);
            }
        }
    }

    public class JSONOperations
    {
        // Converts string to JObject
        public static JObject JSONDeserialize(string deserializeThis)
        {
            JObject deserializedOutput = JObject.Parse(deserializeThis);
            return deserializedOutput;
        }

        // Converts Dictionary to string in JSON Format
        public static string JSONSerialize(Dictionary<string, GDPandPop> serializeThis)
        {
            string serializedoutput = JsonConvert.SerializeObject(serializeThis, Formatting.Indented);
            return serializedoutput;
        }
    }
}