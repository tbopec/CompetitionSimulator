using System;
namespace RoboCoP
{

    public enum ServiceStates
    {
        /// <summary>
        /// Before <see cref="ServiceApp{T}"/> initialization.
        /// </summary>
        ServiceAppInitStart,
        /// <summary>
        /// Parsing configuration file to <see cref="ServiceSettings"/> object.
        /// </summary>
        ConfigParsing,
        /// <summary>
        /// Before <see cref="Service"/> initialization.
        /// </summary>
        ServiceInitStart,
        /// <summary>
        /// Initializing <see cref="Service.Out"/> list using <see cref="IServiceSettings.Out"/> list.
        /// </summary>
        OutListInit,
        /// <summary>
        /// Initializing <see cref="Service.In"/> list using <see cref="IServiceSettings.In"/> list.
        /// </summary>
        InListInit,
        /// <summary>
        /// Initializing <see cref="Service.Com"/> using <see cref="IServiceSettings.Switch"/> data.
        /// Waiting for switch and registration.
        /// </summary>
        CommanderInit,
        /// <summary>
        /// After initialization.
        /// </summary>
        ServiceReady
    }
}