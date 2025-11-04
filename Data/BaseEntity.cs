using MongoDBRepository.Interfaces;

namespace MongoDBRepository.Data;

public abstract class BaseEntity<TKey> : IEntity<TKey> where TKey : notnull
{
    public virtual required TKey Id { get; set; }
    public virtual TKey? TenantId { get; set; } = default;
}

public abstract class BaseEntity<TId, TTenant> : IEntity<TId, TTenant>
    where TId : notnull
    where TTenant : notnull
{
    public virtual required TId Id { get; set; }
    public virtual TTenant? TenantId { get; set; } = default;
}