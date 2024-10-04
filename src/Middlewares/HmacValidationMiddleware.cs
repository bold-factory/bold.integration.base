using System.Security.Cryptography;
using System.Text;

namespace Bold.Integration.Base.Middlewares;

public class HmacValidationMiddleware(RequestDelegate next, string secretKey)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next(context);
            return;
        }
        // Extract signature from the header
        if (!context.Request.Headers.TryGetValue(key: "X-HMAC-Signature", value: out var receivedSignature))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing HMAC signature.");
            return;
        }

        context.Request.EnableBuffering();

        using (var reader = new StreamReader(stream: context.Request.Body, encoding: Encoding.UTF8, leaveOpen: true))
        {
            var requestBody = await reader.ReadToEndAsync();

            // Compute the expected HMAC signature
            var computedSignature = GenerateHmacSignature(payload: requestBody, secret: secretKey);

            // Reset the body stream position for further processing
            context.Request.Body.Position = 0;

            // Compare the provided signature with the computed one
            if (!CryptographicOperations.FixedTimeEquals(left: Encoding.UTF8.GetBytes(receivedSignature!),
                                                         right: Encoding.UTF8.GetBytes(computedSignature)))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid HMAC signature.");
                return;
            }
        }

        await next(context);
    }
    private static string GenerateHmacSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hashBytes).Replace(oldValue: "-", newValue: "").ToLower(); // Matching the sender's signature format
    }
}