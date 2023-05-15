using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smarthomeAPI.Services.EnvironmentService;

namespace smarthomeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnvironmentController : ControllerBase
    {

        private readonly IEnvironmentService _envService;

        public EnvironmentController(IEnvironmentService envService)
        {
            _envService = envService;
        }

        [HttpPost("")]
        public Task<IActionResult> CreateEnvironment(EnvironmentRegisterRequest request)
        {
            return _envService.CreateEnvironment(request);
        }

        [HttpDelete("")]
        public Task<IActionResult> DeleteEnvironment(EnvironmentDeleteRequest request)
        {
            return _envService.DeleteEnvironment(request);
        }

        [HttpGet("")]
        public EnvironmentList[] GetAllValues()
        {
            return _envService.GetAllEnvironments();
        }

        [HttpGet("Token")]
        public IActionResult GetEnvToken(int envID)
        {
            return _envService.GetEnvToken(envID);
        }

        /*
        [HttpGet("get-Environments")]
        public List<EnvironmentType> GetEnvTokens()
        {
            return _envService.GetEnvStrings();
        }
        */

        [HttpGet("Devices")]
        public Device[] GetDevices()
        {
            return _envService.GetDevices();
        }
    }
}
