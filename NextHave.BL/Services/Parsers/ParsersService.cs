using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Messages;
using NextHave.BL.Messages.Input;
using NextHave.BL.Messages.Input.Handshake;
using NextHave.BL.Messages.Input.Messenger;
using NextHave.BL.Messages.Input.Navigators;
using NextHave.BL.Messages.Input.Rooms;
using NextHave.BL.Messages.Input.Rooms.Chat;
using NextHave.BL.Messages.Input.Rooms.Engine;
using NextHave.BL.Messages.Parsers;
using NextHave.BL.Messages.Parsers.Handshake;
using NextHave.BL.Messages.Parsers.Navigators;
using NextHave.BL.Messages.Parsers.Rooms.Chat;
using NextHave.BL.Messages.Parsers.Rooms.Connection;
using NextHave.BL.Messages.Parsers.Rooms.Rooms;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Parsers
{
    [Service(ServiceLifetime.Singleton)]
    class ParsersService : IParsersService, IStartableService
    {
        readonly ConcurrentDictionary<short, IParser> parsers = [];

        async Task IStartableService.StartAsync()
        {
            RegisterDefaultParsers();
            await Task.CompletedTask;
        }

        bool IParsersService.TryGetParser(short header, out IParser? parser)
        {
            if (parsers.TryGetValue(header, out var p))
            {
                parser = p;
                return true;
            }
            parser = default;
            return false;
        }

        void IParsersService.UpsertParser(short header, IParser parser)
        {
            if (parsers.ContainsKey(header))
                parsers[header] = parser;
            else
                parsers.TryAdd(header, parser);
        }

        public void RegisterDefaultParsers()
        {
            parsers.TryAdd(InputCode.SSOTicketMessageEvent, new SSOTicketMessageParser());

            parsers.TryAdd(InputCode.MoveObjectMessageEvent, new MoveObjectParser());

            parsers.TryAdd(InputCode.InfoRetrieveMessageEvent, new BaseParser<InfoRetrieveMessage>());

            parsers.TryAdd(InputCode.StartTypingMessageEvent, new BaseParser<StartTypingMessage>());

            parsers.TryAdd(InputCode.StopTypingMessageEvent, new BaseParser<StopTypingMessage>());

            parsers.TryAdd(InputCode.GetRoomEntryDataMessageEvent, new BaseParser<GetRoomEntryDataMessage>());

            parsers.TryAdd(InputCode.GetFurnitureAliasesMessageEvent, new BaseParser<GetFurnitureAliasesMessage>());

            parsers.TryAdd(InputCode.MoveAvatarMessageEvent, new MoveAvatarParser());

            parsers.TryAdd(InputCode.ChatMessageEvent, new ChatMessageParser());

            parsers.TryAdd(InputCode.ShoutMessageEvent, new ShoutMessageParser());

            parsers.TryAdd(InputCode.OpenFlatConnectionMessageEvent, new OpenFlatParser());

            parsers.TryAdd(InputCode.GetGuestRoomMessageEvent, new GetGuestRoomParser());

            parsers.TryAdd(InputCode.NewNavigatorSearchMessageEvent, new NewNavigatorSearchParser());

            parsers.TryAdd(InputCode.NewNavigatorInitMessageEvent, new BaseParser<NewNavigatorInitMessage>());

            parsers.TryAdd(InputCode.GetUserFlatCatsMessageEvent, new BaseParser<GetUserFlatCatsMessage>());

            parsers.TryAdd(InputCode.GoToHotelViewMessageEvent, new BaseParser<GoToHotelViewMessage>());

            parsers.TryAdd(InputCode.MessengerInitMessageEvent, new BaseParser<MessengerInitMessage>());
        }
    }
}