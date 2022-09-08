using Microsoft.AspNetCore.Mvc;

namespace FooMicroservice.Features.Version;

[ApiController]
[Route("[controller]")]
public class VersionController : Controller
{
    private readonly IConfiguration _configuration;

    public VersionController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public string GetVersion()
    {
        return _configuration.GetValue<string>("version");
    }
}
