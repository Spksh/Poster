using System.IO;

namespace Poster.Core
{
    public static class PathExtensions
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static bool ContainsInvalidFileNameChars(this string path)
        {
            return path.IndexOfAny(InvalidFileNameChars) >= 0;
        }

        public static bool ContainsInvalidPathChars(this string path)
        {
            return path.IndexOfAny(InvalidPathChars) >= 0;
        }
    }
}
