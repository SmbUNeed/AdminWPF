using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AdminWPF
{
    public static class TokenStorage
    {
        private static readonly string Path =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AdminWPF", "token.bin");

        public static void Save(string token)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
            var encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(token), null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(Path, encrypted);
        }

        public static string? Load()
        {
            if (!File.Exists(Path)) return null;
            var decrypted = ProtectedData.Unprotect(File.ReadAllBytes(Path), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }

        public static void Clear()
        {
            if (File.Exists(Path)) File.Delete(Path);
        }
    }
}
