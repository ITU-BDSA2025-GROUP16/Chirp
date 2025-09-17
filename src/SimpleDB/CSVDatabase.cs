using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T> where T : class
{
    private readonly string _filePath;

    public CSVDatabase(string filePath)
    {
        _filePath = filePath;

        // If file doesn't exist, create an empty one with headers
        if (!File.Exists(_filePath))
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<T>();
            writer.WriteLine();
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<T>();
        return limit.HasValue ? records.Take(limit.Value).ToList() : records.ToList();
    }

    public void Store(T record)
    {
        using var writer = new StreamWriter(_filePath, append: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.NextRecord();
        csv.WriteRecord(record);
    }
}