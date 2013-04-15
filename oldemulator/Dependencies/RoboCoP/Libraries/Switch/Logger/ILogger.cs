namespace Switch
{
    public interface ILogger
    {
        void Write(string message);
        void WriteLine(string format, params object[] args);
    }
}