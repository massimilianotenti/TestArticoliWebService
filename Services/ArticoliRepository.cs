using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public class ArticoliRepository : IArticoliRepository
    {
        // Iniezione delle dipendenze
        AlphaShopDbContext dbContext;
        public ArticoliRepository(AlphaShopDbContext alphaShopDbContext)
        {
            this.dbContext = alphaShopDbContext;
        }

        public IEnumerable<Articoli> SelArticoliByDescrizione(string descrizione)
        {
            return this.dbContext.Articoli
                .Where(a => a.Descrizione!.Contains(descrizione))
                .OrderBy(a => a.Descrizione)
                .ToList();
        }

        public Articoli SelArticoloByCodice(string Code)
        {
            return this.dbContext.Articoli
                .Where(a => a.CodArt!.Equals(Code))                
                .FirstOrDefault();
        }

        public Articoli SelArticoloByEna(string Ean)
        {
            return this.dbContext.BarcodeEans
                .Where(b => b.BarCode!.Equals(Ean))
                .Select(b => b.Articolo)
                .FirstOrDefault();
        }

        public bool InsArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }

        public bool UpdateArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }

        public bool DelArticoli(Articoli articolo)
        {
            throw new NotImplementedException();
        }

        public bool Salva()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ArticoloExists(string Code)
        {
            throw new NotImplementedException();
        }
        
    }
}