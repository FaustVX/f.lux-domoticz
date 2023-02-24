using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var domo = new Domoticz();

app.MapGet("/domo", () => domo);
app.MapPost("/domo/{ip}/{id}", domo.SetBrightness);
app.MapPost("/domo/setLight", domo.SetLight);

app.Run();

class Domoticz
{
    private static string Print<T>(T value, [CallerArgumentExpression(nameof(value))]string expr = null!)
    => $"{expr}={value}";

    public int Brightness { get; private set; } = 50;
    public bool IsOn { get; private set; } = false;
    public Task<IResult> SetBrightness(string ip, int id, [FromQuery]int ct, [FromQuery]double bri)
    {
        Brightness = (int)((1 - bri) * 50) + 50;
        return SetBrightness(ip, id);
    }

    private async Task<IResult> SetBrightness(string ip, int id)
    {
        Console.WriteLine($"Connected: {Print(ip)}, {Print(id)}, {Print(Brightness)}");
        try
        {
            if (!IsOn)
                return Ok();
            var resp = await new HttpClient()
                .GetAsync($"http://{ip}/json.htm?type=command&param=setkelvinlevel&idx={id}&kelvin={Brightness}",
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

    public async Task<IResult> SetLight([FromQuery]int value, [FromQuery]Target target, [FromQuery]int id)
    {
        var previous = IsOn;
        if (target is Target.Status)
            IsOn = value != 0;
        Console.WriteLine(Print(IsOn));
        if (!previous && IsOn)
            return await SetBrightness("raspiplex.lan:90", id);
        return Ok(Print(IsOn));
    }

    public enum Target
    {
        Status,
        Level,
    }
}
