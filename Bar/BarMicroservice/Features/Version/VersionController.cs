using FooMicroservice.Client;
using Microsoft.AspNetCore.Mvc;

namespace BarMicroservice.Features.Version;

[ApiController]
[Route("[controller]")]
public class VersionController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IVersionClient _versionClient;

    public VersionController(IConfiguration configuration, IVersionClient versionClient)
    {
        _configuration = configuration;
        _versionClient = versionClient;
    }

    [HttpGet]
    public async Task<Versioning> GetVersion()
    {
        return new Versioning(
            _configuration.GetValue<string>("version"),
            await _versionClient.GetVersionAsync()
        );
    }
}

public record Versioning(
    string BarVersion,
    string FooVersion
    );
