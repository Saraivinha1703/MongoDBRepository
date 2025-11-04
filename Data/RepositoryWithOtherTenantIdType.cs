using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDBRepository.Interfaces;
using MongoDBRepository.Providers;
using MongoDBRepository.Exceptions;

namespace MongoDBRepository.Data;

public abstract class Repository<T, TId, TTenant>(
    string collectionName,
    IMongoContext context,
    UserInfoProvider userInfoProvider,
    ILogger logger
) : IRepository<T, TId, TTenant> 
    where TId : notnull
    where TTenant : notnull
    where T : IEntity<TId, TTenant>
{
    protected readonly IMongoContext _context = context;
    protected readonly IMongoCollection<T> _collection = context.GetCollection<T>(collectionName);
    protected readonly UserInfoProvider _userInfoProvider = userInfoProvider;
    protected readonly ILogger _logger = logger;

    public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            List<T> result;
            IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();
            
            if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
            {
                Expression<Func<T, bool>> tenantFilter = o => o.TenantId != null && o.TenantId.Equals(userInfo.TenantId);
                FilterDefinitionBuilder<T> builder = Builders<T>.Filter;
                FilterDefinition<T> filt = filter != null ? builder.Where(tenantFilter) & builder.Where(filter) : builder.Where(tenantFilter);
                
                result = await _collection.Find(filt, defaultOptions).ToListAsync(cancellationToken ?? CancellationToken.None);
            }
            else
            {
                result = await _collection.Find(filter ?? Builders<T>.Filter.Empty, defaultOptions).ToListAsync(cancellationToken ?? CancellationToken.None);
            }

            return result;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} GetAsync: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("read");
        }
    }

    public virtual async Task<T?> GetByIdAsync(TId id, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            T result;
            IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();

            if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
            {
                result = await _collection.Find(o => o.Id.Equals(id) && o.TenantId != null && o.TenantId.Equals(userInfo.TenantId), defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }
            else 
            {
                result = await _collection.Find(o => o.Id.Equals(id), defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }

            return result;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} GetByIdAsync: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("read");
        }
    }

    public virtual async Task<T?> GetOneAsync(T entity, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            T result;
            IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();

            if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
            {
                result = await _collection.Find(o => o.Id.Equals(entity.Id) && o.TenantId != null && o.TenantId.Equals(userInfo.TenantId), defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }
            else 
            {
                result = await _collection.Find(o => o.Id.Equals(entity.Id), defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }
            return result;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} GetOneAsync: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("read");
        }
    }

    public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>> filter, FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            T result;
            IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();

            if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
            {
                Expression<Func<T, bool>> tenantFilter = o => o.TenantId != null && o.TenantId.Equals(userInfo.TenantId);
                FilterDefinitionBuilder<T> builder = Builders<T>.Filter;
                FilterDefinition<T> filt = builder.Where(tenantFilter) & builder.Where(filter);
                
                result = await _collection.Find(filt, defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }
            else 
            {
                result = await _collection.Find(filter, defaultOptions).FirstOrDefaultAsync(cancellationToken ?? CancellationToken.None);
            }

            return result;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} GetOneAsync: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("read");
        }
    }

    public virtual Task<IQueryable<T>> GetQueryableAsync(FindOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();
            
            if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
            {
                return Task.FromResult(_collection.AsQueryable().Where(o => o.TenantId != null && o.TenantId.Equals(userInfo.TenantId)));
            }
            else
            {
                return Task.FromResult(_collection.AsQueryable());
            }
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} GetQueryableAsync: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("read");
        }
    }

    public virtual Task CreateAsync(T entity, InsertOneOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            InsertOneOptions options = defaultOptions ?? new();
            _context.AddCommand(() => _collection.InsertOneAsync(entity, options, cancellationToken ?? CancellationToken.None));
            _logger.LogInformation("One {EntityType} was created successfully!", typeof(T).FullName);
        
            return Task.CompletedTask;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} creation: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("create");
        }
    }

    public virtual Task CreateAsync(T[] entity, InsertManyOptions? defaultOptions = null, CancellationToken? cancellationToken = null)
    {
         try 
        {
            InsertManyOptions options = defaultOptions ?? new();
            _context.AddCommand(() => _collection.InsertManyAsync(entity, options, cancellationToken ?? CancellationToken.None));
            _logger.LogInformation("Many {EntityType} were created successfully!", typeof(T).FullName);
        
            return Task.CompletedTask;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} creation: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("create");
        }
    }

    public virtual Task UpdateAsync(T entity, ReplaceOptions? options = null, CancellationToken? cancellationToken = null)
    {
        try 
        {
            _context.AddCommand(() => _collection.ReplaceOneAsync(o => o.Id.Equals(entity.Id), entity, options, cancellationToken ?? CancellationToken.None));
            _logger.LogInformation("One {EntityType} updated successfully!", typeof(T).FullName);

            return Task.CompletedTask;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} update: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("update");
        }
    }

    public virtual Task UpdateAsync(T[] entity, BulkWriteOptions? options = null, CancellationToken? cancellationToken = null)
    {
         try 
        {
            List<ReplaceOneModel<T>> updates = [];
            updates = [.. entity.Select(o => new ReplaceOneModel<T>(Builders<T>.Filter.Where(r => r.Id.Equals(o.Id)), o))];
            
            _context.AddCommand(() => _collection.BulkWriteAsync(updates, options, cancellationToken: cancellationToken ?? CancellationToken.None));
            _logger.LogInformation("Many {EntityType} updated successfully!", typeof(T).FullName);
        
            return Task.CompletedTask;
        } catch(Exception ex) 
        {
            _logger.LogError("Error during {EntityType} update: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("update");
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken? cancellationToken = null)
    {
        try 
        {
            _context.AddCommand(() => _collection.DeleteOneAsync(o => o.Id.Equals(entity.Id), cancellationToken ?? CancellationToken.None));            
            _logger.LogInformation("One {EntityType} deleted successfully!", typeof(T).FullName);
            
            return Task.CompletedTask;
        } catch (Exception ex) 
        {
            _logger.LogError("Error during {EntityType} deletion: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("delete");
        }
    }

    public virtual Task DeleteAsync(T[] entity, CancellationToken? cancellationToken = null)
    {
        try 
        {
            _context.AddCommand(() => _collection.DeleteManyAsync(o => entity.Select(e => e.Id).Contains(o.Id), cancellationToken ?? CancellationToken.None));            
            _logger.LogInformation("Many {EntityType} deleted successfully!", typeof(T).FullName);

            return Task.CompletedTask;
        } catch (Exception ex) 
        {
            _logger.LogError("Error during {EntityType} deletion: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("delete");
        }
    }

    public virtual Task DeleteAsync(Expression<Func<T, bool>> filter, CancellationToken? cancellationToken = null)
    {
        try 
        {
            _context.AddCommand(() => _collection.DeleteManyAsync(filter, cancellationToken ?? CancellationToken.None));            
            _logger.LogInformation("Many or one {EntityType} deleted successfully!", typeof(T).FullName);

            return Task.CompletedTask;
        } catch (Exception ex) 
        {
            _logger.LogError("Error during {EntityType} deletion: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("delete");
        }
    }

    public virtual Task DeleteAsync(FilterDefinition<T> filter, CancellationToken? cancellationToken = null)
    {
        try 
        {
            _context.AddCommand(() => _collection.DeleteManyAsync(filter, cancellationToken ?? CancellationToken.None));            
            _logger.LogInformation("Many or one {EntityType} deleted successfully!", typeof(T).FullName);
            
            return Task.CompletedTask;
        } catch (Exception ex) 
        {
            _logger.LogError("Error during {EntityType} deletion: {Message}", typeof(T).FullName, ex.Message);
            throw new MongoDBCrudException("delete");
        }
    }
    
    public virtual async Task<long> CountAsync(CancellationToken? cancellationToken = null)
    {
         IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();
            
        if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
        {
            return await _collection.CountDocumentsAsync(o => o.TenantId != null && o.TenantId.Equals(userInfo.TenantId), cancellationToken: cancellationToken ?? CancellationToken.None);
        }
        else
        {
            return await _collection.CountDocumentsAsync(Builders<T>.Filter.Empty, cancellationToken: cancellationToken ?? CancellationToken.None);
        }
    }

    public virtual async Task<long> CountAsync(T entity, CancellationToken? cancellationToken = null)
    {
        IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();
            
        if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
        {
            return await _collection.CountDocumentsAsync(o => o.Id.Equals(entity.Id) && o.TenantId != null && o.TenantId.Equals(userInfo.TenantId), cancellationToken: cancellationToken ?? CancellationToken.None);
        }
        else
        {
            return await _collection.CountDocumentsAsync(o => o.Id.Equals(entity.Id), cancellationToken: cancellationToken ?? CancellationToken.None);
        }
    }

    public virtual async Task<long> CountAsync(Expression<Func<T, bool>> filter, CancellationToken? cancellationToken = null)
    {
        IUserInfo<TId, TTenant>? userInfo = _userInfoProvider.GetUserInformation<TId, TTenant>();
            
        if(userInfo != null && userInfo.Roles.Contains(_context.RoleToDefaultDatabase))
        {
            Expression<Func<T, bool>> tenantFilter = o => o.TenantId != null && o.TenantId.Equals(userInfo.TenantId);
            FilterDefinitionBuilder<T> builder = Builders<T>.Filter;
            FilterDefinition<T> filt = builder.Where(tenantFilter) & builder.Where(filter);

            return await _collection.CountDocumentsAsync(filt, cancellationToken: cancellationToken ?? CancellationToken.None);
        }
        else
        {
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken ?? CancellationToken.None);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
} 