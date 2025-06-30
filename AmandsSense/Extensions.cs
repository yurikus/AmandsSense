using System.IO;

namespace AmandsSense;

public static class Extensions
{
    public static string CreateDirectory(this string path)
    {
        var di = new DirectoryInfo(path);
        di.Create();
        return di.FullName;
    }

}
