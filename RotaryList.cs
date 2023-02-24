using System.Collections;

public class RotaryList<T> : IEnumerable<T>
{
    private readonly T[] _list;
    private int _index = 0;

    public int Capacity { get; }
    public int Length { get; private set; } = 0;

    public RotaryList(int capacity)
    {
        Capacity = capacity;
        _list = new T[Capacity];
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return _list[(_index - Length + Capacity + i) % Capacity];
    }

    public void Add(T data)
    {
        _list[_index] = data;
        _index = (_index + 1) % Capacity;
        if (Length < Capacity)
            Length++;
    }

    IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();
}
