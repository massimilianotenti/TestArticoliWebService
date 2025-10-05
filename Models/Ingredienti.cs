using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Models
{
    public class Ingredienti
    {
        [Key]
        public string? CodArt { get; set; }
        public string? Info { get; set; }

        public virtual Articoli? Articolo { get; set; }
    
    }
}