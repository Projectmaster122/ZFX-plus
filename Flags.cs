using System.Diagnostics;

namespace ZFXplus
{
    struct Field
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
