using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace Quanta.Forms.Http;

public static class CorsHelper
{
    public static void AddCorsHeaders(HttpResponseData response, string allowedOrigin)
    {
        response.Headers.Add("Access-Control-Allow-Origin", allowedOrigin);
        response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
    }

    public static HttpResponseData ErrorResponse(HttpRequestData req, string allowedOrigin, HttpStatusCode status, string message)
    {
        var res = req.CreateResponse(status);
        AddCorsHeaders(res, allowedOrigin);
        res.Headers.Add("Content-Type", "application/json");
        res.WriteString(JsonSerializer.Serialize(new { success = false, message }));
        return res;
    }
}
