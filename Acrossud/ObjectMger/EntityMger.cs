using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acrossud;

namespace Acrossud
{
    public class EntityMger
    {
        #region Fields

        private static EntityMger _instance;
        private static DataAccess _dataAccess;
        private static string _connStr;
        private static EnumConst.DataAccessProvider _databaseProvider; 

        #endregion

        public static void SetConnStrDataBase(EnumConst.DataAccessProvider provider, string conn_str)
        {
            _connStr = conn_str;
            _databaseProvider = provider;
        }
        
        private EntityMger()
        {
            _dataAccess = DataAccessFactory.CreateDataAccess(_databaseProvider, _connStr);
        }

        /// <summary>
        /// Método para obtener la instancia del manager
        /// </summary>
        public static EntityMger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EntityMger();
                }
                return _instance;
            }
        }

        public IEnumerable<Entity> GetEntities()
        {
            List<Entity> result = new List<Entity>();

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            ds = _dataAccess.ExecuteStoreProcedure("GetEntities", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = new List<Entity>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new Entity(dr));
                }
            }

            return result;
        }

        public List<Property> GetProperties()
        {
            List<Property> result = new List<Property>();

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            ds = _dataAccess.ExecuteStoreProcedure("GetProperties", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = new List<Property>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new Property(dr));
                }
            }

            return result;
        }

        public int SaveEntity(Entity entity)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@Name", entity.Name);
            parameters.Add("@Description", entity.Description);

            ds = _dataAccess.ExecuteStoreProcedure("SaveEntity", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }
    }
}
