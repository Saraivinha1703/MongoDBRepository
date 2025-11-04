using MongoDBRepository.Interfaces;

namespace MongoDBRepository.Data;

public class UnitOfWork(IMongoContext context) : IUnitOfWork
{
    private readonly IMongoContext _context = context;

    public async Task<bool> CommitAsync()
    {
        var changeAmount = await _context.SaveChangesAsync();

        return changeAmount > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
        
        GC.SuppressFinalize(this);
    }
}