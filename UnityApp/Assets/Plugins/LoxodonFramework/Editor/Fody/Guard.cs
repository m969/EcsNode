using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Fody
{
    // Token: 0x02000017 RID: 23
    [Obsolete("Not for public use")]
    public static class Guard
    {
        // Token: 0x06000065 RID: 101 RVA: 0x000029B7 File Offset: 0x00000BB7
        public static void AgainstNull(string argumentName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        // Token: 0x06000066 RID: 102 RVA: 0x000029C3 File Offset: 0x00000BC3
        public static void AgainstNullAndEmpty(string argumentName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        // Token: 0x06000067 RID: 103 RVA: 0x000029D4 File Offset: 0x00000BD4
        public static void FileExists(string argumentName, string path)
        {
            Guard.AgainstNullAndEmpty(argumentName, path);
            if (!File.Exists(path))
            {
                throw new ArgumentException("File not found. Path: " + path);
            }
        }
    }
}
