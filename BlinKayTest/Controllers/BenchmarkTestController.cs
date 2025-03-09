using BlinKayTest.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BlinKayTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenchmarkTestController : ControllerBase
    {
        private readonly IBenchmarkService _benchmarkService;

        public BenchmarkTestController(IBenchmarkService benchmarkService)
        {
            _benchmarkService = benchmarkService;
        }

        [HttpPost("pgInsertion")]
        public async Task<IActionResult> PGInsertion (int numRegistries, int numThreads)
        {
            var result = await _benchmarkService.PGInsertion(numRegistries, numThreads);
            return Ok(result);
        }

        [HttpPost("pgSelectPlusUpdate")]
        public async Task<IActionResult> PGSelectPlusUpdate(int numRegistries, int numThreads)
        {
            var result = await _benchmarkService.PGSelectPlusUpdate(numRegistries, numThreads);
            return Ok(result);
        }

        [HttpPost("pgSelectPlusUpdatePlusInsertion")]
        public async Task<IActionResult> PGSelectPlusUpdatePlusInsertion(int numRegistries, int numThreads)
        {
            var result = await _benchmarkService.PGSelectPlusUpdatePlusInsertion(numRegistries, numThreads);
            return Ok(result);
        }
    }
}
