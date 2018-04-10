using System.Collections.Generic;
using System.IO;
using CsvHelper;
using static System.Reflection.Assembly;

namespace LendingLibrary.Models
{
    public class SeedImporter
    {
        public IEnumerable<T> Get<T>()
        {
            var resource = $"LendingLibrary.App_Data.{typeof(T).Name}s_Seed.csv";

            using (var stream = GetExecutingAssembly().GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.HeaderValidated = null;
                csv.Configuration.MissingFieldFound = null;
                foreach (var record in csv.GetRecords<T>())
                {
                    yield return record;
                }
            }
        }
    }
}
