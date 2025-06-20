using Dolphin.Core.API;
using Microsoft.AspNetCore.Mvc;

namespace NextHave.API.Controllers
{
    public class UsersController : DolphinController
    {
        [HttpGet]
        public async Task Get()
            => await Task.CompletedTask;
    }
}