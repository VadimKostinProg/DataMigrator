using Dapper;
using Microsoft.Data.SqlClient;

namespace DataMigrator;

public class DataProceeder
{
    public async Task Proceed(List<TripModel> data)
    {
        await using SqlConnection connection = new(GlobalSettings.SQLConnectionString);

        await connection.OpenAsync();

        string sql = 
            "INSERT INTO Trips (tpep_pickup_datetime, tpep_dropoff_datetime, passenger_count," +
                "trip_distance, store_and_fwd_flag, PULocationID, DOLocationID, fare_amount, tip_amount) " +
            "VALUES (@TpepPickupDatetime, @TpepDropoffDatetime, @PassengerCount, @TripDistance, @StoreAndFwdFlag, @PULocationID," +
                "@DOLocationID, @FareAmount, @TipAmount)";

        foreach (TripModel trip in data)
        {
            try
            {
                await connection.ExecuteAsync(sql, trip);
            }
            catch (SqlException ex) // Handling duplicate by pickup_datetime, dropoff_datetime, passenger_count
            {
                if (ex.Message.Contains("idx_unique_trip_datetime_passenger_count"))
                    DuplicateTripsHandler.AddToReport(trip);
            }
        }

        await connection.CloseAsync();
    }
}
