using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Mvc;
using ArticoliWebService.Models;
using ArticoliWebService.Dtos;
using AutoMapper;

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
        //
        // Quando utiliziamo il code injection impostiamo le variabili come readonly
        private readonly IArticoliRepository articoliRepository;
        private readonly IMapper mapper;

        public ArticoliController(IArticoliRepository articoliRepository, IMapper mapper)
        {
            this.articoliRepository = articoliRepository;
            this.mapper = mapper;
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

        // Gli attributi servono per dire a Swagger/OpenAPI che cosa viene restituito
        //
        // E' da usare quando si ritorna dataset molto grandi e nel return utiliziamo 
        // yield return che restituisce gli elementi mano a mano che vengono processati        
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IAsyncEnumerable<ArticoliDto>))]
        //
        // Se vieen popolato completamente un List<ArticoliDto> e lo restituisco
        // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArticoliDto>))]
        //
        //
        // Task<IActionResult> è lo standard precedente in ASP.NET Core
        // public async Task<IActionResult> GetArticoliByDesc(string filter)
        //
        // Task<ActionResult< è la pratica raccomandata per le API moderne in ASP.NET Core, semplifica il codice
        // e migliora l'integrazione con swagger. E' anche più veloce
        // public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoliByDesc(string filter)

        /// <summary>
        /// Filtra per descrizione ed ottiene una lista di articoli
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("cerca/descrizione/{filter}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ArticoliDto>))]
        public async Task<ActionResult<IEnumerable<ArticoliDto>>> GetArticoliByDesc(string filter,
            [FromQuery (Name = "cat")] string? idCat)
        {            
            var articoli = await this.articoliRepository.SelArticoliByDescrizione(filter, idCat);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!articoli.Any())
                // Per uniformare le risposte utilizziamo una classe ErrMsg
                //return NotFound(string.Format("Non è stato trovato alcun articolo con la descrizione {0}", filter));
                if(idCat != null)
                    return NotFound(new ErrMsg(string.Format("Non è stato trovato alcun articolo con la descrizione {0} e categoria {1}", filter, idCat),
                            404));
                else
                    return NotFound(new ErrMsg(string.Format("Non è stato trovato alcun articolo con la descrizione {0}", filter),
                            404));
            /*
            var articoliDto = new List<ArticoliDto>();
            foreach (var articolo in articoli)
            {
                var barcodeDto = this.PopolaBarcodeDt(articolo);
                var artDto = this.PopolaArticoloDt(articolo, barcodeDto);
                articoliDto.Add(artDto);
            }*/
            /* posso migliorarla
            var articoliDto = articoli.Select(a =>
            {
                var barcodeDto = this.PopolaBarcodeDt(a);
                return this.PopolaArticoloDt(a, barcodeDto);
            });
            */            
            var articoliDto = this.mapper.Map<IEnumerable<ArticoliDto>>(articoli);
            return Ok(articoliDto.ToList());
        }

        /// <summary>
        /// Filtra per codice ed ottiene un singolo articolo
        /// </summary>
        /// <param name="filter"></param>
        [HttpGet("cerca/codice/{filter}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoliByCode(string filter)
        {
            if (!await this.articoliRepository.ArticoloExists(filter))
                //return NotFound(string.Format("Non è stato trovato l'articolo con il codice {0}", filter));
                return NotFound(new ErrMsg(string.Format("Non è stato trovato l'articolo con il codice {0}", filter),
                        404));

            var articolo = await this.articoliRepository.SelArticoloByCodice(filter);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //var barcodeDto = this.PopolaBarcodeDt(articolo);
            //var articoloDto = this.PopolaArticoloDt(articolo, barcodeDto);
            var articoloDto = this.mapper.Map<ArticoliDto>(articolo);
            return Ok(articoloDto);
        }

        /// <summary>
        /// Filtra per codice a barre ed ottiene un singolo articolo
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("cerca/ean/{filter}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> GetArticoliByEan(string filter)
        {
            var articolo = await this.articoliRepository.SelArticoloByEan(filter);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (articolo == null)
                //return NotFound(string.Format("Non è stato trovato l'articolo con l'EAN {0}", filter));
                return NotFound(new ErrMsg(string.Format("Non è stato trovato l'articolo con l'EAN {0}", filter),
                        404));

            //var barcodeDto = this.PopolaBarcodeDt(articolo);
            //var articoloDto = this.PopolaArticoloDt(articolo, barcodeDto);
            var articoloDto = this.mapper.Map<ArticoliDto>(articolo);

            return Ok(articoloDto);
        }

        [HttpPost("inserisci")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> SaveArticoli([FromBody] Articoli articolo)
        {
            if (articolo == null)
                return BadRequest(new ErrMsg("Dati nuovo Articolo non validi", 400));

            var isPresent = await this.articoliRepository.ArticoloExists(articolo.CodArt);
            if (isPresent)
                return Conflict(new ErrMsg($"Articolo con codice {articolo.CodArt} già presente!", 409));

            if (!ModelState.IsValid)
            {
                string err = "";
                foreach (var v in ModelState.Values)
                    foreach (var e in v.Errors)
                        err += e.ErrorMessage + "|";

                return BadRequest(new ErrMsg(err, 400));
            }

            articolo.DataCreazione = DateTime.Today;

            var retVal = await this.articoliRepository.InsArticoli(articolo);
            if (!retVal)
                return StatusCode(500, new ErrMsg($"Errore durante l'inserimento dell'articolo {articolo.CodArt}", 500));

            return Ok(await this.GetArticoliByCode(articolo.CodArt));
        }

        [HttpPut("modifica")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ArticoliDto))]
        public async Task<IActionResult> UpdateArticoli([FromBody] Articoli articolo)
        {
            if (articolo == null)
                return BadRequest(new ErrMsg("Dati nuovo Articolo non validi", 400));

            if (!ModelState.IsValid)
            {
                string err = "";
                foreach (var v in ModelState.Values)
                    foreach (var e in v.Errors)
                        err += e.ErrorMessage + "|";

                return BadRequest(new ErrMsg(err, 400));
            }

            articolo.DataCreazione = DateTime.Today;

            var retVal = await this.articoliRepository.UpdateArticoli(articolo);
            if (!retVal)
                return StatusCode(500, new ErrMsg($"Errore durante la modifica dell'articolo {articolo.CodArt}", 500));

            return Ok(await this.GetArticoliByCode(articolo.CodArt));
        }

        [HttpDelete("elimina/{codart}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrMsg))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ErrMsg))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrMsg))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(InfoMsg))]
        public async Task<IActionResult> DeleteArticoli(string codart)
        {
            if (codart == "")
                return BadRequest(new ErrMsg("Codice articolo non valido", 400));
            /*if(codart == "150Test")
                return BadRequest(new ErrMsg("Codice articolo non valido", 400));*/

            Articoli articolo = await this.articoliRepository.SelArticoloByCodiceLight(codart);
            if (articolo == null)
                return NotFound(new ErrMsg($"Articolo con codice {codart} non trovato", 404));

            var retVal = await this.articoliRepository.DelArticoli(articolo);
            if (!retVal)
                return StatusCode(500, new ErrMsg($"Errore durante la cancellazione dell'articolo {articolo.CodArt}", 500));

            return Ok(new InfoMsg(DateTime.Today, $"Articolo con codice {articolo.CodArt} cancellato correttamente"));
        }

        /*
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
            //Console.WriteLine(articolo.CodArt);
            var articoloDto = new ArticoliDto
            {
                CodArt = articolo.CodArt,
                Descrizione = string.IsNullOrEmpty(articolo.Descrizione) ? "" : articolo.Descrizione.Trim(),
                Um = string.IsNullOrEmpty(articolo.Um) ? "" : articolo.Um.Trim(),
                // posso utilizzare anche articolo.CodStat?.Trim() ma se non voglio restituire un valore null devo usare string.IsNullOrEmpty
                CodStat = string.IsNullOrEmpty(articolo.CodStat) ? "" : articolo.CodStat.Trim(),
                PzCart = articolo.PzCart,
                PesoNetto = articolo.PesoNetto,
                DataCreazione = articolo.DataCreazione,
                IdStatoArticolo = string.IsNullOrEmpty(articolo.IdStatoArt) ? "" :articolo.IdStatoArt.Trim(),
                Ean = barcodeDto,
                Iva = new IvaDto(articolo.Iva.Descrizione, articolo.Iva.Aliquota),
                Categoria = articolo.FamAssort.Descrizione
            };
            return articoloDto;
        }
        */

    }
}