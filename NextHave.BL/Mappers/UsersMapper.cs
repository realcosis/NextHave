using Dolphin.Core.Extensions;
using NextHave.BL.Extensions;
using NextHave.BL.Localizations;
using NextHave.BL.Models.Users;
using NextHave.BL.Models.Users.Messenger;
using NextHave.DAL.Enums;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    public static class UsersMapper
    {
        public static User MapResult(this UserEntity userEntity)
            => new()
            {
                Id = userEntity.Id,
                IsOnline = userEntity.Online,
                LastOnline = userEntity.LastOnline,
                Motto = userEntity.Motto,
                Rank = userEntity.Rank!.Value,
                Gender = userEntity.Gender!.Value.GetDescription<EnumsDescriptions>(),
                Look = userEntity.Look,
                Username = userEntity.Username,
                HomeRoom = userEntity.HomeRoom
            };

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

        public static MessengerBuddy MapReceiver(this MessengerFriendshipEntity friend)
          => friend.GetMap<MessengerFriendshipEntity, MessengerBuddy>((dest => dest.Look!, src => src.ReceiverUser!.Look!),
                                                                      (dest => dest.Motto!, src => src.ReceiverUser!.Motto!),
                                                                      (dest => dest.UserId!, src => src.ReceiverUser!.Id!),
                                                                      (dest => dest.Username!, src => src.ReceiverUser!.Username!),
                                                                      (dest => dest.Online!, src => src.ReceiverUser!.Online!));

        public static MessengerBuddy MapSender(this MessengerFriendshipEntity friend)
            => friend.GetMap<MessengerFriendshipEntity, MessengerBuddy>((dest => dest.Look!, src => src.SenderUser!.Look!),
                                                                        (dest => dest.Motto!, src => src.SenderUser!.Motto!),
                                                                        (dest => dest.UserId!, src => src.SenderUser!.Id!),
                                                                        (dest => dest.Username!, src => src.SenderUser!.Username!),
                                                                        (dest => dest.Online!, src => src.SenderUser!.Online!));

        public static MessengerRequest MapSender(this MessengerRequestEntity request)
            => request.GetMap<MessengerRequestEntity, MessengerRequest>((dest => dest.FromUser, src => src.Sender),
                                                                        (dest => dest.ToUser, src => src.Receiver),
                                                                        (dest => dest.Username!, src => src.SenderUser!.Username!),
                                                                        (dest => dest.Look!, src => src.SenderUser!.Look!));
    }
}