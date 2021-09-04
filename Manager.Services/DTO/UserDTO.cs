using System.Text.Json.Serialization;

namespace Manager.Services.DTO
{
    public class UserDTO
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public UserDTO() { }

        public UserDTO(ulong id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
