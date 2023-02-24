public class Log<T>
{
    public DateTime DateTime { get; } = DateTime.Now;
    public required T Value { get; init; }
}
