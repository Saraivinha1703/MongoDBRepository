namespace MongoDBRepository.Interfaces;

public interface IUserInfo<TKey>
{
    TKey Sub { get; set; }
    TKey TenantId { get; set; }
    string[] Roles { get; set; }
    string? Jwt { get; set; }
}

public interface IUserInfo<TSub, TTenant>
{
    TSub Sub { get; set; }
    TTenant TenantId { get; set; }
    string[] Roles { get; set; }
    string? Jwt { get; set; }
}