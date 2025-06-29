namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class FurnitureAliasesMessageComposer() : Composer(OutputCode.FurnitureAliasesMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(0);
        }
    }
}