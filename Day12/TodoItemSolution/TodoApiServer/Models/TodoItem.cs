using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApiServer.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "Varchar(100)")]
        public string Title { get; set; }

        public string TodoDate { get; set; }    // DateTime to string
        public int IsComplete { get; set; } //boolean to int
    }
}
