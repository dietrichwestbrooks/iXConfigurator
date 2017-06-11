namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IApplicationService
    {
        string GetClipboardText();

        void SetClipboardText(string text);
    }
}
