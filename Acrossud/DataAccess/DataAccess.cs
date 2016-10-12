using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Acrossud
{
    #region Encryption classes

    public class BCEngine
    {
        private readonly Encoding _encoding;
        private readonly IBlockCipher _blockCipher;
        private PaddedBufferedBlockCipher _cipher;

        private IBlockCipherPadding _padding;
        public BCEngine(IBlockCipher blockCipher, Encoding encoding)
        {
            _blockCipher = blockCipher;
            _encoding = encoding;
        }

        public void SetPadding(IBlockCipherPadding padding)
        {
            if (padding != null)
            {
                _padding = padding;
            }
        }

        public string Decrypt(string cipher, string key)
        {
            byte[] result = BouncyCastleCrypto(false, Convert.FromBase64String(cipher), key);
            return _encoding.GetString(result);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="forEncrypt"></param>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="CryptoException"></exception>
        private byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, string key)
        {
            try
            {
                _cipher = _padding == null ? new PaddedBufferedBlockCipher(_blockCipher) : new PaddedBufferedBlockCipher(_blockCipher, _padding);
                byte[] keyByte = _encoding.GetBytes(key);
                _cipher.Init(forEncrypt, new KeyParameter(keyByte));
                return _cipher.DoFinal(input);
            }
            catch (Org.BouncyCastle.Crypto.CryptoException ex)
            {
                throw new CryptoException(ex.Message);
            }
        }
    }

    public class Action
    {
        public string Decrypt(string codigo, string clave)
        {
            string desencriptado = String.Empty;

            try
            {
                desencriptado = AESDecryption(codigo, clave);
            }
            catch
            {

            }

            return desencriptado;
        }

        private string AESDecryption(string cipher, string key)
        {
            Encoding _encoding = Encoding.ASCII;
            IBlockCipherPadding _padding = new Pkcs7Padding();

            BCEngine bcEngine = new BCEngine(new AesEngine(), _encoding);
            bcEngine.SetPadding(_padding);
            return bcEngine.Decrypt(cipher, key);
        }
    }

    #endregion

    #region DataAccess

    /// <summary>
    /// Clase abstracta que contiene los métodos que deberán implementar los
    /// distintos tipos de DataAccess (MySql, Sql Server, etc.)
    /// </summary>
    public abstract class DataAccess : IDisposable
    {
        #region Fields

        protected string _connStr;

        private static Action _cripto_action = new Action();

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Ejecuta un procedimiento almacenado en la BD
        /// Importante: 
        /// - ADO.NET emplea una técnica de optimización denominada agrupación de conexiones.
        /// - Cada vez que un usuario llama a Open en una conexión, el agrupador comprueba si hay una conexión disponible en el grupo.
        ///   Si hay disponible una conexión agrupada, la devuelve en lugar de abrir una nueva
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento almacenado</param>
        /// <param name="params_values">Parámetros de entrada del procedimiento almacenado</param>
        /// <returns>DataSet con el resultado de la ejecución del procedimiento almacenado</returns>
        public abstract DataSet ExecuteStoreProcedure(string procedureName, Dictionary<string, object> params_values = null);

        public abstract object ExecuteStoreProcedureWithReturnValue(string procedureName, Dictionary<string, object> params_values = null);

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public DataAccess() { }

        /// <summary>
        /// Guarda en la clase el string de conexión
        /// </summary>
        /// <param name="connStr">String de conexión a la BD</param>
        public void SetConnString(string connStr)
        {
            _connStr = connStr;
        }

        /// <summary>
        /// Desencripta y guarda en la clase el string de conexión
        /// </summary>
        ///<param name="connStr">String de conexión a la BD</param>
        public void SetConnStringEncripted(string connStr)
        {
            string cp = null;
            string des = null;

            try
            {
                cp = "Obj.Utilities.Aux";
                string aux = cp.Substring(0, 16);
                aux += cp.Substring(cp.Length - 16, 16);

                des = _cripto_action.Decrypt(connStr, aux);
            }
            catch (Exception ex)
            {
                throw new Exception("El valor que desea descifrar no es válido", ex.InnerException);
            }

            _connStr = des;
        }

        /// <summary>
        /// Desencripta y guarda en la clase el string de conexión
        /// </summary>
        ///<param name="connStr">String de conexión a la BD</param>
        static public string GetConnStringDecripted(string connStr)
        {
            string cp = null;
            string des = null;

            try
            {
                cp = "Obj.Utilities.Aux";
                string aux = cp.Substring(0, 16);
                aux += cp.Substring(cp.Length - 16, 16);

                des = _cripto_action.Decrypt(connStr, aux);
            }
            catch (Exception ex)
            {
                throw new Exception("El valor que desea descifrar no es válido", ex.InnerException);
            }

            return des;
        }

        #endregion

        #region IDisposable

        public void Dispose() { }

        #endregion
    }

    #endregion
}