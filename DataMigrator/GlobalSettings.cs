namespace DataMigrator;

public static class GlobalSettings
{
    public const string SourcePath = "D:\\Programming\\sample-cab-data.csv";
    public const string DuplicateReportPath = "D:\\Programming\\duplicates.csv";
    public const int BatchSize = 3000;
    public const int MaxParallelTasks = 10;
    public const string SQLConnectionString = @"Data Source=LAPTOP-F47OH73P\MSSQLSERVER03;Initial Catalog=TripsDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
}
