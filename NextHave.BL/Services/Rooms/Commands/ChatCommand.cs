using NextHave.BL.Clients;

namespace NextHave.BL.Services.Rooms.Commands
{
    public abstract class ChatCommand : IChatCommand
    {
        public abstract string? Key { get; }

        public abstract string[] OtherKeys { get; }

        public abstract string? Description { get; }

        public abstract string? Usage { get; }

        public string[] Parameters { get; set; } = [];

        async Task IChatCommand.Execute(Client client)
            => await Handle(client);

        protected abstract Task Handle(Client client);
    }
}