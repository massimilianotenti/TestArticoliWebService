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
        Task<Articoli> SelArticoloByCodice(string Code);
        Task<Articoli> SelArticoloByEan(string Ean);
        bool InsArticoli(Articoli articolo);
        bool UpdateArticoli(Articoli articolo);
        bool DelArticoli(Articoli articolo);
        bool Salva();
        Task<bool> ArticoloExists(string Code);
    }
}