using NextHave.BL.Clients;

namespace NextHave.BL.Services.Rooms.Commands
{
    public interface IChatCommand
    {
        string? Key { get; }

        string[] OtherKeys { get; }

        string? Description { get; }

        string? Usage { get; }

        string[] Parameters { get; set; }

        Task Execute(Client client);
    }
}