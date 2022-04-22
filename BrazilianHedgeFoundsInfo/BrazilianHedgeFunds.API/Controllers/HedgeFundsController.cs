using BrazilianHedgeFunds.API.Services.Dtos;
using BrazilianHedgeFunds.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BrazilianHedgeFunds.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HedgeFundsController : ControllerBase
    {
        private readonly IHedgeFundService _service;
        
        public HedgeFundsController(IHedgeFundService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetHedgeFundsBy([FromQuery] HedgeFundInDto inDto)
        {
            var outDto = await _service.GetFundsBy(inDto);

            if (outDto.IsInvalid)
                return BadRequest(outDto.ErrorMessage);

            if (outDto.HedgeFundRecords.Count == 0)
                return NotFound();

            return Ok(new { outDto.PageInfo, outDto.HedgeFundRecords });
        }
    }
}
