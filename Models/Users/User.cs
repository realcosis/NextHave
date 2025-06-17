using NextHave.Clients;

namespace NextHave.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        
        public string? Username { get; set; }
        
        public DateTime? LastOnline { get; set; }
        
        public string? Motto { get; set; }
        
        public bool IsOnline { get; set; }

        public Client? Client { get; set; }
    }
}