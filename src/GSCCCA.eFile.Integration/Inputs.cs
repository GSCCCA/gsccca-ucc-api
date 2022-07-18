using System;
using System.Runtime.CompilerServices;

namespace GSCCCA.eFile.Integration
{
    internal class Inputs<T>
    {
        public static void CheckNull(T parameter, string name, [CallerMemberName] string callername = null)
        {
            if (parameter == null)
                throw new ArgumentNullException($"{name} ({typeof(T)}) at '{callername}'");
        }
        public static void CheckEmpty(T parameter, string name, [CallerMemberName] string callername = null)
        {
            string input = parameter as string;

            if (input == string.Empty)
                throw new ArgumentException($"Parameter cannot be empty.\r\n{name} ({typeof(T)}) at '{callername}'");
        }
    }
}
