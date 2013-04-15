namespace RoboCoP.Internal
{
    /// <summary>
    /// Interface which encapsulates interaction with the switch in RoboCoP network.
    /// </summary>
    public interface ISwitchNegotiator
    {
        /// <summary>
        /// Register the current service at the switch.
        /// This is required immediately after connecting to it.
        /// </summary>
        void RegisterService();

        /// <summary>
        /// Unregister the current service from the switch before disconnecting from it.
        /// </summary>
        void UnregistrateService();

        /// <summary>
        /// Inform the switch that current service wants to receive messages addressed to mailslot
        /// with name <paramref name="mailslotName"/>.
        /// </summary>
        void SubscribeToMailslot(string mailslotName);
    }
}