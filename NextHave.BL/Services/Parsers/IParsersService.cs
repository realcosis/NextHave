using NextHave.BL.Messages;

namespace NextHave.BL.Services.Parsers
{
    public interface IParsersService
    {
        bool TryGetParser(short header, out IParser? parser);

        void UpsertParser(short header, IParser parser);
    }
}