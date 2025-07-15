namespace NextHave.BL.Messages.Output.Navigators
{
    public class TableAlertMessageComposer(List<string> columns, Dictionary<int, Dictionary<int, string>> rows) : Composer(OutputCode.TableAlertMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(columns.Count);
            foreach (var column in columns)
                message.AddString(column);

            message.AddInt32(rows.Count);
            foreach (var row in rows)
            {
                message.AddInt32(row.Key);
                message.AddInt32(row.Value.Count);
                foreach (var value in row.Value)
                {
                    message.AddInt32(value.Key);
                    message.AddString(value.Value);
                }
            }
        }
    }
}