using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using System.Reflection.Metadata;

namespace smarthomeAPI.Services.EnvironmentService
{
    public interface IEnvironmentService
    {

        Task<IActionResult> CreateEnvironment(EnvironmentRegisterRequest request);

        Task<IActionResult> DeleteEnvironment(EnvironmentDeleteRequest request);

        IActionResult GetEnvToken(int envID);

        List<EnvironmentType> GetEnvStrings();

        EnvironmentList[] GetAllEnvironments();

        Device[] GetDevices();

    }
}
