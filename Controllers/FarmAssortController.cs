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
    [Route("api/farmassort")]
    public class FarmAssortController : Controller
    {
        private readonly IArticoliRepository articoliRepository;
        private readonly IMapper mapper;

        public FarmAssortController(IArticoliRepository articoliRepository, IMapper mapper)
        {
            this.articoliRepository = articoliRepository;
            this.mapper = mapper;
        }

        [HttpGet("cerca")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FamAssortDto>))]
        public async Task<ActionResult<IEnumerable<FamAssortDto>>> SelFamAssort()
        {
            var famAssort = await this.articoliRepository.SelFamAssort();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!famAssort.Any())
                return NotFound(new ErrMsg("Non è stato trovato alcun FamAssort", 404));

            var famAssortDto = this.mapper.Map<IEnumerable<FamAssortDto>>(famAssort);
            return Ok(famAssortDto.ToList());
        }

    }

}