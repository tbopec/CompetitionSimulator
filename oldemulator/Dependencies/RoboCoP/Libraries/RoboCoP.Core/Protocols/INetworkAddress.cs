namespace RoboCoP
{
    /// <summary>
    /// Base interface for all classes which represents address of end-point in some network type.
    /// </summary>
    public interface INetworkAddress
    {
        /// <summary>
        /// Name of the protocol for which this address is actual.
        /// </summary>
        string ProtocolType { get; }

        /// <summary>
        /// Get user-friendly string representation of the address.
        /// </summary>
        string ToString();
    }
}