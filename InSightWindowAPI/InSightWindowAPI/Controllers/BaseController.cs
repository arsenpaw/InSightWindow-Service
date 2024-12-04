using InSightWindowAPI.Exeptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using InSightWindowAPI.Enums;

namespace InSightWindowAPI.Controllers;

public abstract class BaseController: ControllerBase
{
    protected virtual Guid UserId => Guid.Parse(_getFromClaims(ClaimTypes.NameIdentifier)!);
    protected virtual string Role => _getFromClaims(ClaimTypes.Role, false) ?? UserRoles.USER;
    protected virtual string Email => _getFromClaims(ClaimTypes.Email)!;
    
    private  string? _getFromClaims(string key, bool strictPolicy = true)
    {
        var claim = User.FindFirst(key);
        if (strictPolicy && (claim == null || string.IsNullOrEmpty(claim.Value)))
        {
            throw new AppException($"{key} claim not found", HttpStatusCode.Unauthorized);
        }
        return claim?.Value;
    }

}