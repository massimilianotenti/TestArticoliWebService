using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticoliWebService.Dtos
{
    public class ArticoliDto
    {
        public string? CodArt { get; set; }
        public string? Descrizione { get; set; }
        public string? Um { get; set; }
        public string? CodStat { get; set; }
        public Int16? PzCart { get; set; }
        public double? PesoNetto { get; set; }
        public DateTime? DataCreazione { get; set; }
        public ICollection<BarcodeEanDto>? Ean { get; set; }
        public FamAssortDto? FamAssort { get; set; }
        public IngredientiDto? Ingredienti { get; set; }
        public IvaDto? Iva { get; set; }
        public string Categoria { get; set; }
        public string? IdStatoArticolo { get; set; }
    }

    public class BarcodeEanDto
    {
        public string? Barcode { get; set; }
        public string? Tipo { get; set; }
    }

    public class FamAssortDto
    {
        public int? Id { get; set; }
        public string? Descrizione { get; set; }
    }

    public class IngredientiDto
    {
        public string? CodArt { get; set; }
        public string? Info { get; set; }
    }

    public class IvaDto
    {
        public IvaDto(string descrizione, Int16 aliquota)
        {            
            this.Descrizione = descrizione;
            this.Aliquota = aliquota;
        }
        
        public string? Descrizione { get; set; }
        public Int16? Aliquota { get; set; }
    }

}
