﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using System.Threading;




namespace CMS.classes   
{
    public class DB_Toolbox
    {
        string constr = System.Configuration.ConfigurationManager.ConnectionStrings["EP_OracleRootConn"].ToString();
        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_OracleUserConn"].ToString();
        string constrSQLServer = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();
        string constrSQLServer1 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLUserConn"].ToString();

        public class DBParameters
        {
            public string ParamName { get; set; }
            public OracleDbType OracleParamDataType { get; set; }
            public SqlDbType MSSqlParamDataType { get; set; }
            public ParameterDirection ParamDirection { get; set; }
            public int ParamSize { get; set; }
            public string ParamValue { get; set; }
            public Boolean ParamReturn { get; set; }
        }

        public class ColumnStructure
        {
            public string _Name { get; set; }
            public string _DataType { get; set; }
            public string _DefaultValue { get; set; }
            public Boolean _IsNull { get; set; }
            public string _Alias { get; set; }
            public string _Table { get; set; }
        }

        public class WhereStructure
        {
            public string OpenParentheses { get; set; }
            public string WhereClause { get; set; }
            public string CloseParentheses { get; set; }
            public string ContinuingOperator { get; set; }           
        }

        public class sqlFunctionStructure
        {
            public string _FunctionName { get; set; }
            public string _FunctionDataType { get; set; }
            public List<sqlProcedureParameterStructure> _Parameters { get; set; }
            public List<sqlProcedureLineStructure> _Declare { get; set; }
            public List<sqlProcedureLineStructure> _Body { get; set; }
            public List<sqlProcedureLineStructure> _Exception { get; set; }

        }

        public class sqlProcedureStructure
        {
            public string _ProcedureName { get; set; }
            public List<sqlProcedureParameterStructure> _Parameters { get; set; }
            public List<sqlProcedureLineStructure> _Declare { get; set; }
            public List<sqlProcedureLineStructure> _Body { get; set; }
            public List<sqlProcedureLineStructure> _Exception { get; set; }

        }

        public class sqlProcedureParameterStructure
        {
            public string _Name { get; set; }
            public ParameterDirection _Direction { get; set; }
            public string _DataType { get; set; }
            public string _DefaultValue { get; set; }
        }

        public class sqlProcedureLineStructure
        {
            public string LineEntry { get; set; }
        }

        public class sqlSelectStructure
        {
            //private Boolean __HasFrom = false;
            public Boolean _HasFrom { get; set; }
            public string _TableName { get; set; }
            public string _TableAlias { get; set; }
            //private Boolean __HasAggregateFunc = false;
            public Boolean _HasAggregateFunc { get; set; }
            public string _AggregateFunctions { get; set; }
            public List<ColumnStructure> _IncludeColumns { get; set; }
            public List<ColumnStructure> _ExcludeColumns { get; set; }
            //private Boolean __HasJoin = false;
            public Boolean _HasJoin { get; set; }
            public string _JoinClause { get; set; }
            public string _JoinOn { get; set; }
            //private Boolean __HasWhere = false;
            public Boolean _HasWhere { get; set; }
            public List<WhereStructure> _WhereClause { get; set; }
            //private Boolean __HasGroupBy = false;
            public Boolean _HasGroupBy { get; set; }
            public string _GroupByClause { get; set; }
            //private Boolean __HasHaving = false;
            public Boolean _HasHaving { get; set; }
            public string _HavingClause { get; set; }
        }

        public class EPDataEntryStruct
        {
            public string _Name { get; set; }
            public ParameterDirection _Direction { get; set; }
        }

        public DataTable ProcDataTable()
        {
            // Create a new DataTable.
            DataTable table = new DataTable("ReturnData");
            DataColumn column;

            // Create first column and add to the DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ChildID";
            column.AutoIncrement = true;
            column.AutoIncrementSeed = 0;
            column.AutoIncrementStep = 1;
            column.Caption = "ID";
            column.ReadOnly = true;
            column.Unique = true;
            table.Columns.Add(column);

            // Create second column and add to the DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ChildType";
            column.AutoIncrement = false;
            column.Caption = "ChildType";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            // Create third column and add to the DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ChildItem";
            column.AutoIncrement = false;
            column.Caption = "ChildItem";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            // Create fourth column and add to the DataTable.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ChildValue";
            column.AutoIncrement = false;
            column.Caption = "ChildValue";
            column.ReadOnly = false;
            column.Unique = false;
            table.Columns.Add(column);

            return table;
        }
//START DB CREATE COMMANDS

        public ArrayList EP_CREATE_DB(string DB_PLATFORM, string connRoot, string connAuth, string connOwner, string SystemName, string Password)
        {

            ArrayList Logger = new ArrayList();

            switch (DB_PLATFORM)
            {

                case "Oracle" :
                case "ORACLE" :

                    Logger.AddRange(EP_DROP_ALL(DB_PLATFORM, connRoot, SystemName));

                    Logger.AddRange(EP_BUILD_CORE_STRUCTURE(DB_PLATFORM, connRoot, connAuth, connAuth, SystemName, Password));

                    Logger.AddRange(EP_BUILD_DICTIONARY(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(EP_LOAD_SYS_DATA(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(EP_BUILD_SYSTEM_STRUCTURE(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(EP_BUILD_SYSTEM_VIEWS(DB_PLATFORM, connAuth, SystemName));

                    //Logger.AddRange(EP_LOAD_TEST_DATA(DB_PLATFORM, connAuth, SystemName));
                break;
                
                case "Microsoft":
                case "MICROSOFT":
                    Logger.AddRange(EP_DROP_ALL(DB_PLATFORM, connRoot, SystemName));
                    
                    Logger.AddRange(EP_BUILD_CORE_STRUCTURE(DB_PLATFORM, connRoot, connAuth, connOwner, SystemName, Password));

                    Logger.AddRange(EP_BUILD_DICTIONARY(DB_PLATFORM, connOwner, SystemName));

                    Logger.AddRange(EP_LOAD_SYS_DATA(DB_PLATFORM, connOwner, SystemName));

                    Logger.AddRange(EP_BUILD_SYSTEM_STRUCTURE(DB_PLATFORM, connOwner, SystemName));

                    //Logger.AddRange(EP_LOAD_TEST_DATA(DB_PLATFORM, connOwner, SystemName));
                break;

            }
            return Logger;
        }

        public ArrayList EP_BUILD_CORE_STRUCTURE(string DB_PLATFORM, string connRoot, string connAuth, string connOwner, string SystemName, string Password)
        {
            ArrayList Logger = new ArrayList();

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    Logger.Add(EP_ADD_ORACLE_PROFILE(constr, SystemName));

                    Logger.Add(EP_ADD_ORACLE_TABLESPACE(SystemName));

                    Logger.Add(EP_ADD_ORACLE_USER(connAuth, SystemName, Password));

                    Logger.Add(EP_ADD_ORACLE_ROLE(constr, SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "CREATE TABLE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "CREATE PROCEDURE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "CREATE SEQUENCE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "CREATE TRIGGER", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "CREATE VIEW", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "EXECUTE ANY PROCEDURE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "SELECT ANY TABLE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "INSERT ANY TABLE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "UPDATE ANY TABLE", SystemName));

                    Logger.Add(EP_GRANT_ORACLE_PRIVILEGE(connRoot, "SELECT ANY DICTIONARY", SystemName));
                    break;

                case "Microsoft":
                case "MICROSOFT":

                   Logger.Add(EP_ADD_MSSQL_LOGIN(connRoot, SystemName, Password, "master"));
                   Logger.Add(EP_ADD_MSSQL_DB(connRoot, SystemName));
                   Logger.Add(EP_ADD_MSSQL_USER(connRoot, SystemName, SystemName, SystemName, "db_owner"));
                   
                   Logger.Add(EP_ADD_MSSQL_ROLE(connAuth, SystemName));   
                   Logger.Add(EP_ADD_MSSQL_SCHEMA(connAuth, SystemName, SystemName));
                   Thread.Sleep(5000);
                   
                   Logger.Add(EP_GRANT_MSSQL_PRIVILEGE(connAuth, "ALL", SystemName + "_RL", SystemName));
                   Logger.Add(EP_ADD_MEMBER_TO_ROLE(connAuth, "db_owner", SystemName));
                   Logger.Add(EP_UPDATE_MSSQL_USER_DEFAULT_SCHEMA(connAuth, SystemName, SystemName, SystemName));
                   Logger.Add(EP_UPDATE_MSSQL_LOGIN_DEFAULT_DB(connAuth, SystemName, SystemName, SystemName));
                   //Logger.Add(EP_ALTER_AUTH_ON_MSSQL_SCHEMA(connAuth, SystemName, SystemName, SystemName));
                   
                break;
            }

            return Logger;
        }

        public ArrayList EP_BUILD_SYSTEM_STRUCTURE(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            ArrayList columnlist = new ArrayList();

            List<ColumnStructure> ScaffoldColumns = new List<ColumnStructure>();
            List<ColumnStructure> ExistingColumnsList = new List<ColumnStructure>();
            List<ColumnStructure> MetaColumnsList = new List<ColumnStructure>();


            

            //---CREATE - ORGANIZATIONS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "Organizations", ScaffoldColumns));

            //---FK - ORGANIZATIONS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "ORGANIZATIONS_1", "ORGANIZATIONS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Groups--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "GROUPS", ScaffoldColumns));

            //---FK - GROUPS FOREIGN KEYS 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GROUP_1", "GROUPS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GROUPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GROUPS_2", "GROUPS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - USERS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "USERS", ScaffoldColumns));



            //---FK - USERS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "USERS_1", "USERS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - GROUPS & USERS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GROUPS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "GROUPS_USERS", ScaffoldColumns));

            //---FK - GROUPS & USERS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GROUPS_USERS_1", "GROUPS_USERS", "Users", ExistingColumnsList, ExistingColumnsList));

            //---FK - GROUPS & USERS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GROUPS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GROUPS_USERS_2", "GROUPS_USERS", "GROUPS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - STAGES
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "STAGES", ScaffoldColumns));



            //---PK - STAGES Primary KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "STAGES_1", "STAGES", ExistingColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            MetaColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "STAGES_1", "STAGES", "OBJECTS", ExistingColumnsList, MetaColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "STAGES_2", "STAGES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "STAGES_3", "STAGES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Grips
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "GRIPS", ScaffoldColumns));

            //---PK - GRIPS Primary KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "GRIPS_1", "GRIPS", ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIPS_1", "GRIPS", "STAGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIPS_2", "GRIPS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIPS_3", "GRIPS", "USERS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - OBJECTS SETS
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJECT_SETS", ScaffoldColumns));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, "OBJECT_SETS_1", "OBJECT_SETS", ExistingColumnsList));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, "OBJECT_SETS_2", "OBJECT_SETS", ExistingColumnsList));


            //---FK - OBJECTS SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECT_SETS_1", "OBJECT_SETS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECT_SET_2", "OBJECT_SETS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECT_SETS_3", "OBJECT_SETS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECT_SETS_5", "OBJECT_SETS", "GRIPS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - OBJECT OPTION SETS
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJ_OPT_SETS", ScaffoldColumns));

            //---FK OBJECT OPTION SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECT_OPT_SETS_1", "OBJ_OPT_SETS", "OBJECT_SETS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Value Datatypes--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "VALUE_DATATYPES", ScaffoldColumns));

            //---UK VALUE DATATYPES Unique KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, "VALUE_DATATYPES_1", "VALUE_DATATYPES", ExistingColumnsList));

            //---CREATE - OBJECTS & PROPERTIES SETS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Property_Name", _DataType = "Characters(250)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "HAS_PARENT", _DataType = "Characters(5)", _DefaultValue = "true", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "HAS_CHILD", _DataType = "Characters(5)", _DefaultValue = "false", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PARENT_OBJ_PROP_SETS_ID", _DataType = "Numbers", _DefaultValue = "0", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PROPERTY_VALUE", _DataType = "Characters(250)", _DefaultValue = "", _IsNull = true });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJ_PROP_SETS", ScaffoldColumns));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJ_PROP_SETS_1", "OBJ_PROP_SETS", "OBJECT_SETS", ExistingColumnsList, ExistingColumnsList));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJ_PROP_SETS_2", "OBJ_PROP_SETS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJ_PROP_SETS_3", "OBJ_PROP_SETS", "VALUE_DATATYPES", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - OBJECTS PROPERTIES OPTIONS SETS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJ_PROP_OPT_SETS", ScaffoldColumns));

            //---FK OBJECTS PROPERTIES OPTIONS SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJ_PROP_OPT_SETS_1", "OBJ_PROP_OPT_SETS", "OBJ_PROP_SETS", ExistingColumnsList, ExistingColumnsList));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, "OBJ_PROP_OPT_SETS_1", "OBJ_PROP_OPT_SETS", ExistingColumnsList));


            //---CREATE - CORES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "CORES", ScaffoldColumns));



            //---FK - CORES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CORES_1", "CORES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CORES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CORES_2", "CORES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CORES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CORES_3", "CORES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - APPLICATIONS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "APPLICATIONS", ScaffoldColumns));



            //---PK - APPLICATIONS PRIMARY KEY--->
            ExistingColumnsList.Add(new ColumnStructure { _Name = "Name", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "APPLICATIONS", "APPLICATIONS", ScaffoldColumns));

            //---FK - APPLICATIONS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            MetaColumnsList.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "APPLICATIONS_1", "APPLICATIONS", "CORES", MetaColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "APPLICATIONS_2", "APPLICATIONS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "APPLICATIONS_3", "APPLICATIONS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "APPLICATIONS_4", "APPLICATIONS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - ROLES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "ROLES", ScaffoldColumns));

            //---FK - ROLES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "ROLES_1", "ROLES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "PRIVELEGES", ScaffoldColumns));

            //---FK - PRIVELEGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "PRIVELEGES_1", "PRIVELEGES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - ROLES & PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ROLES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PRIVELEGES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "ROLES_PRIVELEGES", ScaffoldColumns));

            //---FK - ROLES & PRIVELEGES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ROLES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "ROLES_PRIVELEGES_1", "ROLES_PRIVELEGES", "ROLES", ExistingColumnsList, ExistingColumnsList));

            //---FK - ROLES & PRIVELEGES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "PRIVELEGES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "ROLES_PRIVELEGES_2", "ROLES_PRIVELEGES", "PRIVELEGES", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - USERS & ROLES & PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ROLES_PRIVELEGES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "USERS_ROLES_PRIVELEGES", ScaffoldColumns));

            //---FK - USERS & ROLES & PRIVELEGES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ROLES_PRIVELEGES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "USERS_ROLES_PRIVELEGES_1", "USERS_ROLES_PRIVELEGES", "ROLES_PRIVELEGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - USERS &  ROLES & PRIVELEGES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "USERS_ROLES_PRIVELEGES_2", "USERS_ROLES_PRIVELEGES", "USERS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - CASES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "APPLICATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "CASES", ScaffoldColumns));



            //---FK - CASES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "APPLICATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_1", "CASES", "APPLICATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_2", "CASES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_3", "CASES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 4--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_4", "CASES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - FORMS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Organizations_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "FORMS", ScaffoldColumns));

            //---FK - FORMS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_1", "FORMS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_2", "FORMS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_3", "FORMS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - CASES & FORMS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "CASES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "CASES_FORMS", ScaffoldColumns));

            //---FK - CASES & FORMS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "CASES_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_FORMS_1", "CASES_FORMS", "CASES", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES & FORMS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_FORMS_2", "CASES_FORMS", "FORMS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES & FORMS FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_FORMS_3", "CASES_FORMS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES & FORMS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "CASES_FORMS_4", "CASES_FORMS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - FORMS & STAGES--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "FORMS_STAGES", ScaffoldColumns));

            //---FK - FORMS & STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_STAGES_1", "FORMS_STAGES", "STAGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_STAGES_2", "FORMS_STAGES", "FORMS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_STAGES_3", "FORMS_STAGES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "FORMS_STAGES_4", "FORMS_STAGES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - GRIDS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Organizations_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "GRIDS", ScaffoldColumns));

            //---FK - GRIDS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_1", "GRIDS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIDS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_2", "GRIDS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIDS FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_3", "GRIDS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - GRIDS & FORM & STAGES--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GRIDS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "GRIDS_FORMS_STAGES", ScaffoldColumns));

            //---FK - GRIDS & FORMS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_FORMS_STAGES_1", "GRIDS_FORMS_STAGES", "FORMS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIDS & STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_FORMS_STAGES_2", "GRIDS_FORMS_STAGES", "STAGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIDS & FORMS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIDS_ID", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "GRIDS_FORMS_STAGES_3", "GRIDS_FORMS_STAGES", "GRIDS", ExistingColumnsList, ExistingColumnsList));


            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "USERS"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "STAGES"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "GRIPS"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "CORES"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "APPLICATIONS"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "CASES"));
            Logger.AddRange(EP_ADD_DATA_SCAFFOLD(DB_PLATFORM, connAuth, "FORMS"));
                    
                  
            ////Application_ID, NAME

            //Logger.Add(EP_ADD_TABLE("Events"));  //Has Applications_ID
            ////Events_ID, Date_Created, Application_ID, Event_Type_ID

            //Logger.Add(EP_ADD_TABLE("Event_Types"));   
            ////Event_Type_ID, Value

            //Logger.Add(EP_ADD_TABLE("Errors")); //Has Application_ID
            ////Error_ID, Error_Number, Error_Description, User, Program, Procedure_Name, 

            ////2nd Stage of Core Tables
            ////

            //Logger.Add(EP_ADD_TABLE("Transactions"));
            ////Transaction_ID, Application_ID, Organization_ID, User_ID 

            //Logger.Add(EP_ADD_TABLE("Sessions"));
            ////SESSION_ID, USER_ID, Application_ID

            //Logger.Add(EP_ADD_TABLE("Tokens"));
            ////TOKEN_ID, SESSION_ID

            //Logger.Add(EP_ADD_TABLE("HTML_ELEMENTS"));
            ////HTML_ELEMENT_ID, Name, Start Tag, End Tag, Empty, Depr., DTD, Description
            ////http://www.w3.org/TR/html4/index/elements.html


            //Logger.Add(EP_ADD_TABLE("Case_Types"));
            ////Case_Type_ID, NAME

            //Logger.Add(EP_ADD_TABLE("WorkFlows"));
            ////Workflow_ID

            return Logger;
        }

        public ArrayList EP_BUILD_DICTIONARY(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            ArrayList columnlist = new ArrayList();

            List<ColumnStructure> ScaffoldColumns = new List<ColumnStructure>();
            List<ColumnStructure> ExistingColumnsList = new List<ColumnStructure>();
            List<ColumnStructure> MetaColumnsList = new List<ColumnStructure>();

            //---CREATE - OBJECT Layer--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJECT_LAYERS", ScaffoldColumns));

            //---PK - OBJECT LAYERS Unique KEY--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "OBJECT_LAYERS_1", "OBJECT_LAYERS", ExistingColumnsList));

            //---CREATE - OBJECTS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "OBJECTS", ScaffoldColumns));

            //---PK - OBJECTS Unique KEY--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "OBJECTS_1", "OBJECTS", ExistingColumnsList));

            //---FK - OBJECT LAYERS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "OBJECTS_1", "OBJECTS", "OBJECT_LAYERS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - LOGS--->
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Characters(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Entry", _DataType = "Characters(4000)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "LOGS", ScaffoldColumns));

            //---FK - LOGS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Numbers", _DefaultValue = "", _IsNull = false });
            Logger.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, "LOGS_1", "LOGS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //----CREATE DICTIONARY EP TABLE --->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "TABLE_NAME", _DataType = "Varchar2(30)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(EP_ADD_SCAFFOLD(DB_PLATFORM, connAuth, "EP_TABLES", ScaffoldColumns));

            //---PK -DICTIONARY EP TABLE Unique KEY--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "TABLE_NAME", _DataType = "Varchar2(30)", _DefaultValue = "", _IsNull = false});
            Logger.Add(EP_ADD_PRIMARY_KEY(DB_PLATFORM, connAuth, "TABLE_NAME_1", "EP_TABLES", ExistingColumnsList));

            return Logger;
                
        }

        public ArrayList EP_BUILD_SYSTEM_VIEWS(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            Logger.AddRange(SQL_CREATE_VIEW_Applications(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Organizations(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Users(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Cases(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Objects(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Forms(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Object_Sets(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Object_Set_Props(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Form_Templates(DB_PLATFORM, connAuth));
            Logger.AddRange(SQL_CREATE_VIEW_Form_Template_Details(DB_PLATFORM, connAuth));

            return Logger;
        }

        public string EP_ADD_ENTRY(string DB_PLATFORM, string connAuth, string EntrySource, List<DBParameters> EntryProcedureParameters)
        {
            List<DBParameters> ParamListDynamic = new List<DBParameters>();
            
            switch(DB_PLATFORM)
            {
                case "Oracle":
                case "ORACLE":
                    foreach (DBParameters i in EntryProcedureParameters)
                    {
                        switch (i.OracleParamDataType)
                        {
                            case OracleDbType.Varchar2:
                                ParamListDynamic.Add(new DBParameters
                                {
                                    ParamName = i.ParamName,
                                    ParamDirection = i.ParamDirection,
                                    OracleParamDataType = i.OracleParamDataType,
                                    ParamSize = i.ParamSize,
                                    ParamValue = i.ParamValue
                                }
                                );
                                break;
                            case OracleDbType.Int16:
                            case OracleDbType.Int32:
                            case OracleDbType.Date:
                            case OracleDbType.TimeStamp:
                                ParamListDynamic.Add(new DBParameters
                            {
                                ParamName = i.ParamName,
                                ParamDirection = i.ParamDirection,
                                OracleParamDataType = i.OracleParamDataType,
                                ParamValue = i.ParamValue
                            }
                            );
                                break;
                        }
                        
                    }

                    return SQL_PROCEDURE_GET_VALUE("R_" + EntrySource.ToUpper() + "_ID", SQL_PROCEDURE_PARAMS("ORACLE", connAuth, EntrySource.ToUpper() + "_ADD_RW", ParamListDynamic));
                
                case "Microsoft":
                case "MICROSOFT":

                    foreach (DBParameters i in EntryProcedureParameters)
                    {
                        //if (i.ParamValue != null || i.ParamValue != "")
                            
                        switch (i.MSSqlParamDataType)
                        {
                            case SqlDbType.VarChar:
                            ParamListDynamic.Add(new DBParameters
                            {
                                ParamName = "@" + i.ParamName,
                                ParamDirection = i.ParamDirection,
                                MSSqlParamDataType = i.MSSqlParamDataType,
                                ParamSize = i.ParamSize,
                                ParamValue = i.ParamValue
                            }
                            );
                            break;

                            case SqlDbType.Int:
                            case SqlDbType.Date:
                            case SqlDbType.Timestamp:
                            ParamListDynamic.Add(new DBParameters
                            {
                                ParamName = "@" + i.ParamName,
                                ParamDirection = i.ParamDirection,
                                MSSqlParamDataType = i.MSSqlParamDataType,
                                ParamValue = i.ParamValue
                            }
                            );
                            break;
                        }
                        
                    }

                    return SQL_PROCEDURE_GET_VALUE("@R_" + EntrySource.ToUpper() + "_ID", SQL_PROCEDURE_PARAMS("MICROSOFT", connAuth, EntrySource.ToUpper() + "_ADD_RW", ParamListDynamic));

                default:

                return "Invalid DB Platform";
            
            }
        }

        public string EP_ADD_Dictionary_Table(string DB_PLATFORM, string connAuth, string TableName)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "P_TABLE_NAME",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 30,
                ParamValue = TableName
            });

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "R_EP_TABLES_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,

            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "EP_TABLES", EntryProcedureParameters);


        }
        
        public string EP_ADD_ENTRY_Organizations(string DB_PLATFORM, string connAuth)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "P_OBJECT_TYPE", ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2, MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 50, ParamValue = "Organization"
            });

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "R_ORGANIZATIONS_ID", ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32, MSSqlParamDataType = SqlDbType.Int,
                
            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Organizations", EntryProcedureParameters);

           
        }

        public string EP_ADD_ENTRY_Users(string DB_PLATFORM, string connAuth)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 50,
                ParamValue = "User"
            });

            EntryProcedureParameters.Add(new DBParameters
            {
                ParamName = "R_USERS_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int
            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Users", EntryProcedureParameters);

        }

        public string EP_ADD_ENTRY_Cores(string DB_PLATFORM, string connAuth, string _Users_ID, string _Organizations_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

                    EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = "Core"
                    });

                    EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Users_ID
                    });

                    EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_ORGANIZATIONS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Organizations_ID
                    });

                    EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_CORES_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });
                    return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Cores", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Application(string DB_PLATFORM, string connAuth, string _Core_ID, string _Users_ID, string _Organizations_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = "Application"
                    });

            EntryProcedureParameters.Add(new DBParameters
                    {
                        ParamName = "P_CORES_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Core_ID,

                    });

            EntryProcedureParameters.Add(new DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Users_ID,

                    });

            EntryProcedureParameters.Add(new DBParameters
                    {
                        ParamName = "P_ORGANIZATIONS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Organizations_ID,

                    });

            EntryProcedureParameters.Add(new DBParameters
                    {
                        ParamName = "R_APPLICATIONS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });
                    
            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Applications", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Case(string DB_PLATFORM, string connAuth, string _APP_ID, string _Users_ID, string _Organizations_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 50,
                ParamValue = "Case"
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_APPLICATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _APP_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Organizations_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Users_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_CASES_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int
            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Cases", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Form(string DB_PLATFORM, string connAuth, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 50,
                ParamValue = "Form"
            });


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Organizations_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Users_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_FORMS_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int
            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Forms", EntryProcedureParameters);

                    
        }

        public string EP_ADD_ENTRY_Grid(string DB_PLATFORM, string connAuth, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                 MSSqlParamDataType = SqlDbType.VarChar,
                ParamSize = 50,
                ParamValue = "Grid"
            });


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Organizations_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int,
                ParamValue = _Users_ID
            });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_GRIDS_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32,
                MSSqlParamDataType = SqlDbType.Int

            });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Grids", EntryProcedureParameters);

                   
            
        }

        public string EP_ADD_ENTRY_Form_Property(string DB_PLATFORM, string connAuth, string _FormID, string _UserID, string _Name, string _FieldType, string _Value)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_FORM_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _FormID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _UserID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _Name
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_FIELD_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _FieldType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _Value
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_FORMS_OPTIONS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "FORMS_OPTIONS", EntryProcedureParameters);
                    
        }

        public string SQL_ADD_ROW_Forms_Property_Option()
        {
            return "";
        }

        public string EP_ADD_ENTRY_Object_Layer(string DB_PLATFORM, string connAuth, string _ObjectType)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_LAYER",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _ObjectType
                    });


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJECT_LAYERS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJECT_LAYERS", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Object(string DB_PLATFORM, string connAuth, string _Name, string _ObjectLayer)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _Name
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_LAYER",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _ObjectLayer
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJECTS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJECTS", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Object_Set(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _GripName, string _ObjectType, string _OrganizationID, string _UserID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_GRIP_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _GripName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _ObjectType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_ORGANIZATIONS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _OrganizationID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _UserID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJECT_SETS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });
                    
            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJECT_SETS", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Object_Option_Set(string DB_PLATFORM, string connAuth, string _ObjectSetID, string _OptionValue)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_SETS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _ObjectSetID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_Option_Value",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _OptionValue
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJ_OPT_SETS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJ_OPT_SETS", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Object_Property_Set(string DB_PLATFORM, string connAuth, string _ObjectsSetsID, string _ObjectType, string _ValueDatatype, string _PropertyName, string _HasParent, string _HasChild, string _ParentObjectPropID, string _PropertyValue)
        {
            string PropertyValue = " ";

            string ParentObjectPropID = "0";


            if (_PropertyValue != "")
                PropertyValue = _PropertyValue;

            if (_ParentObjectPropID != "")
                ParentObjectPropID = _ParentObjectPropID;

            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_SETS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _ObjectsSetsID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJECT_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _ObjectType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE_DATATYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _ValueDatatype
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_PROPERTY_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _PropertyName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_HAS_PARENT",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 5,
                        ParamValue = _HasParent
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_HAS_CHILD",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 5,
                        ParamValue = _HasChild
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_PARENT_OBJ_PROP_SETS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = ParentObjectPropID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_PROPERTY_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 250,
                        ParamValue = PropertyValue
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJ_PROP_SETS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJ_PROP_SETS", EntryProcedureParameters);
             
                    
        }

        public string EP_ADD_ENTRY_Object_Property_Option_Set(string DB_PLATFORM, string connAuth, string _ObjectsPropertySetsID, string _OptionValue)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJ_PROP_SETS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _ObjectsPropertySetsID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OPTION_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _OptionValue
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_OBJ_PROP_OPT_SETS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "OBJ_PROP_OPT_SETS", EntryProcedureParameters);

        }

        public string EP_ADD_ENTRY_Grip(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _GripName, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_GRIP_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _GripName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_ORGANIZATIONS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Organizations_ID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Users_ID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_GRIPS_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "GRIPS", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Stage(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_TYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_STAGE_NAME",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = _StageName
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_ORGANIZATIONS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Organizations_ID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_USERS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Users_ID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_STAGES_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "Stages", EntryProcedureParameters);
                    
        }

        public string EP_ADD_ENTRY_Value_Datatype(string DB_PLATFORM, string connAuth, string ValueDataType)
        {
            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();


            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE_DATATYPE",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Varchar2,
                        MSSqlParamDataType = SqlDbType.VarChar,
                        ParamSize = 50,
                        ParamValue = ValueDataType
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_VALUE_DATATYPES_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

            return EP_ADD_ENTRY(DB_PLATFORM, connAuth, "VALUE_DATATYPES", EntryProcedureParameters);
                
        }

        public string EP_ADD_ENTRY_OBJECT_DATA(string DB_PLATFORM, string connAuth, string _ObjectsPropertySetsID, string _Destination, string _Destination_ID, string _ValueDataType, string _Value)
        {
            string sourceDestination = MaxNameLength(_Destination, 14);

            switch (_ValueDataType)
            {
                case "Number":
                case "number":
                case "numb":
                    sourceDestination += "_DAT_NUMB";
                    break;
                case "Var":
                case "Character":
                case "Characters":
                case "CHAR":
                case "CHARS":
                case "Varchar2":
                    sourceDestination += "_DAT_CHAR";
                    break;
                case "Date":
                case "date":
                    sourceDestination += "_DAT_DATE";
                    break;
                default:
                    return "No Source Found";
            }

            List<DBParameters> EntryProcedureParameters = new List<DBParameters>();

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_" + _Destination + "_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _Destination_ID
                    });

            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_OBJ_PROP_SETS_ID",
                        ParamDirection = ParameterDirection.Input,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int,
                        ParamValue = _ObjectsPropertySetsID
                    });

                    switch (_ValueDataType)
                    {
                        case "Number":
                        case "number":
                        case "numb":
                            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                            {
                                ParamName = "P_VALUE",
                                ParamDirection = ParameterDirection.Input,
                                OracleParamDataType = OracleDbType.Int32,
                                MSSqlParamDataType = SqlDbType.Int,
                                ParamValue = _Value
                            });
                            break;
                        case "Var":
                        case "Character":
                        case "Characters":
                        case "CHAR":
                        case "CHARS":
                        case "Varchar2":
                            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                            {
                                ParamName = "P_VALUE",
                                ParamDirection = ParameterDirection.Input,
                                OracleParamDataType = OracleDbType.Varchar2,
                                MSSqlParamDataType = SqlDbType.VarChar,
                                ParamSize = 4000,
                                ParamValue = _Value
                            });
                            break;
                        case "Date":
                        case "date":
                            EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                            {
                                ParamName = "P_VALUE",
                                ParamDirection = ParameterDirection.Input,
                                OracleParamDataType = OracleDbType.Date,
                                MSSqlParamDataType = SqlDbType.DateTime,
                                ParamValue = _Value
                            });
                            break;
                    }

                EntryProcedureParameters.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "R_" + sourceDestination + "_ID",
                        ParamDirection = ParameterDirection.Output,
                        OracleParamDataType = OracleDbType.Int32,
                        MSSqlParamDataType = SqlDbType.Int
                    });

                return EP_ADD_ENTRY(DB_PLATFORM, connAuth, sourceDestination, EntryProcedureParameters);
                    

        }

        public ArrayList SQL_CREATE_VIEW_Object_Set_Props(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();

            List<ColumnStructure> cs_Select = new List<ColumnStructure>();

            cs_Select.Add(new ColumnStructure { _Name = "STAGE_TYPE", _Table = "st", _Alias = "Stage_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGES_ID", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_NAME", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "GRIPS_ID", _Table = "gr" });
            cs_Select.Add(new ColumnStructure { _Name = "GRIP_NAME", _Table = "gr" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _Table = "os" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "os", _Alias="OBJECT_SETS_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "ops" });        
            cs_Select.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_NAME", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "HAS_PARENT", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "HAS_CHILD", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PARENT_OBJ_PROP_SETS_ID", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_VALUE", _Table = "ops" });
            
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJ_PROP_SETS",
                _TableAlias = "ops",
                _IncludeColumns = cs_Select,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJECT_SETS",
                _TableAlias = "os",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "ops.object_sets_id = os.object_sets_id",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "GRIPS",
                _TableAlias = "gr",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "gr.grip_name = os.grip_name",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "STAGES",
                _TableAlias = "st",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "gr.stage_name = st.stage_name",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "All_Objects_Set_Props", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Object_Sets(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();

            List<ColumnStructure> cs_Select = new List<ColumnStructure>();

            cs_Select.Add(new ColumnStructure { _Name = "STAGE_TYPE", _Table = "st", _Alias = "Stage_Object_Type" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_NAME", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGES_ID", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "GRIPS_ID", _Table = "gr" });
            cs_Select.Add(new ColumnStructure { _Name = "GRIP_NAME", _Table = "gr" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _Table = "os" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "os" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJECT_SETS",
                _TableAlias = "os",
                _IncludeColumns = cs_Select,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "GRIPS",
                _TableAlias = "gr",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "gr.grip_name = os.grip_name",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "STAGES",
                _TableAlias = "st",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "gr.stage_name = st.stage_name",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "All_Objects", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Organizations(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            List<ColumnStructure> cs_Organizations = new List<ColumnStructure>();

            cs_Organizations.Add(new ColumnStructure { _Name = "Organizations_ID", _Table = "Org" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Organizations",
                _TableAlias = "Org",
                _IncludeColumns = cs_Organizations,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "Organizations", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Objects(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            List<ColumnStructure> cs_Objects = new List<ColumnStructure>();

            cs_Objects.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "Obj" });
            cs_Objects.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _Table = "Obj" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Objects",
                _TableAlias = "Obj",
                _IncludeColumns = cs_Objects,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "OBJECTS", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Users(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            List<ColumnStructure> cs_Users = new List<ColumnStructure>();

            cs_Users.Add(new ColumnStructure { _Name = "USERS_ID", _Table = "Users" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Users",
                _TableAlias = "",
                _IncludeColumns = cs_Users,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "USERS", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Cases(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<DB_Toolbox.sqlSelectStructure> sqlStruct = new List<DB_Toolbox.sqlSelectStructure>();
            List<DB_Toolbox.ColumnStructure> CS_CASES = new List<DB_Toolbox.ColumnStructure>();
            List<DB_Toolbox.ColumnStructure> CS_APP = new List<DB_Toolbox.ColumnStructure>();
            List<DB_Toolbox.ColumnStructure> CS_USERS = new List<DB_Toolbox.ColumnStructure>();

            CS_CASES.Add(new DB_Toolbox.ColumnStructure { _Name = "CASES_ID", _Table = "Cases" });
            CS_APP.Add(new DB_Toolbox.ColumnStructure { _Name = "APPLICATIONS_ID", _Table = "App" });
            CS_APP.Add(new DB_Toolbox.ColumnStructure { _Name = "DT_CREATED", _Table = "Cases" });
            CS_USERS.Add(new DB_Toolbox.ColumnStructure { _Name = "Users_ID", _Table = "Cases" });


            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Cases",
                _TableAlias = "Cases",
                _IncludeColumns = CS_CASES,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _JoinClause = "INNER JOIN",
                _TableName = "Applications",
                _TableAlias = "App",
                _IncludeColumns = CS_APP,
                _JoinOn = "App.Cores_ID = Cases.Cases_ID",
                _HasFrom = false,
                _HasJoin = true,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _JoinClause = "INNER JOIN",
                _TableName = "Users",
                _TableAlias = "Users",
                _IncludeColumns = CS_USERS,
                _JoinOn = "Cases.USERS_ID = Users.USERS_ID",
                _HasFrom = false,
                _HasJoin = true,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "Cases", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Applications(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<DB_Toolbox.sqlSelectStructure> sqlStruct = new List<DB_Toolbox.sqlSelectStructure>();
            List<DB_Toolbox.ColumnStructure> CS_CORE = new List<DB_Toolbox.ColumnStructure>();
            List<DB_Toolbox.ColumnStructure> CS_APP = new List<DB_Toolbox.ColumnStructure>();
            List<DB_Toolbox.ColumnStructure> CS_USERS = new List<DB_Toolbox.ColumnStructure>();

            CS_CORE.Add(new DB_Toolbox.ColumnStructure { _Name = "CORES_ID", _Table = "Core" });
            CS_APP.Add(new DB_Toolbox.ColumnStructure { _Name = "APPLICATIONS_ID", _Table = "App" });
            CS_APP.Add(new DB_Toolbox.ColumnStructure { _Name = "DT_CREATED", _Table = "App" });
            CS_USERS.Add(new DB_Toolbox.ColumnStructure { _Name = "Users_ID", _Table = "Users" });


            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Applications",
                _TableAlias = "App",
                _IncludeColumns = CS_APP,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _JoinClause = "INNER JOIN",
                _TableName = "CORES",
                _TableAlias = "Core",
                _IncludeColumns = CS_CORE,
                _JoinOn = "App.Cores_ID = Core.Cores_ID ",
                _HasFrom = false,
                _HasJoin = true,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _JoinClause = "INNER JOIN",
                _TableName = "Users",
                _TableAlias = "Users",
                _IncludeColumns = CS_USERS,
                _JoinOn = "App.USERS_ID = Users.USERS_ID",
                _HasFrom = false,
                _HasJoin = true,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "APPLICATIONS", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Forms(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            //List<ColumnStructure> cs_FormsProperties = new List<ColumnStructure>();
            List<ColumnStructure> cs_Select = new List<ColumnStructure>();
            List<WhereStructure> sqlWhere = new List<WhereStructure>();


            cs_Select.Add(new ColumnStructure { _Name = "TM_CREATED", _Table = "FDC" });

            cs_Select.Add(new ColumnStructure { _Name = "Organizations_ID", _Table = "Org" });
            cs_Select.Add(new ColumnStructure { _Name = "USERS_ID", _Table = "Users", _Alias = "Author" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_TYPE", _Table = "OS"});
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_NAME", _Table = "OS", _Alias = "FORM_TEMPLATE_NAME" });
            cs_Select.Add(new ColumnStructure { _Name = "FORMS_ID", _Table = "Forms", _Alias = "FORM_ID" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "OS", _Alias="OBJECT_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "OPS", _Alias="PROP_OBJECT_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_NAME", _Table = "OPS"});
            cs_Select.Add(new ColumnStructure { _Name = "PARENT_OBJ_PROP_SETS_ID", _Table = "OPS" });
            cs_Select.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _Table = "OPS" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_VALUE", _Table = "OPS" });
            cs_Select.Add(new ColumnStructure { _Name = "NVL(FDC.VALUE, TO_CHAR(NVL(FDN.VALUE, TO_CHAR(NVL(FDD.VALUE, '')))))", _Alias = "LATEST_VALUE" });
            

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Forms",
                _TableAlias = "",
                _IncludeColumns = cs_Select,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Organizations",
                _TableAlias = "Org",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "Forms.Organizations_ID = Org.Organizations_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Users",
                _TableAlias = "",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "Users.Users_ID = Forms.Users_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Forms_Dat_Char",
                _TableAlias = "FDC",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "LEFT OUTER JOIN",
                _JoinOn = "FDC.Forms_ID = Forms.Forms_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Forms_Dat_Numb",
                _TableAlias = "FDN",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "LEFT OUTER JOIN",
                _JoinOn = "FDN.Forms_ID = Forms.Forms_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Forms_Dat_Date",
                _TableAlias = "FDD",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "LEFT OUTER JOIN",
                _JoinOn = "FDD.Forms_ID = Forms.Forms_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJ_PROP_SETS",
                _TableAlias = "OPS",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "FDC.OBJ_PROP_SETS_ID = OPS.OBJ_PROP_SETS_ID or FDN.OBJ_PROP_SETS_ID = OPS.OBJ_PROP_SETS_ID or FDD.OBJ_PROP_SETS_ID = OPS.OBJ_PROP_SETS_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJECT_SETS",
                _TableAlias = "OS",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "OS.OBJECT_SETS_ID = OPS.OBJECT_SETS_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlWhere.Add(new WhereStructure { WhereClause = "OS.STAGE_TYPE = 'Form'" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _HasFrom = false,
                _HasJoin = false,
                _HasWhere = true,
                _WhereClause = sqlWhere,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "FORMS", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Form_Templates(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            //List<ColumnStructure> cs_FormsProperties = new List<ColumnStructure>();
            List<ColumnStructure> cs_Select = new List<ColumnStructure>();
            List<WhereStructure> sqlWhere = new List<WhereStructure>();


            cs_Select.Add(new ColumnStructure { _Name = "TM_CREATED", _Table = "st" });

            cs_Select.Add(new ColumnStructure { _Name = "Organizations_ID", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "USERS_ID", _Table = "st", });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_TYPE", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_NAME", _Table = "st" });
            
            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Stages",
                _TableAlias = "st",
                _IncludeColumns = cs_Select,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlWhere.Add(new WhereStructure { WhereClause = "st.STAGE_TYPE = 'Form'" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _HasFrom = false,
                _HasJoin = false,
                _HasWhere = true,
                _WhereClause = sqlWhere,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "FORM_TEMPLATES", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_CREATE_VIEW_Form_Template_Details(string DB_PLATFORM, string connAuth)
        {
            ArrayList Logger = new ArrayList();

            List<sqlSelectStructure> sqlStruct = new List<sqlSelectStructure>();
            //List<ColumnStructure> cs_FormsProperties = new List<ColumnStructure>();
            List<ColumnStructure> cs_Select = new List<ColumnStructure>();
            List<WhereStructure> sqlWhere = new List<WhereStructure>();


            cs_Select.Add(new ColumnStructure { _Name = "TM_CREATED", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "Organizations_ID", _Table = "os" });
            cs_Select.Add(new ColumnStructure { _Name = "USERS_ID", _Table = "os", });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_TYPE", _Table = "st" });
            cs_Select.Add(new ColumnStructure { _Name = "STAGE_NAME", _Table = "st"});
            cs_Select.Add(new ColumnStructure { _Name = "GRIP_NAME", _Table = "gr" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "os", _Alias = "OBJECT_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _Table = "ops", _Alias = "PROP_OBJECT_TYPE" });
            cs_Select.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_NAME", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "HAS_PARENT", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "HAS_CHILD", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PARENT_OBJ_PROP_SETS_ID", _Table = "ops" });
            cs_Select.Add(new ColumnStructure { _Name = "PROPERTY_VALUE", _Table = "ops" });


            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Stages",
                _TableAlias = "st",
                _IncludeColumns = cs_Select,
                _HasFrom = true,
                _HasJoin = false,
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "Grips",
                _TableAlias = "gr",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "st.STAGE_NAME = gr.STAGE_NAME",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJECT_SETS",
                _TableAlias = "os",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "os.STAGE_TYPE = gr.STAGE_TYPE and os.STAGE_NAME = gr.STAGE_NAME and os.GRIP_NAME = gr.GRIP_NAME",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _TableName = "OBJ_PROP_SETS",
                _TableAlias = "ops",
                _HasFrom = false,
                _HasJoin = true,
                _JoinClause = "INNER JOIN",
                _JoinOn = "os.OBJECT_SETS_ID = ops.OBJECT_SETS_ID",
                _HasWhere = false,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });

            sqlWhere.Add(new WhereStructure { WhereClause = "st.STAGE_TYPE = 'Form'" });

            sqlStruct.Add(new DB_Toolbox.sqlSelectStructure
            {
                _HasFrom = false,
                _HasJoin = false,
                _HasWhere = true,
                _WhereClause = sqlWhere,
                _HasAggregateFunc = false,
                _HasGroupBy = false,
                _HasHaving = false
            });


            Logger.Add(EP_ADD_VIEW(DB_PLATFORM, connAuth, "FORM_TEMPLATE_DETAILS", sqlStruct));

            return Logger;
        }

        public ArrayList EP_LOAD_SYS_DATA(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Front-End"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Design"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Framework"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Architecture"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Application"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Presentation"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Session"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Transport"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Network"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Data Link"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Physical"));         
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Universal"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "System"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Grid"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object_Layer(DB_PLATFORM, connAuth, "Table"));

            Logger.Add(EP_ADD_ENTRY_Value_Datatype(DB_PLATFORM, connAuth, "Characters"));
            Logger.Add(EP_ADD_ENTRY_Value_Datatype(DB_PLATFORM, connAuth, "Number"));
            Logger.Add(EP_ADD_ENTRY_Value_Datatype(DB_PLATFORM, connAuth, "Date"));

            
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Table", "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Primary Keys", "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Foreign Keys", "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Unique Keys", "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Linkable Tables", "Dictionary"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Toolbox Modules", "Dictionary"));

            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Core", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Application", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Stage", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Grip", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Object Set", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Case", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Form", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Grid", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Grid Widget", "Grid"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Notification", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Log", "System"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Flow", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Organization", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "User", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Role", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Privelege", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Notes", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Pages", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Comment", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Form Field", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Demographic", "Application"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Conditional_Logic", "Application"));

            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "ID", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Value", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Text_Box", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Attribute", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Paragraph_Text", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Rich_Text", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Check_Box", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Radio_Button", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Drop_Down", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Section", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Page_Break", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Number", "IO Tag"));

            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Email", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Url", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Date", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Time", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Phone", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "Credit_Card", "IO Tag"));
            Logger.Add(EP_ADD_ENTRY_Object(DB_PLATFORM, connAuth, "File_Upload", "IO Tag"));

            return Logger;
        }

        public ArrayList EP_LOAD_TEST_DATA(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            string Organizations_ID = "1000";

            string Users_ID = "1000";

            string Cores_ID = "1000";

            string APP_ID = "1000";

            string Cases_ID = "1000";

            string Stages_ID = "1000";

            string Forms_ID = "1000";

            string GripsID = "1000";
            string ObjectSetsID = "1000";
            string ObjectPropertySetID = "1000";

            string ObjectPropertyOptionSetID = "1000";

            Organizations_ID = EP_ADD_ENTRY_Organizations(DB_PLATFORM, connAuth);
            Users_ID = EP_ADD_ENTRY_Users(DB_PLATFORM, connAuth);
            Cores_ID = EP_ADD_ENTRY_Cores(DB_PLATFORM, connAuth, Users_ID, Organizations_ID);
            APP_ID = EP_ADD_ENTRY_Application(DB_PLATFORM, connAuth, Cores_ID, Users_ID, Organizations_ID);
            Cases_ID = EP_ADD_ENTRY_Case(DB_PLATFORM, connAuth, APP_ID, Users_ID, Organizations_ID);
            Forms_ID = EP_ADD_ENTRY_Form(DB_PLATFORM, connAuth, Organizations_ID, Users_ID);

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Core", "Base Core", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Core", "Base Core", "Core Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Core", "Base Core", "Core Settings", "Core", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Enabled", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Version", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Application", "Base Application", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Application", "Base Application", "Application Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Application", "Base Application", "Application Settings", "Application", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Enabled", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Version", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "User", "Base User", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "User", "Base User", "User Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "User", "Base User", "User Settings", "Demographic", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "First Name", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Last Name", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Date", "Date", "DOB", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Email", "Characters", "Email", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "SSN", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Sex", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Male");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Female");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Unknown");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Clearance Level", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Top Secret /SCI");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Top Secret");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Secret");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Classified");

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Form", "Default Form", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");

            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Drop_Down", "Characters", "Form Style", "false", "false", "", "Test");
            EP_ADD_ENTRY_Object_Option_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Style 1");
            EP_ADD_ENTRY_Object_Option_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Style 2");

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Form", "Default Form", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "False");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "2");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "5");

            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "2");

            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "2");

            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Drop_Down", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "20");

            EP_ADD_ENTRY_Object_Option_Set(DB_PLATFORM, connAuth, ObjectSetsID, "One");
            EP_ADD_ENTRY_Object_Option_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Two");

            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Paragraph_Text", Organizations_ID, Users_ID);
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "10");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, ObjectSetsID, "Forms", Forms_ID, "Characters", "alsjdhflakjsdhf");

            //SQL_ADD_ROW_Object_Data(DB_PLATFORM, connAuth, "FORMS", "Characters", "FORMS_ID", ObjectSetsID, "Test");

            Stages_ID = EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Grid", "First Grid", Organizations_ID, Users_ID);
            GripsID = EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Grid", "First Grid", "Grid Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Grid", "First Grid", "Grid Settings", "Grid Widget", Organizations_ID, Users_ID);

            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-col", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-row", "false", "false", "", "1");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizex", "false", "false", "", "57");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizey", "false", "false", "", "4");
            ObjectPropertySetID = EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "class", "false", "false", "", "gs-w");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1017", "Forms", Forms_ID, "Characters", "Example 101091");         

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1018", "Forms", Forms_ID, "Characters", "Example Organization");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1019", "Forms", Forms_ID, "Characters", "One");
           
            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1020", "Forms", Forms_ID, "Characters", "alsjdhflakjsdhf");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1004", "Applications", APP_ID, "Characters", "First Application");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1005", "Applications", APP_ID, "Characters", "First_Application");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1006", "Applications", APP_ID, "Characters", "True");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1007", "Applications", APP_ID, "Characters", "1.0");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1000", "Cores", Forms_ID, "Characters", "First Core");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1001", "Cores", Forms_ID, "Characters", "First_Core");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1002", "Cores", Forms_ID, "Characters", "True");

            EP_ADD_ENTRY_OBJECT_DATA(DB_PLATFORM, connAuth, "1003", "Cores", Forms_ID, "Characters", "1.0");

            //SQL_ADD_ROW_Object_Data(DB_PLATFORM, connAuth, "FORMS", "Characters", "FORMS_ID", ObjectSetsID, "Test");


            return Logger;
        }


        /*      public Datatable SQL_CREATE_DB_DT(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList rows = new ArrayList();

            //DROP EXISTING SYSTEM TABLES

            rows.Add(EP_DROP_ORACLE_USER(SystemName));

            rows.Add(EP_DROP_ORACLE_TABLESPACE(SystemName));

            rows.Add(EP_DROP_ORACLE_PROFILE(SystemName));

            rows.Add(EP_DROP_ORACLE_ROLE(SystemName + "_ROLE"));

            //CREATE NECESSARY STRUCTURE FOR STORING OBJECTS

            rows.Add(EP_ADD_ORACLE_PROFILE(SystemName));

            rows.Add(EP_ADD_ORACLE_TABLESPACE(SystemName));

            rows.Add(SQL_Add_USER(SystemName));

            rows.Add(EP_ADD_ORACLE_ROLE(SystemName + "_ROLE"));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE TABLE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE PROCEDURE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE SEQUENCE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("SELECT ANY DICTIONARY", SystemName));

            //1st Stage of Core TABLES
            //
            rows.Add(EP_ADD_TABLE("Applications"));
            //Application_ID, NAME

            rows.Add(EP_ADD_TABLE("Events"));  //Has Applications_ID
            //Events_ID, Date_Created, Application_ID, Event_Type_ID

            rows.Add(EP_ADD_TABLE("Logs"));
            //LOG_ID

            rows.Add(EP_ADD_TABLE("Event_Types"));
            //Event_Type_ID, Value

            rows.Add(EP_ADD_TABLE("Errors")); //Has Application_ID
            //Error_ID, Error_Number, Error_Description, User, Program, Procedure_Name, 

            //2nd Stage of Core Tables
            //
            rows.Add(EP_ADD_TABLE("Organizations"));
            //Organization_ID

            rows.Add(EP_ADD_TABLE("Users"));
            //User_ID

            rows.Add(EP_ADD_TABLE("Transactions"));
            //Transaction_ID, Application_ID, Organization_ID, User_ID 

            rows.Add(EP_ADD_TABLE("Sessions"));
            //SESSION_ID, USER_ID, Application_ID

            rows.Add(EP_ADD_TABLE("Tokens"));
            //TOKEN_ID, SESSION_ID

            rows.Add(EP_ADD_TABLE("HTML_ELEMENTS"));
            //HTML_ELEMENT_ID, Name, Start Tag, End Tag, Empty, Depr., DTD, Description
            //http;//www.w3.org/TR/html4/index/elements.html

            rows.Add(EP_ADD_TABLE("Cases"));
            //CASE_ID, CASE_TYPE, ORGANIZATION_ID, 

            rows.Add(EP_ADD_TABLE("Case_Types"));
            //Case_Type_ID, NAME

            rows.Add(EP_ADD_TABLE("Forms"));
            //Form_ID, FIELD_NAME, FIELD_TYPE, VALUE, TEXT

            rows.Add(EP_ADD_TABLE("Forms_HTML_Objects")); //Foreign Keys TO Forms(FormID) & HTML_Objects(HTML_Object_ID)


            rows.Add(EP_ADD_TABLE("WorkFlows"));
            //Workflow_ID

            rows.Add(EP_ADD_TABLE("Roles"));
            //Role_ID, Applications_ID, Privelege_ID

            rows.Add(EP_ADD_TABLE("Roles_Priveleges"));

            rows.Add(EP_ADD_TABLE("USERS_ROLES_PRIVELEGES"));

            rows.Add(EP_ADD_TABLE("Priveleges"));
            //Privelege_ID


        }
*/
//START COMMON DATATABASE COMMANDS
        //
        public string SQL_SELECT_DoesCMSUserExist(string Name)
        {
            OracleConnection connection = new OracleConnection(constr);
            try
            {
                connection.Open();

                string tempSQL = "select username from dba_users where username = '" + Name + "'";

                OracleCommand SQLCommand = new OracleCommand(tempSQL);

                SQLCommand.Connection = connection;

                OracleDataReader reader = SQLCommand.ExecuteReader();

                if (reader.HasRows)
                { return "yes"; }
                else
                { return "no"; }
            }
            catch (OracleException ex)
            { return DB_ERROR_FORMATTER("ORACLE", ex.ToString()); }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public string SQL_EXTEND_TABLE_VIA_COLUMN(string DB_Platform, string ConnAuth, string OneColumn, string ColumnType, string SourceTable)
        {
            ArrayList ResultHolder = new ArrayList();

            string tempstringNAME = MaxNameLength(SourceTable + "_" + OneColumn, 27);
            //SQLCreate Table

            //Create Extending Table 
            ResultHolder.Add(EP_ADD_TABLE("ORACLE", ConnAuth, tempstringNAME));

            ArrayList ColumnList = new ArrayList();
            ColumnList.Add(tempstringNAME + "_ID");

            //Use Table ID as Unique Key
            ResultHolder.Add(EP_ADD_UNIQUE_KEY("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, ColumnList));

            //Add Distinguishing Column
            ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, tempstringNAME, OneColumn, ColumnType, "", false));

            ColumnList.Clear();
            ColumnList.Add(SourceTable + "_ID");
            ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, tempstringNAME, (SourceTable + "_ID"), "Number", "", false));
            ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, SourceTable, OneColumn, ColumnType, "", false));


            ColumnList.Clear();
            ColumnList.Add(SourceTable + "_ID");
            ResultHolder.Add(EP_ADD_FOREIGN_KEY("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, SourceTable, ColumnList, ColumnList));

            //Foreign Key

            return "Extension Completed.";
        }

        public string SQL_EXTEND_TABLE_VIA_COLUMNS(string DB_Platform, string ConnAuth, string SourceTable, List<ColumnStructure> NamingColumns, List<ColumnStructure> NewColumns)
        {
            //Array is Used to Store string values returned
            ArrayList ResultHolder = new ArrayList();

            //Array is Used to Store Columns Names
            ArrayList ColumnNames = new ArrayList();

            //Initialize String Holder
            string tempstringNAME = SourceTable + "_";

            //Initilized Integer is used for tracking Row Count
            int iNumber = 0;
            foreach (ColumnStructure i in NamingColumns)
            {
                //Increase Number by 1
                iNumber++;

                //Append Columns to String Holder
                tempstringNAME = tempstringNAME + i._Name;
                ColumnNames.Add(i._Name);

                //Append the "_" to all non_trailing columns
                if (iNumber < NamingColumns.Count)
                    tempstringNAME = tempstringNAME + "_";
            }

            //Assign Name for Extending Name
            tempstringNAME = MaxNameLength(tempstringNAME, 27);
            ResultHolder.Add("System Assigned Name " + tempstringNAME);

            //Create Table Extending Table
            ResultHolder.Add(EP_ADD_TABLE("ORACLE", ConnAuth, tempstringNAME));

            //This Class has to assign the Unique Key of the ID number.
            ArrayList ColumnList = new ArrayList();
            ColumnList.Add(tempstringNAME + "_ID");

            //Use Table ID as Unique Key
            ResultHolder.Add(EP_ADD_UNIQUE_KEY("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, ColumnList));

            //Add Distinguishing Column
            foreach (ColumnStructure i in NamingColumns)
            {
                //Add Parameters to DBProcedure
                ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, tempstringNAME, i._Name, i._DataType.ToString(), i._DefaultValue, i._IsNull));
            }

            //Add Columns from Source 
            ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, tempstringNAME, (SourceTable + "_ID"), "Number", "", false));

            List<ColumnStructure> ColumnStruct1 = new List<ColumnStructure>();

            ColumnStruct1.Add(new ColumnStructure { _Name = (SourceTable + "_ID") });

            //Foreign Key
            ResultHolder.Add(EP_ADD_FOREIGN_KEY("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, SourceTable, ColumnStruct1, ColumnStruct1));

            //Add New Columns for New Table
            foreach (ColumnStructure i in NewColumns)
            {
                //Add Parameters to DBProcedure
                ResultHolder.Add(EP_ADD_COLUMN("ORACLE", constr1, tempstringNAME, i._Name, i._DataType.ToString(), i._DefaultValue, i._IsNull));
            }

            return "Table Extension Completed.";
        }


        //PERCEPTION COMMON DB METHODS

        //
        public string EP_GENERATE_QUERY(string DB_Platform, List<sqlSelectStructure> QueryStructure)
        {
            string _Columnlist = "";
            string temp1;
            Boolean _FromAdded = false;
            Boolean _WhereAdded = false;
            Boolean _GroupByAdded = false;
            Boolean _HavingAdded = false;
            StringBuilder SQLBuffer = new StringBuilder();

            foreach (sqlSelectStructure i in QueryStructure)
            {

                if (i._IncludeColumns != null)
                {


                    foreach (ColumnStructure ii in i._IncludeColumns)
                    {
                        if (ii._Alias == null || ii._Alias.TrimEnd(' ') == "")
                            temp1 = ii._Alias;
                        else
                            temp1 = " \"" + ii._Alias + "\"";

                        if (ii._Name.ToString().ToUpper().Contains("NVL") == true)
                        {
                            _Columnlist = _Columnlist + ii._Name + " " + temp1 + ", ";
                        }
                        else
                        {
                            _Columnlist = _Columnlist + ii._Table + "." + ii._Name + " " + temp1 + ", ";
                        }

                    }
                }
            }

            _Columnlist = _Columnlist.TrimEnd(',', ' ');


            SQLBuffer.Append("SELECT " + _Columnlist + " ");

            int iNumber = 0;
            foreach (sqlSelectStructure i in QueryStructure)
            {
                iNumber++;

                if (i._HasFrom == true &&
                    i._HasAggregateFunc != true &&
                    i._HasJoin != true &&
                    i._HasWhere != true &&
                    i._HasGroupBy != true &&
                    i._HasHaving != true)
                {
                    if (_FromAdded == false)
                    {
                        SQLBuffer.Append("FROM " + i._TableName + " " + i._TableAlias + " ");
                        _FromAdded = true;
                    }
                }
                else if (i._HasFrom != true &&
                    i._HasAggregateFunc != true &&
                    i._HasJoin == true &&
                    i._HasWhere != true &&
                    i._HasGroupBy != true &&
                    i._HasHaving != true)
                {
                    SQLBuffer.Append(i._JoinClause + " " + i._TableName + " " + i._TableAlias + " ");
                    SQLBuffer.Append("ON (" + i._JoinOn + ") ");
                }
                else if (i._HasFrom != true &&
                    i._HasAggregateFunc != true &&
                    i._HasJoin != true &&
                    i._HasWhere == true &&
                    i._HasGroupBy != true &&
                    i._HasHaving != true)
                {
                    if (_WhereAdded == false)
                    {
                        SQLBuffer.Append("WHERE ");
                        _WhereAdded = true;
                    }

                    foreach (WhereStructure ii in i._WhereClause)
                    {
                        SQLBuffer.Append(ii.OpenParentheses);
                        SQLBuffer.Append(ii.WhereClause);
                        SQLBuffer.Append(ii.CloseParentheses + " " + ii.ContinuingOperator + " ");

                    }
                }
                else if (i._HasFrom != true &&
                    i._HasAggregateFunc != true &&
                    i._HasJoin != true &&
                    i._HasWhere != true &&
                    i._HasGroupBy == true &&
                    i._HasHaving != true)
                {
                    if (_GroupByAdded == false)
                    {
                        SQLBuffer.Append("GROUP BY " + i._GroupByClause + " ");
                        _GroupByAdded = true;
                    }


                }
                else if (i._HasFrom != true &&
                    i._HasAggregateFunc != true &&
                    i._HasJoin != true &&
                    i._HasWhere != true &&
                    i._HasGroupBy != true &&
                    i._HasHaving == true)
                {
                    if (_HavingAdded == false)
                    {
                        SQLBuffer.Append("Having " + i._HavingClause + " ");
                        _HavingAdded = true;
                    }


                }

            }

            return SQLBuffer.ToString();

        }

        public StringBuilder EP_GENERATE_PROCEDURE(string DB_Platform, string Name, List<sqlProcedureStructure> ProcedureStructure)
        {
            List<sqlProcedureLineStructure> plc = new List<sqlProcedureLineStructure>();
            List<sqlProcedureParameterStructure> pps = new List<sqlProcedureParameterStructure>();
            sqlProcedureStructure ps = new sqlProcedureStructure();
            StringBuilder Buffer = new StringBuilder();

            foreach (sqlProcedureStructure i in ProcedureStructure)
            {
                Buffer.AppendLine("CREATE OR REPLACE PROCEDURE " + Name + " IS ");

                int iNumber = 0;
                foreach (sqlProcedureParameterStructure ii in i._Parameters)
                {
                    iNumber++;

                    Buffer.AppendLine("   P_" + ii._Name + " " + ii._Direction.ToString() + " " + ii._DataType);

                    if (i._Parameters.Count < iNumber)
                        Buffer.Append(", ");
                }

                Buffer.AppendLine("DECLARE ");

                foreach (sqlProcedureLineStructure ii in i._Declare)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("BEGIN ");

                foreach (sqlProcedureLineStructure ii in i._Body)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("EXCEPTION ");

                foreach (sqlProcedureLineStructure ii in i._Exception)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("END; ");
            }


            return Buffer;
        }

        public StringBuilder EP_GENERATE_FUNCTION(string DB_Platform, string Name, List<sqlProcedureStructure> ProcedureStructure)
        {
            List<sqlProcedureLineStructure> plc = new List<sqlProcedureLineStructure>();
            List<sqlProcedureParameterStructure> pps = new List<sqlProcedureParameterStructure>();
            sqlProcedureStructure ps = new sqlProcedureStructure();
            StringBuilder Buffer = new StringBuilder();

            foreach (sqlProcedureStructure i in ProcedureStructure)
            {
                Buffer.AppendLine("CREATE OR REPLACE FUNCTION " + Name + " IS ");

                int iNumber = 0;
                foreach (sqlProcedureParameterStructure ii in i._Parameters)
                {
                    iNumber++;

                    Buffer.AppendLine("   P_" + ii._Name + " " + ii._Direction.ToString() + " " + ii._DataType);

                    if (i._Parameters.Count < iNumber)
                        Buffer.Append(", ");
                }

                Buffer.AppendLine("DECLARE ");

                foreach (sqlProcedureLineStructure ii in i._Declare)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("BEGIN ");

                foreach (sqlProcedureLineStructure ii in i._Body)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("EXCEPTION ");

                foreach (sqlProcedureLineStructure ii in i._Exception)
                {
                    Buffer.AppendLine("   " + ii.LineEntry);
                }

                Buffer.AppendLine("END; ");
            }


            return Buffer;
        }

        public string EP_ADD_COLUMN(string DB_PLATFORM, string connAuth, string TableName, string ColumnName, string ObjectType, string DefaultValue, bool isNull)
        {
            //if isNull is true/false influences syntax
            string _isNull = "";

            //if DefaultValue is empty or not influences syntax structure
            string _DefaultValue = "";
            string _ColumnName = "&quot;" + ColumnName + "&quot;";
            string _TableName = "&quot;" + TableName + "&quot;";

            if (isNull == false)
            {
                _isNull = "Not Null";
            }

            if (DefaultValue != "")
            {
                _DefaultValue = "default '" + DefaultValue + "'";
            }
            //Initiate buffer for SQL syntax.
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("alter table " + TableName + " ");
            string tempObjectType = EP_GET_DATATYPE(DB_PLATFORM, ObjectType);
            sqlStatement.Append("add " + MaxNameLength(ColumnName, 30) + " " + tempObjectType + " " + _DefaultValue + " " + _isNull);
                
            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlStatement.ToString(), "Column " + MaxNameLength(_ColumnName, 30) + " has been added to table " + _TableName);
            case "Microsoft":
            case "MICROSOFT":

                    return EP_RUN_NONE_QUERY("Microsoft", connAuth, sqlStatement.ToString(), "Column " + MaxNameLength(_ColumnName, 30) + " has been added to table " + _TableName);
       
            default:
                return "Invalid DB Platform";
            }
        }

        public ArrayList EP_ADD_COLUMNS(string DB_PLATFORM, string connAuth, string TableName, List<ColumnStructure> ColumnList)
        {
            ArrayList HoldResult = new ArrayList();

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":

                    foreach (ColumnStructure i in ColumnList)
                    {
                        HoldResult.Add(EP_ADD_COLUMN(DB_PLATFORM, connAuth, TableName, i._Name, i._DataType, i._DefaultValue, i._IsNull));
                    }
                    break;
                case "Microsoft":
                case "MICROSOFT":
                    
                    foreach (ColumnStructure i in ColumnList)
                    {
                        HoldResult.Add(EP_ADD_COLUMN(DB_PLATFORM, connAuth, TableName, i._Name, i._DataType, i._DefaultValue, i._IsNull));
                    }
                    break;
                    
                default:
                    HoldResult.Add("Invalid DB Platform");
                    break;
            }


            return HoldResult;
        }

        public string EP_ADD_FOREIGN_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, string ParentTable, ArrayList Columns1, ArrayList ParentColumns2)
        {
            //CREATES FOREIGN KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_FK";


            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("FOREIGN KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");
                    sqlBuffer.Append("REFERENCES " + ParentTable + " (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

                    //return sqlBuffer.ToString(); 
                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("FOREIGN KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");
                    sqlBuffer.Append("REFERENCES " + ParentTable + " (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");

                    //return sqlBuffer.ToString(); 
                    return EP_RUN_NONE_QUERY("MICROSOFT", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_FOREIGN_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, string ParentTable, List<ColumnStructure> Columns1, List<ColumnStructure> ParentColumns2)
        {
            //CREATES FOREIGN KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME;
            string tempstringColumns = "";
            string tempstringColumns2 = "";
            int iNumber = 0;
            foreach (ColumnStructure i in Columns1)
            {
                iNumber++;

                tempstringColumns = tempstringColumns + i._Name;

                if (iNumber < Columns1.Count())
                {

                    tempstringColumns = tempstringColumns + ", ";
                }
            }

            tempstringNAME = MaxNameLength(_Name, 27) + "_FK";

            iNumber = 0;
            foreach (ColumnStructure i in ParentColumns2)
            {
                iNumber++;
                tempstringColumns2 = tempstringColumns2 + i._Name;

                if (iNumber < Columns1.Count())
                {
                    tempstringColumns2 = tempstringColumns2 + ", ";
                }

            }

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("FOREIGN KEY (" + tempstringColumns + ") ");
                    sqlBuffer.Append("REFERENCES " + ParentTable + " (" + tempstringColumns2 + ") ENABLE");

                    //return sqlBuffer.ToString(); 
                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                   sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("FOREIGN KEY (" + tempstringColumns + ") ");
                    sqlBuffer.Append("REFERENCES " + ParentTable + " (" + tempstringColumns2 + ") ");

                    //return sqlBuffer.ToString(); 
                    return EP_RUN_NONE_QUERY("MICROSOFT", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";
            }

        }

        public string EP_ADD_PRIMARY_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, ArrayList Columns1)
        {
            //CREATES Primary KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_PK";

            //ALTER TABLE table_name
            //add CONSTRAINT constraint_name PRIMARY KEY (column1, column2, ... column_n);

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("Primary KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

                    //return sqlBuffer.ToString();
                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("Primary KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");

                    //return sqlBuffer.ToString();
                    return EP_RUN_NONE_QUERY("MICROSOFT", connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_PRIMARY_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, List<ColumnStructure> Columns1)
        {
            //CREATES Primary KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME;
            string tempstringColumns = "";
            int iNumber = 0;
            foreach (ColumnStructure i in Columns1)
            {
                iNumber++;

                tempstringColumns = tempstringColumns + i._Name;

                if (iNumber < Columns1.Count())
                {

                    tempstringColumns = tempstringColumns + ", ";
                }
            }

            tempstringNAME = MaxNameLength(_Name, 27) + "_PK";

            //ALTER TABLE table_name
            //add CONSTRAINT constraint_name PRIMARY KEY (column1, column2, ... column_n);

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":

                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("Primary KEY (" + tempstringColumns + ") ENABLE");

                    //return sqlBuffer.ToString();
                    return EP_RUN_NONE_QUERY(DB_PLATFORM, connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("Primary KEY (" + tempstringColumns + ")");

                    //return sqlBuffer.ToString();
                    return EP_RUN_NONE_QUERY(DB_PLATFORM, connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";

            }

        }

        public string EP_ADD_UNIQUE_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, ArrayList Columns1)
        {
            //CREATE UNIQUE KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_UK";
            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("UNIQUE (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("UNIQUE (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");

                    return EP_RUN_NONE_QUERY("Microsoft", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_UNIQUE_KEY(string DB_PLATFORM, string connAuth, string _Name, string ForTable, List<ColumnStructure> Columns1)
        {
            //CREATE UNIQUE KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME;
            string tempstringColumns = "";

            int iNumber = 0;
            foreach (ColumnStructure i in Columns1)
            {
                iNumber++;

                tempstringColumns = tempstringColumns + i._Name;

                if (iNumber < Columns1.Count())
                {
                    tempstringColumns = tempstringColumns + ", ";
                }
            }

            tempstringNAME = MaxNameLength(_Name, 27) + "_UK";
            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("UNIQUE (" + tempstringColumns + ") ENABLE");

                    return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
                case "Microsoft":
                case "MICROSOFT":
                    sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
                    sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
                    sqlBuffer.Append("UNIQUE (" + tempstringColumns + ") ");

                    return EP_RUN_NONE_QUERY("MICROSOFT", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string SQL_Add_Materialized_View_log(string DB_PLATFORM, string connAuth, string ForTable)
        {
            return EP_RUN_NONE_QUERY(DB_PLATFORM, connAuth, "CREATE MATERIALIZED VIEW LOG ON " + ForTable + " WITH ROWID", "MATERIALIZED VIEW LOG " + ForTable + " created.)");
        }

        public string EP_ADD_INSERT_PROCEDURE(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
        {
             //Will Create Procedure for Table
            DataTable dt = new DataTable();
            StringBuilder SQLin = new StringBuilder();
            //string tempstring = "";
            string outID = "";
            string outDataType = "";
            string _ProcedurePrefix;
                    
            if (ProcedurePrefix != "")
                _ProcedurePrefix = ProcedurePrefix;
            else
                _ProcedurePrefix = "ADD_RW";

            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, true);

            foreach (DataRow row in dt.Rows)
            {
                outID = row["COLUMN_NAME"].ToString();
                outDataType = row["DATA_TYPE"].ToString();
            }

            dt.Clear();

            //STEP 1 - Check if Table Exist
            //Step 2 - Get list of all Columns in Table
            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, false);

            SQLin.Append("create ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix);

            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":                  

                    //Step 3 - Create Procedure Begin Definition Syntax                    
                    SQLin.AppendLine(" ( ");

                    //Step 4 - Create Procedure Params
                    foreach (DataRow row in dt.Rows)
                    {
                        SQLin.AppendLine(" P_" + row["COLUMN_NAME"] + " IN " + row["DATA_TYPE"] + ", ");
                    }

                    SQLin.AppendLine(" R_" + outID + " OUT " + outDataType);
                    //Step 5 - Create Proecure Body Syntax
                    SQLin.AppendLine(") AS ");
                    SQLin.AppendLine("BEGIN ");
                    SQLin.AppendLine(" INSERT into " + TableName + " (");

                    int iNumber = 0;
                    int rowCount = dt.Rows.Count;

                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber++;

                        if (iNumber < rowCount)
                            SQLin.AppendLine("  " + row["COLUMN_NAME"] + ", ");
                        else if (iNumber == rowCount)
                            SQLin.AppendLine("  " + row["COLUMN_NAME"] + " ) ");

                    }

                    SQLin.AppendLine(" VALUES ( ");

                    iNumber = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber++;

                        if (iNumber < rowCount)

                            SQLin.AppendLine("  P_" + row["COLUMN_NAME"] + ", ");
                        else if (iNumber == rowCount)
                            SQLin.AppendLine("  P_" + row["COLUMN_NAME"] + " ) ");
                    }

                    SQLin.AppendLine(" RETURNING " + outID + " INTO R_" + outID + ";");


                    //Step 6 - Create Procedure End Definition Syntax
                    SQLin.AppendLine("END; ");

                    //Step 7 - Submit Procedure Syntax to Database/

                    // return SQLin.ToString();
                    return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");

                case "Microsoft":
                case "MICROSOFT":

                    //Step 4 - Create Procedure Params
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["DATA_TYPE"].ToString().ToLower() == "varchar")
                        {
                            SQLin.AppendLine(" @P_" + row["COLUMN_NAME"] + " [" + row["DATA_TYPE"] + "](" + row["DATA_LENGTH"] + "), ");
                        }
                        else
                        {
                            SQLin.AppendLine(" @P_" + row["COLUMN_NAME"] + " [" + row["DATA_TYPE"] + "], ");
                        }
                    }

                    SQLin.AppendLine(" @R_" + outID + "[" + outDataType + "] OUT ");
                   
                    SQLin.AppendLine("AS");
                    SQLin.AppendLine("declare @ID table (ID int)");
                    SQLin.AppendLine("BEGIN");
                    SQLin.AppendLine("    SET NOCOUNT ON");
                    
                    SQLin.AppendLine(" INSERT into " + TableName + " (");

                    int iNumber1 = 0;
                    int rowCount1 = dt.Rows.Count;

                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber1++;

                        if (iNumber1 < rowCount1)
                            SQLin.AppendLine("  " + row["COLUMN_NAME"] + ", ");
                        else if (iNumber1 == rowCount1)
                            SQLin.AppendLine("  " + row["COLUMN_NAME"] + " ) ");

                    }
                    SQLin.AppendLine(" OUTPUT inserted." + outID + " into @ID");
                    SQLin.AppendLine(" VALUES ( ");

                    iNumber1 = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber1++;

                        if (iNumber1 < rowCount1)

                            SQLin.AppendLine("  @P_" + row["COLUMN_NAME"] + ", ");
                        else if (iNumber1 == rowCount1)
                            SQLin.AppendLine("  @P_" + row["COLUMN_NAME"] + " ) ");
                    }


                    //SQLin.AppendLine(" INSERT INTO SamplerDampler(samplerdupler)");
                    //SQLin.AppendLine("    OUTPUT inserted.SamplerDampler_ID into @ID");
                    //SQLin.AppendLine("    VALUES(@samplerdupler)	");   
                    
                    
                    SQLin.AppendLine(" SELECT @R_" + outID + " = ID FROM @ID");
                    SQLin.AppendLine(" END");

                    return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_UPDATE_PROCEDURE(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
        {
            //Will Create Procedure for Table
            DataTable dt = new DataTable();
            StringBuilder SQLin = new StringBuilder();
            string tempstring = "";
            string outID = "";
            string inID = "";
            string inDataType = "";
            string outDataType = "";
            string _ProcedurePrefix;
            int iNumber = 0;
            int rowCount = 0;

            if (ProcedurePrefix != "")
                _ProcedurePrefix = ProcedurePrefix;
            else
                _ProcedurePrefix = "MOD_RW";


            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, true);

            foreach (DataRow row in dt.Rows)
            {
                inID = row["COLUMN_NAME"].ToString();
                inDataType = row["DATA_TYPE"].ToString();
                outID = row["COLUMN_NAME"].ToString();
                outDataType = row["DATA_TYPE"].ToString();
            }

            dt.Clear();

            //STEP 1 - Check if Table Exist
            //Step 2 - Get list of all Columns in Table
            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, false);
            rowCount = dt.Rows.Count;

            //Step 3 - Create Procedure Begin Definition Syntax
            SQLin.Append("create ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix);

            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":
                    SQLin.AppendLine(" ( ");
                    SQLin.AppendLine(" P_" + inID + " IN " + inDataType + ", ");
                    //Step 4 - Create Procedure Params 

                    foreach (DataRow row in dt.Rows)
                    {
                        SQLin.AppendLine(" P_" + row["COLUMN_NAME"] + " IN " + row["DATA_TYPE"] + ", ");
                    }

                    SQLin.AppendLine(tempstring);

                    SQLin.AppendLine("R_" + outID + " OUT " + outDataType);
                    //Step 5 - Create Proecure Body Syntax
                    SQLin.AppendLine(") AS ");
                    SQLin.AppendLine("BEGIN ");

                    SQLin.AppendLine("UPDATE " + TableName + " ");
                    SQLin.AppendLine(" SET ");

                    //tempstring = "";

                    iNumber = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber++;

                        SQLin.AppendLine(row["COLUMN_NAME"] + " = P_" + row["COLUMN_NAME"]);

                        if (iNumber != rowCount)
                            SQLin.Append(", ");
                    }

                    SQLin.AppendLine("Where " + inID + " = P_" + inID + ";");

                    SQLin.AppendLine();
                    SQLin.AppendLine("R_" + inID + " := P_" + inID + ";");

                    //Step 6 - Create Procedure End Definition Syntax
                    SQLin.AppendLine("END; ");

                    //Step 7 - Submit Procedure Syntax to Database/

                    // return SQLin.ToString();
                    return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
                case "Microsoft":
                case "MICROSOFT":
                    SQLin.AppendLine(" @P_" + inID + " [" + inDataType + "], ");
                    //Step 4 - Create Procedure Params
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["DATA_TYPE"].ToString().ToLower() == "varchar")
                        {
                            SQLin.AppendLine(" @P_" + row["COLUMN_NAME"] + " [" + row["DATA_TYPE"] + "](" + row["DATA_LENGTH"] + "), ");
                        }
                        else
                        {
                            SQLin.AppendLine(" @P_" + row["COLUMN_NAME"] + " [" + row["DATA_TYPE"] + "], ");
                        }
                    }

                    SQLin.AppendLine(" @R_" + outID + "[" + outDataType + "] OUT ");
                   
                    SQLin.AppendLine("AS");
                    //SQLin.AppendLine("declare @ID table (ID int)");
                    SQLin.AppendLine("BEGIN");
                    SQLin.AppendLine("    SET NOCOUNT ON");
                    
                    SQLin.AppendLine(" Update " + TableName + " ");
                    SQLin.AppendLine(" SET ");

                    int iNumber1 = 0;
                    int rowCount1 = dt.Rows.Count;

                    foreach (DataRow row in dt.Rows)
                    {
                        iNumber1++;

                        SQLin.AppendLine(row["COLUMN_NAME"] + " = @P_" + row["COLUMN_NAME"]);

                        if (iNumber1 != rowCount)
                            SQLin.Append(", ");

                    }
                    SQLin.AppendLine("Where " + inID + " = @P_" + inID + " ");

                    SQLin.AppendLine();
                    SQLin.AppendLine("SET @R_" + outID + " = @P_" + inID + " ");
                    
                    SQLin.AppendLine(" END");

                    return EP_RUN_NONE_QUERY("Microsoft", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_DELETE_PROCEDURE(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
        {
            //Will Create Procedure for Table
            DataTable dt = new DataTable();
            StringBuilder SQLin = new StringBuilder();
            //string tempstring = "";
            string outID = "";
            string inID = "";
            string inDataType = "";
            string outDataType = "";
            string _ProcedurePrefix;
            //int iNumber = 0;
            //int rowCount = 0;

            if (ProcedurePrefix != "")
                _ProcedurePrefix = ProcedurePrefix;
            else
                _ProcedurePrefix = "DEL_RW";


            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, true);

            foreach (DataRow row in dt.Rows)
            {
                outID = row["COLUMN_NAME"].ToString();
                outDataType = row["DATA_TYPE"].ToString();
                inID = row["COLUMN_NAME"].ToString();
                inDataType = row["DATA_TYPE"].ToString();
            }

            dt.Clear();

            //STEP 1 - Check if Table Exist
            //Step 2 - Get list of all Columns in Table
            dt = EP_GET_COLUMNS_VIA_TABLENAME(DB_Platform, ConnAuth, TableName, false);

            //Step 3 - Create Procedure Begin Definition Syntax
            SQLin.Append("create ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix + " ");
            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":
                    SQLin.AppendLine(" ( ");
                    SQLin.AppendLine(" P_" + inID + " IN " + inDataType + ", ");
                    SQLin.AppendLine(" R_" + outID + " OUT " + outDataType + " ");

                    SQLin.AppendLine(") AS ");
                    SQLin.AppendLine("BEGIN ");
                    SQLin.AppendLine("   DELETE FROM " + TableName + " ");
                    SQLin.AppendLine("    WHERE " + inID + " = P_" + inID + ";");
                    SQLin.AppendLine();
                    SQLin.AppendLine("   R_" + outID + ":= P_" + inID + ";");

                    //Step 6 - Create Procedure End Definition Syntax
                    SQLin.AppendLine("END; ");

                    //Step 7 - Submit Procedure Syntax to Database/

                    // return SQLin.ToString();
                    return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
                case "Microsoft":
                case "MICROSOFT":
                    SQLin.AppendLine("( @P_" + inID + " [" + inDataType + "] , ");

                    SQLin.AppendLine(" @R_" + outID + " [" + outDataType + "] OUT )");
                   
                    SQLin.AppendLine("AS");
                   
                    SQLin.AppendLine("BEGIN");
                    SQLin.AppendLine("    SET NOCOUNT ON");
                    
                    SQLin.AppendLine(" Delete from " + TableName + " ");
                    
                    SQLin.AppendLine("Where " + inID + " = @P_" + inID + " ");

                    SQLin.AppendLine();
                    SQLin.AppendLine("SET @R_" + outID + " = @P_" + inID + " ");
                    
                    SQLin.AppendLine(" END");

                    return EP_RUN_NONE_QUERY("Microsoft", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
                default:
                    return "Invalid DB Platform";
            }
        }

        public string EP_ADD_TABLE(string DB_Platform, string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27);

            switch (DB_Platform)
            {

                case "Oracle" :
                case "ORACLE" :

                string tableresult, sequenceResult, triggerResult;    
                //Step 1 Create Table
                tableresult = EP_RUN_NONE_QUERY(DB_Platform, ConnAuth, "CREATE TABLE " + tempstringNAME + "(" + tempstringNAME + "_ID number not null, enabled char(1) default 'N', TM_CREATED timestamp default sysdate not null,  DT_CREATED date default sysdate not null, DT_UPDATED date default sysdate not null, DT_AVAILABLE date, DT_END date )", "Table &quot;" + Name + "&quot; created.");
                
                //Step 2 Create Sequence
                sequenceResult = EP_ADD_ORACLE_SEQUENCE(DB_Platform, ConnAuth, Name);
            
                //Step 3 Create Trigger
                triggerResult = EP_ADD_SEQUENCE_TRIGGER(DB_Platform, ConnAuth, Name, "Before", "INSERT", (Name + "_SQ"), (Name + "_ID"));

                //Add Table Information to Dictionary
                EP_ADD_Dictionary_Table(DB_Platform, ConnAuth, Name); 

                if (tableresult.ToLower().Contains("err") || sequenceResult.ToLower().Contains("err") || triggerResult.ToLower().Contains("err"))
                {
                    return "Error Creating Oracle Table, Sequence, and/or Trigger";
                }
                else
                {
                    return "Table, Sequence, and Triger Created";
                }
                 
                case "Microsoft" :
                case "MICROSOFT" :

                return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "CREATE TABLE " + tempstringNAME + "(" + tempstringNAME + "_ID integer not null IDENTITY(1000,1), enabled char(1) default 'N', TM_CREATED timestamp,  DT_CREATED datetime default getdate() not null, DT_UPDATED date default getdate() not null, DT_AVAILABLE datetime, DT_END datetime )", "Table " + Name + " successfully created.");

                default :

                return "Wrong Platform";
            }
        }

        public string EP_ADD_VIEW(string DB_Platform, string ConnAuth, string ViewName, List<sqlSelectStructure> QueryStructure)
        {

            string _ViewName = MaxNameLength(ViewName, 27) + "_VW";
            StringBuilder SQLBuffer = new StringBuilder();

            SQLBuffer.Append("CREATE VIEW " + _ViewName + " AS ");
            SQLBuffer.Append(EP_GENERATE_QUERY(DB_Platform, QueryStructure));

            return EP_RUN_NONE_QUERY(DB_Platform, ConnAuth, SQLBuffer.ToString(), "View " + _ViewName + " created.").ToString();
            //return SQLBuffer.ToString();
        }

        public ArrayList EP_ADD_SCAFFOLD(string DB_PLATFORM, string connAuth, string Name, List<ColumnStructure> ColumnsList)
        {
            ArrayList HoldResult = new ArrayList();

            HoldResult.Add("--Start Scaffold " + Name + "");
            ArrayList ColumnsList2 = new ArrayList();
            ColumnsList2.Add(Name + "_ID");
            HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Name));
            HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Name, ColumnsList));
            HoldResult.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, Name + "_ID", Name, ColumnsList2));
           // HoldResult.Add(EP_ADD_ORACLE_SEQUENCE(DB_PLATFORM, connAuth, Name));
           // HoldResult.Add(EP_ADD_SEQUENCE_TRIGGER(DB_PLATFORM, connAuth, Name, "Before", "INSERT", (Name + "_SQ"), (Name + "_ID")));
            HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(EP_ADD_UPDATE_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(EP_ADD_DELETE_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add("--EndScaffold " + Name + "");

            return HoldResult;
        }

        public ArrayList EP_ADD_DATA_SCAFFOLD(string DB_PLATFORM, string connAuth, string TableName)
        {
            ArrayList HoldResult = new ArrayList();

            string newTableName = MaxNameLength(TableName, 14);
            string seq_Name = MaxNameLength(TableName, 14) + "_data";
            string Char_Table = newTableName + "_Dat_Char";
            string Date_Table = newTableName + "_Dat_Date";
            string Number_Table = newTableName + "_Dat_Numb";

            HoldResult.Add("---Start Data Scaffold " + TableName + "");
            List<ColumnStructure> ColumnsList = new List<ColumnStructure>();
            
            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":

                    //ColumnsList2.Add(TableName + "_ID");
                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Char_Table));
                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Number_Table));
                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Date_Table));

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "varchar2(4000)", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Char_Table, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Number_Table, ColumnsList));

                    ColumnsList.Clear();
                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "date", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Date_Table, ColumnsList));


                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Char_Table + "_1", Char_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Char_Table + "_2", Char_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Number_Table + "_1", Number_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Number_Table + "_2", Number_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Date_Table + "_1", Date_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Date_Table + "_2", Date_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    //HoldResult.Add(EP_ADD_ORACLE_SEQUENCE(DB_PLATFORM, connAuth, seq_Name));
                    //HoldResult.Add(EP_ADD_ORACLE_SEQUENCE(DB_PLATFORM, connAuth, Number_Table));
                    //HoldResult.Add(EP_ADD_ORACLE_SEQUENCE(DB_PLATFORM, connAuth, Date_Table));

                    //HoldResult.Add(EP_ADD_SEQUENCE_TRIGGER(DB_PLATFORM, connAuth, Char_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Char_Table + "_ID")));
                    //HoldResult.Add(EP_ADD_SEQUENCE_TRIGGER(DB_PLATFORM, connAuth, Number_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Number_Table + "_ID")));
                    //HoldResult.Add(EP_ADD_SEQUENCE_TRIGGER(DB_PLATFORM, connAuth, Date_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Date_Table + "_ID")));

                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Char_Table, ""));
                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Number_Table, ""));
                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Date_Table, ""));

                    //HoldResult.Add(EP_ADD_UPDATE_PROCEDURE(DB_PLATFORM, connAuth, TableName, ""));
                    //HoldResult.Add(EP_ADD_DELETE_PROCEDURE(DB_PLATFORM, connAuth, TableName, ""));
                   

                    break;
                case "Microsoft":
                case "MICROSOFT":

                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Char_Table));
                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Number_Table));
                    HoldResult.Add(EP_ADD_TABLE(DB_PLATFORM, connAuth, Date_Table));

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "varchar(4000)", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Char_Table, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Number_Table, ColumnsList));

                    ColumnsList.Clear();
                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });
                    ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "datetime", _IsNull = false, _DefaultValue = "" });

                    HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Date_Table, ColumnsList));


                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Char_Table + "_1", Char_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Char_Table + "_2", Char_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Number_Table + "_1", Number_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Number_Table + "_2", Number_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Date_Table + "_1", Date_Table, TableName, ColumnsList, ColumnsList));

                    ColumnsList.Clear();

                    ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "integer", _IsNull = false, _DefaultValue = "" });

                    HoldResult.Add(EP_ADD_FOREIGN_KEY(DB_PLATFORM, connAuth, Date_Table + "_2", Date_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Char_Table, ""));
                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Number_Table, ""));
                    HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Date_Table, ""));

                   
                    break;
            }
            HoldResult.Add("---End Data Scaffold " + TableName + "");
            return HoldResult;
        }

        public ArrayList EP_EXTEND_SCAFFOLD(string DB_PLATFORM, string connAuth, string Name)
        {

            ArrayList HoldResult = new ArrayList();

            HoldResult.Add("Scaffold " + Name + "");
            ArrayList ColumnsList2 = new ArrayList();
            ColumnsList2.Add(Name + "_ID");
            //Add(EP_ADD_TABLE(Name));
            //HoldResult.AddRange(EP_ADD_COLUMNS(DB_PLATFORM, connAuth, Name, ColumnsList));
            //HoldResult.Add(EP_ADD_UNIQUE_KEY(DB_PLATFORM, connAuth, Name, ColumnsList2));
            HoldResult.Add(EP_ADD_ORACLE_SEQUENCE(DB_PLATFORM, connAuth, Name));
            HoldResult.Add(EP_ADD_SEQUENCE_TRIGGER(DB_PLATFORM, connAuth, Name, "Before", "INSERT", (Name + "_SQ"), (Name + "_ID")));
            HoldResult.Add(EP_ADD_INSERT_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(EP_ADD_UPDATE_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(EP_ADD_DELETE_PROCEDURE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add("Scaffold " + Name + "");

            return HoldResult;
        }

        public ArrayList EP_DROP_ALL(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();


            switch (DB_PLATFORM)
            {

                case "ORACLE":
                case "Oracle":
                    Logger.Add(EP_DROP_ORACLE_USER(connAuth, SystemName));

                    Logger.Add(EP_DROP_ORACLE_TABLESPACE(connAuth, SystemName));

                    Logger.Add(EP_DROP_ORACLE_PROFILE(connAuth, SystemName));

                    Logger.Add(EP_DROP_ORACLE_ROLE(connAuth, SystemName));
                    break;
                case "Microsoft":
                case "MICROSOFT":
                    Logger.Add(EP_DROP_MSSQL_DB(connAuth, SystemName));
                    Logger.Add(EP_DROP_MSSQL_USER(connAuth, SystemName));
                    break;
            }
            return Logger;
        }


        //MICROSOFT STRUCTURE METHODS

        //   
        
        public string EP_ADD_MSSQL_DB(string ConnAuth, string Name)
        {
            //Create Database in SqlServer
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                ServerConnection conn = new ServerConnection(sqlConnection);
                Server srv = new Server(conn);


                //Connect to the local, default instance of SQL Server. 
                // Server srv = new Server);
                //Define a Database object variable by supplying the server and the database name arguments in the constructor. 
                Database db;
                db = new Database(srv, Name);
                //Create the database on the instance of SQL Server. 
                db.Create();
                //db.SetOwner(Name);
                db.Alter();
                //Reference the database and display the date when it was created. 
                //db = srv.Databases["Test_SMO_Database"];
                //Console.WriteLine(db.CreateDate);
                //Remove the database. 
                //db.Drop();

                return "SQL Server Database Created";
            }
            catch (Exception e)
            {
                return e.ToString();
            }



        }

        public string EP_ADD_MSSQL_LOGIN(string ConnAuth, string LoginName, string Password, string DefaultDatabase)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                ServerConnection conn = new ServerConnection(sqlConnection);
                Server srv = new Server(conn);

                Login newLogin = new Login(srv, LoginName);
                newLogin.LoginType = Microsoft.SqlServer.Management.Smo.LoginType.SqlLogin;

                //newLogin.AddToRole("sysadmin");

                newLogin.DefaultDatabase = DefaultDatabase;

                newLogin.Create(Password);

                return "SQL Server Login " + LoginName + " Created. The defaultdb is " + newLogin.DefaultDatabase.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string EP_ADD_MSSQL_ROLE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_RL";

            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "Create Role " + tempstringNAME, "Role &quot;" + Name + "&quot; Created");

            /* try
             {
                 String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();

                 SqlConnection sqlConnection = new SqlConnection(connectionString);
                 ServerConnection conn = new ServerConnection(sqlConnection);
                 Server svr = new Server(conn);

                 Database db = svr.Databases["TESTDB1"];

                 //Login login = new Login(svr, Name);
                 //login.LoginType = LoginType.SqlLogin;
                 //login.Create("password@1");

                 //User user1 = new User(db, "User1");
                 //user1.Login = "login1";
                 //user1.Create(); 

                 DatabaseRole dbrole = new DatabaseRole()

                 dbrole.Create 

                 //User user1 = db.Users["User1"];
                 //user1.Drop();

                 Login Login1 = svr.Logins[Name];
                 Login1.Drop();


                 return "MSSQL User " + Name + " has been succesfully dropped";
             }
             catch (Exception e)
             {
                 return e.ToString();
             }*/
        }

        public string EP_ADD_MSSQL_USER(string ConnAuth, string UserName, string LoginName, string DatabaseName, string DefaultSchema)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                ServerConnection conn = new ServerConnection(sqlConnection);
                Server srv = new Server(conn);
                Database db = srv.Databases[DatabaseName];

                User newUser = new User(db, UserName);

                newUser.DefaultSchema = DefaultSchema;
                newUser.Login = LoginName;

                Thread.Sleep(5000);

                newUser.Create();

                DatabasePermissionSet perms = new DatabasePermissionSet();

                perms.Connect = true;
                perms.CreateTable = true;

                perms.Select = true;
                perms.Insert = true;
                perms.Delete = true;
                perms.Execute = true;
                perms.CreateSchema = true;
                perms.CreateRole = true;
                perms.CreateTable = true;
                perms.CreateProcedure = true;
                perms.CreateFunction = true;
                perms.Control = true;
                perms.TakeOwnership = true;


                db.Grant(perms, UserName);



                return "SQL Server UserName " + UserName + " Created for Login " + LoginName + ". The defaultdb is " + DatabaseName;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string EP_ADD_MSSQL_SCHEMA(string ConnAuth, string SchemaName, string SchemaOwner)
        {
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "CREATE SCHEMA " + SchemaName + " AUTHORIZATION " + SchemaOwner, "Schema Successfully created");
        }

        public string EP_GRANT_MSSQL_PRIVILEGE(string ConnAuth, string Privilege, string Grantee, string SourceDB)
        {
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "GRANT " + Privilege + " TO " + Grantee, "Grant Successfully issued");
        }

        public string EP_ADD_MEMBER_TO_ROLE(string ConnAuth, string Role, string MemberName)
        {

            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "EXEC sp_addrolemember '" + Role + "','" + MemberName + "'", MemberName + " added to role " + Role + " successfully");
        }

        public string EP_UPDATE_MSSQL_USER_DEFAULT_SCHEMA(string ConnAuth, string DatabaseName, string UserName, string DefaultSchema)
        {
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "ALTER USER " + UserName + " with DEFAULT_SCHEMA =" + DefaultSchema + "", UserName + " default schema changed to " + DefaultSchema + " successfully");

        }

        public string EP_UPDATE_MSSQL_LOGIN_DEFAULT_DB(string ConnAuth, string DatabaseName, string LoginName, string DefaultDatabase)
        {
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "ALTER LOGIN " + LoginName + " with DEFAULT_DATABASE  =" + DefaultDatabase + "", LoginName + " default db changed to " + DefaultDatabase + " successfully");

        }

        public string EP_ALTER_AUTH_ON_MSSQL_SCHEMA(string ConnAuth, string DatabaseName, string Assignee, string SchemaName)
        {
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "ALTER AUTHORIZATION ON SCHEMA::" + DatabaseName + "." + SchemaName + " TO " + Assignee, Assignee + " has been authorized for Schema " + SchemaName + " successfully");

        }

        public string EP_DROP_MSSQL_USER(string ConnAuth, string Name)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                ServerConnection conn = new ServerConnection(sqlConnection);
                Server svr = new Server(conn);

                Database db = svr.Databases["TESTDB1"];

                //Login login = new Login(svr, Name);
                //login.LoginType = LoginType.SqlLogin;
                //login.Create("password@1");

                //User user1 = new User(db, "User1");
                //user1.Login = "login1";
                //user1.Create(); 

                //User user1 = db.Users["User1"];
                //user1.Drop();

                Login Login1 = svr.Logins[Name];
                Login1.Drop();


                return "MSSQL User " + Name + " has been succesfully dropped";
            }
            catch (Exception e)
            {
                return e.ToString();
            }


        }

        public string EP_DROP_MSSQL_DB(string ConnAuth, string Name)
        {
            //Drop Database in SqlServer
            try
            {
                String connectionString = ConnAuth;

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                ServerConnection conn = new ServerConnection(sqlConnection);
                Server srv = new Server(conn);

                //Connect to the local, default instance of SQL Server. 
                // Server srv = new Server);
                //Define a Database object variable by supplying the server and the database name arguments in the constructor. 
                Database db;
                //db = new Database(srv, Name);
                srv.KillAllProcesses(Name);

                db = srv.Databases[Name];

                //Drop the database on the instance of SQL Server. 
                db.Drop();
                //Reference the database and display the date when it was created. 
                //db = srv.Databases["Test_SMO_Database"];
                //Console.WriteLine(db.CreateDate);
                //Remove the database. 
                //db.Drop();

                return "SQL Server Database Dropped";
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }

        //ORACLE STRUCTURE METHODS

        //
        //Creates User in Oracle


        public string EP_ADD_ORACLE_USER(string ConnAuth, string Name, string Password)
        {
            using (OracleConnection connection = new OracleConnection(constr))
            {
                try
                {
                    Int32 NonQueryReturn = 0;
                    connection.Open();

                    OracleCommand sqlplus = connection.CreateCommand();

                    StringBuilder sqlStatement = new StringBuilder();

                    sqlStatement.Append("CREATE USER " + MaxNameLength(Name, 30) + " PROFILE ");
                    sqlStatement.Append("DEFAULT IDENTIFIED BY " + Password + " ");
                    sqlStatement.Append("DEFAULT TABLESPACE " + Name + "_TS ");
                    sqlStatement.Append("TEMPORARY TABLESPACE TEMP ");
                    sqlStatement.Append("QUOTA UNLIMITED ON " + Name + "_TS ");
                    sqlStatement.Append("ACCOUNT UNLOCK ");


                    sqlplus.CommandText = sqlStatement.ToString();

                    NonQueryReturn = sqlplus.ExecuteNonQuery();

                    //Clear sqlstatement for new sql.
                    sqlStatement.Clear();
                    sqlStatement.Append("GRANT CONNECT TO " + MaxNameLength(Name, 30));

                    sqlplus.CommandText = sqlStatement.ToString();
                    sqlplus.ExecuteNonQuery();
                    //  return sqlStatement.ToString();
                    return ("User " + Name + " created.");


                }
                catch (OracleException ex)
                {
                    return DB_ERROR_FORMATTER("ORACLE", ex.ToString());
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();

                }

            }
        }
        
        public string EP_ADD_ORACLE_PROFILE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_PF";

            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("CREATE PROFILE " + tempstringNAME + " LIMIT CPU_PER_SESSION UNLIMITED ");
            sqlStatement.Append("CPU_PER_CALL UNLIMITED ");
            sqlStatement.Append("CONNECT_TIME UNLIMITED ");
            sqlStatement.Append("IDLE_TIME UNLIMITED ");
            sqlStatement.Append("SESSIONS_PER_USER UNLIMITED ");
            sqlStatement.Append("LOGICAL_READS_PER_SESSION UNLIMITED ");
            sqlStatement.Append("LOGICAL_READS_PER_CALL UNLIMITED ");
            sqlStatement.Append("PRIVATE_SGA UNLIMITED ");
            sqlStatement.Append("COMPOSITE_LIMIT UNLIMITED ");
            sqlStatement.Append("PASSWORD_LIFE_TIME 180 ");
            sqlStatement.Append("PASSWORD_GRACE_TIME 7 ");
            sqlStatement.Append("PASSWORD_REUSE_MAX UNLIMITED ");
            sqlStatement.Append("PASSWORD_REUSE_TIME UNLIMITED ");
            sqlStatement.Append("PASSWORD_LOCK_TIME 1 ");
            sqlStatement.Append("FAILED_LOGIN_ATTEMPTS 10 ");
            sqlStatement.Append("PASSWORD_VERIFY_FUNCTION NULL ");

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, sqlStatement.ToString(), "Profile &quot;" + tempstringNAME + "&quot; created.");

        }

        public string EP_ADD_ORACLE_ROLE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_RL";

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "Create Role " + tempstringNAME, "Role &quot;" + Name + "&quot; Created");
        }

        public string EP_ADD_ORACLE_SEQUENCE(string DB_Platform, string ConnAuth, string Name)
        {

            string tempstringNAME = MaxNameLength(Name, 27) + "_SQ";

            return EP_RUN_NONE_QUERY("ORACLE", constr1, "Create Sequence " + tempstringNAME + " INCREMENT BY 1 START WITH 1000", "Sequence &quot;" + MaxNameLength(Name, 30) + "&quot; Created");
        }

        public string EP_ADD_SEQUENCE_TRIGGER(string DB_Platform, string ConnAuth, string TableName, string BeforeOrAfter, string Action, string SequenceName, string OneColumnName)
        {

            string tempstringNAME = MaxNameLength(OneColumnName + "_" + Action + "_TR", 27);

            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("CREATE or Replace Trigger " + tempstringNAME + " " + BeforeOrAfter + " " + Action + " ");
            sqlStatement.AppendLine("ON " + TableName + " ");
            sqlStatement.AppendLine("FOR EACH ROW ");
            sqlStatement.AppendLine("BEGIN ");
            sqlStatement.AppendLine("SELECT " + SequenceName + ".nextval INTO :new." + OneColumnName + " from dual; ");
            sqlStatement.AppendLine("END;");

            //return sqlStatement.ToString();
            return EP_RUN_NONE_QUERY("ORACLE", constr1, sqlStatement.ToString(), "Trigger &quot;" + tempstringNAME + "&quot; created.");
        }
       
        public string EP_GRANT_ORACLE_PRIVILEGE(string ConnAuth, string PrivType, string Grantee)
        {
            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "Grant " + PrivType + " to " + Grantee, PrivType + " granted to &quot;" + Grantee + "&quot;");
        }

        public string EP_ADD_ORACLE_TABLESPACE(string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_TS";

            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("CREATE SMALLFILE TABLESPACE " + tempstringNAME + " ");
            sqlStatement.Append("DATAFILE '" + ConfigurationManager.AppSettings["OracleOraDataFile"].ToString() + "\\" + Name + "_DF_1.dbf' ");
            sqlStatement.Append("SIZE 50M REUSE LOGGING ");
            sqlStatement.Append("EXTENT MANAGEMENT LOCAL SEGMENT SPACE MANAGEMENT AUTO ");
            //sqlStatement.Append("DEFAULT STORAGE(ENCRYPT) ENCRYPTION USING 'AES192'");

            return EP_RUN_NONE_QUERY("ORACLE", constr, sqlStatement.ToString(), "Tablespace " + tempstringNAME + " created.");

        }

        public string EP_DROP_ORACLE_PROFILE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_PF";

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "DROP Profile " + tempstringNAME, "Profile " + tempstringNAME + " dropped.").ToString();
        }

        public string EP_DROP_ORACLE_ROLE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_RL";

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "Drop Role " + tempstringNAME, "Role " + tempstringNAME + " dropped.");
        }

        public string EP_DROP_ORACLE_TABLE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27);

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "DROP TABLE " + tempstringNAME, "Table &quot;" + tempstringNAME + "&quot; dropped.");
        }

        public string EP_DROP_ORACLE_TABLESPACE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_TS";

            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "DROP Tablespace " + tempstringNAME, "Tablespace &quot;" + tempstringNAME + "&quot; dropped.").ToString();
        }

        public string EP_DROP_ORACLE_USER(string ConnAuth, string Name)
        {
            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "DROP USER " + Name + " CASCADE ", "User &quot;" + Name + "&quot; dropped.");
        }

        public string SQL_KILL_SESSIONS(string UserName)
        {

            return EP_RUN_NONE_QUERY("ORACLE", constr, "begin CMS.KILL_SESSIONS(" + UserName + ") end;", "User Sessions Killed ");
        }

        // START  DB SQL Executes, USER for DDL, and DML

        //Only for None Query SQL statements.
        public string EP_RUN_NONE_QUERY(string DB_Platform, string ConnAuth, string SQLin, string SuccessMessage)
        {
            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":
                    using (OracleConnection connection = new OracleConnection(ConnAuth))
                    {
                        try
                        {
                            //Open Connection
                            connection.Open();

                            //Initiate sqlplus.
                            OracleCommand sqlplus = connection.CreateCommand();

                            //Initiate buffer for SQL syntax.
                            StringBuilder sqlStatement = new StringBuilder();

                            //Input Received SQL.
                            sqlStatement.Append(SQLin);

                            //Stage SQL Statement.
                            sqlplus.CommandText = sqlStatement.ToString();

                            //Run the SQL Statement.
                            sqlplus.ExecuteNonQuery();

                            return (SuccessMessage);

                        }
                        catch (OracleException ex)
                        {
                            //Return Oracle Error.
                            return DB_ERROR_FORMATTER("ORACLE", ex.ToString());
                        }
                        finally
                        {
                            //Close connection.
                            connection.Close();
                            connection.Dispose();
                        }

                    }
                   
                case "Microsoft":
                case "MICROSOFT":
                    using (SqlConnection connection = new SqlConnection(ConnAuth))
                    {
                        try
                        {
                            connection.Open();

                            SqlCommand sqlplus = connection.CreateCommand();

                            StringBuilder sqlStatement = new StringBuilder();

                            sqlStatement.Append(SQLin);

                            sqlplus.CommandText = sqlStatement.ToString();

                            sqlplus.ExecuteNonQuery();

                            return (SuccessMessage);
                        }
                        catch (SqlException ex)
                        {
                            return DB_ERROR_FORMATTER("MICROSOFT", ex.ToString());
                        }
                        finally
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                    
                default:
                    return "Invalid DB Platform";
                   
            }

        }

        public DataTable EP_RUN_QUERY(string DB_Platform, string ConnAuth, string SQLin)
        {
            DataTable dt = new DataTable("ReturnData");
            DataColumn column;
            DataRow row;
            
            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":
                    using (OracleConnection connection = new OracleConnection(ConnAuth))
                    {
                        try
                        {
                            //Open Connection
                            connection.Open();

                            //Initiate sqlplus.
                            OracleCommand sqlplus = connection.CreateCommand();

                            //Initiate buffer for SQL syntax.
                            StringBuilder sqlStatement = new StringBuilder();

                            //Input Received SQL.
                            sqlStatement.Append(SQLin);

                            //Stage SQL Statement.
                            sqlplus.CommandText = sqlStatement.ToString();

                            //Run the SQL Statement and load data into Reader.
                            OracleDataReader dr = sqlplus.ExecuteReader();

                            //Pass Data to Datatable
                            dt.Load(dr);

                            // return (SuccessMessage);

                        }
                        catch (OracleException ex)
                        {
                            //Return Oracle Error.
                            //return ex.ToString();

                            // Create first column and add to the DataTable.
                            column = new DataColumn();
                            column.DataType = System.Type.GetType("System.Int32");
                            column.ColumnName = "ChildID";
                            column.AutoIncrement = true;
                            column.AutoIncrementSeed = 0;
                            column.AutoIncrementStep = 1;
                            column.Caption = "ID";
                            column.ReadOnly = true;
                            column.Unique = true;
                            dt.Columns.Add(column);

                            // Create second column and add to the DataTable.
                            column = new DataColumn();
                            column.DataType = System.Type.GetType("System.String");
                            column.ColumnName = "ChildType";
                            column.AutoIncrement = false;
                            column.Caption = "ChildType";
                            column.ReadOnly = false;
                            column.Unique = false;
                            dt.Columns.Add(column);

                            // Create third column and add to the DataTable.
                            column = new DataColumn();
                            column.DataType = System.Type.GetType("System.String");
                            column.ColumnName = "ChildItem";
                            column.AutoIncrement = false;
                            column.Caption = "ChildItem";
                            column.ReadOnly = false;
                            column.Unique = false;
                            dt.Columns.Add(column);

                            // Create fourth column and add to the DataTable.
                            column = new DataColumn();
                            column.DataType = System.Type.GetType("System.String");
                            column.ColumnName = "ChildValue";
                            column.AutoIncrement = false;
                            column.Caption = "ChildValue";
                            column.ReadOnly = false;
                            column.Unique = false;
                            dt.Columns.Add(column);

                            row = dt.NewRow();
                            row["ChildType"] = "RETURN";
                            row["ChildItem"] = "ERROR";
                            row["ChildValue"] = DB_ERROR_FORMATTER("ORACLE", ex.ToString());
                            dt.Rows.Add(row);


                            //return dt; 
                        }
                        finally
                        {
                            //Close connection.
                            connection.Close();
                            connection.Dispose();
                        }

                    }

            return dt;
                case "Microsoft":
                case "MICROSOFT":
            using (SqlConnection connection = new SqlConnection(ConnAuth))
            {
                try
                {
                    connection.Open();

                    SqlCommand sqlplus = connection.CreateCommand();

                    StringBuilder sqlStatement = new StringBuilder();

                    sqlStatement.Append(SQLin);

                    sqlplus.CommandText = sqlStatement.ToString();

                    SqlDataReader dr = sqlplus.ExecuteReader();

                    dt.Load(dr);
                }
                catch (SqlException ex)
                {
                    //Return Oracle Error.
                    //return ex.ToString();

                    // Create first column and add to the DataTable.
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.Int32");
                    column.ColumnName = "ChildID";
                    column.AutoIncrement = true;
                    column.AutoIncrementSeed = 0;
                    column.AutoIncrementStep = 1;
                    column.Caption = "ID";
                    column.ReadOnly = true;
                    column.Unique = true;
                    dt.Columns.Add(column);

                    // Create second column and add to the DataTable.
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "ChildType";
                    column.AutoIncrement = false;
                    column.Caption = "ChildType";
                    column.ReadOnly = false;
                    column.Unique = false;
                    dt.Columns.Add(column);

                    // Create third column and add to the DataTable.
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "ChildItem";
                    column.AutoIncrement = false;
                    column.Caption = "ChildItem";
                    column.ReadOnly = false;
                    column.Unique = false;
                    dt.Columns.Add(column);

                    // Create fourth column and add to the DataTable.
                    column = new DataColumn();
                    column.DataType = System.Type.GetType("System.String");
                    column.ColumnName = "ChildValue";
                    column.AutoIncrement = false;
                    column.Caption = "ChildValue";
                    column.ReadOnly = false;
                    column.Unique = false;
                    dt.Columns.Add(column);

                    row = dt.NewRow();
                    row["ChildType"] = "RETURN";
                    row["ChildItem"] = "ERROR";
                    row["ChildValue"] = DB_ERROR_FORMATTER("MICROSOFT", ex.ToString());
                    dt.Rows.Add(row);

                }
                finally
                {                 
                    connection.Close();
                    connection.Dispose();
                }

                return dt;
            }
                    
                default:
            return dt;

            }
        }

        // START DB Procedure Methods
        //

        public DataTable SQL_PROCEDURE(string DB_Platform, string ConnAuth, string ProcedureName, List<DBParameters> _dbParameters)
        {

            //string _return = "";
            //DataSet ProcReturn = new DataSet();

            DataTable table = ProcDataTable();
            DataRow row;
            switch (DB_Platform)
            {
                case "Oracle":
                case "ORACLE":
                    using (OracleConnection connection = new OracleConnection(ConnAuth))
                    {

                        OracleCommand DBProcedure = new OracleCommand();

                        //Pass DB Connection Settings
                        DBProcedure.Connection = connection;
                        //Pass Procedure Name
                        DBProcedure.CommandText = ProcedureName;
                        //Set Commandtype to Storeprocedure
                        DBProcedure.CommandType = CommandType.StoredProcedure;

                        //Iterate through all passed parameters.
                        foreach (DBParameters i in _dbParameters)
                        {
                            //Add Parameters to DBProcedure
                            DBProcedure.Parameters.Add(i.ParamName, i.OracleParamDataType, i.ParamSize, i.ParamValue, i.ParamDirection);
                        }

                        try
                        {
                            //Open Connection
                            connection.Open();

                            DBProcedure.ExecuteNonQuery();

                            int iNumber = 0;

                            foreach (DBParameters ii in _dbParameters)
                            {
                                //Resulting Data will be added to returning table.
                                row = table.NewRow();
                                row["ChildType"] = ii.ParamDirection.ToString();
                                row["ChildItem"] = DBProcedure.Parameters[iNumber].ParameterName.ToString();
                                row["ChildValue"] = DBProcedure.Parameters[iNumber].Value.ToString();
                                table.Rows.Add(row);

                                iNumber++;
                            }

                        }
                        catch (OracleException ex)
                        {
                            //Return Oracle Error.
                            row = table.NewRow();
                            row["ChildType"] = "Error";
                            row["ChildItem"] = "Exception";
                            row["ChildValue"] = DB_ERROR_FORMATTER("ORACLE", ex.ToString());
                            table.Rows.Add(row);
                        }
                        finally
                        {
                            //Close connection.
                            connection.Close();
                            connection.Dispose();
                        }

                    }
                break;
                case "Microsoft":
                case "MICROSOFT":
                using (SqlConnection connection = new SqlConnection(ConnAuth))
                    {
                        SqlCommand DBProcedure = new SqlCommand();

                        //Pass DB Connection Settings
                        DBProcedure.Connection = connection;
                        //Pass Procedure Name
                        DBProcedure.CommandText = ProcedureName;
                        //Set Commandtype to Storeprocedure
                        DBProcedure.CommandType = CommandType.StoredProcedure;

                        SqlParameter[] param = new SqlParameter[_dbParameters.Count];
                        int paramcount = 0;

                        //Iterate through all passed parameters.
                        foreach (DBParameters i in _dbParameters)
                        {
                            //Add Parameters to DBProcedure
                            //DBProcedure.Parameters.Add(i.ParamName, i.MSSqlParamDataType, i.ParamSize, i.ParamValue, i.ParamDirection);
                            //DBProcedure.Parameters.Add(i.ParamName, i.MSSqlParamDataType).Value = i.ParamValue;

                            
                            param[paramcount] = new SqlParameter();
                            param[paramcount].Direction = i.ParamDirection;
                            param[paramcount].ParameterName = i.ParamName;
                            param[paramcount].SqlDbType = i.MSSqlParamDataType;
                            
                            if (param[paramcount].SqlDbType == SqlDbType.Int)
                                if (i.ParamValue == "" || i.ParamValue == null)
                                    param[paramcount].Value = 0;
                                else
                                    param[paramcount].Value = Convert.ToInt32(i.ParamValue);
                            else
                                param[paramcount].Value = i.ParamValue;
                            

                            DBProcedure.Parameters.Add(param[paramcount]);
                            paramcount++;
                        }

                        try
                        {
                            //Open Connection
                            connection.Open();

                            DBProcedure.ExecuteNonQuery();

                            int iNumber = 0;

                            foreach (DBParameters ii in _dbParameters)
                            {
                                //Resulting Data will be added to returning table.
                                row = table.NewRow();
                                row["ChildType"] = ii.ParamDirection.ToString();
                                row["ChildItem"] = DBProcedure.Parameters[iNumber].ParameterName.ToString();
                                row["ChildValue"] = DBProcedure.Parameters[iNumber].Value.ToString();
                                table.Rows.Add(row);

                                iNumber++;
                            }

                        }
                        catch (SqlException ex)
                        {
                            //Return Oracle Error.
                            row = table.NewRow();
                            row["ChildType"] = "Error";
                            row["ChildItem"] = "Exception";
                            row["ChildValue"] = DB_ERROR_FORMATTER("Microsoft", ex.ToString());
                            table.Rows.Add(row);
                        }
                        finally
                        {
                            //Close connection.
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                break;
            }

            return table;
        }

        public DataTable SQL_PROCEDURE_PARAMS(string DB_Platform, string ConnAuth, string ProcedureName, List<DBParameters> _dbParameters)
        {

            DBParameters[] dbParameters = new DBParameters[_dbParameters.Count];
           

            List<DBParameters> infoList = new List<DBParameters>();

            int iNumber = 0;
            switch(DB_Platform)
            {
                case "Oracle":
                case "ORACLE":
                    foreach (DBParameters i in _dbParameters)
                    {
                        dbParameters[iNumber] = new DB_Toolbox.DBParameters();
                        dbParameters[iNumber].ParamName = i.ParamName;
                        dbParameters[iNumber].OracleParamDataType = i.OracleParamDataType;
                        dbParameters[iNumber].ParamDirection = i.ParamDirection;
                        dbParameters[iNumber].ParamValue = i.ParamValue;
                        dbParameters[iNumber].ParamSize = i.ParamSize;
                        dbParameters[iNumber].ParamReturn = i.ParamReturn;

                        infoList.Add(dbParameters[iNumber]);

                        iNumber++; //++ Increments by 1.
                    }
                    break;
                case "Microsoft":
                case "MICROSOFT":
                    foreach (DBParameters i in _dbParameters)
                    {
                        dbParameters[iNumber] = new DB_Toolbox.DBParameters();
                        dbParameters[iNumber].ParamName = i.ParamName;
                        dbParameters[iNumber].MSSqlParamDataType = i.MSSqlParamDataType;
                        dbParameters[iNumber].ParamDirection = i.ParamDirection; //Not used by MSSQL
                        dbParameters[iNumber].ParamValue = i.ParamValue; //Not used by MSSQL
                        dbParameters[iNumber].ParamSize = i.ParamSize;
                        dbParameters[iNumber].ParamReturn = i.ParamReturn;

                        infoList.Add(dbParameters[iNumber]);

                        iNumber++; //++ Increments by 1.
                    }
                    break;
            }

            return SQL_PROCEDURE(DB_Platform, ConnAuth, ProcedureName, infoList);
        }

        public string SQL_PROCEDURE_GET_VALUE(string parameter, DataTable Dataset1)
        {
            string returnVar = "";
            
            if (Dataset1.Columns.Contains("ChildItem"))
            {
                DataTable tempDT = ProcDataTable();

                tempDT = Dataset1;

                foreach (DataRow i in tempDT.Rows)
                {
                    if (i["ChildItem"].ToString() == parameter)
                    {
                        returnVar = i["ChildValue"].ToString();
                    }
                }
            }
            return returnVar;
        }

        // START DB SELECTS FROM DICTIONARY VIEWS
        //
        public DataTable EP_GET_COLUMNS_VIA_TABLENAME(string DB_Platform, string ConnAuth, string TableName, Boolean _isSeqID)
        {
            StringBuilder SQLin = new StringBuilder();
            SQLin.Append("Select COLUMN_NAME, DATA_TYPE, ");

            switch (DB_Platform)
            {

                case "Oracle":
                case "ORACLE":
                    SQLin.Append("DATA_LENGTH ");
                    SQLin.Append("from USER_TAB_COLUMNS ");
                    SQLin.Append("where upper(TABLE_NAME) = upper('" + TableName + "') ");
                    if (_isSeqID == false)
                    {
                        SQLin.Append("and column_id > 7  ");
                    }
                    else
                    {
                        SQLin.Append("and column_id = 1  ");
                    }
                    SQLin.Append("order by COLUMN_ID ASC ");

                    return EP_RUN_QUERY("Oracle", ConnAuth, SQLin.ToString());
                case "Microsoft":
                case "MICROSOFT":
                    SQLin.Append("CHARACTER_OCTET_LENGTH DATA_LENGTH ");
                    SQLin.Append("from INFORMATION_SCHEMA.COLUMNS ");
                    SQLin.Append("where upper(TABLE_NAME) = upper('" + TableName + "') ");
                    if (_isSeqID == false)
                    {
                        SQLin.Append("and ORDINAL_POSITION > 7  ");
                    }
                    else
                    {
                        SQLin.Append("and ORDINAL_POSITION = 1  ");
                    }
                    SQLin.Append("order by ORDINAL_POSITION ASC ");
                    return EP_RUN_QUERY("Microsoft", ConnAuth, SQLin.ToString());
                default:
                    return new DataTable();
            }

        }

        // START TOOL FORMATTING
        //        
        public string DB_ERROR_FORMATTER(string DB_Platform, string DB_ERROR)
        {
            //If set to true returns Oracle formatted Error
            Boolean runOracle = false;

            //If set to true returns Microst formatted Error
            Boolean runMicrosoft = false;

            StringBuilder returnString = new StringBuilder("");

            switch (DB_Platform)
            {
                case "Oracle":
                case "ORACLE":
                    runOracle = true;
                    break;
                case "Microsoft":
                case "MICROSOFT":
                    runMicrosoft = true;
                    break;
                default:
                    break;
            }

            if (runOracle)
            {
                if (DB_ERROR.Contains("Oracle.DataAccess.Client.OracleException"))
                    returnString.Append(ExtractString(DB_ERROR, "Oracle.DataAccess.Client.OracleException", "at Oracle.DataAccess.Client.OracleException.HandleErrorHelper"));
            }

            if (runMicrosoft)
            {
                //nothing yet.
                returnString.Append(ExtractString(DB_ERROR.ToString(), "System.Data.SqlClient.SqlException", "at System.Data.SqlClient.SqlInternalConnection.OnError"));
            }

            return returnString.ToString();
        }

        public string ExtractString(string s, string tag, string tag2)
        {
            // You should check for errors in real-world code, omitted for brevity
            try
            {
                var startTag = tag;
                int startIndex = s.IndexOf(startTag) + startTag.Length;
                var startTag2 = tag2;
                int startIndex2 = s.IndexOf(startTag2) - s.IndexOf(startTag);

                string temp = s.Substring(startIndex, startIndex2);
                return s.Substring(startIndex, startIndex2);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string MaxNameLength(string name, int maxLength)
        {
            string returnString = name;

            if (name.Length > maxLength)
                returnString = name.Substring(0, maxLength);

            return returnString;

            // END TOOL FORMATTING
            //
        }

        public string CreateFilePath(string path1)
        {
            return path1.Replace(@"\",@"\\");
        }

        //START TOOL UNIVERSAL 
        //
        public string EP_GET_DATATYPE(string DB_Platform, string column)
        {
            string tempColumn = column.ToLower(); //ExtractString(column, "", "(");

            if (column.ToLower().Contains("characters") || column.ToLower().Contains("varchar"))
                tempColumn = ExtractString(column, "", "(");

            switch(DB_Platform)
            {
                case "Oracle":
                case "ORACLE":

                    switch (tempColumn.ToLower())
                    {
                        case "characters":
                        case "varchar2":
                        case "varchar":     
                            return "varchar2(" + ExtractString(column, "(", ")");
                        case "numbers":
                        case "number":
                        case "integer":
                        case "numbs":
                        case "int":
                        case "ints":
                            return "number";
                        case "dates":
                        case "date":
                            return "date";
                        case "times":
                        case "timestamp":
                            return "timestamp";
                        default:
                            return "Invalid Column Type";
                    }

                case "Microsoft":
                case "MICROSOFT":

                    switch (tempColumn.ToLower())
                    {
                        case "characters":
                        case "varchar2":
                        case "varchar":
                            return "varchar(" + ExtractString(column, "(", ")");
                        case "numbers":
                        case "integer":
                        case "numbs":
                        case "int":
                        case "ints":
                            return "integer";
                        case "dates":
                        case "date":
                            return "datetime";
                        case "times":
                        case "timestamp":
                            return "timestamp";
                        default:
                            return "Invalid Column Type";
                    }

                default:
                    return "Invalid DB Platform";
            }
        }
    }
}