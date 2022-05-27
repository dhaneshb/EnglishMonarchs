namespace EnglishMonarchs
{
    public interface IConfigurationManager
    {
        public string GetMonarchSourceUrl();
    }

    public abstract class ConfigurationManagerBase : IConfigurationManager
    {
        public abstract string GetMonarchSourceUrl();
    }

    public class ConfigurationManager : ConfigurationManagerBase
    {
        public override string GetMonarchSourceUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["MonarchSourceUrl"];
        }
    }
}
