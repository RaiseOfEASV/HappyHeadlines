
namespace Feature.Flags;

public class ConfigProfanity
{
    private bool isProfanityEnabled;
    
    public ConfigProfanity(bool isProfanityEnabled)
    {
        this.isProfanityEnabled = isProfanityEnabled;
    }

    public bool IsProfanityEnabled
    {
        get => isProfanityEnabled;
    }
    
}