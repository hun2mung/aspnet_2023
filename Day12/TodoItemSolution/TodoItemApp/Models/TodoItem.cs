using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TodoItemApp.Models
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
