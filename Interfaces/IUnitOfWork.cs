namespace MongoDBRepository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync();
}