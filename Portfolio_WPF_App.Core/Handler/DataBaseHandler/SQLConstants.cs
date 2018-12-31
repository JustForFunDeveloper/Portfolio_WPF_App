namespace Portfolio_WPF_App.Core.Handler.DataBaseHandler
{
    /// <summary>
    /// A summary of well known commands used in SQL. (not only SQLITE)
    /// This class should be only used from the <see cref="SQLQueryBuilder"/>.
    /// </summary>
    public class SQLConstants
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static string AFTER = "AFTER ";
        public static string ASC = "ASC ";
        public static string BEGIN = "BEGIN ";
        public static string CREATE = "CREATE ";
        public static string DELETE = "DELETE ";
        public static string DESC = "DESC ";
        public static string DISTINCT = "DISTINCT ";
        public static string DROP_TABLE = "DROP TABLE ";
        public static string END = "END ";
        public static string FROM = "FROM ";
        public static string GROUP_BY = "GROUP BY ";
        public static string HAVING = "HAVING ";
        public static string IF_NOT_EXISTS = "IF NOT EXISTS ";
        public static string IGNORE = "IGNORE ";
        public static string IN = "IN ";
        public static string INSERT = "INSERT ";
        public static string INSERT_INTO = "INSERT INTO ";
        public static string INTO = "INTO ";
        public static string JOIN = "JOIN ";
        public static string LIMIT = "LIMIT ";
        public static string OFFSET = "OFFSET ";
        public static string ON = "ON ";
        public static string OR = "OR ";
        public static string ORDER_BY = "ORDER BY ";
        public static string SELECT = "SELECT ";
        public static string SET = "SET ";
        public static string TABLE = "TABLE ";
        public static string TOP = "TOP ";
        public static string TRIGGER = "TRIGGER ";
        public static string UPDATE = "UPDATE ";
        public static string VALUES = "VALUES ";
        public static string WHERE = "WHERE ";

        public static string APOSTROPHE = "'{0}' ";
        public static string BRACKETS = "({0}) ";
        public static string COUNT = "COUNT({0}) ";
        public static string MAX = "MAX({0}) ";

        public static string ALL = "* ";
        public static string AND = "AND ";
        public static string COMMA = ", ";
        public static string COMMA_POINT = ";";
        public static string EQUAL = "= ";
        public static string GREATER = "> ";
        public static string GREATER_THEN = ">= ";
        public static string LESSER = "< ";
        public static string LESSER_THEN = "<= ";

        public static string TYPE_BLOB = "BLOB ";
        public static string TYPE_INTEGER = "INTEGER ";
        public static string TYPE_REAL = "REAL ";
        public static string TYPE_TEXT = "TEXT ";

        public static string NULL = "NULL ";

        public static string PARAM_FOREIGN_KEY = "FOREIGN KEY ";
        public static string PARAM_NOT = "NOT ";
        public static string PARAM_PRIMARY_KEY = "PRIMARY KEY ";
        public static string PARAM_UNIQUE = "UNIQUE ";

        public static string COMMAND_SPACEUSED = "exec sp_spaceused ";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
