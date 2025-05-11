using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        //Console.WriteLine("Hello World! from Controller");
        return Ok("Hello World! from Controller");
    }

    [HttpPost]
    public IActionResult Post([FromBody] CreateCustomerDto createCustomer)
    {
        //Console.WriteLine("Hello World! from Controller");
        return Ok(createCustomer);
    }

}