using System.Collections.Concurrent;

namespace DataMigrator;

public static class DuplicateTripsHandler
{
    private static ConcurrentBag<TripModel> _duplicateReport = new();

    public static void AddToReport(TripModel trip)
    {
        _duplicateReport.Add(trip);
    }

    public static void OutputReport()
    {
        List<string> csvLines = _duplicateReport
            .Select(CSVParser.ParseTripModelToCSVLine)
            .ToList();

        string header = "tpep_pickup_datetime,tpep_dropoff_datetime,passenger_count,trip_distance," +
            "store_and_fwd_flag,PULocationID,DOLocationID,fare_amount,tip_amount";

        csvLines.Insert(0, header);

        File.WriteAllLines(GlobalSettings.DuplicateReportPath, csvLines);
    }
}
