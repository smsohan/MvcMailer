namespace Mvc.Mailer
{
    public interface IPostProcessor
    {
        string Process(string body);
    }
}