using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        #region Articoli Selezione
        public async Task<IEnumerable<Articoli>> SelArticoliByDescrizione(string descrizione, string? idCat)
        {
            // .Where(a => a.Descrizione!.Contains(descrizione))
            // equivale a dire "Caro compilatore, vedo che mi stai avvisando che 
            // a.Descrizione potrebbe essere null e che chiamare .Contains() 
            // potrebbe causare un errore. Non preoccuparti, ti garantisco io che in 
            // questo punto del codice non sarà mai null."
            //
            // Con questa piccola modifica, la tua query ora si legge: "trova tutti gli articoli 
            // dove la descrizione esiste e contiene il testo cercato". Questo previene 
            // completamente il rischio di crash a runtime.
            // .Where(a => a.Descrizione != null && a.Descrizione.Contains(descrizione))
            // In sintesi, l'operatore ! è uno strumento da usare con estrema cautela, solo quando 
            // sei assolutamente certo al 100% che un valore non possa essere null. Nella maggior 
            // parte dei casi, un controllo esplicito != null è la scelta migliore per un codice 
            // robusto e professionale.                

            var query = this.dbContext.Articoli
                .Where(a => a.Descrizione != null && a.Descrizione.Contains(descrizione));

            if (!string.IsNullOrWhiteSpace(idCat) && int.TryParse(idCat, out int catId))
                query = query.Where(a => a.IdFamAss == catId);

            return await query
                .OrderBy(a => a.Descrizione)
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .ToListAsync();
        }        

        public async Task<Articoli?> SelArticoloByCodice(string Code)
        {
            return await this.dbContext.Articoli
                .Where(a => a.CodArt == Code)
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Articoli?> SelArticoloByCodiceLight(string Code)
        {
            return await this.dbContext.Articoli
                .Where(a => a.CodArt == Code)
                .FirstOrDefaultAsync();
        }

        public async Task<Articoli?> SelArticoloByEan(string Ean)
        {
            return await this.dbContext.Articoli
                .Include(b => b.Barcode)
                .Include(c => c.FamAssort)
                .Include(d => d.Ingrediente)
                .Include(e => e.Iva)
                .Where(a => a.Barcode != null && a.Barcode.Any(b => b.BarCode == Ean))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ArticoloExists(string Code)
        {
            return await this.dbContext.Articoli
                .AnyAsync(c => c.CodArt == Code);
        }

        #endregion

        #region Articoli Variazioni
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
        
        #endregion

        #region Tabelle Selezione

        public async Task<IEnumerable<Iva>> SelIva()
        {
            return await this.dbContext.Iva
                .OrderBy(i => i.Aliquota)
                .ToListAsync();
        }

        public async Task<IEnumerable<FamAssort>> SelFamAssort()
        {
            return await this.dbContext.FamAssorts
                .OrderBy(f => f.Descrizione)
                .ToListAsync();
        }

        #endregion
        
    }
}