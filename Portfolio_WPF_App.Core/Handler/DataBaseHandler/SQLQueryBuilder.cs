using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio_WPF_App.Core.Handler.DataBaseHandler
{
    /// <summary>
    /// This class is used to build fast query strings.
    /// If you use this builder you don't need to worry about typos anymore.
    /// Every added command should also be tested in the <see cref="SQLQueryBuilder"/> Test project class.
    /// </summary>
    public class SQLQueryBuilder
    {
        private String _query = "";

        #region SQLite commands implementations
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public SQLQueryBuilder After()
        {
            _query += SQLConstants.AFTER;

            return this;
        }

        public SQLQueryBuilder Asc()
        {
            _query += SQLConstants.ASC;

            return this;
        }

        public SQLQueryBuilder Begin()
        {
            _query += SQLConstants.BEGIN;

            return this;
        }

        public SQLQueryBuilder Create()
        {

            _query += SQLConstants.CREATE;
            return this;
        }

        public SQLQueryBuilder Delete()
        {
            _query += SQLConstants.DELETE;
            return this;
        }

        public SQLQueryBuilder Desc()
        {
            _query += SQLConstants.DESC;
            return this;
        }

        public SQLQueryBuilder Distinct()
        {
            _query += SQLConstants.DISTINCT;
            return this;
        }

        public SQLQueryBuilder DropTable()
        {
            _query += SQLConstants.DROP_TABLE;
            return this;
        }

        public SQLQueryBuilder End()
        {
            _query += SQLConstants.END;
            return this;
        }

        public SQLQueryBuilder From()
        {
            _query += SQLConstants.FROM;
            return this;
        }

        public SQLQueryBuilder GroupBy()
        {
            _query += SQLConstants.GROUP_BY;
            return this;
        }

        public SQLQueryBuilder Having()
        {
            _query += SQLConstants.HAVING;
            return this;
        }

        public SQLQueryBuilder IfNotExists()
        {
            _query += SQLConstants.IF_NOT_EXISTS;
            return this;
        }

        public SQLQueryBuilder Ignore()
        {
            _query += SQLConstants.IGNORE;
            return this;
        }

        public SQLQueryBuilder In()
        {
            _query += SQLConstants.IN;
            return this;
        }

        public SQLQueryBuilder Insert()
        {
            _query += SQLConstants.INSERT;
            return this;
        }

        public SQLQueryBuilder InsertInto()
        {
            _query += SQLConstants.INSERT_INTO;
            return this;
        }

        public SQLQueryBuilder Into()
        {
            _query += SQLConstants.INTO;
            return this;
        }

        public SQLQueryBuilder Join()
        {
            _query += SQLConstants.JOIN;
            return this;
        }

        public SQLQueryBuilder Limit()
        {
            _query += SQLConstants.LIMIT;
            return this;
        }

        public SQLQueryBuilder Offset()
        {
            _query += SQLConstants.OFFSET;
            return this;
        }

        public SQLQueryBuilder On()
        {
            _query += SQLConstants.ON;
            return this;
        }

        public SQLQueryBuilder Or()
        {
            _query += SQLConstants.OR;
            return this;
        }

        public SQLQueryBuilder OrderBY()
        {
            _query += SQLConstants.ORDER_BY;
            return this;
        }

        public SQLQueryBuilder Select()
        {
            _query += SQLConstants.SELECT;
            return this;
        }

        public SQLQueryBuilder Set()
        {
            _query += SQLConstants.SET;
            return this;
        }

        public SQLQueryBuilder Table()
        {
            _query += SQLConstants.TABLE;
            return this;
        }

        public SQLQueryBuilder Top()
        {
            _query += SQLConstants.TOP;
            return this;
        }

        public SQLQueryBuilder Trigger()
        {
            _query += SQLConstants.TRIGGER;
            return this;
        }

        public SQLQueryBuilder Update()
        {
            _query += SQLConstants.UPDATE;
            return this;
        }

        public SQLQueryBuilder Values()
        {
            _query += SQLConstants.VALUES;
            return this;
        }

        public SQLQueryBuilder Where()
        {
            _query += SQLConstants.WHERE;
            return this;
        }

        public SQLQueryBuilder Apostrophe(String value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(SQLConstants.APOSTROPHE, value);
            _query += sb.ToString();
            return this;
        }

        public SQLQueryBuilder Apostrophe_Multiple(List<String> values)
        {
            String items = "";
            for (int iter = 0; iter < values.Count; iter++)
            {
                items += SQLConstants.APOSTROPHE;
                items = items.Replace("{0}", values[iter]);
                items = items.Remove(items.Length - 1);

                if (iter < values.Count - 1)
                    items += SQLConstants.COMMA;
            }
            _query += items + " ";
            return this;
        }

        public SQLQueryBuilder Brackets(String value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(SQLConstants.BRACKETS, value);
            _query += sb.ToString();
            return this;
        }

        public SQLQueryBuilder Brackets_Multiple(List<String> values, Boolean withSpace)
        {
            String items = "";
            for (int iter = 0; iter < values.Count; iter++)
            {
                items += values[iter];
                if (withSpace)
                    items = items.Remove(items.Length - 1);

                if (iter < values.Count - 1)
                    items += SQLConstants.COMMA;
            }
            String brackets = SQLConstants.BRACKETS;
            brackets = brackets.Replace("{0}", items);
            _query += brackets;
            return this;
        }

        public SQLQueryBuilder Count(String value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(SQLConstants.COUNT, value);
            _query += sb.ToString();
            return this;
        }

        public SQLQueryBuilder Max(String value)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(SQLConstants.MAX, value);
            _query += sb.ToString();
            return this;
        }

        public SQLQueryBuilder Max_Multiple(List<String> values)
        {
            String items = "";
            for (int iter = 0; iter < values.Count; iter++)
            {
                items += SQLConstants.MAX;
                items = items.Replace("{0}", values[iter]);
                items = items.Remove(items.Length - 1);

                if (iter < values.Count - 1)
                    items += SQLConstants.COMMA;
            }
            _query += items + " ";
            return this;
        }

        public SQLQueryBuilder ALL()
        {
            _query += SQLConstants.ALL;
            return this;
        }

        public SQLQueryBuilder AND()
        {
            _query += SQLConstants.AND;
            return this;
        }

        public SQLQueryBuilder Comma()
        {
            try
            {
                _query = _query.Remove(_query.Length - 1);
                _query += SQLConstants.COMMA;
                return this;
            }
            catch (Exception e)
            {
                //TODO: Change that.
                throw;
            }
        }

        public SQLQueryBuilder CommaPoint(bool withSpace = false)
        {
            _query = _query.Remove(_query.Length - 1);
            if (withSpace)
                _query += SQLConstants.COMMA_POINT + " ";
            else
                _query += SQLConstants.COMMA_POINT;
            return this;
        }

        public SQLQueryBuilder Equal()
        {
            _query += SQLConstants.EQUAL;
            return this;
        }

        public SQLQueryBuilder Greater()
        {
            _query += SQLConstants.GREATER;
            return this;
        }

        public SQLQueryBuilder GreaterThen()
        {
            _query += SQLConstants.GREATER_THEN;
            return this;
        }

        public SQLQueryBuilder Lesser()
        {
            _query += SQLConstants.LESSER;
            return this;
        }

        public SQLQueryBuilder LesserThen()
        {
            _query += SQLConstants.LESSER_THEN;
            return this;
        }

        public SQLQueryBuilder TypeBlob()
        {
            _query += SQLConstants.TYPE_BLOB;
            return this;
        }

        public SQLQueryBuilder TypeInteger()
        {
            _query += SQLConstants.TYPE_INTEGER;
            return this;
        }

        public SQLQueryBuilder TypeReal()
        {
            _query += SQLConstants.TYPE_REAL;
            return this;
        }

        public SQLQueryBuilder TypeText()
        {
            _query += SQLConstants.TYPE_TEXT;
            return this;
        }

        public SQLQueryBuilder Null()
        {
            _query += SQLConstants.NULL;
            return this;
        }

        public SQLQueryBuilder ParamForeignKey()
        {
            _query += SQLConstants.PARAM_FOREIGN_KEY;
            return this;
        }

        public SQLQueryBuilder ParamPrimaryKey()
        {
            _query += SQLConstants.PARAM_PRIMARY_KEY;
            return this;
        }

        public SQLQueryBuilder ParamNot()
        {
            _query += SQLConstants.PARAM_NOT;
            return this;
        }

        public SQLQueryBuilder ParamUnique()
        {
            _query += SQLConstants.PARAM_UNIQUE;
            return this;
        }

        public SQLQueryBuilder AddValue(String value)
        {
            _query += value + " ";
            return this;
        }

        public SQLQueryBuilder AddValues(List<String> values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String item in values)
            {
                sb.Append(item).Append(SQLConstants.COMMA);
            }
            _query += sb.ToString();
            _query = _query.Remove(_query.Length - 2) + " ";
            return this;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region commands
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SQLQueryBuilder CommandSpaceUsed()
        {
            _query = SQLConstants.COMMAND_SPACEUSED;
            return this;
        }
        #endregion

        /// <summary>
        /// The Flush takes the current query and removes any whitespaces in the back and returns it.
        /// Used to create seperate strings to use in the query. The _query string will also be reseted.
        /// </summary>
        /// <returns>returns the current query string.</returns>
        public string Flush()
        {
            string returnString = _query;
            _query = "";
            char compare = returnString[returnString.Length - 1];
            if (compare.Equals(' '))
                returnString = returnString.Remove(returnString.Length - 1);

            return returnString;
        }

        /// <summary>
        /// Returns the current string with the comma point in the end.
        /// </summary>
        /// <returns>returns the current query string</returns>
        public override string ToString()
        {
            _query = _query.Remove(_query.Length - 1);
            return _query += SQLConstants.COMMA_POINT;
        }
    }
}
