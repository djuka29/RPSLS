using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;

namespace RPSLS.Infrastructure.LoggingMiddleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;

    public LoggingMiddleware(RequestDelegate next, string logFilePath)
    {
        _next = next;
        _logFilePath = logFilePath;
    }

    public async Task Invoke(HttpContext context)
    {
        // Log request
        context.Request.EnableBuffering();
        var requestBody = "";
        if (context.Request.ContentLength > 0)
        {
            context.Request.Body.Position = 0;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Check if the endpoint is login or register and filter password
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.Contains("/login") || path.Contains("/register")))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(requestBody);
                    var root = jsonDoc.RootElement;

                    using var stream = new MemoryStream();
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        writer.WriteStartObject();
                        foreach (var property in root.EnumerateObject())
                        {
                            if (property.NameEquals("password"))
                            {
                                writer.WriteString("password", "***REDACTED***");
                            }
                            else
                            {
                                property.WriteTo(writer);
                            }
                        }
                        writer.WriteEndObject();
                    }
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                catch
                {
                    // TO DO:
                    // If parsing fails, fallback to original requestBody
                }
            }
        }

        var requestLog = $"[{DateTime.Now}] Request: {context.Request.Method} {context.Request.Path} {requestBody}";
        await File.AppendAllTextAsync(_logFilePath, requestLog + Environment.NewLine);

        // Log response
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseLog = $"[{DateTime.Now}] Response: {context.Response.StatusCode} {responseText}";
        await File.AppendAllTextAsync(_logFilePath, responseLog + Environment.NewLine);

        await responseBody.CopyToAsync(originalBodyStream);
    }
}
