using NextHave.BL.Messages.Output;

namespace NextHave.BL.Messages.Output.Navigators
{
    public class CollapsedCategoriesMessageComposer() : Composer(OutputCode.CollapsedCategoriesMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(46);
            message.AddString("new_ads");
            message.AddString("friend_finding");
            message.AddString("staffpicks");
            message.AddString("with_friends");
            message.AddString("with_rights");
            message.AddString("query");
            message.AddString("recommended");
            message.AddString("my_groups");
            message.AddString("favorites");
            message.AddString("history");
            message.AddString("top_promotions");
            message.AddString("campaign_target");
            message.AddString("friends_rooms");
            message.AddString("groups");
            message.AddString("metadata");
            message.AddString("history_freq");
            message.AddString("highest_score");
            message.AddString("competition");
            message.AddString("category__Agencies");
            message.AddString("category__Role Playing");
            message.AddString("category__Global Chat & Discussi");
            message.AddString("category__GLOBAL BUILDING AND DE");
            message.AddString("category__global party");
            message.AddString("category__global games");
            message.AddString("category__global fansite");
            message.AddString("category__global help");
            message.AddString("category__Trading");
            message.AddString("category__global personal space");
            message.AddString("category__Habbo Life");
            message.AddString("category__TRADING");
            message.AddString("category__global official");
            message.AddString("category__global trade");
            message.AddString("category__global reviews");
            message.AddString("category__global bc");
            message.AddString("category__global personal space");
            message.AddString("eventcategory__Hottest Events");
            message.AddString("eventcategory__Parties & Music");
            message.AddString("eventcategory__Role Play");
            message.AddString("eventcategory__Help Desk");
            message.AddString("eventcategory__Trading");
            message.AddString("eventcategory__Games");
            message.AddString("eventcategory__Debates & Discuss");
            message.AddString("eventcategory__Grand Openings");
            message.AddString("eventcategory__Friending");
            message.AddString("eventcategory__Jobs");
            message.AddString("eventcategory__Group Events");
        }
    }
}