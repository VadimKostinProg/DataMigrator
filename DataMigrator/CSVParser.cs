using System.Globalization;

namespace DataMigrator;

public static class CSVParser
{
    public static bool TryParseCSVLineToTripModel(string line, out TripModel? result)
    {
        if (string.IsNullOrEmpty(line))
        {
            result = null;
            return false;
        }

        try
        {
            string[] args = line.Split(',');

            DateTime? tpepPickupDatetime = !string.IsNullOrWhiteSpace(args[1])
                ? DateTime.ParseExact(args[1], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                : null;
            DateTime? tpepDropoffDatetime = !string.IsNullOrWhiteSpace(args[2])
                ? DateTime.ParseExact(args[2], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                : null;
            int? passengerCount = !string.IsNullOrWhiteSpace(args[3])
                ? int.Parse(args[3])
                : null;
            double? tripDistance = !string.IsNullOrWhiteSpace(args[4])
                ? double.Parse(args[4])
                : null;
            string? storeAndFwdFlag = args[6]?.Trim(); // Handle whitespaces
            int? PULocationID = !string.IsNullOrWhiteSpace(args[7])
                ? int.Parse(args[7])
                : null;
            int? DOLocationID = !string.IsNullOrWhiteSpace(args[8])
                ? int.Parse(args[8])
                : null;
            decimal? fareAmount = !string.IsNullOrWhiteSpace(args[10])
                ? decimal.Parse(args[10])
                : null;
            decimal? tipAmount = !string.IsNullOrWhiteSpace(args[13])
                ? decimal.Parse(args[13])
                : null;

            // Convert date time from EST to UTC
            ConvertESTToUTC(ref tpepPickupDatetime, ref tpepDropoffDatetime);

            // Convert N to No and Y to Yes
            ConvertStoreAndFwdFlag(ref storeAndFwdFlag);

            result = new TripModel
            {
                TpepPickupDatetime = tpepPickupDatetime,
                TpepDropoffDatetime = tpepDropoffDatetime,
                PassengerCount = passengerCount,
                TripDistance = tripDistance,
                StoreAndFwdFlag = storeAndFwdFlag,
                PULocationID = PULocationID,
                DOLocationID = DOLocationID,
                FareAmount = fareAmount,
                TipAmount = tipAmount,
            };

            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            result = null;
            return false;
        }
    }

    public static string ParseTripModelToCSVLine(TripModel trip)
    {
        return string.Join(',', trip.TpepPickupDatetime, trip.TpepDropoffDatetime, trip.PassengerCount, trip.TripDistance,
                    trip.StoreAndFwdFlag, trip.PULocationID, trip.DOLocationID, trip.FareAmount, trip.TipAmount);
    }

    private static void ConvertESTToUTC(ref DateTime? tpepPickupDatetime, ref DateTime? tpepDropoffDatetime)
    {
        TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        if (tpepPickupDatetime != null)
            tpepPickupDatetime = TimeZoneInfo.ConvertTimeToUtc(tpepPickupDatetime.Value, estTimeZone);
        if (tpepDropoffDatetime != null)
            tpepDropoffDatetime = TimeZoneInfo.ConvertTimeToUtc(tpepDropoffDatetime.Value, estTimeZone);
    }

    private static void ConvertStoreAndFwdFlag(ref string? storeAndFwdFlag)
    {
        switch (storeAndFwdFlag)
        {
            case "N":
                storeAndFwdFlag = "No";
                break;
            case "Y":
                storeAndFwdFlag = "Yes";
                break;
        }
    }
}
