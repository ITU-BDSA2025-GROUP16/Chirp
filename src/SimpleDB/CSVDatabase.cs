using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T> where T : class
{
    private static readonly Lazy<CSVDatabase<T>> _lazyInstance =
        new Lazy<CSVDatabase<T>>(() => new CSVDatabase<T>("../../data/chirp_cli_db.csv"));

    public static CSVDatabase<T> Instance => _lazyInstance.Value;

    private readonly string _filePath;

    private CSVDatabase(string filePath)
    {
        _filePath = filePath;

        EnsureFileExists();
    }
    
    public CSVDatabase(string filePath, bool forTest = true)
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    private void EnsureFileExists()
    {
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
        
        if (new FileInfo(_filePath).Length == 0)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }
        
        csv.NextRecord();
        csv.WriteRecord(record);
    }
    
    public void DeleteLast()
    {
        var records = Read().ToList();
        if (!records.Any()) return;

        records.RemoveAt(records.Count - 1);

        using var writer = new StreamWriter(_filePath, false);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<T>();
        writer.WriteLine();
        csv.WriteRecords(records);
    }
}