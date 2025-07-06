using NextHave.BL.Messages.Output;
using NextHave.BL.Models.Navigators;

namespace NextHave.BL.Messages.Parsers.Navigators
{
    public class NavigatorSearchResultBlocksMessageComposer(string searchCode, string searchQuery, List<SearchResultList> resultList) : Composer(OutputCode.NavigatorSearchResultBlocksMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(searchCode);
            message.AddString(searchQuery);
            message.AddInt32(resultList.Count);

            foreach (var result in resultList)
                result.Serialize(message);
        }
    }
}