namespace NextHave.DAL.Mongo.Entities.Items
{
    public class ItemLimitedMongoEntity
    {
        public int TotalStack { get; set; }

        public int CurrentStack { get; set; }
    }
}