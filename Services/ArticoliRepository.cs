using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Articoli>> SelArticoliByDescrizione(string descrizione)
        {
            return await this.dbContext.Articoli
                .Where(a => a.Descrizione!.Contains(descrizione))
                .OrderBy(a => a.Descrizione)
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .ToListAsync();
        }

        public async Task<Articoli> SelArticoloByCodice(string Code)
        {
            return await this.dbContext.Articoli
                .Where(a => a.CodArt!.Equals(Code))
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Articoli> SelArticoloByCodiceLight(string Code)
        {
            return await this.dbContext.Articoli
                .Where(a => a.CodArt!.Equals(Code))
                .FirstOrDefaultAsync();
        }

        public async Task<Articoli> SelArticoloByEan(string Ean)
        {            
            return await this.dbContext.Articoli
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .Where(a => a.Barcode.Any(b => b.BarCode.Equals(Ean)))
                .FirstOrDefaultAsync();
    
        }

        public async Task<bool> InsArticoli(Articoli articolo)
        {
            await this.dbContext.AddAsync(articolo);
            return await this.Salva();
        }

        public async Task<bool> UpdateArticoli(Articoli articolo)
        {
            this.dbContext.Update(articolo);
            return await this.Salva();
        }

        public async Task<bool> DelArticoli(Articoli articolo)
        {
            this.dbContext.Remove(articolo);
            return await this.Salva();
        }

        private async Task<bool> Salva()
        {
            var saved = await dbContext.SaveChangesAsync();
            return saved >= 0 ? true : false;
        }

        public async Task<bool> ArticoloExists(string Code)
        {
            return await this.dbContext.Articoli
                .AnyAsync(c => c.CodArt == Code);
        }
        
    }
}