using NextHave.BL.Messages.Output;

namespace NextHave.BL.Messages.Input.Navigators
{
    public class NavigatorMetaDataMessageComposer() : Composer(OutputCode.NavigatorMetaDataMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(3);
            message.AddString("official_view");
            message.AddInt32(0);
            message.AddString("hotel_view");
            message.AddInt32(0);
            message.AddString("myworld_view");
            message.AddInt32(0);
        }
    }
}