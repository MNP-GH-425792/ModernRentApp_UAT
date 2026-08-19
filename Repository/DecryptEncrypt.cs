using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Configuration;
using RENT_MVC_PROJECT.Controllers;
using System.Security.Policy;

namespace RENT_MVC_PROJECT.Repository
{
    public class DecryptEncrypt
    {


        #region   Hash

        /// <summary>
        /// Hash functions are fundamental to modern cryptography. These functions map binary 
        /// strings of an arbitrary length to small binary strings of a fixed length, known as 
        /// hash values. A cryptographic hash function has the property that it is computationally
        /// infeasible to find two distinct inputs that hash to the same value. Hash functions 
        /// are commonly used with digital signatures and for data integrity.
        /// </summary>
        public partial class Hash
        {

            /// <summary>
            /// Type of hash; some are security oriented, others are fast and simple
            /// </summary>
            public enum Provider
            {
                /// <summary>
                /// Cyclic Redundancy Check provider, 32-bit
                /// </summary>
                CRC32,
                /// <summary>
                /// Secure Hashing Algorithm provider, SHA-1 variant, 160-bit
                /// </summary>
                SHA1,
                /// <summary>
                /// Secure Hashing Algorithm provider, SHA-2 variant, 256-bit
                /// </summary>
                SHA256,
                /// <summary>
                /// Secure Hashing Algorithm provider, SHA-2 variant, 384-bit
                /// </summary>
                SHA384,
                /// <summary>
                /// Secure Hashing Algorithm provider, SHA-2 variant, 512-bit
                /// </summary>
                SHA512,
                /// <summary>
                /// Message Digest algorithm 5, 128-bit
                /// </summary>
                MD5
            }

            private HashAlgorithm _Hash;

            private Data _HashValue = new Data();

            private Hash()
            {
            }

            /// <summary>
            /// Instantiate a new hash of the specified type
            /// </summary>
            public Hash(Provider p)
            {
                switch (p)
                {
                    case Provider.CRC32:
                        {
                            _Hash = new CRC32();
                            break;
                        }
                    case Provider.MD5:
                        {
                            _Hash = new MD5CryptoServiceProvider();
                            break;
                        }
                    case Provider.SHA1:
                        {
                            _Hash = new SHA1Managed();
                            break;
                        }
                    case Provider.SHA256:
                        {
                            _Hash = new SHA256Managed();
                            break;
                        }
                    case Provider.SHA384:
                        {
                            _Hash = new SHA384Managed();
                            break;
                        }
                    case Provider.SHA512:
                        {
                            _Hash = new SHA512Managed();
                            break;
                        }
                }
            }

            /// <summary>
            /// Returns the previously calculated hash
            /// </summary>
            public Data Value
            {
                get
                {
                    return _HashValue;
                }
            }

            /// <summary>
            /// Calculates hash on a stream of arbitrary length
            /// </summary>
            public Data Calculate(ref Stream s)
            {
                _HashValue.Bytes = _Hash.ComputeHash(s);
                return _HashValue;
            }

            /// <summary>
            /// Calculates hash for fixed length <see cref="Data"/>
            /// </summary>
            public Data Calculate(Data d)
            {
                return CalculatePrivate(d.Bytes);
            }

            /// <summary>
            /// Calculates hash for a string with a prefixed salt value. 
            /// A "salt" is random data prefixed to every hashed value to prevent 
            /// common dictionary attacks.
            /// </summary>
            public Data Calculate(Data d, Data salt)
            {
                var nb = new byte[(d.Bytes.Length + salt.Bytes.Length)];
                salt.Bytes.CopyTo(nb, 0);
                d.Bytes.CopyTo(nb, salt.Bytes.Length);
                return CalculatePrivate(nb);
            }

            /// <summary>
            /// Calculates hash for an array of bytes
            /// </summary>
            private Data CalculatePrivate(byte[] b)
            {
                _HashValue.Bytes = _Hash.ComputeHash(b);
                return _HashValue;
            }

            #region   CRC32 HashAlgorithm
            private partial class CRC32 : HashAlgorithm
            {

                private int result = int.MinValue + 0x7FFFFFFF;

                protected override void HashCore(byte[] array, int ibStart, int cbSize)
                {
                    int lookup;
                    for (int i = ibStart, loopTo = cbSize - 1; i <= loopTo; i++)
                    {
                        lookup = result & 0xFF ^ array[i];
                        result = (result & int.MinValue + 0x7FFFFF00) / 0x100 & 0xFFFFFF;
                        result = result ^ crcLookup[lookup];
                    }
                }

                protected override byte[] HashFinal()
                {
                    byte[] b = BitConverter.GetBytes(~result);
                    Array.Reverse(b);
                    return b;
                }

                public override void Initialize()
                {
                    result = int.MinValue + 0x7FFFFFFF;
                }

                private int[] crcLookup = new int[] { 0x0, 0x77073096, int.MinValue + 0x6E0E612C, int.MinValue + 0x190951BA, 0x76DC419, 0x706AF48F, int.MinValue + 0x6963A535, int.MinValue + 0x1E6495A3, 0xEDB8832, 0x79DCB8A4, int.MinValue + 0x60D5E91E, int.MinValue + 0x17D2D988, 0x9B64C2B, 0x7EB17CBD, int.MinValue + 0x67B82D07, int.MinValue + 0x10BF1D91, 0x1DB71064, 0x6AB020F2, int.MinValue + 0x73B97148, int.MinValue + 0x04BE41DE, 0x1ADAD47D, 0x6DDDE4EB, int.MinValue + 0x74D4B551, int.MinValue + 0x03D385C7, 0x136C9856, 0x646BA8C0, int.MinValue + 0x7D62F97A, int.MinValue + 0x0A65C9EC, 0x14015C4F, 0x63066CD9, int.MinValue + 0x7A0F3D63, int.MinValue + 0x0D080DF5, 0x3B6E20C8, 0x4C69105E, int.MinValue + 0x556041E4, int.MinValue + 0x22677172, 0x3C03E4D1, 0x4B04D447, int.MinValue + 0x520D85FD, int.MinValue + 0x250AB56B, 0x35B5A8FA, 0x42B2986C, int.MinValue + 0x5BBBC9D6, int.MinValue + 0x2CBCF940, 0x32D86CE3, 0x45DF5C75, int.MinValue + 0x5CD60DCF, int.MinValue + 0x2BD13D59, 0x26D930AC, 0x51DE003A, int.MinValue + 0x48D75180, int.MinValue + 0x3FD06116, 0x21B4F4B5, 0x56B3C423, int.MinValue + 0x4FBA9599, int.MinValue + 0x38BDA50F, 0x2802B89E, 0x5F058808, int.MinValue + 0x460CD9B2, int.MinValue + 0x310BE924, 0x2F6F7C87, 0x58684C11, int.MinValue + 0x41611DAB, int.MinValue + 0x36662D3D, 0x76DC4190, 0x1DB7106, int.MinValue + 0x18D220BC, int.MinValue + 0x6FD5102A, 0x71B18589, 0x6B6B51F, int.MinValue + 0x1FBFE4A5, int.MinValue + 0x68B8D433, 0x7807C9A2, 0xF00F934, int.MinValue + 0x1609A88E, int.MinValue + 0x610E9818, 0x7F6A0DBB, 0x86D3D2D, int.MinValue + 0x11646C97, int.MinValue + 0x66635C01, 0x6B6B51F4, 0x1C6C6162, int.MinValue + 0x056530D8, int.MinValue + 0x7262004E, 0x6C0695ED, 0x1B01A57B, int.MinValue + 0x0208F4C1, int.MinValue + 0x750FC457, 0x65B0D9C6, 0x12B7E950, int.MinValue + 0x0BBEB8EA, int.MinValue + 0x7CB9887C, 0x62DD1DDF, 0x15DA2D49, int.MinValue + 0x0CD37CF3, int.MinValue + 0x7BD44C65, 0x4DB26158, 0x3AB551CE, int.MinValue + 0x23BC0074, int.MinValue + 0x54BB30E2, 0x4ADFA541, 0x3DD895D7, int.MinValue + 0x24D1C46D, int.MinValue + 0x53D6F4FB, 0x4369E96A, 0x346ED9FC, int.MinValue + 0x2D678846, int.MinValue + 0x5A60B8D0, 0x44042D73, 0x33031DE5, int.MinValue + 0x2A0A4C5F, int.MinValue + 0x5D0D7CC9, 0x5005713C, 0x270241AA, int.MinValue + 0x3E0B1010, int.MinValue + 0x490C2086, 0x5768B525, 0x206F85B3, int.MinValue + 0x3966D409, int.MinValue + 0x4E61E49F, 0x5EDEF90E, 0x29D9C998, int.MinValue + 0x30D09822, int.MinValue + 0x47D7A8B4, 0x59B33D17, 0x2EB40D81, int.MinValue + 0x37BD5C3B, int.MinValue + 0x40BA6CAD, int.MinValue + 0x6DB88320, int.MinValue + 0x1ABFB3B6, 0x3B6E20C, 0x74B1D29A, int.MinValue + 0x6AD54739, int.MinValue + 0x1DD277AF, 0x4DB2615, 0x73DC1683, int.MinValue + 0x63630B12, int.MinValue + 0x14643B84, 0xD6D6A3E, 0x7A6A5AA8, int.MinValue + 0x640ECF0B, int.MinValue + 0x1309FF9D, 0xA00AE27, 0x7D079EB1, int.MinValue + 0x700F9344, int.MinValue + 0x0708A3D2, 0x1E01F268, 0x6906C2FE, int.MinValue + 0x7762575D, int.MinValue + 0x006567CB, 0x196C3671, 0x6E6B06E7, int.MinValue + 0x7ED41B76, int.MinValue + 0x09D32BE0, 0x10DA7A5A, 0x67DD4ACC, int.MinValue + 0x79B9DF6F, int.MinValue + 0x0EBEEFF9, 0x17B7BE43, 0x60B08ED5, int.MinValue + 0x56D6A3E8, int.MinValue + 0x21D1937E, 0x38D8C2C4, 0x4FDFF252, int.MinValue + 0x51BB67F1, int.MinValue + 0x26BC5767, 0x3FB506DD, 0x48B2364B, int.MinValue + 0x580D2BDA, int.MinValue + 0x2F0A1B4C, 0x36034AF6, 0x41047A60, int.MinValue + 0x5F60EFC3, int.MinValue + 0x2867DF55, 0x316E8EEF, 0x4669BE79, int.MinValue + 0x4B61B38C, int.MinValue + 0x3C66831A, 0x256FD2A0, 0x5268E236, int.MinValue + 0x4C0C7795, int.MinValue + 0x3B0B4703, 0x220216B9, 0x5505262F, int.MinValue + 0x45BA3BBE, int.MinValue + 0x32BD0B28, 0x2BB45A92, 0x5CB36A04, int.MinValue + 0x42D7FFA7, int.MinValue + 0x35D0CF31, 0x2CD99E8B, 0x5BDEAE1D, int.MinValue + 0x1B64C2B0, int.MinValue + 0x6C63F226, 0x756AA39C, 0x26D930A, int.MinValue + 0x1C0906A9, int.MinValue + 0x6B0E363F, 0x72076785, 0x5005713, int.MinValue + 0x15BF4A82, int.MinValue + 0x62B87A14, 0x7BB12BAE, 0xCB61B38, int.MinValue + 0x12D28E9B, int.MinValue + 0x65D5BE0D, 0x7CDCEFB7, 0xBDBDF21, int.MinValue + 0x06D3D2D4, int.MinValue + 0x71D4E242, 0x68DDB3F8, 0x1FDA836E, int.MinValue + 0x01BE16CD, int.MinValue + 0x76B9265B, 0x6FB077E1, 0x18B74777, int.MinValue + 0x08085AE6, int.MinValue + 0x7F0F6A70, 0x66063BCA, 0x11010B5C, int.MinValue + 0x0F659EFF, int.MinValue + 0x7862AE69, 0x616BFFD3, 0x166CCF45, int.MinValue + 0x200AE278, int.MinValue + 0x570DD2EE, 0x4E048354, 0x3903B3C2, int.MinValue + 0x27672661, int.MinValue + 0x506016F7, 0x4969474D, 0x3E6E77DB, int.MinValue + 0x2ED16A4A, int.MinValue + 0x59D65ADC, 0x40DF0B66, 0x37D83BF0, int.MinValue + 0x29BCAE53, int.MinValue + 0x5EBB9EC5, 0x47B2CF7F, 0x30B5FFE9, int.MinValue + 0x3DBDF21C, int.MinValue + 0x4ABAC28A, 0x53B39330, 0x24B4A3A6, int.MinValue + 0x3AD03605, int.MinValue + 0x4DD70693, 0x54DE5729, 0x23D967BF, int.MinValue + 0x33667A2E, int.MinValue + 0x44614AB8, 0x5D681B02, 0x2A6F2B94, int.MinValue + 0x340BBE37, int.MinValue + 0x430C8EA1, 0x5A05DF1B, 0x2D02EF8D };

                public override byte[] Hash
                {
                    get
                    {
                        byte[] b = BitConverter.GetBytes(~result);
                        Array.Reverse(b);
                        return b;
                    }
                }
            }

            #endregion

        }
        #endregion


        #region   Data

        /// <summary>
        /// represents Hex, Byte, Base64, or String data to encrypt/decrypt;
        /// use the .Text property to set/get a string representation 
        /// use the .Hex property to set/get a string-based Hexadecimal representation 
        /// use the .Base64 to set/get a string-based Base64 representation 
        /// </summary>
        public partial class Data
        {
            private byte[] _b;
            private int _MaxBytes = 0;
            private int _MinBytes = 0;
            private int _StepBytes = 0;

            /// <summary>
            /// Determines the default text encoding across ALL Data instances
            /// </summary>
            /// 


            

            public static Encoding DefaultEncoding = Encoding.GetEncoding("Windows-1252");

           //public Encoding encoding = Encoding.Default;

            public Encoding Encoding = Encoding.UTF8;

            // System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;

            /// <summary>
            /// Determines the default text encoding for this Data instance
            /// </summary>
            //public Encoding Encoding = DefaultEncoding;

            /// <summary>
            /// Creates new, empty encryption data
            /// </summary>
            public Data()
            {
            }

            /// <summary>
            /// Creates new encryption data with the specified byte array
            /// </summary>
            public Data(byte[] b)
            {
                _b = b;
            }

            /// <summary>
            /// Creates new encryption data with the specified string; 
            /// will be converted to byte array using default encoding
            /// </summary>
            public Data(string s)
            {
                Text = s;
            }

            /// <summary>
            /// Creates new encryption data using the specified string and the 
            /// specified encoding to convert the string to a byte array.
            /// </summary>
            public Data(string s, Encoding encoding)
            {
                Encoding = encoding;
                Text = s;
            }

            /// <summary>
            /// returns true if no data is present
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    if (_b is null)
                    {
                        return true;
                    }
                    if (_b.Length == 0)
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// allowed step interval, in bytes, for this data; if 0, no limit
            /// </summary>
            public int StepBytes
            {
                get
                {
                    return _StepBytes;
                }
                set
                {
                    _StepBytes = value;
                }
            }

            /// <summary>
            /// allowed step interval, in bits, for this data; if 0, no limit
            /// </summary>
            public int StepBits
            {
                get
                {
                    return _StepBytes * 8;
                }
                set
                {
                    _StepBytes = value / 8;
                }
            }

            /// <summary>
            /// minimum number of bytes allowed for this data; if 0, no limit
            /// </summary>
            public int MinBytes
            {
                get
                {
                    return _MinBytes;
                }
                set
                {
                    _MinBytes = value;
                }
            }

            /// <summary>
            /// minimum number of bits allowed for this data; if 0, no limit
            /// </summary>
            public int MinBits
            {
                get
                {
                    return _MinBytes * 8;
                }
                set
                {
                    _MinBytes = value / 8;
                }
            }

            /// <summary>
            /// maximum number of bytes allowed for this data; if 0, no limit
            /// </summary>
            public int MaxBytes
            {
                get
                {
                    return _MaxBytes;
                }
                set
                {
                    _MaxBytes = value;
                }
            }

            /// <summary>
            /// maximum number of bits allowed for this data; if 0, no limit
            /// </summary>
            public int MaxBits
            {
                get
                {
                    return _MaxBytes * 8;
                }
                set
                {
                    _MaxBytes = value / 8;
                }
            }

            /// <summary>
            /// Returns the byte representation of the data; 
            /// This will be padded to MinBytes and trimmed to MaxBytes as necessary!
            /// </summary>
            public byte[] Bytes
            {
                get
                {
                    if (_MaxBytes > 0)
                    {
                        if (_b.Length > _MaxBytes)
                        {
                            var b = new byte[_MaxBytes];
                            Array.Copy(_b, b, b.Length);
                            _b = b;
                        }
                    }
                    if (_MinBytes > 0)
                    {
                        if (_b.Length < _MinBytes)
                        {
                            var b = new byte[_MinBytes];
                            Array.Copy(_b, b, _b.Length);
                            _b = b;
                        }
                    }
                    return _b;
                }
                set
                {
                    _b = value;
                }
            }

            /// <summary>
            /// Sets or returns text representation of bytes using the default text encoding
            /// </summary>
            public string Text
            {
                get
                {
                    if (_b is null)
                    {
                        return "";
                    }
                    else
                    {
                        // -- need to handle nulls here; oddly, C# will happily convert
                        // -- nulls into the string whereas VB stops converting at the
                        // -- first null!
                        int i = Array.IndexOf(_b, (byte)0);
                        if (i >= 0)
                        {
                            return Encoding.GetString(_b, 0, i);
                        }
                        else
                        {
                            return Encoding.GetString(_b);
                        }
                    }
                }
                set
                {
                    _b = Encoding.GetBytes(value);
                }
            }

            /// <summary>
            /// Sets or returns Hex string representation of this data
            /// </summary>
            public string Hex
            {
                get
                {
                    return Utils.ToHex(_b);
                }
                set
                {
                    _b = Utils.FromHex(value);
                }
            }

            /// <summary>
            /// Sets or returns Base64 string representation of this data
            /// </summary>
            public string Base64
            {
                get
                {
                    return Utils.ToBase64(_b);
                }
                set
                {
                    _b = Utils.FromBase64(value);
                }
            }

            /// <summary>
            /// Returns text representation of bytes using the default text encoding
            /// </summary>
            public new string ToString()
            {
                return Text;
            }

            /// <summary>
            /// returns Base64 string representation of this data
            /// </summary>
            public string ToBase64()
            {
                return Base64;
            }

            /// <summary>
            /// returns Hex string representation of this data
            /// </summary>
            public string ToHex()
            {
                return Hex;
            }

        }

        #endregion

        #region   Utils

        /// <summary>
        /// Friend class for shared utility methods used by multiple Encryption classes
        /// </summary>
        internal partial class Utils
        {

            /// <summary>
            /// converts an array of bytes to a string Hex representation
            /// </summary>
            internal static string ToHex(byte[] ba)
            {
                if (ba is null || ba.Length == 0)
                {
                    return "";
                }
                const string HexFormat = "{0:X2}";
                var sb = new StringBuilder();
                foreach (byte b in ba)
                    sb.Append(string.Format(HexFormat, b));
                return sb.ToString();
            }

            /// <summary>
            /// converts from a string Hex representation to an array of bytes
            /// </summary>
            internal static byte[] FromHex(string hexEncoded)
            {
                if (hexEncoded is null || hexEncoded.Length == 0)
                {
                    return null;
                }
                try
                {
                    int l = Convert.ToInt32(hexEncoded.Length / 2d);
                    var b = new byte[l];
                    for (int i = 0, loopTo = l - 1; i <= loopTo; i++)
                        b[i] = Convert.ToByte(hexEncoded.Substring(i * 2, 2), 16);
                    return b;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The provided string does not appear to be Hex encoded:" + Environment.NewLine + hexEncoded + Environment.NewLine, ex);
                }
            }

            /// <summary>
            /// converts from a string Base64 representation to an array of bytes
            /// </summary>
            internal static byte[] FromBase64(string base64Encoded)
            {
                if (base64Encoded is null || base64Encoded.Length == 0)
                {
                    return null;
                }
                try
                {
                    return Convert.FromBase64String(base64Encoded);
                }
                catch (FormatException ex)
                {
                    throw new FormatException("The provided string does not appear to be Base64 encoded:" + Environment.NewLine + base64Encoded + Environment.NewLine, ex);
                }
            }

            /// <summary>
            /// converts from an array of bytes to a string Base64 representation
            /// </summary>
            internal static string ToBase64(byte[] b)
            {
                if (b is null || b.Length == 0)
                {
                    return "";
                }
                return Convert.ToBase64String(b);
            }

            /// <summary>
            /// retrieve an element from an XML string
            /// </summary>
            internal static string GetXmlElement(string xml, string element)
            {
                Match m;
                m = Regex.Match(xml, "<" + element + ">(?<Element>[^>]*)</" + element + ">", RegexOptions.IgnoreCase);
                if (m is null)
                {
                    throw new Exception("Could not find <" + element + "></" + element + "> in provided Public Key XML.");
                }
                return m.Groups["Element"].ToString();
            }

            /// <summary>
            /// Returns the specified string value from the application .config file
            /// </summary>
            internal static string GetConfigString(string key, bool isRequired = true)
            {
                string s = System.Configuration.ConfigurationManager.AppSettings[key];

                //string s = (string)ConfigurationSettings.AppSettings.Get(key);
                if (string.IsNullOrEmpty(s))
                {
                    if (isRequired)
                    {
                        throw new ConfigurationException("key <" + key + "> is missing from .config file");
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return s;
                }
            }

            internal static string WriteConfigKey(string key, string value)
            {
                string s = "<add key=\"{0}\" value=\"{1}\" />" + Environment.NewLine;
                return string.Format(s, key, value);
            }

            internal static string WriteXmlElement(string element, string value)
            {
                string s = "<{0}>{1}</{0}>" + Environment.NewLine;
                return string.Format(s, element, value);
            }

            internal static string WriteXmlNode(string element, bool isClosing = false)
            {
                string s;
                if (isClosing)
                {
                    s = "</{0}>" + Environment.NewLine;
                }
                else
                {
                    s = "<{0}>" + Environment.NewLine;
                }
                return string.Format(s, element);
            }

        }

        #endregion


        #region   Symmetric

        /// <summary>
        /// Symmetric encryption uses a single key to encrypt and decrypt. 
        /// Both parties (encryptor and decryptor) must share the same secret key.
        /// </summary>
        public partial class Symmetric
        {

            private const string _DefaultIntializationVector = "%1Az=-@qT";
            private const int _BufferSize = 2048;

            public enum Provider
            {
                /// <summary>
                /// The Data Encryption Standard provider supports a 64 bit key only
                /// </summary>
                DES,
                /// <summary>
                /// The Rivest Cipher 2 provider supports keys ranging from 40 to 128 bits, default is 128 bits
                /// </summary>
                RC2,
                /// <summary>
                /// The Rijndael (also known as AES) provider supports keys of 128, 192, or 256 bits with a default of 256 bits
                /// </summary>
                Rijndael,
                /// <summary>
                /// The TripleDES provider (also known as 3DES) supports keys of 128 or 192 bits with a default of 192 bits
                /// </summary>
                TripleDES
            }

            private Data _data;
            private Data _key;
            private Data _iv;
            private SymmetricAlgorithm _crypto;
            private byte[] _EncryptedBytes;
            private bool _UseDefaultInitializationVector;

            private Symmetric()
            {
            }

            /// <summary>
            /// Instantiates a new symmetric encryption object using the specified provider.
            /// </summary>
            public Symmetric(Provider provider, bool useDefaultInitializationVector = true)
            {
                switch (provider)
                {
                    case Provider.DES:
                        {
                            _crypto = new DESCryptoServiceProvider();
                            break;
                        }
                    case Provider.RC2:
                        {
                            _crypto = new RC2CryptoServiceProvider();
                            break;
                        }
                    case Provider.Rijndael:
                        {
                            _crypto = new RijndaelManaged();
                            break;
                        }
                    case Provider.TripleDES:
                        {
                            _crypto = new TripleDESCryptoServiceProvider();
                            break;
                        }
                }

                // -- make sure key and IV are always set, no matter what
                Key = RandomKey();
                if (useDefaultInitializationVector)
                {
                    IntializationVector = new Data(_DefaultIntializationVector);
                }
                else
                {
                    IntializationVector = RandomInitializationVector();
                }
            }

            /// <summary>
            /// Key size in bytes. We use the default key size for any given provider; if you 
            /// want to force a specific key size, set this property
            /// </summary>
            public int KeySizeBytes
            {
                get
                {
                    return _crypto.KeySize / 8;
                }
                set
                {
                    _crypto.KeySize = value * 8;
                    _key.MaxBytes = value;
                }
            }

            /// <summary>
            /// Key size in bits. We use the default key size for any given provider; if you 
            /// want to force a specific key size, set this property
            /// </summary>
            public int KeySizeBits
            {
                get
                {
                    return _crypto.KeySize;
                }
                set
                {
                    _crypto.KeySize = value;
                    _key.MaxBits = value;
                }
            }

            /// <summary>
            /// The key used to encrypt/decrypt data
            /// </summary>
            public Data Key
            {
                get
                {
                    return _key;
                }
                set
                {
                    _key = value;
                    _key.MaxBytes = _crypto.LegalKeySizes[0].MaxSize / 8;
                    _key.MinBytes = _crypto.LegalKeySizes[0].MinSize / 8;
                    _key.StepBytes = _crypto.LegalKeySizes[ 0].SkipSize / 8;
                }
            }

            /// <summary>
            /// Using the default Cipher Block Chaining (CBC) mode, all data blocks are processed using
            /// the value derived from the previous block; the first data block has no previous data block
            /// to use, so it needs an InitializationVector to feed the first block
            /// </summary>
            public Data IntializationVector
            {
                get
                {
                    return _iv;
                }
                set
                {
                    _iv = value;
                    _iv.MaxBytes = _crypto.BlockSize / 8;
                    _iv.MinBytes = _crypto.BlockSize / 8;
                }
            }

            /// <summary>
            /// generates a random Initialization Vector, if one was not provided
            /// </summary>
            public Data RandomInitializationVector()
            {
                _crypto.GenerateIV();
                var d = new Data(_crypto.IV);
                return d;
            }

            /// <summary>
            /// generates a random Key, if one was not provided
            /// </summary>
            public Data RandomKey()
            {
                _crypto.GenerateKey();
                var d = new Data(_crypto.Key);
                return d;
            }

            /// <summary>
            /// Ensures that _crypto object has valid Key and IV
            /// prior to any attempt to encrypt/decrypt anything
            /// </summary>
            private void ValidateKeyAndIv(bool isEncrypting)
            {
                if (_key.IsEmpty)
                {
                    if (isEncrypting)
                    {
                        _key = RandomKey();
                    }
                    else
                    {
                        throw new CryptographicException("No key was provided for the decryption operation!");
                    }
                }
                if (_iv.IsEmpty)
                {
                    if (isEncrypting)
                    {
                        _iv = RandomInitializationVector();
                    }
                    else
                    {
                        throw new CryptographicException("No initialization vector was provided for the decryption operation!");
                    }
                }
                _crypto.Key = _key.Bytes;
                _crypto.IV = _iv.Bytes;
            }

            /// <summary>
            /// Encrypts the specified Data using provided key
            /// </summary>
            public Data Encrypt(Data d, Data key)
            {
                Key = key;
                return Encrypt(d);
            }

            /// <summary>
            /// Encrypts the specified Data using preset key and preset initialization vector
            /// </summary>
            public Data Encrypt(Data d)
            {
                var ms = new MemoryStream();

                ValidateKeyAndIv(true);

                var cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(d.Bytes, 0, d.Bytes.Length);
                cs.Close();
                ms.Close();

                return new Data(ms.ToArray());
            }

            /// <summary>
            /// Encrypts the stream to memory using provided key and provided initialization vector
            /// </summary>
            public Data Encrypt(Stream s, Data key, Data iv)
            {
                IntializationVector = iv;
                Key = key;
                return Encrypt(s);
            }

            /// <summary>
            /// Encrypts the stream to memory using specified key
            /// </summary>
            public Data Encrypt(Stream s, Data key)
            {
                Key = key;
                return Encrypt(s);
            }

            /// <summary>
            /// Encrypts the specified stream to memory using preset key and preset initialization vector
            /// </summary>
            public Data Encrypt(Stream s)
            {
                var ms = new MemoryStream();
                var b = new byte[2049];
                int i;

                ValidateKeyAndIv(true);

                var cs = new CryptoStream(ms, _crypto.CreateEncryptor(), CryptoStreamMode.Write);
                i = s.Read(b, 0, _BufferSize);
                while (i > 0)
                {
                    cs.Write(b, 0, i);
                    i = s.Read(b, 0, _BufferSize);
                }

                cs.Close();
                ms.Close();

                return new Data(ms.ToArray());
            }

            /// <summary>
            /// Decrypts the specified data using provided key and preset initialization vector
            /// </summary>
            public Data Decrypt(Data encryptedData, Data key)
            {
                Key = key;
                return Decrypt(encryptedData);
            }

            /// <summary>
            /// Decrypts the specified stream using provided key and preset initialization vector
            /// </summary>
            public Data Decrypt(Stream encryptedStream, Data key)
            {
                Key = key;
                return Decrypt(encryptedStream);
            }

            /// <summary>
            /// Decrypts the specified stream using preset key and preset initialization vector
            /// </summary>
            public Data Decrypt(Stream encryptedStream)
            {
                var ms = new MemoryStream();
                var b = new byte[2049];

                ValidateKeyAndIv(false);
                var cs = new CryptoStream(encryptedStream, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

                int i;
                i = cs.Read(b, 0, _BufferSize);

                while (i > 0)
                {
                    ms.Write(b, 0, i);
                    i = cs.Read(b, 0, _BufferSize);
                }
                cs.Close();
                ms.Close();

                return new Data(ms.ToArray());
            }

            /// <summary>
            /// Decrypts the specified data using preset key and preset initialization vector
            /// </summary>
            public Data Decrypt(Data encryptedData)
            {
                try
                {
                    var ms = new MemoryStream(encryptedData.Bytes, 0, encryptedData.Bytes.Length);
                    byte[] b = new byte[encryptedData.Bytes.Length];

                    ValidateKeyAndIv(false);
                    var cs = new CryptoStream(ms, _crypto.CreateDecryptor(), CryptoStreamMode.Read);

                    try
                    {
                        cs.Read(b, 0, encryptedData.Bytes.Length - 1);
                    }
                    catch (CryptographicException ex)
                    {
                        throw new CryptographicException("Unable to decrypt data. The provided key may be invalid.", ex);
                    }
                    finally
                    {
                        cs.Close();
                    }
                    return new Data(b);
                }
                catch
                {
                }

                return default;
            }

        }

        #endregion


        #region   Asymmetric

        /// <summary>
        /// Asymmetric encryption uses a pair of keys to encrypt and decrypt.
        /// There is a "public" key which is used to encrypt. Decrypting, on the other hand, 
        /// requires both the "public" key and an additional "private" key. The advantage is 
        /// that people can send you encrypted messages without being able to decrypt them.
        /// </summary>
        /// <remarks>
        /// The only provider supported is the <see cref="RSACryptoServiceProvider"/>
        /// </remarks>
        public partial class Asymmetric
        {

            private RSACryptoServiceProvider _rsa;
            private string _KeyContainerName = "Encryption.AsymmetricEncryption.DefaultContainerName";
            private bool _UseMachineKeystore = true;
            private int _KeySize = 1024;

            private const string _ElementParent = "RSAKeyValue";
            private const string _ElementModulus = "Modulus";
            private const string _ElementExponent = "Exponent";
            private const string _ElementPrimeP = "P";
            private const string _ElementPrimeQ = "Q";
            private const string _ElementPrimeExponentP = "DP";
            private const string _ElementPrimeExponentQ = "DQ";
            private const string _ElementCoefficient = "InverseQ";
            private const string _ElementPrivateExponent = "D";

            // -- http://forum.java.sun.com/thread.jsp?forum=9&thread=552022&tstart=0&trange=15 
            private const string _KeyModulus = "PublicKey.Modulus";
            private const string _KeyExponent = "PublicKey.Exponent";
            private const string _KeyPrimeP = "PrivateKey.P";
            private const string _KeyPrimeQ = "PrivateKey.Q";
            private const string _KeyPrimeExponentP = "PrivateKey.DP";
            private const string _KeyPrimeExponentQ = "PrivateKey.DQ";
            private const string _KeyCoefficient = "PrivateKey.InverseQ";
            private const string _KeyPrivateExponent = "PrivateKey.D";

            #region   PublicKey Class
            /// <summary>
            /// Represents a public encryption key. Intended to be shared, it 
            /// contains only the Modulus and Exponent.
            /// </summary>
            public partial class PublicKey
            {
                public string Modulus;
                public string Exponent;

                public PublicKey()
                {
                }

                public PublicKey(string KeyXml)
                {
                    LoadFromXml(KeyXml);
                }

                /// <summary>
                /// Load public key from App.config or Web.config file
                /// </summary>
                public void LoadFromConfig()
                {
                    Modulus = Utils.GetConfigString(_KeyModulus);
                    Exponent = Utils.GetConfigString(_KeyExponent);
                }

                /// <summary>
                /// Returns *.config file XML section representing this public key
                /// </summary>
                public string ToConfigSection()
                {
                    var sb = new StringBuilder();
                    sb.Append(Utils.WriteConfigKey(_KeyModulus, Modulus));
                    sb.Append(Utils.WriteConfigKey(_KeyExponent, Exponent));
                    return sb.ToString();
                }

                /// <summary>
                /// Writes the *.config file representation of this public key to a file
                /// </summary>
                public void ExportToConfigFile(string filePath)
                {
                    var sw = new StreamWriter(filePath, false);
                    sw.Write(ToConfigSection());
                    sw.Close();
                }

                /// <summary>
                /// Loads the public key from its XML string
                /// </summary>
                public void LoadFromXml(string keyXml)
                {
                    Modulus = Utils.GetXmlElement(keyXml, "Modulus");
                    Exponent = Utils.GetXmlElement(keyXml, "Exponent");
                }

                /// <summary>
                /// Converts this public key to an RSAParameters object
                /// </summary>
                public RSAParameters ToParameters()
                {
                    var r = new RSAParameters();
                    r.Modulus = Convert.FromBase64String(Modulus);
                    r.Exponent = Convert.FromBase64String(Exponent);
                    return r;
                }

                /// <summary>
                /// Converts this public key to its XML string representation
                /// </summary>
                public string ToXml()
                {
                    var sb = new StringBuilder();
                    sb.Append(Utils.WriteXmlNode(_ElementParent));
                    sb.Append(Utils.WriteXmlElement(_ElementModulus, Modulus));
                    sb.Append(Utils.WriteXmlElement(_ElementExponent, Exponent));
                    sb.Append(Utils.WriteXmlNode(_ElementParent, true));
                    return sb.ToString();
                }

                /// <summary>
                /// Writes the Xml representation of this public key to a file
                /// </summary>
                public void ExportToXmlFile(string filePath)
                {
                    var sw = new StreamWriter(filePath, false);
                    sw.Write(ToXml());
                    sw.Close();
                }

            }
            #endregion

            #region   PrivateKey Class

            /// <summary>
            /// Represents a private encryption key. Not intended to be shared, as it 
            /// contains all the elements that make up the key.
            /// </summary>
            public partial class PrivateKey
            {
                public string Modulus;
                public string Exponent;
                public string PrimeP;
                public string PrimeQ;
                public string PrimeExponentP;
                public string PrimeExponentQ;
                public string Coefficient;
                public string PrivateExponent;

                public PrivateKey()
                {
                }

                public PrivateKey(string keyXml)
                {
                    LoadFromXml(keyXml);
                }

                /// <summary>
                /// Load private key from App.config or Web.config file
                /// </summary>
                public void LoadFromConfig()
                {
                    Modulus = Utils.GetConfigString(_KeyModulus);
                    Exponent = Utils.GetConfigString(_KeyExponent);
                    PrimeP = Utils.GetConfigString(_KeyPrimeP);
                    PrimeQ = Utils.GetConfigString(_KeyPrimeQ);
                    PrimeExponentP = Utils.GetConfigString(_KeyPrimeExponentP);
                    PrimeExponentQ = Utils.GetConfigString(_KeyPrimeExponentQ);
                    Coefficient = Utils.GetConfigString(_KeyCoefficient);
                    PrivateExponent = Utils.GetConfigString(_KeyPrivateExponent);
                }

                /// <summary>
                /// Converts this private key to an RSAParameters object
                /// </summary>
                public RSAParameters ToParameters()
                {
                    var r = new RSAParameters();
                    r.Modulus = Convert.FromBase64String(Modulus);
                    r.Exponent = Convert.FromBase64String(Exponent);
                    r.P = Convert.FromBase64String(PrimeP);
                    r.Q = Convert.FromBase64String(PrimeQ);
                    r.DP = Convert.FromBase64String(PrimeExponentP);
                    r.DQ = Convert.FromBase64String(PrimeExponentQ);
                    r.InverseQ = Convert.FromBase64String(Coefficient);
                    r.D = Convert.FromBase64String(PrivateExponent);
                    return r;
                }

                /// <summary>
                /// Returns *.config file XML section representing this private key
                /// </summary>
                public string ToConfigSection()
                {
                    var sb = new StringBuilder();
                    sb.Append(Utils.WriteConfigKey(_KeyModulus, Modulus));
                    sb.Append(Utils.WriteConfigKey(_KeyExponent, Exponent));
                    sb.Append(Utils.WriteConfigKey(_KeyPrimeP, PrimeP));
                    sb.Append(Utils.WriteConfigKey(_KeyPrimeQ, PrimeQ));
                    sb.Append(Utils.WriteConfigKey(_KeyPrimeExponentP, PrimeExponentP));
                    sb.Append(Utils.WriteConfigKey(_KeyPrimeExponentQ, PrimeExponentQ));
                    sb.Append(Utils.WriteConfigKey(_KeyCoefficient, Coefficient));
                    sb.Append(Utils.WriteConfigKey(_KeyPrivateExponent, PrivateExponent));
                    return sb.ToString();
                }

                /// <summary>
                /// Writes the *.config file representation of this private key to a file
                /// </summary>
                public void ExportToConfigFile(string strFilePath)
                {
                    var sw = new StreamWriter(strFilePath, false);
                    sw.Write(ToConfigSection());
                    sw.Close();
                }

                /// <summary>
                /// Loads the private key from its XML string
                /// </summary>
                public void LoadFromXml(string keyXml)
                {
                    Modulus = Utils.GetXmlElement(keyXml, "Modulus");
                    Exponent = Utils.GetXmlElement(keyXml, "Exponent");
                    PrimeP = Utils.GetXmlElement(keyXml, "P");
                    PrimeQ = Utils.GetXmlElement(keyXml, "Q");
                    PrimeExponentP = Utils.GetXmlElement(keyXml, "DP");
                    PrimeExponentQ = Utils.GetXmlElement(keyXml, "DQ");
                    Coefficient = Utils.GetXmlElement(keyXml, "InverseQ");
                    PrivateExponent = Utils.GetXmlElement(keyXml, "D");
                }

                /// <summary>
                /// Converts this private key to its XML string representation
                /// </summary>
                public string ToXml()
                {
                    var sb = new StringBuilder();
                    sb.Append(Utils.WriteXmlNode(_ElementParent));
                    sb.Append(Utils.WriteXmlElement(_ElementModulus, Modulus));
                    sb.Append(Utils.WriteXmlElement(_ElementExponent, Exponent));
                    sb.Append(Utils.WriteXmlElement(_ElementPrimeP, PrimeP));
                    sb.Append(Utils.WriteXmlElement(_ElementPrimeQ, PrimeQ));
                    sb.Append(Utils.WriteXmlElement(_ElementPrimeExponentP, PrimeExponentP));
                    sb.Append(Utils.WriteXmlElement(_ElementPrimeExponentQ, PrimeExponentQ));
                    sb.Append(Utils.WriteXmlElement(_ElementCoefficient, Coefficient));
                    sb.Append(Utils.WriteXmlElement(_ElementPrivateExponent, PrivateExponent));
                    sb.Append(Utils.WriteXmlNode(_ElementParent, true));
                    return sb.ToString();
                }

                /// <summary>
                /// Writes the Xml representation of this private key to a file
                /// </summary>
                public void ExportToXmlFile(string filePath)
                {
                    var sw = new StreamWriter(filePath, false);
                    sw.Write(ToXml());
                    sw.Close();
                }

            }

            #endregion

            /// <summary>
            /// Instantiates a new asymmetric encryption session using the default key size; 
            /// this is usally 1024 bits
            /// </summary>
            public Asymmetric()
            {
                _rsa = GetRSAProvider();
            }

            /// <summary>
            /// Instantiates a new asymmetric encryption session using a specific key size
            /// </summary>
            public Asymmetric(int keySize)
            {
                _KeySize = keySize;
                _rsa = GetRSAProvider();
            }

            /// <summary>
            /// Sets the name of the key container used to store this key on disk; this is an 
            /// unavoidable side effect of the underlying Microsoft CryptoAPI. 
            /// </summary>
            /// <remarks>
            /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
            /// </remarks>
            public string KeyContainerName
            {
                get
                {
                    return _KeyContainerName;
                }
                set
                {
                    _KeyContainerName = value;
                }
            }

            /// <summary>
            /// Returns the current key size, in bits
            /// </summary>
            public int KeySizeBits
            {
                get
                {
                    return _rsa.KeySize;
                }
            }

            /// <summary>
            /// Returns the maximum supported key size, in bits
            /// </summary>
            public int KeySizeMaxBits
            {
                get
                {
                    return _rsa.LegalKeySizes[0].MaxSize;
                }
            }

            /// <summary>
            /// Returns the minimum supported key size, in bits
            /// </summary>
            public int KeySizeMinBits
            {
                get
                {
                    return _rsa.LegalKeySizes[0].MinSize;
                }
            }

            /// <summary>
            /// Returns valid key step sizes, in bits
            /// </summary>
            public int KeySizeStepBits
            {
                get
                {
                    return _rsa.LegalKeySizes[0].SkipSize;
                }
            }

            /// <summary>
            /// Returns the default public key as stored in the *.config file
            /// </summary>
            public PublicKey DefaultPublicKey
            {
                get
                {
                    var pubkey = new PublicKey();
                    pubkey.LoadFromConfig();
                    return pubkey;
                }
            }

            /// <summary>
            /// Returns the default private key as stored in the *.config file
            /// </summary>
            public PrivateKey DefaultPrivateKey
            {
                get
                {
                    var privkey = new PrivateKey();
                    privkey.LoadFromConfig();
                    return privkey;
                }
            }

            /// <summary>
            /// Generates a new public/private key pair as objects
            /// </summary>
            public void GenerateNewKeyset(ref PublicKey publicKey, ref PrivateKey privateKey)
            {
                var PublicKeyXML = default(string);
                var PrivateKeyXML = default(string);
                GenerateNewKeyset(ref PublicKeyXML, ref PrivateKeyXML);
                publicKey = new PublicKey(PublicKeyXML);
                privateKey = new PrivateKey(PrivateKeyXML);
            }

            /// <summary>
            /// Generates a new public/private key pair as XML strings
            /// </summary>
            public void GenerateNewKeyset(ref string publicKeyXML, ref string privateKeyXML)
            {
                RSA rsa = System.Security.Cryptography.RSA.Create();
                publicKeyXML = rsa.ToXmlString(false);
                privateKeyXML = rsa.ToXmlString(true);
            }

            /// <summary>
            /// Encrypts data using the default public key
            /// </summary>
            public Data Encrypt(Data d)
            {
                var PublicKey = DefaultPublicKey;
                return Encrypt(d, PublicKey);
            }

            /// <summary>
            /// Encrypts data using the provided public key
            /// </summary>
            public Data Encrypt(Data d, PublicKey publicKey)
            {
                _rsa.ImportParameters(publicKey.ToParameters());
                return EncryptPrivate(d);
            }

            /// <summary>
            /// Encrypts data using the provided public key as XML
            /// </summary>
            public Data Encrypt(Data d, string publicKeyXML)
            {
                LoadKeyXml(publicKeyXML, false);
                return EncryptPrivate(d);
            }

            private Data EncryptPrivate(Data d)
            {
                try
                {
                    return new Data(_rsa.Encrypt(d.Bytes, false));
                }
                catch (CryptographicException ex)
                {
                    if (ex.Message.IndexOf("bad length") > -1)
                    {
                        throw new CryptographicException("Your data is too large; RSA encryption is designed to encrypt relatively small amounts of data. The exact byte limit depends on the key size. To encrypt more data, use symmetric encryption and then encrypt that symmetric key with asymmetric RSA encryption.", ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            /// <summary>
            /// Decrypts data using the default private key
            /// </summary>
            public Data Decrypt(Data encryptedData)
            {
                var PrivateKey = new PrivateKey();
                PrivateKey.LoadFromConfig();
                return Decrypt(encryptedData, PrivateKey);
            }

            /// <summary>
            /// Decrypts data using the provided private key
            /// </summary>
            public Data Decrypt(Data encryptedData, PrivateKey PrivateKey)
            {
                _rsa.ImportParameters(PrivateKey.ToParameters());
                return DecryptPrivate(encryptedData);
            }

            /// <summary>
            /// Decrypts data using the provided private key as XML
            /// </summary>
            public Data Decrypt(Data encryptedData, string PrivateKeyXML)
            {
                LoadKeyXml(PrivateKeyXML, true);
                return DecryptPrivate(encryptedData);
            }

            private void LoadKeyXml(string keyXml, bool isPrivate)
            {
                try
                {
                    _rsa.FromXmlString(keyXml);
                }
                catch (System.Security.XmlSyntaxException ex)
                {
                    string s;
                    if (isPrivate)
                    {
                        s = "private";
                    }
                    else
                    {
                        s = "public";
                    }
                    throw new System.Security.XmlSyntaxException(string.Format("The provided {0} encryption key XML does not appear to be valid.", s), ex);
                }
            }

            private Data DecryptPrivate(Data encryptedData)
            {
                return new Data(_rsa.Decrypt(encryptedData.Bytes, false));
            }

            /// <summary>
            /// gets the default RSA provider using the specified key size; 
            /// note that Microsoft's CryptoAPI has an underlying file system dependency that is unavoidable
            /// </summary>
            /// <remarks>
            /// http://support.microsoft.com/default.aspx?scid=http://support.microsoft.com:80/support/kb/articles/q322/3/71.asp&amp;NoWebContent=1
            /// </remarks>
            private RSACryptoServiceProvider GetRSAProvider()
            {
                var rsa = default(RSACryptoServiceProvider);
                var csp = default(CspParameters);
                try
                {
                    csp = new CspParameters();
                    csp.KeyContainerName = _KeyContainerName;
                    rsa = new RSACryptoServiceProvider(_KeySize, csp);
                    rsa.PersistKeyInCsp = false;
                    RSACryptoServiceProvider.UseMachineKeyStore = true;
                    return rsa;
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    if (ex.Message.ToLower().IndexOf("csp for this implementation could not be acquired") > -1)
                    {
                        throw new Exception("Unable to obtain Cryptographic Service Provider. " + "Either the permissions are incorrect on the " + @"'C:\Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys' " + "folder, or the current security context '" + System.Security.Principal.WindowsIdentity.GetCurrent().Name + "'" + " does not have access to this folder.", ex);



                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (rsa is not null)
                    {
                        rsa = default;
                    }
                    if (csp is not null)
                    {
                        csp = default;
                    }
                }
            }

        }

        #endregion




    }


}


