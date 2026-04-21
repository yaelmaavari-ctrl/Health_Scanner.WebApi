using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace Health_Scanner.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AllergenController(IAllergenService allergenService) : ControllerBase
    {
        private readonly IAllergenService _service = allergenService;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AllergenCreateDto dto)
        {
            var result = await _service.Add(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AllergenDto>> Get(int id)
        {
            var result = await _service.GetById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllergenDto>>> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AllergenCreateDto dto)
        {
            var result = await _service.Update(id, dto);
            return Ok(result);
        }
    }
}
