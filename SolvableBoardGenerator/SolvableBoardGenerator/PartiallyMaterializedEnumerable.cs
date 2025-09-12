public class PartiallyMaterializedEnumerable<T>
{
    private List<T> MaterialzedPart = new();
    private IEnumerator<T> Rest;
    private bool HasAdvancedToRest = false;
    private bool IsEmpty = false;
    public PartiallyMaterializedEnumerable(IEnumerable<T> sequence, uint materializeQuantity)
    {
        IEnumerator<T> enumerator = sequence.GetEnumerator();
        for (int i = 0; i < materializeQuantity; i++)
        {
            if (enumerator.MoveNext())
            {
                MaterialzedPart.Add(enumerator.Current);
            }
            else
            {
                break;
            }
        }
        Rest = enumerator;
        if (materializeQuantity > 0 && MaterialzedPart.Count == 0)
        {
            IsEmpty = true;
        }
    }
    public IEnumerable<T> GetEnumerable()
    {
        if (IsEmpty) { yield break; }
        foreach (var item in MaterialzedPart)
        {
            yield return item;
        }
        if (HasAdvancedToRest)
        {
            throw new Exception($"Tried to enumerate the non materialized part of {nameof(PartiallyMaterializedEnumerable<T>)} multiple times, which isn't allowed.");
        }
        HasAdvancedToRest = true;
        while (Rest.MoveNext())
        {
            yield return Rest.Current;
        }
    }
}