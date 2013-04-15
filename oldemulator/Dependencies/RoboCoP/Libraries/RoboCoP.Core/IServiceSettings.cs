namespace RoboCoP
{
    /// <summary>
    /// Data contract with all data required for the <see cref="Service"/> class for it's initialization.
    /// </summary>
    public interface IServiceSettings
    {
        /// <summary>
        /// Name of the service.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Endpoints to take data from via <see cref="Service.In"/> interface.
        /// </summary>
        INetworkAddress[] In { get; }

        /// <summary>
        /// Endpoints to bind <see cref="Service.Out"/> interface to.
        /// </summary>
        INetworkAddress[] Out { get; }

        /// <summary>
        /// Switch's endpoint to connect to.
        /// </summary>
        INetworkAddress Switch { get; }
    }
}