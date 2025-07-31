using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ubiminds.Application.Commands.ConvertToXml;
using Ubiminds.Domain.Models;
using Ubiminds.Domain.Models.InputModels;

namespace Ubiminds.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class XmlController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Convert([FromBody] DocumentInputModel data)
    {
        var result = await mediator.Send(new ConvertToXmlCommand(data));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}