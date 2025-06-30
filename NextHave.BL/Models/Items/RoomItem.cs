using NextHave.BL.Messages;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Utils;
using NextHave.DAL.Enums;

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

        public void Serialize(ServerMessage Message, int userId)
        {
            if (Base != default && Base.Type == ItemTypes.Floor)
            {
                Message.AddInt32(Id);
                Message.AddInt32(Base.SpriteId!.Value);
                Message.AddInt32(Point!.GetX);
                Message.AddInt32(Point!.GetY);
                Message.AddInt32(Rotation);
                Message.AddString(Point.GetZ.GetString());
                Message.AddString(Base.Height!.Value.GetString());
                if (Base.ItemName!.Equals("ads_mpu_720") || Base.ItemName.Equals("ads_background") || Base.ItemName.Equals("ads_mpu_300") || Base.ItemName.Equals("ads_mpu_160"))
                {
                    Message.AddInt32(0);
                    Message.AddInt32(1);
                    var array = ExtraData!.Split(Convert.ToChar(9)).ToArray();
                    if (ExtraData != "" && array.Length > 1)
                    {
                        Message.AddInt32(unchecked(array.Length / 2));
                        for (int i = 0; i <= array.Length - 1; i++)
                        {
                            Message.AddString(array[i]);
                        }
                    }
                    else
                    {
                        Message.AddInt32(0);
                    }
                }
                else if (Base.ItemName.Equals("boutique_mannequin1"))
                {
                    Message.AddInt32(0);
                    Message.AddInt32(1);
                    Message.AddInt32(3);
                    Message.AddString("GENDER");
                    Message.AddString("m");
                    if (ExtraData!.Contains('\n'))
                    {
                        Message.AddString("FIGURE");
                        Message.AddString(ExtraData.Split('\n')[0]);
                        Message.AddString("OUTFIT_NAME");
                        Message.AddString(ExtraData.Split('\n')[1]);
                    }
                    else
                    {
                        Message.AddString("FIGURE");
                        Message.AddString(ExtraData);
                        Message.AddString("OUTFIT_NAME");
                        Message.AddString("Il mio look");
                    }
                }
                else
                {
                    Message.AddInt32(0);
                    if (Base.ItemName.Contains("badge_display"))
                    {
                        if (!string.IsNullOrWhiteSpace(ExtraData) && ExtraData.Split(Convert.ToChar(1)).Length >= 3)
                        {
                            var array5 = ExtraData.Split(Convert.ToChar(1)).ToArray();
                            Message.AddInt32(2);
                            Message.AddInt32(4);
                            Message.AddString("0");
                            Message.AddString((array5[0] != "") ? array5[0] : "BR011");
                            Message.AddString((array5[1] != "") ? array5[1] : "FalseStyle:3");
                            Message.AddString((array5[2] != "") ? array5[2] : (DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year));
                        }
                        else
                        {
                            Message.AddInt32(2);
                            Message.AddInt32(4);
                            Message.AddString("0");
                            Message.AddString("BR011");
                            Message.AddString("FalseStyle:3");
                            Message.AddString(DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
                        }
                    }
                    else if (CurrentStack > 0)
                    {
                        Message.AddString("");
                        Message.AddBoolean(value: true);
                        Message.AddBoolean(value: false);
                        Message.AddString(ExtraData!);
                        Message.AddInt32(CurrentStack);
                        Message.AddInt32(TotalStack);
                    }
                    else
                    {
                        Message.AddInt32(0);
                        Message.AddString(ExtraData!);
                    }
                }
                Message.AddInt32(-1);
                Message.AddInt32(Base.InteractionCount!.Value);
                Message.AddInt32(userId);
                Message.AddInt32(RareValue);
                Message.AddBoolean(ValidRare);
                Message.AddString(PaymentReason ?? string.Empty);
            }
        }
    }
}