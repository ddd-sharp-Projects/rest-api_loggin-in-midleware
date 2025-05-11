var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// app.Use( async (context, next) =>
// {
//     Console.WriteLine("Request Path: " + context.Request.Path);
//     Console.WriteLine("Request Method: " + context.Request.Method);
//     Console.WriteLine("Request Headers: " + context.Request.Headers.ToString());
//     await next.Invoke();
//     Console.WriteLine("Response Status Code: " + context.Response.StatusCode);
// });

// app.Use(async (context, next) =>
// {
//     Console.WriteLine("Middleware 1: Before");
//     await next.Invoke();
//     Console.WriteLine("Middleware 1: After");
// });



// app.Run(async (context) =>
// {
//     Console.WriteLine("Accept Header:==> " + context.Request.Headers["Accept"]);
//     Console.WriteLine("Accept: ==>" + context.Request.Headers["Accept-Encoding"]);  
//     await context.Response.WriteAsync("Hello World! test");
//     //Console.WriteLine("Hello World!");
// });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseMiddleware<LoggingMiddleware>();
app.UseAuthorization();

app.UseHttpsRedirection();


app.Run();
