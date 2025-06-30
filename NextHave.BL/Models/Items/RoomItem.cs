using NextHave.BL.Utils;
using NextHave.DAL.Enums;
using NextHave.BL.Messages;
using NextHave.BL.Services.Items.Interactions;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Models.Items
{
    public class RoomItem
    {
        public IRoomInstance? RoomInstance { get; set; }

        public int Id { get; set; }

        public int RoomId { get; set; }

        public int BaseItem { get; set; }

        public ThreeDPoint? Point { get; set; }

        public int Rotation { get; set; }

        public string? WallPosition { get; set; }

        public string? ExtraData { get; set; }

        public int RareValue { get; set; }

        public bool ValidRare { get; set; }

        public string? PaymentReason { get; set; }

        public int TotalStack { get; set; }

        public int CurrentStack { get; set; }

        public ItemDefinition? Base { get; set; }

        public bool IsWired
            => Base!.InteractionType!.Value.IsWired();

        public bool IsCondition
            => Base!.InteractionType!.Value.IsCondition();

        public void Serialize(ServerMessage message, int userId)
        {
            if (RoomInstance == default)
                return;

            if (Base != default && Base.Type == ItemTypes.Floor)
            {
                message.AddInt32(Id);
                message.AddInt32(Base.SpriteId!.Value);
                message.AddInt32(Point!.GetX);
                message.AddInt32(Point!.GetY);
                message.AddInt32(Rotation);
                message.AddString(Point.GetZ.GetString());
                message.AddString(Base.Height!.Value.GetString());
                if (Base.ItemName!.Equals("ads_mpu_720") || Base.ItemName.Equals("ads_background") || Base.ItemName.Equals("ads_mpu_300") || Base.ItemName.Equals("ads_mpu_160"))
                {
                    message.AddInt32(0);
                    message.AddInt32(1);
                    var array = ExtraData!.Split(Convert.ToChar(9)).ToArray();
                    if (ExtraData != "" && array.Length > 1)
                    {
                        message.AddInt32(unchecked(array.Length / 2));
                        for (int i = 0; i <= array.Length - 1; i++)
                        {
                            message.AddString(array[i]);
                        }
                    }
                    else
                    {
                        message.AddInt32(0);
                    }
                }
                else if (Base.InteractionType!.Value.Equals(InteractionTypes.RecordsBoard) || Base.InteractionType!.Value.Equals(InteractionTypes.WonGamesRecordsBoard))
                {
                    message.AddInt32(1);
                    message.AddInt32(6);
                    message.AddString(ExtraData!);
                    message.AddInt32(2);
                    message.AddInt32(1);
                    message.AddInt32(0); // count
                        //Message.AddInt32(scoresRecords.Item2); x count
                        //Message.AddInt32(1);
                        //Message.AddString(scoresRecords.Item1)
                }
                else if (Base.ItemName.Equals("boutique_mannequin1"))
                {
                    message.AddInt32(0);
                    message.AddInt32(1);
                    message.AddInt32(3);
                    message.AddString("GENDER");
                    message.AddString("m");
                    if (ExtraData!.Contains('\n'))
                    {
                        message.AddString("FIGURE");
                        message.AddString(ExtraData.Split('\n')[0]);
                        message.AddString("OUTFIT_NAME");
                        message.AddString(ExtraData.Split('\n')[1]);
                    }
                    else
                    {
                        message.AddString("FIGURE");
                        message.AddString(ExtraData);
                        message.AddString("OUTFIT_NAME");
                        message.AddString("Il mio look");
                    }
                }
                else if (Base.ItemName == "roombg_color")
                {
                    message.AddInt32(0);
                    message.AddInt32(5);
                    message.AddInt32(4);
                    message.AddInt32(RoomInstance.Toner!.Enabled ? 1 : 0);
                    message.AddInt32(RoomInstance.Toner!.Hue);
                    message.AddInt32(RoomInstance.Toner!.Saturation);
                    message.AddInt32(RoomInstance.Toner!.Brightness);
                }
                else
                {
                    message.AddInt32(0);
                    if (Base.ItemName.Contains("badge_display"))
                    {
                        if (!string.IsNullOrWhiteSpace(ExtraData) && ExtraData.Split(Convert.ToChar(1)).Length >= 3)
                        {
                            var array5 = ExtraData.Split(Convert.ToChar(1)).ToArray();
                            message.AddInt32(2);
                            message.AddInt32(4);
                            message.AddString("0");
                            message.AddString((array5[0] != "") ? array5[0] : "BR011");
                            message.AddString((array5[1] != "") ? array5[1] : "FalseStyle:3");
                            message.AddString((array5[2] != "") ? array5[2] : (DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year));
                        }
                        else
                        {
                            message.AddInt32(2);
                            message.AddInt32(4);
                            message.AddString("0");
                            message.AddString("BR011");
                            message.AddString("FalseStyle:3");
                            message.AddString(DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                        }
                    }
                    else if (CurrentStack > 0)
                    {
                        message.AddString("");
                        message.AddBoolean(value: true);
                        message.AddBoolean(value: false);
                        message.AddString(ExtraData!);
                        message.AddInt32(CurrentStack);
                        message.AddInt32(TotalStack);
                    }
                    else
                    {
                        message.AddInt32(0);
                        if (!Base.InteractionType.Value.Equals(InteractionTypes.FBGate))
                            message.AddString(ExtraData!);
                        else
                            message.AddString(string.Empty);
                    }
                }
                message.AddInt32(-1);
                message.AddInt32(Base.InteractionCount!.Value);
                message.AddInt32(userId);
                message.AddInt32(RareValue);
                message.AddBoolean(ValidRare);
                message.AddString(PaymentReason ?? string.Empty);
            }
        }
    }
}