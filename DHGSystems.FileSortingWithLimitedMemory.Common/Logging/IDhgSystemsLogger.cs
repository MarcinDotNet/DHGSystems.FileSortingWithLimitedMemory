namespace DHGSystems.FileSortingWithLimitedMemory.Common.Logging
{
    public interface IDhgSystemsLogger
    {
        public void Error(string message);

        public void Error(string className, string message);

        public void Error(string serviceId, string className, string message);

        public void Error(string serviceId, string className, string functionName, string message);

        public void Error(string serviceId, string className, string functionName, string message, Exception ex);

        public void Warn(string message);

        public void Warn(string className, string message);

        public void Warn(string serviceId, string className, string message);

        public void Warn(string serviceId, string className, string functionName, string message);

        public void Trace(string message);

        public void Trace(string className, string message);

        public void Trace(string serviceId, string className, string message);

        public void Trace(string serviceId, string className, string functionName, string message);

        public void Info(string message);

        public void Info(string className, string message);

        public void Info(string serviceId, string className, string message);

        public void Info(string serviceId, string className, string functionName, string message);

        public void Debug(string message);

        public void Debug(string serviceId, string className, string message);

        public void Debug(string serviceId, string className, string functionName, string message);

        public void LogJsonObject(string jsonMessage);
    }
}