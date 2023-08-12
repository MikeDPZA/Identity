namespace Reception.Infrastructure.Exceptions;

public class NoProviderException: Exception
{
    public NoProviderException(): base("No provider was specified.")
    {
        
    }
}