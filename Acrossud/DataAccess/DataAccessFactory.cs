using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acrossud;

namespace Acrossud
{
    /// <summary>
    /// Fabrica encargada de crear el objeto DataAccess adecuado
    /// </summary>
    public static class DataAccessFactory
    {
        /// <summary>
        /// Método que crea un objeto DataAccess de acuerdo al tipo especificado (MySql, Sql Server, etc.).
        /// Asigna además el string de conexión con la BD.
        /// </summary>
        /// <param name="type">Enumerado que indica el tipo de DataAccess que se desea obtener</param>
        /// <param name="connStr">String de conexión con la BD</param>
        /// <returns>Un objeto DataAccess del tipo especificado</returns>
        public static DataAccess CreateDataAccess(EnumConst.DataAccessProvider type, string connStr)
        {
            DataAccess dataAccess = null;

            switch (type)
            {
                case EnumConst.DataAccessProvider.SqlServer:
                    dataAccess = new SqlServerDataAccess(connStr);
                    break;
            }

            return dataAccess;
        }
    }
}