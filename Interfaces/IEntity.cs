namespace MongoDBRepository.Interfaces;

public interface IEntity<TKey> where TKey : notnull
{
    TKey Id { get; set; }

    TKey? TenantId { get; set; }
}

public interface IEntity<TId, TTenant> 
    where TId : notnull 
    where TTenant : notnull
{
    TId Id { get; set; }

    TTenant? TenantId { get; set; }
}