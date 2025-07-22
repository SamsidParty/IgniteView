using System.Diagnostics;
using System.Text;

namespace IgniteView.FileDialogs;

internal class Helpers
{
    internal static readonly Encoder Encoder = Encoding.UTF8.GetEncoder();

    internal static unsafe int GetNullTerminatedStringLength(byte* nullTerminatedString)
    {
        int count = 0;
        var ptr = nullTerminatedString;
        while (*ptr != 0)
        {
            ptr++;
            count++;
        }

        return count;
    }

    internal static unsafe string FromUtf8(byte* nullTerminatedString)
    {
        return Encoding.UTF8.GetString(nullTerminatedString, GetNullTerminatedStringLength(nullTerminatedString));
    }

    internal static unsafe byte[] ToUtf8(string s)
    {
        var byteCount = Encoding.UTF8.GetByteCount(s);
        var bytes = new byte[byteCount + 1];
        fixed (byte* o = bytes)
        fixed (char* input = s)
        {
            Encoder.Convert(input, s.Length, o, bytes.Length, true, out _, out var _, out var completed);
            Debug.Assert(completed);
        }

        return bytes;
    }
}