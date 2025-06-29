namespace NextHave.BL.Messages
{
    public abstract class Composer(short id)
    {
        public ServerMessage Write()
        {
            var message = new ServerMessage(id);
            message.Init();
            Compose(message);

            return message;
        }

        public abstract void Compose(ServerMessage message);
    }
}