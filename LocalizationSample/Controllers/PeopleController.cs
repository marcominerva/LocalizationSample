using LocalizationSample.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace LocalizationSample.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(string? name = null)
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();

        return NoContent();
    }

    [HttpGet("hello")]
    public IActionResult Hello(string name)
    {
        var message = string.Format(Messages.HelloMessage, name);
        return Ok(new { Message = message });
    }

    [HttpPost]
    public IActionResult Post(Person person)
        => NoContent();
}

public class Person
{
    //[Required]
    //[Display(Name = "Nome")]
    public string? FirstName { get; set; }

    //[Required]
    public string? LastName { get; set; }
}