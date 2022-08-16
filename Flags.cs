using System.Diagnostics;

namespace ZFX
{
    public struct Field
    {
        // init bitfield for enableBoot

        public bool enableBoot;
        public bool DebugMessages;

        public void WriteDebug(string Message)
        {
            if (DebugMessages)
            {
                Debug.WriteLine("[DEBUG] " + Message);
            }
        }
    }
}
