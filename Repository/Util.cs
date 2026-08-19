using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace RENT_MVC_PROJECT.Repository
{
    public class Util  
    {


   private static DecryptEncrypt.Data skey = new DecryptEncrypt.Data("al12");

  
        public string Decrypt(string hexstream)
        {
            try
            {
                var sym = new DecryptEncrypt.Symmetric(DecryptEncrypt.Symmetric.Provider.Rijndael);
                var encryptedData = new DecryptEncrypt.Data();
                encryptedData.Hex = hexstream;
                DecryptEncrypt.Data decryptedData;
                decryptedData = sym.Decrypt(encryptedData, skey);
                return decryptedData.ToString();
            }
            catch
            { 
            }

            return default;
        }


        public string Encrypt(string text)
        {
                var sym = new DecryptEncrypt.Symmetric(DecryptEncrypt.Symmetric.Provider.Rijndael);
                DecryptEncrypt.Data encryptedData;
                encryptedData = sym.Encrypt(new DecryptEncrypt.Data(text), skey);
                return encryptedData.ToHex();
            }
        }

    }

