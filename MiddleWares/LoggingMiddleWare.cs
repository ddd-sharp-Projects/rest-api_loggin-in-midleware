using System.Text;
using System.Text.Json;
using Common.Classes;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next; 
    

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string uid = Guid.NewGuid().ToString();

        context.Request.EnableBuffering();

        context.Request.Body.Position = 0;
        var requestBody = await ReadStreamAsync(context.Request.Body);
        context.Request.Body.Position = 0;

        LogFile.Log("uid: " + uid + " | Request=> Method: " + context.Request.Method + " | Path: " + context.Request.Path + "| Body: " + requestBody);
       
        // Hold original response body stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        // Call the next middleware in the pipeline
        await _next(context);
        
        // Log Response
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        //context.Response.Body.Seek(0, SeekOrigin.Begin);

        object modifiedResponse;

        if (IsJson(responseText))
        {
            // Parse existing JSON and add UID
            using var document = JsonDocument.Parse(responseText);
            var root = document.RootElement;

            var dictionary = new Dictionary<string, object>();

            foreach (var property in root.EnumerateObject())
            {
                dictionary[property.Name] = property.Value.Deserialize<object>();
            }

            dictionary["uid"] = uid; // Add UID field

            modifiedResponse = dictionary;
        }
        else
        {
            // If not JSON, wrap in a new object
            modifiedResponse = new { uid, data = responseBody };
        }

        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";

        var newResponseBody = JsonSerializer.Serialize(modifiedResponse);
        await context.Response.WriteAsync(newResponseBody);


        LogFile.Log("uid: " + uid + " | Response=> " + newResponseBody);

        // await responseBody.CopyToAsync(originalBodyStream);

    }
    private async Task<string> ReadStreamAsync(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var text = await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return text;
    }
    private bool IsJson(string input)
    {
        input = input?.Trim();
        return (input.StartsWith("{") && input.EndsWith("}")) || (input.StartsWith("[") && input.EndsWith("]"));
    }
}