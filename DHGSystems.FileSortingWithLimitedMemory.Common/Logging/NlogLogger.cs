using NLog;


namespace DHGSystems.FileSortingWithLimitedMemory.Common.Logging
{
    public class NlogLogger : IDhgSystemsLogger
    {
        private readonly Logger _nlogLogger;

        public NlogLogger()
        {
            _nlogLogger = LogManager.GetCurrentClassLogger();
        }

        public void Debug(string message)
        {
            _nlogLogger.Debug(message);
        }

        public void Debug(string serviceId, string className, string message)
        {
            this.Debug(serviceId, className, "", message);
        }

        public void Debug(string serviceId, string className, string functionName, string message)
        {
            _nlogLogger.Debug($"Service ID {serviceId} || {className}  ||  {functionName}  ||  {message}");
        }

        public void Error(string message)
        {
            _nlogLogger.Error(message);
        }

        public void Error(string serviceId, string className, string message)
        {
            this.Error(serviceId, className, "", message);
        }

        public void Error(string serviceId, string className, string functionName, string message)
        {
            _nlogLogger.Error($"Service ID {serviceId} || {className}  ||  {functionName}  ||  {message}");
        }

        public void Error(string serviceId, string className, string functionName, string message, Exception ex)
        {
            _nlogLogger.Error($"Service ID {serviceId} || {className}  ||  {functionName}  ||  {message}  || {ex.Message} ||  {ex.InnerException} || {ex.StackTrace} ");
        }

        public void Error(string className, string message)
        {
            _nlogLogger.Error($"{className} ||  {message}");
        }

        public void Info(string message)
        {
            _nlogLogger.Info(message);
        }

        public void Info(string serviceId, string className, string message)
        {
            this.Info(serviceId, className, "", message);
        }

        public void Info(string serviceId, string className, string functionName, string message)
        {
            _nlogLogger.Info($"Service ID {serviceId} || {className}  ||  {functionName}  ||  {message}");
        }

        public void Info(string className, string message)
        {
            _nlogLogger.Info($"{className} ||  {message}");
        }

        public void LogJsonObject(string jsonMessage)
        {
            _nlogLogger.Error(jsonMessage);
        }

        public void Trace(string message)
        {
            _nlogLogger.Trace(message);
        }

        public void Trace(string providerId, string className, string message)
        {
            this.Trace(providerId, className, "", message);
        }

        public void Trace(string providerId, string className, string functionName, string message)
        {
            _nlogLogger.Trace($"Service ID {providerId} || {className}  ||  {functionName}  ||  {message}");
        }

        public void Trace(string className, string message)
        {
            _nlogLogger.Trace($"{className} ||  {message}");
        }

        public void Warn(string message)
        {
            _nlogLogger.Warn(message);
        }

        public void Warn(string serviceId, string className, string message)
        {
            this.Warn(serviceId, className, "", message);
        }

        public void Warn(string serviceId, string className, string functionName, string message)
        {
            _nlogLogger.Warn($"Service ID {serviceId} || {className}  ||  {functionName}  ||  {message}");
        }

        public void Warn(string className, string message)
        {
            _nlogLogger.Warn($"{className} ||  {message}");
        }
    }
}
