using System;

namespace Switch
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        UnknowSignalsToSwitch = 1,
        KnownSignalsToSwitch = 2,
        ErrorsToSwitch = 4,
        MessageSent = 8,
        MessageCaught = 16,
        MessageLost = 32,
        ServiceSubscribed = 64,
        ServieDisconnected = 128,
        ConnectionAdded = 256,
        ConnectionRemoved = 512
    }
}