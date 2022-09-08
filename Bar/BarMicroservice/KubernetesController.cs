using Microsoft.AspNetCore.Mvc;

namespace BarMicroservice;

public class KubernetesController : ControllerBase
{
    [HttpGet("Bridge")]
    public IActionResult Bridge(string header)
    {
        Response.Headers["kubernetes-route-as"] = header;
        return Redirect(Request.PathBase);
    }

}
