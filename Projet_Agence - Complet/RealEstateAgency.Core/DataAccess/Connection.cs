using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RealEstateAgency.Core.Tools;
using SQLite.Net.Attributes;

namespace RealEstateAgency.Core.DataAccess
{
    public class Connection //: IDisposable
    {
        private static Connection _connection = null;
        /// <summary>
        /// Permet de récupérer la connexion en cours
        /// </summary>
        /// <returns>Objet permettant de gérer la connexion à la base de données</returns>
        public static Connection GetCurrent(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform = null, 
                                            string dbFilePath = "")
        {
            if (_connection == null || !_connection._connectionInitialized)
            {
                _connection = new Connection();
                _connection.Initialize(sqlitePlatform, dbFilePath, false);
            }

            return _connection;
        }
        /// <summary>
        /// Permet de récupérer la connexion en cours
        /// </summary>
        /// <returns>Objet permettant de gérer la connexion à la base de données</returns>
        public static async Task<Connection> GetCurrentAsync(object sqlitePlatform = null,
                                                             string dbFilePath = "")
        {
            if (_connection == null || !_connection._connectionInitialized)
            {
                _connection = new Connection();
                await _connection.Initialize((SQLite.Net.Interop.ISQLitePlatform)sqlitePlatform, dbFilePath, true);
            }

            return _connection;
        }





        private SQLite.Net.SQLiteConnection _conn;
        private SQLite.Net.Async.SQLiteAsyncConnection _asyncConn;
        private string _databasePath;
        private bool _connectionInitialized;
        private List<string> _errors;

        public string DatabasePath
        {
            get { return _databasePath; }
        }
        public bool ConnectionInitialized
        {
            get { return _connectionInitialized; }
        }
        public List<string> Errors
        {
            get { return _errors; }
        }

        private Connection()
        {
            this._connectionInitialized = false;
            this._databasePath = "";
            this._errors = new List<string>();
        }

        private async Task<bool> Initialize(SQLite.Net.Interop.ISQLitePlatform sqlitePlatform = null,
                                            string dbFilePath = "",
                                            bool async = true)
        {
            _errors.Clear();

            try
            {
                // Si le chemin à la base de données est vide, on propose un chemin par défaut
                if (string.IsNullOrEmpty(dbFilePath))
                {
                    dbFilePath = System.IO.Path.Combine(PCLStorage.FileSystem.Current.LocalStorage.Path, "estate_agency.db");
                }

                // Initialisation de la base de données
                _databasePath = dbFilePath.Replace(@"\", @"/");

                if (async)
                {
                    _asyncConn = new SQLite.Net.Async.SQLiteAsyncConnection(() => new SQLite.Net.SQLiteConnectionWithLock(sqlitePlatform, new SQLite.Net.SQLiteConnectionString(_databasePath, storeDateTimeAsTicks: true)));
                    _conn = null;
                }
                else
                {
                    _asyncConn = null;
                    _conn = new SQLite.Net.SQLiteConnectionWithLock(sqlitePlatform, new SQLite.Net.SQLiteConnectionString(_databasePath, storeDateTimeAsTicks: true));
                }

                this._connectionInitialized = true;
            }
            catch (Exception ex)
            {
                _errors.Add(ex.Message);
                this._connectionInitialized = false;
            }

            return _errors.Count <= 0;
        }

        private bool CheckIfInstanceIsInitialized(bool async)
        {
            if (!this._connectionInitialized)
            {
                this._errors.Add("Base de données non initialisée !");
                return false;
            }

            if ((async && _asyncConn == null) || (!async && _conn == null))
            {
                this._errors.Add("Connexion non ouverte !");
                return false;
            }

            return true;
        }


        #region Synchrone

        public int Insert<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateInsertQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de l'insertion de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return  Execute(query);
                }
                else
                {
                    return  _conn.Insert(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public int InsertOrReplace<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    if (Exists(item))
                    {
                        return Update(item, sqlQuery);
                    }
                    else
                    {
                        return Insert(item, sqlQuery);
                    }
                }
                else
                {
                    return  _conn.InsertOrReplace(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public int Update<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateUpdateQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la mise à jour de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return  Execute(query);
                }
                else
                {
                    if (_conn != null) return _conn.Update(item);
                    return  _conn.Update(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public int Delete<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateDeleteQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la suppression de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return  Execute(query);
                }
                else
                {
                    if (_conn != null) return _conn.Delete(item);
                    return  _conn.Delete(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public int Delete<T>() where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                return  _conn.DeleteAll<T>();
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }

        public long SelectLastInsertedAutoKey<T>()
        {
            return  ExecuteScalar<long>("SELECT last_insert_rowid()");
        }

        public bool Exists<T>(T item) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;

            try
            {
                List<string> pk_names = new List<string>();
                List<object> pk_values = new List<object>();
                foreach (var pkey in Database.GetPrimaryKeysList<T>())
                {
                    pk_names.Add(pkey.Key);
                    pk_values.Add(pkey.Value.Property.GetValue(item));
                }
                if (pk_names.Count != pk_values.Count) return false;

                string query = "SELECT COUNT(*) FROM " + Database.GetTableName<T>() + Database.GenereWhere(pk_names, pk_values);
                return  this.ExecuteScalar<int>(query) > 0;
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return false;
            }
        }
        public int SelectCount<T>() where T : class
        {
            return  SelectCount<T>(null);
        }
        public int SelectCount<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (predicate == null)
                {
                    return  this._conn.Table<T>().Count();
                }
                else
                {
                    return  this._conn.Table<T>().Where(predicate).Count();
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public TValue SelectMin<TItem, TValue>(Expression<Func<TItem, TValue>> minimum) where TItem : class
        {
            return  SelectMin(minimum, null);
        }
        public TValue SelectMin<TItem, TValue>(Expression<Func<TItem, TValue>> minimum, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.TableQuery<TItem> query = this._conn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item =  query.OrderBy(minimum).FirstOrDefault();

                if (item != null) return minimum.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }
        }
        public TValue SelectMax<TItem, TValue>(Expression<Func<TItem, TValue>> maximum) where TItem : class
        {
            return  SelectMax(maximum, null);
        }
        public TValue SelectMax<TItem, TValue>(Expression<Func<TItem, TValue>> maximum, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.TableQuery<TItem> query = this._conn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item =  query.OrderByDescending(maximum).FirstOrDefault();

                if (item != null) return maximum.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }
        }
        public TValue SelectValue<TItem, TValue>(Expression<Func<TItem, TValue>> value) where TItem : class
        {
            return  SelectValue(value, null);
        }
        public TValue SelectValue<TItem, TValue>(Expression<Func<TItem, TValue>> value, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.TableQuery<TItem> query = this._conn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item =  query.FirstOrDefault();

                if (item != null) return value.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }
        }
        public T SelectItem<T>(object pk) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return  this._conn.Find<T>(pk);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }
        }
        public T SelectItem<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return  this._conn.Find(predicate);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }
        }
        public ObservableCollection<T> SelectItems<T>() where T : class
        {
            return  SelectItems<T>(null);
        }
        public ObservableCollection<T> SelectItems<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                ObservableCollection<T> items = new ObservableCollection<T>();

                SQLite.Net.TableQuery<T> query = this._conn.Table<T>();
                if (predicate != null) query = query.Where(predicate);
                foreach (T item in  query.ToList())
                {
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }
        }


        public bool TableExists<T>() where T : class
        {
            try
            {
                string name = Tools.Database.GetTableName<T>();
                if (string.IsNullOrEmpty(name)) return false;
                return TableExists(name);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return false;
            }
        }
        public bool TableExists(string tableName)
        {
            return  ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'") == 1;
        }

        public T ExecuteScalar<T>(string sqlQuery)
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(T);

            try
            {
                return  this._conn.ExecuteScalar<T>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(T);
            }
        }
        public List<T> ExecuteQuery<T>(string sqlQuery) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return  this._conn.Query<T>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }
        }
        public int Execute(string sqlQuery)
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                return  this._conn.Execute(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }

        #endregion


        #region Asynchrone

        public async Task<int> InsertAsync<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateInsertQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de l'insertion de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return await ExecuteAsync(query);
                }
                else
                {
                    return await _asyncConn.InsertAsync(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }   
        }
        public async Task<int> InsertOrReplaceAsync<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    if (await ExistsAsync(item))
                    {
                        return await UpdateAsync(item, sqlQuery);
                    }
                    else
                    {
                        return await InsertAsync(item, sqlQuery);
                    }
                }
                else
                {
                    return await _asyncConn.InsertOrReplaceAsync(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public async Task<int> UpdateAsync<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateUpdateQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la mise à jour de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return await ExecuteAsync(query);
                }
                else
                {
                    if (_conn != null) return _conn.Update(item);
                    return await _asyncConn.UpdateAsync(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public async Task<int> DeleteAsync<T>(T item, bool sqlQuery = false) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (sqlQuery)
                {
                    string query = GenerateDeleteQuery(item);
                    if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la suppression de données dans la table '{0}'.\n\rRequête :\n\r{1}", Database.GetTableName<T>(), query));
                    return await ExecuteAsync(query);
                }
                else
                {
                    if (_conn != null) return _conn.Delete(item);
                    return await _asyncConn.DeleteAsync(item);
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public async Task<int> DeleteAsync<T>() where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                return await _asyncConn.DeleteAllAsync<T>();
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }            
        }

        public async Task<long> SelectLastInsertedAutoKeyAsync<T>()
        {
            return await ExecuteScalarAsync<long>("SELECT last_insert_rowid()");
        }

        public async Task<bool> ExistsAsync<T>(T item) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;

            try
            {
                List<string> pk_names = new List<string>();
                List<object> pk_values = new List<object>();
                foreach (var pkey in Database.GetPrimaryKeysList<T>())
                {
                    pk_names.Add(pkey.Key);
                    pk_values.Add(pkey.Value.Property.GetValue(item));
                }
                if (pk_names.Count != pk_values.Count) return false;

                string query = "SELECT COUNT(*) FROM " + Database.GetTableName<T>() + Database.GenereWhere(pk_names, pk_values);
                return await this.ExecuteScalarAsync<int>(query) > 0;
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return false;
            }
        }
        public async Task<int> SelectCountAsync<T>() where T : class
        {
            return await SelectCountAsync<T>(null);
        }
        public async Task<int> SelectCountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                if (predicate == null)
                {
                    return await this._asyncConn.Table<T>().CountAsync();
                }
                else
                {
                    return await this._asyncConn.Table<T>().Where(predicate).CountAsync();
                }
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }
        public async Task<TValue> SelectMinAsync<TItem, TValue>(Expression<Func<TItem, TValue>> minimum) where TItem : class
        {
            return await SelectMinAsync(minimum, null);
        }
        public async Task<TValue> SelectMinAsync<TItem, TValue>(Expression<Func<TItem, TValue>> minimum, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.Async.AsyncTableQuery<TItem> query = this._asyncConn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item = await query.OrderBy(minimum).FirstOrDefaultAsync();
   
                if (item != null) return minimum.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }           
        }
        public async Task<TValue> SelectMaxAsync<TItem, TValue>(Expression<Func<TItem, TValue>> maximum) where TItem : class
        {
            return await SelectMaxAsync(maximum, null);
        }
        public async Task<TValue> SelectMaxAsync<TItem, TValue>(Expression<Func<TItem, TValue>> maximum, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.Async.AsyncTableQuery<TItem> query = this._asyncConn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item = await query.OrderByDescending(maximum).FirstOrDefaultAsync();

                if (item != null) return maximum.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }
        }
        public async Task<TValue> SelectValueAsync<TItem, TValue>(Expression<Func<TItem, TValue>> value) where TItem : class
        {
            return await SelectValueAsync(value, null);
        }
        public async Task<TValue> SelectValueAsync<TItem, TValue>(Expression<Func<TItem, TValue>> value, Expression<Func<TItem, bool>> where) where TItem : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(TValue);

            try
            {
                TItem item = null;

                SQLite.Net.Async.AsyncTableQuery<TItem> query = this._asyncConn.Table<TItem>();
                if (where != null) query = query.Where(where);
                item = await query.FirstOrDefaultAsync();

                if (item != null) return value.Compile()(item);

                return default(TValue);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(TValue);
            }
        }
        public async Task<T> SelectItemAsync<T>(object pk) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return await this._asyncConn.FindAsync<T>(pk);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }            
        }
        public async Task<T> SelectItemAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return await this._asyncConn.FindAsync(predicate);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }            
        }
        public async Task<ObservableCollection<T>> SelectItemsAsync<T>() where T : class
        {
            return await SelectItemsAsync<T>(null);
        }
        public async Task<ObservableCollection<T>> SelectItemsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                ObservableCollection<T> items = new ObservableCollection<T>();

                SQLite.Net.Async.AsyncTableQuery<T> query = this._asyncConn.Table<T>();
                if (predicate != null) query = query.Where(predicate);
                foreach (T item in await query.ToListAsync())
                {
                    items.Add(item);
                }

                return items;
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }
        }


        public async Task<bool> TableExistsAsync<T>() where T : class
        {
            try
            {
                string name = Tools.Database.GetTableName<T>();
                if (string.IsNullOrEmpty(name)) return false;
                return await TableExistsAsync(name);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return false;
            }
        }
        public async Task<bool> TableExistsAsync(string tableName)
        {
            return await ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'") == 1;
        }

        public async Task<T> ExecuteScalarAsync<T>(string sqlQuery)
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return default(T);

            try
            {
                return await this._asyncConn.ExecuteScalarAsync<T>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return default(T);
            }            
        }
        public async Task<List<T>> ExecuteQueryAsync<T>(string sqlQuery) where T : class
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return null;

            try
            {
                return await this._asyncConn.QueryAsync<T>(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return null;
            }            
        }
        public async Task<int> ExecuteAsync(string sqlQuery)
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return -1;

            try
            {
                return await this._asyncConn.ExecuteAsync(sqlQuery);
            }
            catch (Exception ex)
            {
                this._errors.Add(ex.Message);
                return -1;
            }
        }

        #endregion


        #region Initialisation de la base de données

        public bool InitializeDatabase(Interfaces.IInfoService info, bool reloadData = false)
        {
            List<string> errors = new List<string>();

            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;


            // Lecture de la version de la base
            if (TableExists<Model.Parameter>())
            {
                Model.Parameter param = SelectItem((Model.Parameter p) => p.Key == Model.Parameter.PARAMETER_KEY_DB_VERSION);
                if (param != null && param.Value == info.AppVersionName)
                {
                    return true;
                }
            }


            // Création des tables
            //----------------------------------------------------------------------------------

            // Table des paramètres
            this.CreateTable<Model.Parameter>();
            errors.AddRange(_errors);
            // Table des personnes
            this.CreateTable<Model.Person>();
            errors.AddRange(_errors);
            // Table des biens immobiliers
            this.CreateTable<Model.Estate>();
            errors.AddRange(_errors);
            // Table des photos
            this.CreateTable<Model.Photo>();
            errors.AddRange(_errors);
            // Table des transaction
            this.CreateTable<Model.Transaction>();
            errors.AddRange(_errors);

            //----------------------------------------------------------------------------------
            // TODO : Ajouter le code de création des tables
            //----------------------------------------------------------------------------------




            // Mise à jour de la version de la base de données
            Model.Parameter pVersion = new Model.Parameter();
            pVersion.Key = Model.Parameter.PARAMETER_KEY_DB_VERSION;
            pVersion.Value = info.AppVersionName;
            InsertOrReplace(pVersion);

            // Mise à jour des erreurs
            this._errors.Clear();
            this._errors.AddRange(errors);

            return this._errors.Count == 0;
        }
        public async Task<bool> InitializeDatabaseAsync(Interfaces.IInfoService info, bool reloadData = false)
        {
            List<string> errors = new List<string>();

            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;


            // Lecture de la version de la base
            if (await TableExistsAsync<Model.Parameter>())
            {
                Model.Parameter param = await SelectItemAsync((Model.Parameter p) => p.Key == Model.Parameter.PARAMETER_KEY_DB_VERSION);
                if (param != null && param.Value == info.AppVersionName)
                {
                    return true;
                }
            }
            

            // Création des tables
            //----------------------------------------------------------------------------------

            // Table des paramètres
            await this.CreateTableAsync<Model.Parameter>();
            errors.AddRange(_errors);
            // Table des personnes
            await this.CreateTableAsync<Model.Person>();
            errors.AddRange(_errors);
            // Table des biens immobiliers
            await this.CreateTableAsync<Model.Estate>();
            errors.AddRange(_errors);
            // Table des photos
            await this.CreateTableAsync<Model.Photo>();
            errors.AddRange(_errors);
            // Table des transaction
            await this.CreateTableAsync<Model.Transaction>();
            errors.AddRange(_errors);

            //----------------------------------------------------------------------------------
            // TODO : Ajouter le code de création des tables
            //----------------------------------------------------------------------------------




            // Mise à jour de la version de la base de données
            Model.Parameter pVersion = new Model.Parameter();
            pVersion.Key = Model.Parameter.PARAMETER_KEY_DB_VERSION;
            pVersion.Value = info.AppVersionName;
            await InsertOrReplaceAsync(pVersion);

            // Mise à jour des erreurs
            this._errors.Clear();
            this._errors.AddRange(errors);

            return this._errors.Count == 0;
        }


        protected bool CreateTable<T>() where T : class, new()
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;

            // Si la table existe déjà
            string name = Database.GetTableName<T>();
            string tmp_name = "";
            if (TableExists<T>())
            {
                // On en crée une copie
                tmp_name = name + "_tmp";
                Execute("CREATE TABLE '" + tmp_name + "' AS SELECT * FROM " + name);

                // Et on supprime la table actuelle
                Execute("DROP TABLE '" + name + "'");
            }

            // On génère la requête de création de la table
            string query = GenerateCreateTableQuery<T>(true);
            if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la génération de la requête de création de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

            // On crée la table
            Execute(query);
            if (!TableExists<T>()) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la création de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

            // Si une copie existe
            if (!string.IsNullOrEmpty(tmp_name))
            {
                // On génère la requête de copie des lignes depuis la table temporaire

                query = GenerateCopyTableQuery<T>(tmp_name, name);
                if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la génération de la requête de copie de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

                // On recopie les lignes depuis la table temporaire
                Execute(query);

                // On supprime la table temporaire
                Execute("DROP TABLE '" + tmp_name + "'");
            }

            return _errors.Count == 0;
        }
        protected async Task<bool> CreateTableAsync<T>() where T : class, new()
        {
            this._errors.Clear();
            if (!CheckIfInstanceIsInitialized(true)) return false;

            // Si la table existe déjà
            string name = Database.GetTableName<T>();
            string tmp_name = "";
            if (await TableExistsAsync<T>())
            {
                // On en crée une copie
                tmp_name = name + "_tmp";
                await ExecuteAsync("CREATE TABLE '" + tmp_name + "' AS SELECT * FROM " + name);

                // Et on supprime la table actuelle
                await ExecuteAsync("DROP TABLE '" + name + "'");
            }

            // On génère la requête de création de la table
            string query = GenerateCreateTableQuery<T>(true);
            if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la génération de la requête de création de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

            // On crée la table
            await ExecuteAsync(query);
            if (!await TableExistsAsync<T>()) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la création de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

            // Si une copie existe
            if (!string.IsNullOrEmpty(tmp_name))
            {
                // On génère la requête de copie des lignes depuis la table temporaire

                query = GenerateCopyTableQuery<T>(tmp_name, name);
                if (string.IsNullOrEmpty(query)) throw new Exception(string.Format("Une ou plusieurs erreurs se sont produites lors de la génération de la requête de copie de la table '{0}'.\n\rRequête :\n\r{1}", name, query));

                // On recopie les lignes depuis la table temporaire
                await ExecuteAsync(query);

                // On supprime la table temporaire
                await ExecuteAsync("DROP TABLE '" + tmp_name + "'");
            }

            return _errors.Count == 0;
        }

        #endregion


        #region Génération de requêtes

        internal string GenerateCreateTableQuery<T>(bool allowAutoIncrement) where T : class, new()
        {
            string tableName = Database.GetTableName<T>();
            if (string.IsNullOrEmpty(tableName)) return "";

            int nbProp = 0;
            List<string> pkeys = new List<string>();
            string query = "CREATE TABLE '" + tableName + "' (";

            foreach (var col in Database.GetColumnsList<T>())
            {
                string name = "", type = "", notnull = "", increment = "";
                Attribute attr = null;

                // Column
                name = col.Value.Name;
                if (string.IsNullOrEmpty(name)) continue;

                // Type
                Type pt = col.Value.Property.PropertyType;
                if (pt.GenericTypeArguments.Length > 0) pt = pt.GenericTypeArguments[0];

                if (pt == typeof(int) || pt == typeof(long) || pt == typeof(short) || pt.GetTypeInfo().BaseType == typeof(Enum))
                    type = "integer";
                else if (pt == typeof(float) || pt == typeof(double))
                    type = "real";
                else if (pt == typeof(decimal))
                    type = "numeric";
                else if (pt == typeof(bool))
                    type = "boolean";
                else if (pt == typeof(DateTime))
                    type = "datetime";
                else if (pt == typeof(string))
                    type = "text";
                else
                    continue;

                // NotNull
                attr = col.Value.Property.GetCustomAttribute(typeof(NotNullAttribute));
                if (attr != null) notnull = "not null";

                // PrimaryKey et AutoIncrement
                attr = col.Value.PrimaryKeyAttribute;
                if (attr != null)
                {
                    if (allowAutoIncrement)
                    {
                        attr = col.Value.Property.GetCustomAttribute(typeof(AutoIncrementAttribute));
                        if (attr != null) increment = " autoincrement";
                    }
                    pkeys.Add(name + increment);
                    increment = "";
                }

                // Génération de la ligne
                if (nbProp > 0) query += ",";
                query += "'" + name + "' " + type + (notnull == "" ? "" : " " + notnull) + (increment == "" ? "" : " " + increment);
                nbProp++;
            }

            string pkey = "";
            if (pkeys.Count > 0)
            {
                pkey = ",PRIMARY KEY (";
                for (int i = 0; i < pkeys.Count; i++)
                {
                    if (i > 0) pkey += ",";
                    pkey += pkeys[i];
                }
                pkey += ")";
            }

            query += pkey + ")";

            if (nbProp == 0) return "";
            return query;
        }

        internal string GenerateInsertQuery<T>(T item) where T : class
        {
            string tname = Database.GetTableName<T>();
            if (string.IsNullOrEmpty(tname)) return "";

            string col_names = "";
            string col_values = "";

            foreach (var col in Database.GetColumnsList<T>())
            {
                if (col.Value.IsPrimaryKey && col.Value.Property.GetCustomAttribute(typeof(AutoIncrementAttribute)) != null) continue;
                if (!string.IsNullOrEmpty(col_names)) col_names += ",";
                col_names += col.Key;

                object value = null;
                if (typeof(T) == col.Value.Property.DeclaringType || typeof(T).IsChildOf(col.Value.Property.DeclaringType))
                {
                    value = col.Value.Property.GetValue(item);
                }
                else
                {
                    object model = typeof(T).GetRuntimeProperty("Model")?.GetValue(item) ?? null;
                    if (model != null) value = col.Value.Property.GetValue(model);
                }

                if (!string.IsNullOrEmpty(col_values)) col_values += ",";
                col_values += Database.FormatSQL(value);
            }

            return "INSERT INTO " + tname + " (" + col_names + ")  VALUES (" + col_values + ")";
        }

        internal string GenerateUpdateQuery<T>(T item) where T : class
        {
            string tname = Database.GetTableName<T>();
            if (string.IsNullOrEmpty(tname)) return "";

            string col_affectations = "";
            string key_tests = "";

            foreach (var col in Database.GetColumnsList<T>())
            {
                if (col.Value.IsPrimaryKey)
                {
                    if (!string.IsNullOrEmpty(key_tests)) key_tests += " AND ";
                    key_tests += col.Key;

                    object value = null;
                    if (typeof(T) == col.Value.Property.DeclaringType || typeof(T).IsChildOf(col.Value.Property.DeclaringType))
                    {
                        value = col.Value.Property.GetValue(item);
                    }
                    else
                    {
                        object model = typeof(T).GetRuntimeProperty("Model")?.GetValue(item) ?? null;
                        if (model != null) value = col.Value.Property.GetValue(model);
                    }

                    if (value == null)
                        key_tests += " is null";
                    else
                        key_tests += "=" + Database.FormatSQL(value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(col_affectations)) col_affectations += ",";
                    col_affectations += col.Key + "=";

                    object value = null;
                    if (typeof(T) == col.Value.Property.DeclaringType || typeof(T).IsChildOf(col.Value.Property.DeclaringType))
                    {
                        value = col.Value.Property.GetValue(item);
                    }
                    else
                    {
                        object model = typeof(T).GetRuntimeProperty("Model")?.GetValue(item) ?? null;
                        if (model != null) value = col.Value.Property.GetValue(model);
                    }

                    col_affectations += Database.FormatSQL(value);
                }

            }

            return "UPDATE " + tname + " SET " + col_affectations + " WHERE " + key_tests;
        }

        internal string GenerateDeleteQuery<T>(T item) where T : class
        {
            string tname = Database.GetTableName<T>();
            if (string.IsNullOrEmpty(tname)) return "";

            string key_tests = "";

            foreach (var col in Database.GetPrimaryKeysList<T>())
            {
                if (!string.IsNullOrEmpty(key_tests)) key_tests += " AND ";
                key_tests += col.Key;

                object value = null;
                if (typeof(T) == col.Value.Property.DeclaringType || typeof(T).IsChildOf(col.Value.Property.DeclaringType))
                {
                    value = col.Value.Property.GetValue(item);
                }
                else
                {
                    object model = typeof(T).GetRuntimeProperty("Model")?.GetValue(item) ?? null;
                    if (model != null) value = col.Value.Property.GetValue(model);
                }

                if (value == null)
                    key_tests += " is null";
                else
                    key_tests += "=" + Database.FormatSQL(value);
            }

            return "DELETE FROM " + tname + " WHERE " + key_tests;
        }

        internal string GenerateCopyTableQuery<T>(string originName, string destName, List<string> colNames = null, List<object> colValues = null, List<string> whereColNames = null, List<object> whereColValues = null, bool format = true, bool includeAutoIncrement = false) where T : class
        {
            Type t = typeof(T);

            if (_conn == null) return "";

            // Lecture des colonnes existantes dans l'origine
            var originCols = _conn.GetTableInfo(originName);
            // Lecture des colonnes existantes dans la destination
            var destCols = _conn.GetTableInfo(destName);

            int nbProp = 0;
            string cols = "", colsv = "";
            foreach (PropertyInfo p in t.GetRuntimeProperties())
            {
                if (!GenerateColumnsList(p, originCols, destCols, includeAutoIncrement, ref nbProp, ref cols, ref colsv)) continue;
            }
            
            if (colNames != null && colValues != null && colNames.Count == colValues.Count)
            {
                for (int i = 0; i < colNames.Count; i++)
                {
                    // Génération de la ligne
                    if (nbProp > 0)
                    {
                        cols += ",";
                        colsv += ",";
                    }
                    cols += colNames[i];
                    colsv += (format ? Database.FormatSQL(colValues[i]) : colValues[i].ToString());
                    nbProp++;
                }
            }

            if (nbProp == 0) return "";

            return "INSERT INTO '" + destName + "' (" + cols + ")  SELECT " + colsv + " FROM '" + originName + "'" + Database.GenereWhere(whereColNames, whereColValues, format);
        }
        private bool GenerateColumnsList(PropertyInfo p, List<SQLite.Net.SQLiteConnection.ColumnInfo> originCols, List<SQLite.Net.SQLiteConnection.ColumnInfo> destCols, bool includeAutoIncrement,
                                         ref int nbProp, ref string cols, ref string colsv)
        {
            Attribute attr = null;
            string name = "";

            // Column
            attr = p.GetCustomAttribute(typeof(ColumnAttribute));
            if (attr == null) attr = p.GetCustomAttribute(typeof(ColumnAttribute));
            if (attr == null) return false;

            name = ((ColumnAttribute)attr).Name;
            if (string.IsNullOrEmpty(name)) return false;

            // AutoIncrement
            if (!includeAutoIncrement)
            {
                attr = p.GetCustomAttribute(typeof(AutoIncrementAttribute));
                if (attr != null) return false;
            }

            // Vérification de l'existence des deux côtés
            if (!originCols.Contains(item => item.Name.ToLower() == name.ToLower()) || !destCols.Contains(item => item.Name.ToLower() == name.ToLower()))
                return false;

            // Génération de la ligne
            if (nbProp > 0)
            {
                cols += ",";
                colsv += ",";
            }
            cols += name;
            colsv += name;
            nbProp++;

            return true;
        }

        #endregion

    }
}
