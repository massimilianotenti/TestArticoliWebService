using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Models
{
    public class Articoli
    {
        [Key]
        [MinLength(5, ErrorMessage = "Il numero minimo di caratteri è 5")]
        [MaxLength(30, ErrorMessage = "Il numero massimo di caratteri è 30")]
        public string? CodArt { get; set; }

        [MinLength(5, ErrorMessage = "Il numero minimo di caratteri è 5")]
        [MaxLength(80, ErrorMessage = "Il numero massimo di caratteri è 80")]
        public string? Descrizione { get; set; }
        public string? Um { get; set; }
        public string? CodStat { get; set; }

        [Range(0, 100, ErrorMessage = "I pezzi per cartone devonoessere fra 0 e 100")]
        public Int16? PzCart { get; set; }

        [Range(0.1, 100, ErrorMessage = "Il peso deve essere tra 0.1 e 100")]
        public double? PesoNetto { get; set; }
        public int? IdIva { get; set; }
        public int? IdFamAss { get; set; }
        public string? IdStatoArt { get; set; }
        public DateTime? DataCreazione { get; set; }

        // Con Virtual si indica ad EF di fare un leazy Loadin, carica solo quando necessario
        public virtual ICollection<BarcodeEan>? Barcode { get; set; }
        public virtual Ingredienti? Ingrediente { get; set; }
        public virtual Iva? Iva { get; set; }
        public virtual FamAssort? FamAssort { get; set; }

    }
        
    
}