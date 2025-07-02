using System.Collections.Concurrent;
using NextHave.BL.Models.Permissions;

namespace NextHave.BL.Services.Permissions
{
    public interface IPermissionsService
    {
        ConcurrentDictionary<int, Permission> Groups { get; }
    }
}