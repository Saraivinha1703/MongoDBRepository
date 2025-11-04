using System.Linq.Expressions;
using MongoDB.Driver;

namespace MongoDBRepository.Interfaces;

public interface IRepository<T, TKey> : IDisposable 
    where TKey : notnull
    where T : IEntity<TKey>
{
    Task CreateAsync(T entity, InsertOneOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task CreateAsync(T[] entity, InsertManyOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task UpdateAsync(T entity, ReplaceOptions? options = null, CancellationToken? cancellationToken = null);
    Task UpdateAsync(T[] entity, BulkWriteOptions? options = null, CancellationToken? cancellationToken = null);
    Task<T?> GetByIdAsync(TKey id, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task<T?> GetOneAsync(T entity, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task<T?> GetOneAsync(Expression<Func<T, bool>> filter, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task<IQueryable<T>> GetQueryableAsync(FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null);
    Task DeleteAsync(T entity, CancellationToken? cancellationToken = null);
    Task DeleteAsync(T[] entity, CancellationToken? cancellationToken = null);
    Task DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken? cancellationToken = null);
    Task DeleteAsync(FilterDefinition<T> filter, CancellationToken? cancellationToken = null);
    Task<long> CountAsync(CancellationToken? cancellationToken = null);
    Task<long> CountAsync(T entity, CancellationToken? cancellationToken = null);
    Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken? cancellationToken = null);
}