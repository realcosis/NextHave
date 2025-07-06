namespace NextHave.BL.Messages.Parsers
{
    public class BaseParser<T> : AbstractParser<T> where T : IInput, new()
    {
        public sealed override IInput Parse(ClientMessage packet)
            => new T();
    }
}