using NextHave.BL.Clients;

namespace NextHave.BL.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        
        public string? Username { get; set; }
        
        public DateTime? LastOnline { get; set; }
        
        public string? Motto { get; set; }
        
        public bool IsOnline { get; set; }

        public string? Look { get; set; }

        public int Rank { get; set; }

        public string? Gender { get; set; }

        public Client? Client { get; set; }
    }
}