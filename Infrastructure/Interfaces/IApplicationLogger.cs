using System;
using Prism.Logging;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IApplicationLogger : ILoggerFacade
    {
        void Log(Exception ex, Priority priority = Priority.High);
    }
}
