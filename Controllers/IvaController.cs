using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ArticoliWebService.Dtos;

namespace ArticoliWebService.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/iva")]  
    public class IvaController: Controller
    {
        private readonly IArticoliRepository articoliRepository;
        private readonly IMapper mapper;

        public IvaController(IArticoliRepository articoliRepository, IMapper mapper)
        {
            this.articoliRepository = articoliRepository;
            this.mapper = mapper;
        }

        [HttpGet("cerca")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IvaDto>))]
        public async Task<ActionResult<IEnumerable<IvaDto>>> SelIva()
        {
            var iva = await this.articoliRepository.SelIva();
            // a verifica !ModelState.IsValid non è necessaria per un'azione GET che non 
            // riceve parametri complessi da un body. ModelState viene usato principalmente 
            // per validare i dati in ingresso da richieste POST o PUT
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            if (!iva.Any())
                return NotFound(new ErrMsg("Non è stato trovato alcun iva", 404));

            var ivaDto = this.mapper.Map<IEnumerable<IvaDto>>(iva);
            return Ok(ivaDto.ToList());            
        }


    }
}