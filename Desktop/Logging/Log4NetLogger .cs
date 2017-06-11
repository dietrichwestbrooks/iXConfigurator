using System;
using log4net;
using Microsoft.Practices.ServiceLocation;
using Prism.Logging;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Desktop.Logging
{
    public class Log4NetLogger : IApplicationLogger
    {
        #region Fields

        // Member variables
        private ILog _logger = LogManager.GetLogger(typeof(Log4NetLogger));

        #endregion

        public Log4NetLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        #region ILoggerFacade Members

        /// <summary>
        /// Writes a log message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The message category.</param>
        /// <param name="priority">Not used by Log4Net; pass Priority.None.</param>
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    _logger.Debug(message);
                    break;
                case Category.Warn:
                    _logger.Warn(message);
                    break;
                case Category.Exception:
                    _logger.Error(message);
                    break;
                case Category.Info:
                    _logger.Info(message);
                    break;
            }
        }

        public void Log(Exception ex, Priority priority = Priority.High)
        {
            if (ex.InnerException != null)
            {
                Log(ex.InnerException, priority);
            }

            var settings = ServiceLocator.Current.GetInstance<IApplicationSettings>();

            if (settings.IsAdminMode)
            {
                Log($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}", Category.Exception, priority);
            }
            else
            {
                Log($"{ex.GetType().Name}: {ex.Message}", Category.Exception, priority);
            }
        }

        #endregion
    }
}
