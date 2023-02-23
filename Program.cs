using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/domo/{ip}/{id}", Post);

app.Run();

static async Task<IResult> Post(string ip, int id, [FromQuery]int ct, [FromQuery]double bri)
{
    Console.WriteLine($"Connected ip={ip}, id={id}, ct={ct}, bri={bri}");
    try
    {
        var level = await new HttpClient()
            .GetAsync($"http://{ip}/json.htm?type=devices&rid={id}",
                new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
        Return? json = level.IsSuccessStatusCode ? await level.Content.ReadFromJsonAsync<Return>() : null;
        if (json is { status: "OK", result: [{ Data: "Off" } or { Status: "Off" }] })
            return Ok();
        var resp = await new HttpClient()
            .GetAsync($"http://{ip}/json.htm?type=command&param=setkelvinlevel&idx={id}&kelvin={ct/65/2+50}",
                new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
        if (resp is { IsSuccessStatusCode: false, StatusCode: var code })
            return StatusCode((int)code);
        Console.WriteLine("OK");
        return Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"catch {ex}");
        return Microsoft.AspNetCore.Http.Results.Problem(title: ex.GetType().FullName, detail: ex.ToString());
    }
    finally
    {
        Console.WriteLine("Disconnected");
    }
}

readonly record struct Result(string Data, string Status);
readonly record struct Return(Result[] result, string status);
