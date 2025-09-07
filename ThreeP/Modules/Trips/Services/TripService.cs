namespace ThreeP.Modules.Trips;

public class TripService(IDbContextFactory<ApplicationDbContext> dbFactory, ILoggerFactory loggerFactory)
    : GenericService<Trip>(dbFactory, loggerFactory)
{
    public async Task<Result> UpsertTrip(Trip? incoming, CancellationToken cancel = default)
    {
        if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
        return await Upsert(incoming, (src, dst) =>
        {
            dst.Name = src.Name;
            dst.Description = src.Description;
            dst.SetId = src.SetId;
            dst.UserId = src.UserId;
        }, cancel);
    }


    /*
    public async Task<Result> UpsertTrip(Trip? incoming, CancellationToken cancel = default)
    {
        if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);

            var exist = await db.Trips.AsNoTracking().AnyAsync(x => x.Id == incoming.Id, cancel);

            var trip = new Trip
            {
                Id = incoming.Id,
                UserId = incoming.UserId,
            };
            if (!exist) await db.Trips.AddAsync(trip, cancel);
            else db.Trips.Attach(trip);

            trip.Name = incoming.Name;
            trip.Description = incoming.Description;
            trip.SetId = incoming.SetId;

            var saved = await db.SaveChangesAsync(cancel) > 0;
            return saved
                ? Result.Ok().WithSuccess(LogText.SaveOk)
                : Result.Fail($"{LogText.SaveFail} - {incoming.Name}");
        }
        catch (OperationCanceledException e)
        {
            return Result.Fail($"{LogText.OperationCancelled} - {e.Message}");
        }
        catch (Exception e)
        {
            logger.LogError(e, "{LogText}, Id: {Id}", LogText.ExceptionOccurred, incoming.Id);
            return Result.Fail([LogText.ExceptionOccurred, e.Message]);
        }
    }
*/
}