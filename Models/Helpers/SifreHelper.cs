using System.Security.Cryptography;
using System.Text;

namespace ETicaretSitesi.Models.Helpers
{
    public static class SifreHelper
    {
        public static string HashSifre(string sifre)
        {
            return BCrypt.Net.BCrypt.HashPassword(sifre);
        }

        public static bool SifreDogrula(string girilenSifre, string hashSifre)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(girilenSifre, hashSifre);
            }
            catch
            {
                return false;
            }
        }
    }
} 