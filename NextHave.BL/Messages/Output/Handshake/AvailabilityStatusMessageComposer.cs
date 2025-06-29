namespace NextHave.BL.Messages.Output.Handshake
{
    public class AvailabilityStatusMessageComposer(bool isOpen, bool isShuttingDown, bool isAuthenticHabbo) : Composer(OutputCode.AvailabilityStatusMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddBoolean(isOpen);
            message.AddBoolean(isShuttingDown);
            message.AddBoolean(isAuthenticHabbo);
        }
    }
}