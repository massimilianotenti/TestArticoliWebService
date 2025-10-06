using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Mvc;
using ArticoliWebService.Models;
using ArticoliWebService.Dtos;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/articoli")]
    public class ArticoliController : Controller
    {
        // Costruttore e iniezione di dipendenza: 
        // Il controller non si preoccupa di come o da dove i dati vengono recuperati 
        // (database, file, servizio esterno). Si affida all'interfaccia (IArticoliRepository), 
        // rendendo il codice più pulito, testabile e disaccoppiato.
        //
        // Si usa un'interfaccia (come IArticoliRepository) invece di riferirsi direttamente alla 
        // sua classe concreta (ad esempio, ArticoliRepository) per promuovere la loose coupling e 
        // supportare il principio Dipendenza dall'Astrazione (Dependency Inversion Principle - D.I.P.):
        //
        // 1 - potresti decidere di passare da un'implementazione che usa Entity Framework ad una che usa Dapper,
        //     se il controller usa l'interfaccia, non devi cambiare una singola riga di codice nel controller. 
        //
        // 2 - per le Unit Test puoi creare un oggetto fittizio che implementa IArticoliRepository e simula il 
        //     comportamento desiderato (es. "restituisci 3 articoli", "simula un errore del database"). 
        //     In questo modo, testi solo la logica del controller, isolandola completamente dal database.
        //
        // 3 - L'Iniezione di Dipendenza, gestita in ASP.NET Core dal Service Container, lavora al meglio con le interfacce.
        private IArticoliRepository articoliRepository;
        public ArticoliController(IArticoliRepository articoliRepository)
        {
            this.articoliRepository = articoliRepository;
        }

        // Status Code
        // 200 Successo
        // 201 Creato
        // 204 Nessun contenuto
        //
        // 400 Bad Request
        // 401 Non autenticato
        // 402 Non autorizzato
        // 404 Not Found
        // 405 Metodo non permesso
        // 406 Non accettabile
        // 415 Non supportato
        //
        // 500 Errore server
        // 501 Non implementato
        // 502 Bad Gateway        
        // 504 Timeout
        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IAsyncEnumerable<ArticoliDto>))]
        public async Task<IActionResult> GetArticoliByDesc(string filter)
        {
            var articoliDto = new List<ArticoliDto>();
            var articoli = await this.articoliRepository.SelArticoliByDescrizione(filter);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!articoli.Any())
                return NotFound(string.Format("Nonn è stato trovato alcun articolo con la descrizione {0}", filter));

            foreach (var articolo in articoli)
            {
                var barcodeDto = this.PopolaBarcodeDt(articolo);
                var artDto = this.PopolaArticoloDt(articolo, barcodeDto);
                articoliDto.Add(artDto);
            }
            return Ok(articoliDto);
        }

        [HttpGet("cerca/codice/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoliByCode(string filter)
        {
            if (!await this.articoliRepository.ArticoloExists(filter))
                return NotFound(string.Format("Non è stato trovato l'articolo con il codice {0}", filter));

            var articolo = await this.articoliRepository.SelArticoloByCodice(filter);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var barcodeDto = this.PopolaBarcodeDt(articolo);
            var articoloDto = this.PopolaArticoloDt(articolo, barcodeDto);

            return Ok(articoloDto);
        }

        [HttpGet("cerca/ean/{filter}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoliByEan(string filter)
        {
            Console.WriteLine(">>> 1");
            var articolo = await this.articoliRepository.SelArticoloByEan(filter);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Console.WriteLine(">>> 2");
            if (articolo == null)
                return NotFound(string.Format("Non è stato trovato l'articolo con l'EAN {0}", filter));

            Console.WriteLine(">>> 3");
            var barcodeDto = this.PopolaBarcodeDt(articolo);
            Console.WriteLine(">>> 4");
            var articoloDto = this.PopolaArticoloDt(articolo, barcodeDto);

            Console.WriteLine(">>> 5");
            return Ok(articoloDto);
        }

        private List<BarcodeEanDto> PopolaBarcodeDt(Articoli articolo)
        {
            var barcodeDto = new List<BarcodeEanDto>();
            if (articolo.Barcode != null)
            {
                foreach (var barcode in articolo.Barcode)
                {
                    barcodeDto.Add(new BarcodeEanDto
                    {
                        Barcode = barcode.BarCode,
                        Tipo = barcode.IdTipoArt
                    });
                }
            }
            return barcodeDto;
        }

        private ArticoliDto PopolaArticoloDt(Articoli articolo, List<BarcodeEanDto> barcodeDto)
        {
            var articoloDto = new ArticoliDto
            {
                CodArt = articolo.CodArt,
                Descrizione = articolo.Descrizione,
                Um = articolo.Um,
                CodStat = articolo.CodStat,
                PzCart = articolo.PzCart,
                PesoNetto = articolo.PesoNetto,
                DataCreazione = articolo.DataCreazione,
                Ean = barcodeDto,
                Iva = new IvaDto(articolo.Iva.Descrizione, articolo.Iva.Aliquota),
                Categoria = articolo.FamAssort.Descrizione
            };
            return articoloDto;
        }


    }
}