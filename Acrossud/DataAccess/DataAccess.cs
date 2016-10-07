using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acrossud
{
    public class SQLDataAccess : IDisposable
    {
        #region Fields

        // Conexión con la base
        private SqlConnection _conn;
        // Transacción a ser ejecutada
        private SqlTransaction _trans;

        // Cantidad máxima de intentos para reconectarse a la BD
        private int _max_reintentos = 1;

        private static Action _cripto_action = new Action();

        #endregion

        #region Static Properties

        // Stirng de conexión a la BD
        private static string _connStr { get; set; }

        public static bool IsConnectionStringSetted
        {
            get
            {
                return !string.IsNullOrEmpty(_connStr);
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Se conecta a la base de datos.
        /// </summary>
        public SQLDataAccess()
        {
            Connect();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Se conecta a la base de datos.
        /// Importante: 
        /// - ADO.NET emplea una técnica de optimización denominada agrupación de conexiones.
        /// - Cada vez que un usuario llama a Open en una conexión, el agrupador comprueba si hay una conexión disponible en el grupo.
        ///   Si hay disponible una conexión agrupada, la devuelve en lugar de abrir una nueva
        /// </summary>
        private void Connect()
        {
            try
            {
                if (!string.IsNullOrEmpty(_connStr))
                {
                    _conn = new SqlConnection(_connStr);
                    _conn.Open();
                    _trans = null;
                }
                else
                {
                    throw new Exception("El String de Conexion a la base de datos es vacio");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo establecer la conexión con la base de datos.", ex.InnerException);
            }
        }

        /// <summary>
        /// Crea un nuevo hilo e intenta al reconexión, no lleva try y catch a propósito
        /// Es importante entender que esto lo que hace es intentar RECONECTAR, es decir que ya se estuvo previamente conectado
        /// NO crea nuevamente el objeto SqlConnection conn, si el conn es null no se intenta volver a crearlo (a propósito)
        /// Ya que puede ser cuando se está cerrando y no se quiere volver a conectar o porque no quedaron bien seteados los parámetros de conexión o algo similar
        /// </summary>
        private void ReConnect()
        {
            Thread thread = new Thread(new ThreadStart(TryToReconnect));
            thread.Start();
        }

        /// <summary>
        /// Importante:
        ///  - Cuando la aplicación llama a Close en la conexión, el agrupador la devuelve al conjunto agrupado de conexiones activas en lugar de cerrarla. 
        ///  - Una vez que la conexión vuelve al grupo, ya está preparada para volverse a utilizar en la siguiente llamada a Open.
        /// </summary>
        private void Close()
        {
            try
            {
                if ((_conn != null) && (_conn.State != ConnectionState.Closed))
                {
                    _conn.Close();
                    _conn = null;
                }
            }
            // No se hace nada a propósito, se supone que esto es cuando estás cerrando
            catch (Exception ex)
            {
                throw new Exception("No se pudo cerrar la conexión con la base de datos.", ex.InnerException);
            }
        }

        /// <summary>
        /// Para cuando se pierde la conexión con la BD que estaba previamente establecida. NO es para cuando conn es nulo, no hace new de conn.
        /// </summary>
        private void TryToReconnect()
        {
            int reintentos = 1;
            while (reintentos <= _max_reintentos)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    //Thread.Sleep(2000);
                    try
                    {
                        reintentos++;
                        _conn.Open();
                    }
                    catch { }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Guarda en la clase el string de conexión
        /// </summary>
        /// <param name="connStr">String de conexión a la BD</param>
        public void SetConnString(string connStr)
        {
            _connStr = connStr;
        }

        /// <summary>
        /// Guarda en la clase el string de conexión
        /// </summary>
        /// <param name="connStr">String de conexión a la BD</param>
        public static void SetConnStringEncripted(string connStr)
        {
            string cp = null;
            string des = null;

            try
            {
                cp = "ObjetoVentana.txtDescripcion";
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
        /// Inicia una transacción, donde todo lo que se ejecute dentro de la misma será de forma atómica
        /// </summary>
        public void StartTrans()
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    if (_trans == null)
                    {
                        _trans = _conn.BeginTransaction();
                    }
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                // Si existe transacción, hay conexión y el estado de la transacción es Open o Executing
                // se hace rollback no para cancelar lo pendiente sino para dejar bien el estado de la transacción
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta y finaliza una transacción que fue iniciada previamente
        /// </summary>
        public void ExecuteTrans()
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    _trans.Commit();
                    _trans = null;
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        /// <summary>
        /// Para invocar un RollBack desde código y no porque se haya roto algo dentro del SQLDataAccess
        /// </summary>
        public void Rollback()
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    if (_trans != null)
                    {
                        _trans.Rollback();
                        //Luego de la transaccion se "cierra" la conexion.
                        _conn.Close();
                        _trans = null;
                    }
                }
            }
            catch (Exception)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                /// Se comenta el throw ya que se llama a Rollback cuando se está en un catch. Si se hace throw acá, no se ejecutaría el resto del código del catch desde el cual se invoca
                // throw ex;
            }
        }

        /// <summary>
        /// Ejecuta una consulta sin parametros de entrada. Se setea el timeout en 260
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia a ser ejecutada</param>
        /// <returns>DataSet con el resultado de lo ejecutado</returns>
        public DataSet ExecuteQuery(string sentenciaSQL)
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    DataSet ds = new DataSet();

                    using (SqlDataAdapter da = new SqlDataAdapter(sentenciaSQL, _conn))
                    {
                        if (_trans != null)
                        {
                            da.SelectCommand.Transaction = _trans;
                        }
                        da.SelectCommand.CommandTimeout = 260;
                        da.Fill(ds);
                    }

                    if (_trans == null)
                    {
                        //Luego de la transaccion se "cierra" la conexion.
                        _conn.Close();
                    }

                    return ds;
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta una sentencia en la BD, es decir algo que afecta cierta cantidad de tuplas, pero no algo que tiene que retornar datos
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia a ser ejecutada</param>
        /// <param name="params_values">Parámetros de la consulta</param>
        /// <returns>Indica si se afectaron tuplas en la consulta o no</returns>
        public bool ExecuteSentenceParameterized(string sentenciaSQL, Dictionary<string, object> params_values)
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    int result = 0;
                    // Transacción a ser ejecutada en la BD

                    using (SqlCommand sqlCmd = new SqlCommand())
                    {
                        // Esto es por si el trans ya tenía cosas de antes (si se está adentro de una StartTrans)
                        if (_trans != null)
                        {
                            sqlCmd.Transaction = _trans;
                        }

                        sqlCmd.Connection = _conn;
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.CommandText = sentenciaSQL;

                        // Se recorre la lista de <parametro,valor> agregando cada uno a la sentencia
                        foreach (KeyValuePair<string, object> pair in params_values)
                        {
                            if (pair.Value.GetType() == typeof(int))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Int, 4).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(byte[]))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Binary).Value = pair.Value;
                            else if (pair.Value.GetType() == typeof(double))
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.Float).Value = pair.Value;
                            else
                                sqlCmd.Parameters.Add(pair.Key, SqlDbType.VarChar, 6000).Value = pair.Value;
                        }
                        // Ejecuta la consulta y retorna la cantidad de tuplas afectadas
                        result = sqlCmd.ExecuteNonQuery();

                    }

                    if (_trans == null)
                    {
                        //Luego de la transaccion se "cierra" la conexion.
                        _conn.Close();
                    }

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta una consulta que retorna algo, generalmente un select. Se setea el timeout en 260
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia a ser ejecutada</param>
        /// <param name="params_values">Parámetros de la consulta</param>
        /// <returns>DataSet con el resultado de lo ejecutado</returns>
        public DataSet ExecuteQueryParameterized(string sentenciaSQL, Dictionary<string, object> params_values)
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    // Para realizar una consulta que retorna DataSet
                    DataSet ds = new DataSet();

                    using (SqlDataAdapter da = new SqlDataAdapter(sentenciaSQL, _conn))
                    {
                        // Se recorre la lista de <parametro,valor> agregando cada uno al adapter
                        foreach (KeyValuePair<string, object> pair in params_values)
                        {
                            SqlParameter param;
                            if (pair.Value.GetType() == typeof(int))
                                param = new SqlParameter(pair.Key, SqlDbType.Int, 4);
                            else if (pair.Value.GetType() == typeof(byte[]))
                                param = new SqlParameter(pair.Key, SqlDbType.Binary);
                            else if (pair.Value.GetType() == typeof(double))
                                param = new SqlParameter(pair.Key, SqlDbType.Float);
                            else
                                param = new SqlParameter(pair.Key, SqlDbType.VarChar, 6000);
                            param.Direction = ParameterDirection.Input;

                            param.Value = pair.Value;
                            da.SelectCommand.Parameters.Add(param);
                        }

                        // Esto es por si el trans ya tenía cosas de antes (si se está adentro de una StartTrans)
                        if (_trans != null)
                        {
                            da.SelectCommand.Transaction = _trans;
                        }
                        da.SelectCommand.CommandTimeout = 260;

                        // Se realiza la consulta y se llena el DataSet
                        da.Fill(ds);
                    }

                    if (_trans == null)
                    {
                        //Luego de la transaccion se "cierra" la conexion.
                        _conn.Close();
                    }

                    return ds;
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta una consulta que retorna algo, generalmente un select. El timeout es el que viene por defecto
        /// </summary>
        /// <param name="sentenciaSQL">Sentencia a ser ejecutada</param>
        /// <param name="params_values">Parámetros de la consulta</param>
        /// <returns>DataSet con el resultado de lo ejecutado</returns>
        public DataSet ExecuteQueryParameterizedWithDefaultTimeOut(string sentenciaSQL, Dictionary<string, object> params_values)
        {
            try
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    throw new Exception("La base de datos se encuentra desconectada");
                }
                else
                {
                    DataSet ds = new DataSet();
                    using (SqlDataAdapter da = new SqlDataAdapter(sentenciaSQL, _conn))
                    {
                        // Se recorre la lista de <parametro,valor> agregando cada uno al adapter
                        foreach (KeyValuePair<string, object> pair in params_values)
                        {
                            SqlParameter param;
                            if (pair.Value.GetType() == typeof(int))
                                param = new SqlParameter(pair.Key, SqlDbType.Int, 4);
                            else if (pair.Value.GetType() == typeof(byte[]))
                                param = new SqlParameter(pair.Key, SqlDbType.Binary);
                            else if (pair.Value.GetType() == typeof(double))
                                param = new SqlParameter(pair.Key, SqlDbType.Float);
                            else
                                param = new SqlParameter(pair.Key, SqlDbType.VarChar, 6000);
                            param.Direction = ParameterDirection.Input;

                            param.Value = pair.Value;
                            da.SelectCommand.Parameters.Add(param);
                        }

                        // Esto es por si el trans ya tenía cosas de antes (si se está adentro de una StartTrans)
                        if ((_trans != null))
                        {
                            da.SelectCommand.Transaction = _trans;
                        }

                        // Se realiza la consulta y se llena el DataSet
                        da.Fill(ds);
                    }

                    if (_trans == null)
                    {
                        //Luego de la transaccion se "cierra" la conexion.
                        _conn.Close();
                    }

                    return ds;
                }
            }
            catch (Exception ex)
            {
                if ((_conn != null) && (_conn.State == ConnectionState.Closed))
                {
                    ReConnect();
                }
                if ((_trans != null) && (_trans.Connection != null) && (_trans.Connection.State == ConnectionState.Executing || _trans.Connection.State == ConnectionState.Open))
                {
                    _trans.Rollback();
                    //Luego de la transaccion se "cierra" la conexion.
                    _conn.Close();
                }
                _trans = null;
                throw ex;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

    }

    #region Clases para cifrado

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
}
