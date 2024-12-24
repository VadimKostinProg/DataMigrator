using System.Collections.Concurrent;
using System.Diagnostics;

namespace DataMigrator;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConcurrentQueue<List<TripModel>> batchQueue = new();

        Console.WriteLine("Data processing has started.");

        Stopwatch timer = new();

        timer.Start();

        Task producerTask = Task.Run(() => ReadData(batchQueue));

        List<Task> consumerTasks = [];

        for (int i = 0; i < GlobalSettings.MaxParallelTasks; i++)
        {
            consumerTasks.Add(Task.Run(async () => await ProceedData(batchQueue)));
        }

        await producerTask;

        await Task.WhenAll(consumerTasks);

        timer.Stop();

        DuplicateTripsHandler.OutputReport();

        Console.WriteLine($"Data processing has completed in {timer.ElapsedMilliseconds} ms");
    }

    private static void ReadData(ConcurrentQueue<List<TripModel>> batchQueue)
    {
        List<TripModel> batch = [];

        using StreamReader stream = new(GlobalSettings.SourcePath);

        stream.ReadLine();

        int count = 0;

        while (!stream.EndOfStream)
        {
            string line = stream.ReadLine()!;

            if (CSVParser.TryParseCSVLineToTripModel(line, out TripModel? result))
                batch.Add(result!);

            count++;

            if (count == GlobalSettings.BatchSize)
            {
                batchQueue.Enqueue(new(batch));

                batch.Clear();

                count = 0;
            }
        }

        if (batch.Count > 0)
        {
            batchQueue.Enqueue(batch);

            batch.Clear();
        }
    }

    private static async Task ProceedData(ConcurrentQueue<List<TripModel>> batchQueue)
    {
        DataProceeder proceeder = new();

        while(true)
        {
            if (batchQueue.TryDequeue(out var batch))
            {
                await proceeder.Proceed(batch);
                break;
            }
        }
    }
}