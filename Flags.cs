using System;

namespace ZFX
{
    public struct Field
    {
        // init bitfield for enableBoot

        public bool enableBoot;
        public int ring;
        public bool DebugMessages;

        public void Field.WriteDebug(string Message)
        {
            if (Flags.DebugMessages)
            {
                Debug.WriteLine("[DEBUG] " + Message);
            }
        }
    }

    internal class Flags
    {

    }
}
