using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace RealEstateAgency.Core.Tools
{
    public class Database
    {
        public class ColumnDefinition
        {
            private ColumnAttribute _columnAttribute;
            private PrimaryKeyAttribute _pkAttribute;
            private PropertyInfo _columnProperty;

            public string Name { get { return _columnAttribute?.Name ?? ""; } }
            public bool IsPrimaryKey { get { return _pkAttribute != null; } }
            public PrimaryKeyAttribute PrimaryKeyAttribute { get { return _pkAttribute; } }
            public PropertyInfo Property { get { return _columnProperty; } }

            internal ColumnDefinition(ColumnAttribute attr, PrimaryKeyAttribute pk, PropertyInfo prop)
            {
                _columnAttribute = attr;
                _pkAttribute = pk;
                _columnProperty = prop;
            }
        }

        public static string GetTableName<T>() where T : class
        {
            Type t = typeof(T);
            TypeInfo ti = t.GetTypeInfo();

            string tname = "";

            Attribute attr = ti.GetCustomAttribute(typeof(TableAttribute));
            if (attr != null) tname = ((TableAttribute)attr).Name;
                        
            return tname;
        }

        public static Dictionary<string, ColumnDefinition> GetColumnsList<T>() where T : class
        {
            Dictionary<string, ColumnDefinition> result = new Dictionary<string, ColumnDefinition>();

            Type t = typeof(T);
            foreach (PropertyInfo property in t.GetRuntimeProperties())
            {
                Attribute attr = property.GetCustomAttribute(typeof(ColumnAttribute));
                Attribute pk = property.GetCustomAttribute(typeof(PrimaryKeyAttribute));
                if (attr == null) continue;
                result.Add(((ColumnAttribute)attr).Name, new ColumnDefinition((ColumnAttribute)attr, (PrimaryKeyAttribute)pk, property));
            }
            
            return result;
        }

        public static Dictionary<string, ColumnDefinition> GetPrimaryKeysList<T>() where T : class
        {
            Dictionary<string, ColumnDefinition> result = new Dictionary<string, ColumnDefinition>();

            Type t = typeof(T);
            foreach (PropertyInfo property in t.GetRuntimeProperties())
            {
                Attribute attr = property.GetCustomAttribute(typeof(ColumnAttribute));
                if (attr == null) continue;
                Attribute pk = property.GetCustomAttribute(typeof(PrimaryKeyAttribute));
                if (pk == null) continue;
                result.Add(((ColumnAttribute)attr).Name, new ColumnDefinition((ColumnAttribute)attr, (PrimaryKeyAttribute)pk, property));
            }
            
            return result;
        }

        public static string GenereWhere(List<string> names, List<object> values, bool format = true, bool addIsNullTest = false)
        {
            string query = "";
            if (names == null || names.Count == 0
             || values == null || values.Count == 0
             || names.Count != values.Count) return query;

            bool addAnd = false;
            query += " WHERE ";
            for (int i = 0; i < names.Count; i++)
            {
                if (addAnd) query += " AND ";
                query += "(" + names[i] + "=" + (format ? FormatSQL(values[i]) : values[i].ToString());
                if (addIsNullTest) query += " OR " + names[i] + " is null";
                query += ")";
                addAnd = true;
            }

            return query;
        }

        public static string FormatSQL<TSelf>(TSelf valeur, bool formatNull = false)
        {
            Type tValeur = null;

            if (valeur == null)
            {
                if (formatNull)
                    tValeur = GetDeclaredType(valeur);
                else
                    return "null";
            }
            else
                tValeur = valeur.GetType();

            return FormatSQL(valeur, tValeur);
        }
        private static string FormatSQL(object valeur, Type tValeur)
        {
            string strValeur = "";

            if (tValeur == typeof(DateTime) || tValeur == typeof(DateTime?))
            {
                if (valeur == null) return "null";
                strValeur = string.Format("Datetime('{0}')", ((DateTime)valeur).ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

            }
            else if (tValeur == typeof(string))
            {
                if (valeur == null) return "''";
                strValeur = string.Format("'{0}'", ((string)valeur).Replace("'", "''"));
            }
            else if (tValeur == typeof(char) || tValeur == typeof(char?))
            {
                if (valeur == null) return "''";
                strValeur = string.Format("'{0}'", ((char)valeur).ToString().Replace("'", "''"));
            }
            else if (tValeur == typeof(float) || tValeur == typeof(double) || tValeur == typeof(decimal) || tValeur == typeof(float?) || tValeur == typeof(double?) || tValeur == typeof(decimal?))
            {
                if (valeur == null) return "0";
                strValeur = valeur.ToString().Replace(",", ".");
            }
            else if (tValeur == typeof(bool) || tValeur == typeof(bool?))
            {
                if (valeur == null) return "0";
                strValeur = ((bool)valeur) ? "1" : "0";
            }
            else if (tValeur.GetTypeInfo().IsEnum)
            {
                if (valeur == null) return "-1";
                strValeur = ((int)valeur).ToString();
            }
            else
            {
                if (valeur == null) return "null";
                strValeur = valeur.ToString();
            }

            return strValeur;
        }
        private static Type GetDeclaredType<TSelf>(TSelf self)
        {
            return typeof(TSelf);
        }

    }
}
