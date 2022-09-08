using Microsoft.AspNetCore.Mvc;

namespace SIS.Application.Features.People;

[ApiController]
[Route("[controller]")]
public class VersionController : ControllerBase
{
    private readonly BarMicroservice.Client.IVersionClient _barVersionClient;
    private readonly FooMicroservice.Client.IVersionClient _fooVersionClient;
    private readonly IConfiguration _configuration;

    public VersionController(BarMicroservice.Client.IVersionClient barVersionClient,
        FooMicroservice.Client.IVersionClient fooVersionClient,
        IConfiguration configuration)
    {
        _barVersionClient = barVersionClient;
        _fooVersionClient = fooVersionClient;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetVersions()
    {
        var barVersionTask = _barVersionClient.GetVersionAsync();
        var fooVersionTask = _fooVersionClient.GetVersionAsync();

        await Task.WhenAll(barVersionTask, fooVersionTask);

        return Ok(new
        {
            BarVersion = barVersionTask.Result,
            FooVersion = fooVersionTask.Result,
            GatewayVersion = _configuration.GetValue<string>("version")
        });
    }
}
