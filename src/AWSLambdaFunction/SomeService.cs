public class SomeService : ISomeService
{
    public string ToUpperCase(string text)
    {
        Console.WriteLine("Inside the DIed service");
        return text.ToUpper();
    }
}

public interface ISomeService
{
    string ToUpperCase(string text);
}

