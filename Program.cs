using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var execute = new Execute();

app.MapGet("/domo", () => execute.Brightness);
app.MapPost("/domo/{ip}/{id}", execute.Post);
app.MapPost("/domo/setLight", execute.SetLight);

app.Run();

class Execute
{
    private static string Print<T>(T value, [CallerArgumentExpression(nameof(value))]string expr = null!)
    => $"{expr}={value}";

    public int Brightness { get; private set; } = 50;
    public bool IsOn { get; private set; } = false;
    public async Task<IResult> Post(string ip, int id, [FromQuery]int ct, [FromQuery]double bri)
    {
        Brightness = (int)((1 - bri) * 50) + 50;
        Console.WriteLine($"Connected: {Print(ip)}, {Print(id)}, {Print(ct)}, {Print(bri)}, {Print(Brightness)}");
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

    public IResult SetLight([FromQuery]int value, [FromQuery]Target target)
    {
        if (target is Target.Status)
            IsOn = value != 0;
        Console.WriteLine(Print(IsOn));
        return Ok(Print(IsOn));
    }

    public enum Target
    {
        Status,
        Level,
    }
}
