using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class User
    {
        public User() { }

        public User(int id, string username, string password, string role)
        {
            Id = id;
            Username = username;
            Password = password;
            Role = role;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Este campo é obrigatório")]
        [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage ="Este campo é obrigatório")]
        [MaxLength(20, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 20 caracteres.")]
        public string Password { get; set; }

        public string Role { get; set; }
        
        


    }
}