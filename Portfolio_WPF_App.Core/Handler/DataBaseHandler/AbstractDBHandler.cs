using System;
using System.Collections.Generic;

namespace Portfolio_WPF_App.Core.Handler.DataBaseHandler
{
    /// <summary>
    /// This is the abstract base class for all DBHandler
    /// </summary>
    public abstract class AbstractDBHandler
    {
        private long _maxDeleteRowSize = 100000;

        public long MaxDeleteRowSize { get => _maxDeleteRowSize; set => _maxDeleteRowSize = value; }

        /// <summary>
        /// Creates all saved tables in the database.
        /// </summary>
        /// <param name="tables"></param>
        public abstract void CreateTables(Dictionary<string, Table> tables);

        /// <summary>
        /// Adds tables and creates them at the given database.
        /// </summary>
        /// <param name="tables">A list of tables to add to the database.</param>
        public abstract void AddTables(Dictionary<string, Table> tables);

        /// <summary>
        /// Drops the tables with the given table names.
        /// </summary>
        /// <param name="tableNames">A list of table names to delete.</param>
        public abstract void DropTables(List<string> tableNames);

        /// <summary>
        /// Inserts the given objects into the given table. The first object "id" is ignored due to the auto increment,
        /// </summary>
        /// <param name="tableName">The name of the table to insert rows to.</param>
        /// <param name="rows">A list of rows with all column objects to insert.</param>
        /// <param name="tables">All saved tables.</param>
        public abstract void InsertIntoTable(string tableName, Dictionary<string, Table> tables, List<List<object>> rows);

        /// <summary>
        /// Updates the given columns with the given id in the first column.
        /// Each row of rowsToUpdate must have the same size as rowsData.
        /// </summary>
        /// <param name="tableName">The table where rows should be updated.</param>
        /// <param name="rowsToUpdate">The rows with the name and data type to update.</param>
        /// <param name="rowsData">The rows with all column data which should be updated.</param>
        public abstract void UpdateTable(string tableName, List<Dictionary<string, Type>> rowsToUpdate, List<List<object>> rowsData);

        /// <summary>
        /// Get's the last n rows from the specified table.
        /// </summary>
        /// <param name="rows">number of the rows to display.</param>
        /// <param name="table">The table to get the values from.</param>
        /// <returns></returns>
        public abstract List<List<object>> GetLastNRowsFromTable(Table table, int rows);

        /// <summary>
        /// Gets all rows in the given DateTime slot.
        /// </summary>
        /// <param name="table">The name of the table to get the data from.</param>
        /// <param name="DateTimeColumnName">The name of the column with the DateTime values.</param>
        /// <param name="from">A DateTime object with the beginning of the timeslot.</param>
        /// <param name="until">A DateTime object with the end of the timeslot.</param>
        /// <returns></returns>
        public abstract List<List<object>> GetRowsFromTableWithTime(Table table, string DateTimeColumnName, DateTime from, DateTime until);

        /// <summary>
        /// Gets all rows in the given id slot.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public abstract List<List<object>> GetRowsFromTableWithIndex(Table table, int start, int end);

        /// <summary>
        /// Deletes the last n rows of the given table.
        /// </summary>
        /// <param name="table">The table to delete the last n data from.</param>
        /// <param name="rows">The amount of data to delete.</param>
        public abstract void DeleteLastNRows(Table table, int rows);

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
        /// <param name="tables">All saved tables.</param>
        public abstract void OnDeleteTimer(Dictionary<string, Table> tables);

        /// <summary>
        /// Stops the delete timer and all active threads.
        /// </summary>
        public abstract void StopDeleteThread();

        /// <summary>
        /// Gets the current rows from the table. Since this is accomplished with a trigger table its just a small querie.
        /// </summary>
        /// <param name="table">The table to get the current rows from.</param>
        /// <returns>Returns the number of rows in the given table</returns>
        public abstract int GetCurrentRowsFromTable(Table table);

        #region Low Level Methods
        /// <summary>
        /// The event to get update events on the <see cref="FetchQuery(string, List{KeyValuePair{int, string}}, int)"/> method.
        /// </summary>
        public abstract event EventHandler<FetchArgs> FetchEvent;

        /// <summary>
        /// A method for queries which don't need an answer or result.
        /// Like insert, update, create etc.
        /// </summary>
        /// <param name="query"></param>
        public abstract void CommitQuery(string query);

        /// <summary>
        /// A method for queries which give an answer or a result.
        /// Like Select. With this method it's also possible to define the columns which are needed for the given table.
        /// For Each row the needed column is seperated with '|'.
        /// At the moment only string and int64 values are implemented. Every other type is ignored.
        /// </summary>
        /// <param name="query">The select query.</param>
        /// <param name="columns">A list of pairs witch asks for the the given column and type of column in the order of the list.</param>
        /// <returns></returns>
        public abstract List<List<object>> ReadQuery(string query, List<KeyValuePair<int, Type>> columns);

        /// <summary>
        /// This is an asynchron version of the <see cref="ReadQuery(string, List{KeyValuePair{int, string}})"/> method.
        /// The answer is send through an event in the given fetchsize.
        /// </summary>
        /// <param name="query">The select query.</param>
        /// <param name="columns">A list of pairs witch asks for the the given column and type of column in the order of the list.</param>
        /// <param name="fetchsize">The fetch size defines the length of the list which is sent through an event.</param>
        public abstract void FetchQuery(string query, List<KeyValuePair<int, Type>> columns, int fetchsize);

        /// <summary>
        /// This command starts the FetchThread again and you will get the next fetch of the data.
        /// </summary>
        public abstract void NextFetch();

        /// <summary>
        /// This command cancels the current fetch command.
        /// </summary>
        public abstract void CancelFetch();

        /// <summary>
        /// Commits all queries at once.
        /// </summary>
        /// <param name="queryList">The list with all queries to execute.</param>
        public abstract void CommitBatchQuery(List<string> queryList);
        #endregion
    }

    #region EventArgs classes
    /// <summary>
    /// The argument class for this event.
    /// </summary>
    public class FetchArgs : EventArgs
    {
        /// <summary>
        /// All fetched Rows
        /// </summary>
        public List<List<object>> Rows;

        /// <summary>
        /// This class is used to sent Arguments through an event.
        /// </summary>
        /// <param name="rows">All rows with the specific data in it.</param>
        public FetchArgs(List<List<object>> rows)
        {
            Rows = rows;
        }
    }
    #endregion
}
