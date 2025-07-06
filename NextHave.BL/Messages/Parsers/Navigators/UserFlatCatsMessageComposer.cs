using NextHave.BL.Messages.Output;
using NextHave.BL.Models.Rooms.Navigators;

namespace NextHave.BL.Messages.Parsers.Navigators
{
    public class UserFlatCatsMessageComposer(List<NavigatorCategory> navigatorCategories) : Composer(OutputCode.UserFlatCatsMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(navigatorCategories.Count);
            foreach (var category in navigatorCategories)
            {
                message.AddInt32(category.Id);
                message.AddString(category.Name!);
                message.AddBoolean(true);
                message.AddBoolean(false);
                message.AddString(category.Name!);
                if (category.Name!.StartsWith("${"))
                    message.AddString("");
                else
                    message.AddString(category.Name!);
                message.AddBoolean(false);
            }
        }
    }
}