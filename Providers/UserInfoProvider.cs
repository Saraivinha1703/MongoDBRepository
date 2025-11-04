using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MongoDBRepository.Interfaces;

namespace MongoDBRepository.Providers;

public class UserInfoProvider(IHttpContextAccessor httpContextAccessor, ILogger logger)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger _logger = logger;

    public IUserInfo<TKey>? GetUserInformation<TKey>()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        string? bearerToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization;

        if (bearerToken is null)
        {
            _logger.LogInformation("no jwt to get the information was found in the HTTP context.");
            return null;
        }


        string payload = bearerToken.Split(" ")[1].Split(".")[1];
        byte[] bytePayload = WebEncoders.Base64UrlDecode(payload);
        string json = Encoding.UTF8.GetString(bytePayload);

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        IUserInfo<TKey> userInfo = JsonSerializer.Deserialize<IUserInfo<TKey>>(json, jsonOptions) ?? throw new JsonException("something went wrong while trying to deserialize the token payload");
        userInfo.Jwt = bearerToken;

        return userInfo;
    }

    public IUserInfo<TSub, TTenant>? GetUserInformation<TSub, TTenant>()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        string? bearerToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization;

        if (bearerToken is null)
        {
            _logger.LogInformation("no jwt to get the information was found in the HTTP context.");
            return null;
        }


        string payload = bearerToken.Split(" ")[1].Split(".")[1];
        byte[] bytePayload = WebEncoders.Base64UrlDecode(payload);
        string json = Encoding.UTF8.GetString(bytePayload);

        JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        IUserInfo<TSub, TTenant> userInfo = JsonSerializer.Deserialize<IUserInfo<TSub, TTenant>>(json, jsonOptions) ?? throw new JsonException("something went wrong while trying to deserialize the token payload");
        userInfo.Jwt = bearerToken;

        return userInfo;
    }
    
     public string? GetUserJwt()
    {
        if(_httpContextAccessor.HttpContext is null) {
            return null;
        }

        string? bearerToken = _httpContextAccessor.HttpContext.Request.Headers.Authorization;

        if(bearerToken is null) 
        {
            _logger.LogInformation("no jwt to get the information was found in the HTTP context.");
            return null;
        }
        
        return bearerToken.Split(" ")[1];
    }
}