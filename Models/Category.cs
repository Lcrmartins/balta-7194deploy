using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema; para o caso das datas anotations para o esquema do bd.

namespace Shop.Models
{

    // [Table("Category")] para especificar o nome da tabela
    public class Category
    {
        
        public Category() { }
        public Category(int id, string title)
        {
            Id = id;
            Title = title;
        }

        [Key]
        // [Column("Cat_ID")] para especificar o nome da coluna
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres.")]
        //[DataType("nvarchar")] para especificar o tipo de dados
        public string Title { get; set; }
    }
}