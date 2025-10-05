using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Models
{
    public class BarcodeEan
    {
        public string? CodArt { get; set; }

        [Key]
        [StringLength(13, MinimumLength = 8, ErrorMessage = "Il barcode deve essere tra 8 e 13 caratteri")]
        public string? BarCode { get; set; }

        [Required]
        public string? IdTipoArt { get; set; }
        
        public virtual Articoli? Articolo { get; set; }
    
    }
}