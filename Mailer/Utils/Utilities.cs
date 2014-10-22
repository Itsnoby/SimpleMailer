#region Usages

using System;
using System.IO;
using System.Reflection;

#endregion

namespace SimpleMailer.Mailer.Utils
{
    class Utilities
    {
        static public string GetAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static void MakeCheck<T>(Predicate<T> p, T checkObject, string message)
        {
            if (!p.Invoke(checkObject))
                throw new Exception(message);
        }

        public static void MakeCheckNotNull(object checkObject, string message)
        {
            Predicate<object> p = PNotNull;
            MakeCheck(p, checkObject, message);
        }

        private static bool PNotNull(object o)
        {
            return (o != null);
        }
    }
}
