using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;

namespace ArticoliWebService.Services
{
    public interface IArticoliRepository
    {
        Task<IEnumerable<Articoli>> SelArticoliByDescrizione(string descrizione);
        Task<Articoli?> SelArticoloByCodice(string Code);
        Task<Articoli?> SelArticoloByCodiceLight(string Code);
        Task<Articoli?> SelArticoloByEan(string Ean);
        Task<bool> InsArticoli(Articoli articolo);
        Task<bool> UpdateArticoli(Articoli articolo);
        Task<bool> DelArticoli(Articoli articolo);        
        Task<bool> ArticoloExists(string Code);
    }
}