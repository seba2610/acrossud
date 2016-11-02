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
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new Entity(dr));
                }
            }

            return result;
        }

        public IEnumerable<Entity> GetEntitiesFiltered(string property_name, EnumConst.PropertyOperator op, EnumConst.PropertyValue value)
        {
            List<Entity> result = new List<Entity>();

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@PropertyName", property_name);
            parameters.Add("@PropertyOperator", op.ToString());
            parameters.Add("@Value", value.ToString());

            ds = _dataAccess.ExecuteStoreProcedure("GetEntitiesFiltered", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new Entity(dr));
                }
            }

            return result;
        }

        public Entity GetEntity(int id)
        {
            Entity result = null;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", id);

            ds = _dataAccess.ExecuteStoreProcedure("GetEntity", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = new Entity(ds.Tables[0].Rows[0]);
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

        public List<Property> GetValueOfPropertiesByEntityId(int entity_id)
        {
            List<Property> result = new List<Property>();

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", entity_id);

            ds = _dataAccess.ExecuteStoreProcedure("GetValueOfPropertiesByEntityId", parameters);

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

        public Property GetPropertyByName(string property_name)
        {
            Property result = null;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@Name", property_name);

            ds = _dataAccess.ExecuteStoreProcedure("GetPropertyByName", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = new Property(ds.Tables[0].Rows[0]);
            }

            return result;
        }

        public int CreateEntity(Entity entity)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@Name", entity.Name);
            parameters.Add("@Description", entity.Description);

            ds = _dataAccess.ExecuteStoreProcedure("CreateEntity", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }

        public int UpdateEntity(Entity entity)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", entity.Id);
            parameters.Add("@Name", entity.Name);
            parameters.Add("@Description", entity.Description);

            ds = _dataAccess.ExecuteStoreProcedure("UpdateEntity", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }

        public int AddOrUpdateEntityProperty(EntityProperty ep)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", ep.EntityId);
            parameters.Add("@PropertyId", ep.PropertyId);

            if (ep.Value != null)
                parameters.Add("@Value", ep.Value.ToString());
            else
                parameters.Add("@Value", DBNull.Value);

            ds = _dataAccess.ExecuteStoreProcedure("AddOrUpdateEntityProperty", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }

        public Property GetValueOfPropertyByName(int entity_id, string property_name)
        {
            Property result = null;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntitiyId", entity_id);
            parameters.Add("@PropertyName", property_name);

            ds = _dataAccess.ExecuteStoreProcedure("GetValueOfPropertyByName", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = new Property(ds.Tables[0].Rows[0]);
            }

            return result;
        }

        public int DeleteEntity(int entity_id)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", entity_id);

            ds = _dataAccess.ExecuteStoreProcedure("DeleteEntity", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }

        public int DeleteEntityPropertyByName(int entity_id, string property_name)
        {
            int result = -1;

            Dictionary<string, object> parameters = null;
            DataSet ds = null;

            parameters = new Dictionary<string, object>();
            parameters.Add("@EntityId", entity_id);
            parameters.Add("@PropertyName", property_name);

            ds = _dataAccess.ExecuteStoreProcedure("DeleteEntityPropertyByName", parameters);

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }

            return result;
        }
    }
}
