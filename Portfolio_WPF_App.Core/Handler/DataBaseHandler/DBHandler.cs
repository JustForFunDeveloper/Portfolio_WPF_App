using Portfolio_WPF_App.Core.Handler.DataBaseHandler.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Portfolio_WPF_App.Core.Handler.DataBaseHandler
{
    /// <summary>
    /// A class to define the tables in a database.
    /// </summary>
    public class Table
    {
        private long _rowsToDelete;
        private bool _deleteThreadActive = false;

        /// <summary>
        /// The table name.
        /// </summary>
        public string TableName;
        /// <summary>
        /// A list of all columns with the prefered data type.
        /// </summary>
        public Dictionary<string, Type> Columns;
        /// <summary>
        /// Max allowed rows for this table. The DBHandler clean thread will delete a percentage of the old rows.
        /// </summary>
        public long MaxRows;
        /// <summary>
        /// To safe the amount of RowsToDelete if the delete thread is active.
        /// </summary>
        public long RowsToDelete { get => _rowsToDelete; set => _rowsToDelete = value; }
        /// <summary>
        /// Only one deleteThread should be activated at a time.
        /// </summary>
        public bool DeleteThreadActive { get => _deleteThreadActive; set => _deleteThreadActive = value; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="tableName"><see cref="TableName"/></param>
        /// <param name="columns"><see cref="Columns"/></param>
        /// <param name="maxRows"><see cref="MaxRows"/></param>
        public Table(string tableName, Dictionary<string, Type> columns, long maxRows)
        {
            TableName = tableName;
            Columns = columns;
            MaxRows = maxRows;
        }
    }

    /// <summary>
    /// Enum to declare the db type.
    /// </summary>
    public enum DataBaseType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        SQLITE,
        MSSQL,
        MYSQL
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// An easy to use DBHandler class which allows to choose between multiple Database Types.
    /// </summary>
    public class DBHandler
    {
        #region Private Members
        private AbstractDBHandler _abstractDBHandler;
        private string _dbName;
        private string _dbPath;
        private Dictionary<string, Table> _tables;
        private DataBaseType _dataBaseType;
        private System.Timers.Timer _deleteTimer;
        private bool _deleteTimerStatus = false;

        #endregion

        #region Public Members
        /// <summary>
        /// The name of the current database.
        /// </summary>
        public string DbName { get => _dbName; }
        /// <summary>
        /// Path for the current DB File.
        /// SQLite Only!
        /// </summary>
        public string DbPath { get => _dbPath; }
        /// <summary>
        /// A Dictoionary of tables whch should be created in the database if necessary.
        /// </summary>
        public Dictionary<string, Table> Tables { get => _tables; }
        /// <summary>
        /// The current database type.
        /// </summary>
        public DataBaseType DataBaseType { get => _dataBaseType; }
        /// <summary>
        /// The maximum size of rows which should be deleted at once.
        /// </summary>
        public long MaxDeleteRowSize { get => _abstractDBHandler.MaxDeleteRowSize; set => _abstractDBHandler.MaxDeleteRowSize = value; }

        /// <summary>
        /// Subscription for the FetchEevents when <see cref="FetchQuery(string, List{KeyValuePair{int, Type}}, int)"/> is used.
        /// </summary>
        public event EventHandler<FetchArgs> FetchEvent
        {
            add
            {
                if (_abstractDBHandler != null)
                    _abstractDBHandler.FetchEvent += value;
                else
                    throw new NullReferenceException("SQLiteHandler not instantiated");
            }
            remove
            {
                if (_abstractDBHandler != null)
                    _abstractDBHandler.FetchEvent -= value;
                else
                    throw new NullReferenceException("SQLiteHandler not instantiated");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="dbName"><see cref="DbName"/></param>
        /// <param name="dbPath"><see cref="DbPath"/></param>
        /// <param name="dataBaseType"><see cref="DataBaseType"/></param>
        /// <param name="createIfNotExists">If true creates a database if it doesn't exist allready.</param>
        public DBHandler(string dbName, string dbPath, DataBaseType dataBaseType, bool createIfNotExists = true)
        {
            _dbName = dbName;
            _dbPath = dbPath;
            _dataBaseType = dataBaseType;
            _tables = new Dictionary<string, Table>();

            try
            {
                ConnectToDatabase(DataBaseType, createIfNotExists);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Basic constructor and creates the given tables.
        /// </summary>
        /// <param name="dbName"><see cref="DbName"/></param>
        /// <param name="dbPath"><see cref="DbPath"/></param>
        /// <param name="tables"><see cref="Tables"/></param>
        /// <param name="dataBaseType"><see cref="DataBaseType"/></param>
        /// <param name="createIfNotExists">If true creates a database if it doesn't exist allready.</param>
        public DBHandler(string dbName, string dbPath, List<Table> tables, DataBaseType dataBaseType, bool createIfNotExists = true)
        {
            _dbName = dbName;
            _dbPath = dbPath;
            _tables = tables.ToDictionary(t => t.TableName);
            _dataBaseType = dataBaseType;

            try
            {
                ConnectToDatabase(DataBaseType, createIfNotExists);
                _abstractDBHandler.CreateTables(_tables);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds tables and creates them at the given database.
        /// </summary>
        /// <param name="tables">A list of tables to add to the database.</param>
        public void AddTables(List<Table> tables)
        {
            try
            {
                foreach (Table table in tables)
                {
                    _tables.Add(table.TableName, table);
                }
                _abstractDBHandler.AddTables(_tables);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Drops the tables with the given table names.
        /// </summary>
        /// <param name="tableNames">A list of table names to delete.</param>
        public void DropTables(List<string> tableNames)
        {
            foreach (var tableName in tableNames)
            {
                if (!_tables.ContainsKey(tableName))
                    throw new KeyNotFoundException("Table doesn't exists. Drop table exited without changes to the database!");
            }
            foreach (var tableName in tableNames)
            {
                _tables.Remove(tableName);
            }
            _abstractDBHandler.DropTables(tableNames);
        }

        /// <summary>
        /// Inserts the given objects into the given table. The first object "id" is ignored due to the auto increment,
        /// </summary>
        /// <param name="tableName">The name of the table to insert rows to.</param>
        /// <param name="rows">A list of rows with all column objects to insert.</param>
        public void InsertIntoTable(string tableName, List<List<object>> rows)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");
            if (!rows.First().Count().Equals(_tables[tableName].Columns.Count))
                throw new InvalidDataException("Table row size doesn't fit with the given table");

            _abstractDBHandler.InsertIntoTable(tableName, _tables, rows);
        }

        /// <summary>
        /// Updates the given columns with the given id in the first column.
        /// Each row of rowsToUpdate must have the same size as rowsData.
        /// </summary>
        /// <param name="tableName">The table where rows should be updated.</param>
        /// <param name="rowsToUpdate">The rows with the name and data type to update.</param>
        /// <param name="rowsData">The rows with all column data which should be updated.</param>
        public void UpdateTable(string tableName, List<Dictionary<string, Type>> rowsToUpdate, List<List<object>> rowsData)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");

            if (!rowsToUpdate.Count.Equals(rowsData.Count))
                throw new InvalidDataException("rowsToUpdate size does'nt match rowsData size");

            _abstractDBHandler.UpdateTable(tableName, rowsToUpdate, rowsData);
        }

        /// <summary>
        /// Get's the last n rows from the specified table.
        /// </summary>
        /// <param name="tableName">The table name to get the data from.</param>
        /// <param name="rows">number of the rows to display.</param>
        /// <returns></returns>
        public List<List<object>> GetLastNRowsFromTable(string tableName, int rows = 100)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");

            Table table = _tables[tableName];

            return _abstractDBHandler.GetLastNRowsFromTable(table, rows);
        }

        /// <summary>
        /// Gets all rows in the given DateTime slot.
        /// </summary>
        /// <param name="tableName">The name of the table to get the data from.</param>
        /// <param name="DateTimeColumnName">The name of the column with the DateTime values.</param>
        /// <param name="from">A DateTime object with the beginning of the timeslot.</param>
        /// <param name="until">A DateTime object with the end of the timeslot.</param>
        /// <returns></returns>
        public List<List<object>> GetRowsFromTableWithTime(string tableName, string DateTimeColumnName, DateTime from, DateTime until)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");

            Table table = _tables[tableName];

            if (table.Columns[DateTimeColumnName] != typeof(DateTime))
                throw new TypeLoadException("DateTimeColumn isn't a DateTime column!");

            return _abstractDBHandler.GetRowsFromTableWithTime(table, DateTimeColumnName, from, until);
        }

        /// <summary>
        /// Gets all rows in the given id slot.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<List<object>> GetRowsFromTableWithIndex(string tableName, int start, int end)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");

            Table table = _tables[tableName];

            return _abstractDBHandler.GetRowsFromTableWithIndex(table, start, end);
        }

        /// <summary>
        /// Deletes the last n rows of the given table.
        /// </summary>
        /// <param name="tableName">The name of the table to delete the last n data from.</param>
        /// <param name="rows">The amount of data to delete.</param>
        public void DeleteLastNRows(string tableName, int rows)
        {
            if (!_tables.ContainsKey(tableName))
                throw new KeyNotFoundException("Table doesn't exist!");

            if (rows > _abstractDBHandler.MaxDeleteRowSize)
                throw new InvalidDataException("rows max size: " + _abstractDBHandler.MaxDeleteRowSize);

            _abstractDBHandler.DeleteLastNRows(_tables[tableName], rows);
        }

        /// <summary>
        /// A Timer which will be called at a given interval and calls the OnDeleteTimer method.
        /// </summary>
        /// <param name="millis">The interval of the check in hours.</param>
        public void StartDeleteThread(int millis)
        {
            //TODO Rework delete thread.
            throw new NotImplementedException();

            if (_deleteTimerStatus)
                throw new InvalidOperationException("Timer already started!");

            _deleteTimer = new System.Timers.Timer(millis);
            _deleteTimer.Elapsed += OnDeleteTimer;
            _deleteTimer.AutoReset = true;
            _deleteTimer.Enabled = true;
            _deleteTimerStatus = true;
        }

        /// <summary>
        /// Stops the delete timer and all active threads.
        /// </summary>
        public void StopDeleteThread()
        {
            throw new NotImplementedException();

            if (!_deleteTimerStatus)
                throw new InvalidOperationException("Timer not yet started!");

            _deleteTimer.Stop();
            _deleteTimer.Dispose();
            _abstractDBHandler.StopDeleteThread();
        }

        #region Low Level Methods
        /// <summary>
        /// <see cref="AbstractDBHandler.CommitQuery(string)"/>
        /// </summary>
        /// <param name="query"></param>
        public void CommitQuery(string query)
        {
            _abstractDBHandler.CommitQuery(query);
        }

        /// <summary>
        /// <see cref="AbstractDBHandler.CommitBatchQuery(List{string})"/>
        /// </summary>
        /// <param name="queryList"></param>
        public void CommitBatchQuery(List<string> queryList)
        {
            _abstractDBHandler.CommitBatchQuery(queryList);
        }

        /// <summary>
        /// <see cref="AbstractDBHandler.ReadQuery(string, List{KeyValuePair{int, Type}})"/>
        /// </summary>
        public List<List<object>> ReadQuery(string query, List<KeyValuePair<int, Type>> columns)
        {
            return _abstractDBHandler.ReadQuery(query, columns);
        }

        /// <summary>
        /// <see cref="AbstractDBHandler.FetchQuery(string, List{KeyValuePair{int, Type}}, int)"/>
        /// </summary>
        public void FetchQuery(string query, List<KeyValuePair<int, Type>> columns, int fetchsize)
        {
            _abstractDBHandler.FetchQuery(query, columns, fetchsize);
        }

        /// <summary>
        /// <see cref="AbstractDBHandler.CancelFetch"/>
        /// </summary>
        public void CancelFetch()
        {
            _abstractDBHandler.CancelFetch();
        }

        /// <summary>
        /// <see cref="AbstractDBHandler.NextFetch"/>
        /// </summary>
        public void NextFetch()
        {
            _abstractDBHandler.NextFetch();
        }
        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// Connects to given <see cref="DataBaseType"/>
        /// </summary>
        /// <param name="dataBaseType">The database type to connect to.</param>
        /// <param name="createIfNotExists">If true will create the database if it doesn't exist.</param>
        private void ConnectToDatabase(DataBaseType dataBaseType, bool createIfNotExists)
        {
            switch (dataBaseType)
            {
                case DataBaseType.SQLITE:
                    ConnectToSQLiteDB(createIfNotExists);
                    break;
                case DataBaseType.MSSQL:
                    throw new NotSupportedException("MSSQL Database not implemented yet!");
                case DataBaseType.MYSQL:
                    throw new NotSupportedException("MYSQL Database not implemented yet!");
                default:
                    throw new NotSupportedException("Wrong Database type!");
            }
        }

        /// <summary>
        /// Checks the createIfNotExists flag and connects to the database.
        /// </summary>
        /// <param name="createIfNotExists">If true will create the database if it doesn't exist.</param>
        private void ConnectToSQLiteDB(bool createIfNotExists)
        {
            if (_dbPath.Equals(""))
                _dbPath = Directory.GetCurrentDirectory();
            else if (!Directory.Exists(_dbPath))
                throw new DirectoryNotFoundException("Invalid path!");

            string fullPath = _dbPath + "\\" + _dbName;
            if (!createIfNotExists)
                if (!File.Exists(fullPath))
                    throw new FileNotFoundException("Can't find file!");

            _abstractDBHandler = new SQLiteHandler("Data Source = " + fullPath + "; Version = 3;");
        }

        /// <summary>
        /// Gets called if the Timer.Elapsed event is raised.
        /// If the current rowCount value is higher then the MaxRows value in each table a deleteThread will be started.
        /// Now the size of the table will be shrinked to 70% of the MaxRows value. The oldest values will be deleted.
        /// If the amount of the rows to be deleted is higher than the global _maxDeleteRowSize then only every 5 seconds the highest amount possible
        /// will be deleted until the the initial calculated RowsToDelete, which is stored in the Table object, is 0.
        /// The thread is also stored in a global Dictionary so it can be aborted.
        /// Every table can only have one DeleteThread active which is marked with the DeleteThreadActive variable.
        ///  <para/>
        /// Example: <para/>
        /// If you have 50 000 000 as MaxRows defined. Then 70% would be 35 000 000 rows. So 15 000 000 would have to be deleted.
        /// If youre _maxDeleteRowSize is 100 000 this would mean 150 turns every 5 seconds. This would take then round about 12,5 minutes to delete all of them.
        /// If you calculated with 500 entries per second. This would only mean 375 000 entries in these 12,5 minutes.
        /// So this should be enough time to delete all entries with plenty of time in between for other sources to write to the database.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleteTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
            _abstractDBHandler.OnDeleteTimer(_tables);
        }
        #endregion
    }
}
