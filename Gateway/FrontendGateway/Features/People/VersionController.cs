using BarMicroservice.Client;
using Microsoft.AspNetCore.Mvc;

namespace SIS.Application.Features.People;

public class VersionController : ControllerBase
{
    private readonly BarMicroservice.Client.IVersionClient _barVersionClient;
    private readonly FooMicroservice.Client.IVersionClient _fooVersionClient;

    public VersionController(BarMicroservice.Client.IVersionClient barVersionClient,
        FooMicroservice.Client.IVersionClient fooVersionClient)
    {
        _barVersionClient = barVersionClient;
        _fooVersionClient = fooVersionClient;
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
        });
    }
}
