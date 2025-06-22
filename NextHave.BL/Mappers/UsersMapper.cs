using Dolphin.Core.Extensions;
using NextHave.BL.Extensions;
using NextHave.BL.Localizations;
using NextHave.BL.Models.Users;
using NextHave.DAL.Enums;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    public static class UsersMapper
    {
        public static User? MapResult(this UserEntity userEntity)
        {
            return new User
            {
                Id = userEntity.Id,
                IsOnline = userEntity.Online,
                LastOnline = userEntity.LastOnline,
                Motto = userEntity.Motto,
                Rank = userEntity.Rank!.Value,
                Gender = userEntity.Gender!.Value.GetDescription<EnumsDescriptions>(),
                Look = userEntity.Look,
                Username = userEntity.Username
            };
        }

        public static UserEntity MapRegistration(this UserRegistrationWrite userRegistration, string? registrationIp, string? hotelName, string? defaultLook)
            => new()
            {
                AccountCreated = DateTime.Now,
                CurrentIp = registrationIp,
                Gender = GenderTypes.Male,
                Mail = userRegistration.Mail,
                Motto = $"I Love {hotelName}!♥️",
                Volume = "100,100,100,100",
                Look = defaultLook,
                Password = userRegistration.Password?.HashPassword(),
                Online = false,
                Rank = 1,
                RegistrationIp = registrationIp,
                Username = userRegistration.Username
            };
    }
}