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
        string constr = System.Configuration.ConfigurationManager.ConnectionStrings["SystemDBConnection"].ToString();
        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        string constrSQLServer = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();
        string constrSQLServer1 = System.Configuration.ConfigurationManager.ConnectionStrings["MSSQLDBConnection"].ToString();

        public class DBParameters
        {
            public string ParamName { get; set; }
            public OracleDbType ParamDBType { get; set; }
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

        public ArrayList EP_CREATE_DB(string DB_PLATFORM, string connRoot, string connAuth, string SystemName, string Password)
        {

            ArrayList Logger = new ArrayList();

            switch (DB_PLATFORM)
            {

                case "Oracle" :
                case "ORACLE" :
                    
                    Logger.AddRange(EP_DROP_ALL(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(EP_BUILD_CORE_STRUCTURE(DB_PLATFORM, connRoot, connAuth, SystemName, Password));

                    Logger.AddRange(EP_BUILD_SYSTEM_STRUCTURE(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(EP_BUILD_SYSTEM_VIEWS(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(SQL_DATA_DEFAULT(DB_PLATFORM, connAuth, SystemName));

                    Logger.AddRange(SQL_DATA_EXAMPLE(DB_PLATFORM, connAuth, SystemName));
                break;
                
                case "Microsoft":
                case "MICROSOFT":
                case "MSSQLSVR":
                    Logger.AddRange(EP_DROP_ALL(DB_PLATFORM, connRoot, SystemName));
                    Logger.AddRange(EP_BUILD_CORE_STRUCTURE(DB_PLATFORM, connRoot, connAuth, SystemName, Password));
                    

                break;

            }
            return Logger;
        }

        public ArrayList EP_BUILD_CORE_STRUCTURE(string DB_PLATFORM, string connRoot, string connAuth, string SystemName, string Password)
        {
            ArrayList Logger = new ArrayList();

            switch (DB_PLATFORM)
            {

                case "Oracle":
                case "ORACLE":
                    Logger.Add(EP_ADD_ORACLE_PROFILE(constr, SystemName));

                    Logger.Add(SQL_Add_Tablespace(SystemName));

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
                   Logger.Add(EP_ADD_MSSQL_DB(connAuth, SystemName));
                   Logger.Add(EP_ADD_MSSQL_USER(connAuth, SystemName, SystemName, SystemName, "db_owner"));
                   Logger.Add(EP_ADD_MICROSOFT_ROLE(connAuth, SystemName));
                      
                   Logger.Add(EP_ADD_MSSQL_SCHEMA(connAuth, SystemName, SystemName));
                   Logger.Add(EP_GRANT_MSSQL_PRIVILEGE(connAuth, "ALL", SystemName, SystemName));        
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

            //---CREATE - OBJECT Layer--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJECT_LAYERS", ScaffoldColumns));

            //---PK - OBJECT LAYERS Unique KEY--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Primary(DB_PLATFORM, constr1, "OBJECT_LAYERS_1", "OBJECT_LAYERS", ExistingColumnsList));

            //---CREATE - OBJECTS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJECTS", ScaffoldColumns));

            //---PK - OBJECTS Unique KEY--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Primary(DB_PLATFORM, constr1, "OBJECTS_1", "OBJECTS", ExistingColumnsList));

            //---FK - OBJECT LAYERS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_LAYER", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECTS_1", "OBJECTS", "OBJECT_LAYERS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - LOGS--->
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Entry", _DataType = "varchar2(4000)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "LOGS", ScaffoldColumns));

            //---FK - LOGS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "LOGS_1", "LOGS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - ORGANIZATIONS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "Organizations", ScaffoldColumns));

            //---FK - ORGANIZATIONS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "ORGANIZATIONS_1", "ORGANIZATIONS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Groups--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "GROUPS", ScaffoldColumns));

            //---FK - GROUPS FOREIGN KEYS 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GROUP_1", "GROUPS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GROUPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GROUPS_2", "GROUPS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - USERS--->
                ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
                ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "USERS", ScaffoldColumns));

            

            //---FK - USERS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "USERS_1", "USERS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - GROUPS & USERS--->
                ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
                ScaffoldColumns.Add(new ColumnStructure { _Name = "GROUPS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
                ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "GROUPS_USERS", ScaffoldColumns));

            //---FK - GROUPS & USERS FOREIGN KEY 1--->
                ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();        
                ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GROUPS_USERS_1", "GROUPS_USERS", "Users", ExistingColumnsList, ExistingColumnsList));

            //---FK - GROUPS & USERS FOREIGN KEY 2--->
                ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
                ExistingColumnsList.Add(new ColumnStructure { _Name = "GROUPS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GROUPS_USERS_2", "GROUPS_USERS", "GROUPS", ExistingColumnsList, ExistingColumnsList)); 

            //---CREATE - STAGES
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "STAGES", ScaffoldColumns));

            

            //---PK - STAGES Primary KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Primary(DB_PLATFORM, constr1, "STAGES_1", "STAGES", ExistingColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            MetaColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "STAGES_1", "STAGES", "OBJECTS", ExistingColumnsList, MetaColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "STAGES_2", "STAGES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "STAGES_3", "STAGES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Grips
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "GRIPS", ScaffoldColumns));         

            //---PK - GRIPS Primary KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Primary(DB_PLATFORM, constr1, "GRIPS_1", "GRIPS", ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GRIPS_1", "GRIPS", "STAGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GRIPS_2", "GRIPS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - GRIPS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "GRIPS_3", "GRIPS", "USERS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - OBJECTS SETS
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJECT_SETS", ScaffoldColumns));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Number", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Unique(DB_PLATFORM, connAuth, "OBJECT_SETS_1", "OBJECT_SETS", ExistingColumnsList));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "Number", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Unique(DB_PLATFORM, connAuth, "OBJECT_SETS_2", "OBJECT_SETS", ExistingColumnsList));


            //---FK - OBJECTS SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECT_SETS_1", "OBJECT_SETS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECT_SET_2", "OBJECT_SETS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECT_SETS_3", "OBJECT_SETS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - OBJECTS_SETS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "GRIP_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });  
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECT_SETS_5", "OBJECT_SETS", "GRIPS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - OBJECT OPTION SETS
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJ_OPT_SETS", ScaffoldColumns));

            //---FK OBJECT OPTION SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJECT_OPT_SETS_1", "OBJ_OPT_SETS", "OBJECT_SETS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - Value Datatypes--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "VALUE_DATATYPES", ScaffoldColumns));

            //---UK VALUE DATATYPES Unique KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Unique(DB_PLATFORM, constr1, "VALUE_DATATYPES_1", "VALUE_DATATYPES", ExistingColumnsList));

            //---CREATE - OBJECTS & PROPERTIES SETS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Property_Name", _DataType = "Varchar2(250)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "HAS_PARENT", _DataType = "Varchar2(5)", _DefaultValue = "true", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "HAS_CHILD", _DataType = "Varchar2(5)", _DefaultValue = "false", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PARENT_OBJ_PROP_SETS_ID", _DataType = "number", _DefaultValue = "0", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PROPERTY_VALUE", _DataType = "Varchar2(250)", _DefaultValue = "", _IsNull = true });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJ_PROP_SETS", ScaffoldColumns));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJ_PROP_SETS_1", "OBJ_PROP_SETS", "OBJECT_SETS", ExistingColumnsList, ExistingColumnsList));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJ_PROP_SETS_2", "OBJ_PROP_SETS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK OBJECTS & PROPERTIES SETS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "VALUE_DATATYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJ_PROP_SETS_3", "OBJ_PROP_SETS", "VALUE_DATATYPES", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - OBJECTS PROPERTIES OPTIONS SETS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "OBJ_PROP_OPT_SETS", ScaffoldColumns));

            //---FK OBJECTS PROPERTIES OPTIONS SETS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "OBJ_PROP_OPT_SETS_1", "OBJ_PROP_OPT_SETS", "OBJ_PROP_SETS", ExistingColumnsList, ExistingColumnsList));

            //---UK - OBJECTS SETS UNIQUE KEY 1---> 
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "Number", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "Option_Value", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Unique(DB_PLATFORM, connAuth, "OBJ_PROP_OPT_SETS_1", "OBJ_PROP_OPT_SETS", ExistingColumnsList));


            //---CREATE - CORES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "CORES", ScaffoldColumns));

            

            //---FK - CORES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CORES_1", "CORES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CORES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CORES_2", "CORES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CORES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CORES_3", "CORES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            
            //---CREATE - APPLICATIONS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "APPLICATIONS", ScaffoldColumns));

            

            //---PK - APPLICATIONS PRIMARY KEY--->
            ExistingColumnsList.Add(new ColumnStructure { _Name = "Name", _DataType = "varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Primary(DB_PLATFORM, constr1, "APPLICATIONS", "APPLICATIONS", ScaffoldColumns));

            //---FK - APPLICATIONS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            MetaColumnsList.Add(new ColumnStructure { _Name = "CORES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "APPLICATIONS_1", "APPLICATIONS", "CORES", MetaColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "APPLICATIONS_2", "APPLICATIONS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "APPLICATIONS_3", "APPLICATIONS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - APPLICATIONS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "APPLICATIONS_4", "APPLICATIONS", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - ROLES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "ROLES", ScaffoldColumns));

            //---FK - ROLES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "ROLES_1", "ROLES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));
            
            //---CREATE - PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "PRIVELEGES", ScaffoldColumns));

            //---FK - PRIVELEGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "PRIVELEGES_1", "PRIVELEGES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));
            
            //---CREATE - ROLES & PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ROLES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "PRIVELEGES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "ROLES_PRIVELEGES", ScaffoldColumns));

            //---FK - ROLES & PRIVELEGES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ROLES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "ROLES_PRIVELEGES_1", "ROLES_PRIVELEGES", "ROLES", ExistingColumnsList, ExistingColumnsList));

            //---FK - ROLES & PRIVELEGES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "PRIVELEGES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "ROLES_PRIVELEGES_2", "ROLES_PRIVELEGES", "PRIVELEGES", ExistingColumnsList, ExistingColumnsList));

            //---CREATE - USERS & ROLES & PRIVELEGES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ROLES_PRIVELEGES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "USERS_ROLES_PRIVELEGES", ScaffoldColumns));

            //---FK - USERS & ROLES & PRIVELEGES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ROLES_PRIVELEGES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "USERS_ROLES_PRIVELEGES_1", "USERS_ROLES_PRIVELEGES", "ROLES_PRIVELEGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - USERS &  ROLES & PRIVELEGES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "USERS_ROLES_PRIVELEGES_2", "USERS_ROLES_PRIVELEGES", "USERS", ExistingColumnsList, ExistingColumnsList));


 
            //---CREATE - CASES--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "APPLICATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });              
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "CASES", ScaffoldColumns));

            

            //---FK - CASES FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "APPLICATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_1", "CASES", "APPLICATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_2", "CASES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_3", "CASES", "OBJECTS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES FOREIGN KEY 4--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_4", "CASES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));



            //---CREATE - FORMS--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "Organizations_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "FORMS", ScaffoldColumns));

            //---FK - FORMS FOREIGN KEY 1--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_1", "FORMS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS FOREIGN KEY 2--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_2", "FORMS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS FOREIGN KEY 3--->
            ScaffoldColumns.Clear(); ExistingColumnsList.Clear(); MetaColumnsList.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "OBJECT_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_3", "FORMS", "OBJECTS", ExistingColumnsList, ExistingColumnsList)); 

            //---CREATE - CASES & FORMS--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "CASES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "CASES_FORMS", ScaffoldColumns));

            //---FK - CASES & FORMS FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "CASES_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_FORMS_1", "CASES_FORMS", "CASES", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES & FORMS FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_FORMS_2", "CASES_FORMS", "FORMS", ExistingColumnsList, ExistingColumnsList));  

            //---FK - CASES & FORMS FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_FORMS_3", "CASES_FORMS", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - CASES & FORMS FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "CASES_FORMS_4", "CASES_FORMS", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));


            //---CREATE - FORMS & STAGES--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            ScaffoldColumns.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.AddRange(SQL_CREATE_SCAFFOLD(DB_PLATFORM, constr1, "FORMS_STAGES", ScaffoldColumns));

            //---FK - FORMS & STAGES FOREIGN KEY 1--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_TYPE", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            ExistingColumnsList.Add(new ColumnStructure { _Name = "STAGE_NAME", _DataType = "Varchar2(50)", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_STAGES_1", "FORMS_STAGES", "STAGES", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 2--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "FORMS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_STAGES_2", "FORMS_STAGES", "FORMS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 3--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "USERS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_STAGES_3", "FORMS_STAGES", "USERS", ExistingColumnsList, ExistingColumnsList));

            //---FK - FORMS & OBJECTS STAGES FOREIGN KEY 4--->
            ExistingColumnsList.Clear(); MetaColumnsList.Clear(); ScaffoldColumns.Clear();
            ExistingColumnsList.Add(new ColumnStructure { _Name = "ORGANIZATIONS_ID", _DataType = "number", _DefaultValue = "", _IsNull = false });
            Logger.Add(SQL_Add_Key_Foreign(DB_PLATFORM, constr1, "FORMS_STAGES_4", "FORMS_STAGES", "ORGANIZATIONS", ExistingColumnsList, ExistingColumnsList));



            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "USERS"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "STAGES"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "GRIPS"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "CORES"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "APPLICATIONS"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "CASES"));
            Logger.AddRange(SQL_CREATE_DATA_SCAFFOLD(DB_PLATFORM, constr1, "FORMS"));

            ////Application_ID, NAME

            //Logger.Add(SQL_Add_Table("Events"));  //Has Applications_ID
            ////Events_ID, Date_Created, Application_ID, Event_Type_ID

            //Logger.Add(SQL_Add_Table("Event_Types"));   
            ////Event_Type_ID, Value

            //Logger.Add(SQL_Add_Table("Errors")); //Has Application_ID
            ////Error_ID, Error_Number, Error_Description, User, Program, Procedure_Name, 

            ////2nd Stage of Core Tables
            ////

            //Logger.Add(SQL_Add_Table("Transactions"));
            ////Transaction_ID, Application_ID, Organization_ID, User_ID 

            //Logger.Add(SQL_Add_Table("Sessions"));
            ////SESSION_ID, USER_ID, Application_ID

            //Logger.Add(SQL_Add_Table("Tokens"));
            ////TOKEN_ID, SESSION_ID

            //Logger.Add(SQL_Add_Table("HTML_ELEMENTS"));
            ////HTML_ELEMENT_ID, Name, Start Tag, End Tag, Empty, Depr., DTD, Description
            ////http://www.w3.org/TR/html4/index/elements.html


            //Logger.Add(SQL_Add_Table("Case_Types"));
            ////Case_Type_ID, NAME

            //Logger.Add(SQL_Add_Table("WorkFlows"));
            ////Workflow_ID

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
       
        public string SQL_Add_Column(string TableName, string ColumnName, string ObjectType, string DefaultValue, bool isNull)
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
            sqlStatement.Append("add " + MaxNameLength(ColumnName, 30) + " " + ObjectType + " " + _DefaultValue + " " + _isNull);

            return EP_RUN_NONE_QUERY("ORACLE", constr1, sqlStatement.ToString(), "Column " + MaxNameLength(_ColumnName, 30) + " has been added to table " + _TableName);

        }

        public ArrayList SQL_Add_Columns(string DB_PLATFORM, string connAuth, string TableName, List<ColumnStructure> ColumnList)
        {
            ArrayList HoldResult = new ArrayList();

            foreach (ColumnStructure i in ColumnList)
            {
                HoldResult.Add(SQL_Add_Column(TableName, i._Name, i._DataType, i._DefaultValue, i._IsNull));
            }

            return HoldResult;
        }

        public string SQL_Add_Key_Foreign(string DB_PLATFORM, string connAuth, string _Name, string ForTable, string ParentTable, ArrayList Columns1, ArrayList ParentColumns2)
        {
            //CREATES FOREIGN KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_FK";

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("FOREIGN KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ");
            sqlBuffer.Append("REFERENCES " + ParentTable + " (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

            //return sqlBuffer.ToString(); 
            return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");
        }

        public string SQL_Add_Key_Foreign(string DB_PLATFORM, string connAuth, string _Name, string ForTable, string ParentTable, List<ColumnStructure> Columns1, List<ColumnStructure> ParentColumns2)
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

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("FOREIGN KEY (" + tempstringColumns + ") ");
            sqlBuffer.Append("REFERENCES " + ParentTable + " (" + tempstringColumns2 + ") ENABLE");

            //return sqlBuffer.ToString(); 
            return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Foreign Key Constraint " + tempstringNAME + " created");

        }

        public string SQL_Add_Key_Primary(string DB_PLATFORM, string connAuth, string _Name, string ForTable, ArrayList Columns1)
        {
            //CREATES Primary KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_PK";

            //ALTER TABLE table_name
            //add CONSTRAINT constraint_name PRIMARY KEY (column1, column2, ... column_n);

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("Primary KEY (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

            //return sqlBuffer.ToString();
            return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");

        }

        public string SQL_Add_Key_Primary(string DB_PLATFORM, string connAuth, string _Name, string ForTable, List<ColumnStructure> Columns1)
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

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("Primary KEY (" + tempstringColumns + ") ENABLE");

            //return sqlBuffer.ToString();
            return EP_RUN_NONE_QUERY(DB_PLATFORM, connAuth, sqlBuffer.ToString(), "Primary Key Constraint " + tempstringNAME + " created");

        }

        public string SQL_Add_Key_Unique(string DB_PLATFORM, string connAuth, string _Name, string ForTable, ArrayList Columns1)
        {
            //CREATE UNIQUE KEY
            StringBuilder sqlBuffer = new StringBuilder();
            string tempstringNAME = MaxNameLength(_Name, 27) + "_UK";

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("UNIQUE (" + string.Join(",", (string[])Columns1.ToArray(Type.GetType("System.String"))) + ") ENABLE");

            return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
        }

        public string SQL_Add_Key_Unique(string DB_PLATFORM, string connAuth, string _Name, string ForTable, List<ColumnStructure> Columns1)
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

            sqlBuffer.Append("ALTER TABLE " + ForTable + " ");
            sqlBuffer.Append("add CONSTRAINT " + tempstringNAME + " ");
            sqlBuffer.Append("UNIQUE (" + tempstringColumns + ") ENABLE");

            return EP_RUN_NONE_QUERY("ORACLE", connAuth, sqlBuffer.ToString(), "Unique Constraint " + tempstringNAME + " created");
        }

        public string SQL_Add_Materialized_View_log(string DB_PLATFORM, string connAuth, string ForTable)
        {
            return EP_RUN_NONE_QUERY(DB_PLATFORM, connAuth, "CREATE MATERIALIZED VIEW LOG ON " + ForTable + " WITH ROWID", "MATERIALIZED VIEW LOG " + ForTable + " created.)");
        }

        public string SQL_Add_PROC_INSERT(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
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

            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, true);

            foreach (DataRow row in dt.Rows)
            {
                outID = row["COLUMN_NAME"].ToString();
                outDataType = row["DATA_TYPE"].ToString();
            }

            dt.Clear();

            //STEP 1 - Check if Table Exist
            //Step 2 - Get list of all Columns in Table
            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, false);

            //Step 3 - Create Procedure Begin Definition Syntax
            SQLin.Append("create or replace ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix + " ( ");

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
        }

        public string SQL_Add_PROC_UPDATE(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
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


            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, true);

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
            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, false);
            rowCount = dt.Rows.Count;

            //Step 3 - Create Procedure Begin Definition Syntax
            SQLin.Append("create or replace ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix + " ( ");
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
        }

        public string SQL_Add_PROC_DELETE(string DB_Platform, string ConnAuth, string TableName, string ProcedurePrefix)
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


            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, true);

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
            dt = SQL_SELECT_COLUMNS_FROM_USER_TABLES(DB_Platform, ConnAuth, TableName, false);

            //Step 3 - Create Procedure Begin Definition Syntax
            SQLin.Append("create or replace ");
            SQLin.AppendLine("PROCEDURE " + MaxNameLength(TableName, 23) + "_" + _ProcedurePrefix + " ( ");
            SQLin.AppendLine(" P_" + inID + " IN " + inDataType + " ");

            SQLin.AppendLine(") AS ");
            SQLin.AppendLine("BEGIN ");
            SQLin.AppendLine("   DELETE FROM " + TableName + " ");
            SQLin.AppendLine("    WHERE " + inID + " = P_" + inID + ";");

            //Step 6 - Create Procedure End Definition Syntax
            SQLin.AppendLine("END; ");

            //Step 7 - Submit Procedure Syntax to Database/

            // return SQLin.ToString();
            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, SQLin.ToString(), "Procedure " + MaxNameLength(TableName, 19) + "_" + _ProcedurePrefix + " created");
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

        public string EP_ADD_MICROSOFT_ROLE(string ConnAuth, string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_RL";

            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "Create Role " + tempstringNAME, "Role &quot;" + Name + "&quot; Created");

           /* try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();

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

        public string SQL_ADD_ROW_Organizations(string DB_PLATFORM, string connAuth)
        {
            List<DBParameters> ParamListOrganizations = new List<DBParameters>();

            ParamListOrganizations.Add(new DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "Organization"
            });

            ParamListOrganizations.Add(new DBParameters
            {
                ParamName = "R_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });

            return SQL_PROCEDURE_GET_VALUE("R_ORGANIZATIONS_ID", SQL_PROCEDURE_PARAMS("ORACLE", connAuth, "ORGANIZATIONS_ADD_RW", ParamListOrganizations));

        }

        public string SQL_ADD_ROW_Users(string DB_PLATFORM, string connAuth)
        {
            List<DBParameters> ParamListUsers = new List<DBParameters>();

            ParamListUsers.Add(new DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "User"
            });

            ParamListUsers.Add(new DBParameters
            {
                ParamName = "R_USERS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });

            return SQL_PROCEDURE_GET_VALUE("R_USERS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "USERS_ADD_RW", ParamListUsers));
        }

        public string SQL_ADD_ROW_Cores(string DB_PLATFORM, string connAuth, string _Users_ID, string _Organizations_ID)
        {
            List<DBParameters> ParamListCores = new List<DBParameters>();

            ParamListCores.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "Core"
            });

            ParamListCores.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Users_ID
            });

            ParamListCores.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Organizations_ID
            });

            ParamListCores.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_CORES_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });
            return SQL_PROCEDURE_GET_VALUE("R_CORES_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "CORES_ADD_RW", ParamListCores));
        }

        public string SQL_ADD_ROW_Applications(string DB_PLATFORM, string connAuth, string _Core_ID, string _Users_ID, string _Organizations_ID)
        {
            List<DBParameters> ParamListApplications = new List<DBParameters>();

            ParamListApplications.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "Application"
            });

            ParamListApplications.Add(new DBParameters
            {
                ParamName = "P_CORES_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Core_ID,

            });

            ParamListApplications.Add(new DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Users_ID,

            });

            ParamListApplications.Add(new DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Organizations_ID,

            });

            ParamListApplications.Add(new DBParameters
            {
                ParamName = "R_APPLICATIONS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });

            return SQL_PROCEDURE_GET_VALUE("R_APPLICATIONS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "APPLICATIONS_ADD_RW", ParamListApplications));
        }

        public string SQL_ADD_ROW_Cases(string DB_PLATFORM, string connAuth, string _APP_ID, string _Users_ID)
        {
            List<DBParameters> ParamListCases = new List<DBParameters>();

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "Case"
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_APPLICATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _APP_ID
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamSize = 100,
                ParamValue = _Users_ID
            });



            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_CASES_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_CASES_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "CASES_ADD_RW", ParamListCases));
        }

        public string SQL_ADD_ROW_Forms(string DB_PLATFORM, string connAuth, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> ParamListCases = new List<DBParameters>();
            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = "Form"
            });


            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATION_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Organizations_ID
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Users_ID
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_FORMS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_FORMS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "FORMS_ADD_RW", ParamListCases));
        }

        public string SQL_ADD_ROW_Forms_Property(string DB_PLATFORM, string connAuth, string _FormID, string _UserID, string _Name, string _FieldType, string _Value)
        {
            List<DBParameters> ParamListCases = new List<DBParameters>();

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_FORM_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _FormID
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _UserID
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _Name
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_FIELD_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _FieldType
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_VALUE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _Value
            });

            ParamListCases.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_FORMS_OPTIONS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_FORMS_OPTIONS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "FORMS_OPTIONS_ADD_RW", ParamListCases));
        }

        public string SQL_ADD_ROW_Forms_Property_Option()
        {
            return "";
        }

        public string SQL_ADD_ROW_Object_Layer(string DB_PLATFORM, string connAuth, string _ObjectType)
        {
            List<DBParameters> ParamListObjectType = new List<DBParameters>();

            ParamListObjectType.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_LAYER",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _ObjectType
            });


            ParamListObjectType.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJECT_LAYERS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_OBJECT_LAYERS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJECT_LAYERS_ADD_RW", ParamListObjectType));
        }

        public string SQL_ADD_ROW_Objects(string DB_PLATFORM, string connAuth, string _Name, string _ObjectLayer)
        {
            List<DBParameters> ParamListObjects = new List<DBParameters>();

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _Name
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_LAYER",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _ObjectLayer
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJECTS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_OBJECTS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJECTS_ADD_RW", ParamListObjects));
        }

        public string SQL_ADD_ROW_Object_Sets(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _GripName, string _ObjectType, string _OrganizationID, string _UserID)
        {
            List<DBParameters> ParamListObjects = new List<DBParameters>();

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageType
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageName
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_GRIP_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _GripName
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _ObjectType
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _OrganizationID
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _UserID
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJECT_SETS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });

            return SQL_PROCEDURE_GET_VALUE("R_OBJECT_SETS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJECT_SETS_ADD_RW", ParamListObjects));
        }

        public string SQL_ADD_ROW_OBJ_OPT_SETS(string DB_PLATFORM, string connAuth, string _ObjectSetID, string _OptionValue)
        {
            List<DBParameters> ParamListObjects = new List<DBParameters>();

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_SETS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _ObjectSetID
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_Option_Value",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _OptionValue
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJ_OPT_SETS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_OBJ_OPT_SETS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJ_OPT_SETS_ADD_RW", ParamListObjects));

        }

        public string SQL_ADD_ROW_Object_Property_Sets(string DB_PLATFORM, string connAuth, string _ObjectsSetsID, string _ObjectType, string _ValueDatatype, string _PropertyName, string _HasParent, string _HasChild, string _ParentObjectPropID, string _PropertyValue)
        {
            string PropertyValue = " ";

            string ParentObjectPropID = "0";


            if (_PropertyValue != "")
                PropertyValue = _PropertyValue;

            if (_ParentObjectPropID != "")
                ParentObjectPropID = _ParentObjectPropID;

            List<DBParameters> ParamList = new List<DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECTS_SETS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _ObjectsSetsID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJECT_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _ObjectType
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_VALUE_DATATYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _ValueDatatype
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_PROPERTY_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _PropertyName
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_HAS_PARENT",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 5,
                ParamValue = _HasParent
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_HAS_CHILD",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 5,
                ParamValue = _HasChild
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_PARENT_OBJ_PROP_SETS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = ParentObjectPropID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_PROPERTY_VALUE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 250,
                ParamValue = PropertyValue
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJ_PROP_SETS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });

            return SQL_PROCEDURE_GET_VALUE("R_OBJ_PROP_SETS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJ_PROP_SETS_ADD_RW", ParamList));
        }

        public string SQL_ADD_ROW_Object_Property_Option_Sets(string DB_PLATFORM, string connAuth, string _ObjectsPropertySetsID, string _OptionValue)
        {
            List<DBParameters> ParamListObjects = new List<DBParameters>();

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJ_PROP_SETS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _ObjectsPropertySetsID
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OPTION_VALUE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _OptionValue
            });

            ParamListObjects.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_OBJ_PROP_OPT_SETS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_OBJ_PROP_OPT_SETS_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "OBJECT_PROP_OPT_SET_ADD_RW", ParamListObjects));

        }

        public string SQL_ADD_ROW_Grip(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _GripName, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> ParamList = new List<DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageType
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageName
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_GRIP_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _GripName
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Organizations_ID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Users_ID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_GRIPS_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_GRIPS_ID", SQL_PROCEDURE_PARAMS(DB_PLATFORM, connAuth, "GRIPS_ADD_RW", ParamList));
        }

        public string SQL_ADD_ROW_Stage(string DB_PLATFORM, string connAuth, string _StageType, string _StageName, string _Organizations_ID, string _Users_ID)
        {
            List<DBParameters> ParamList = new List<DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_TYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageType
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_STAGE_NAME",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = _StageName
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_ORGANIZATIONS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Organizations_ID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Users_ID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_STAGES_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_STAGES_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "STAGES_ADD_RW", ParamList));
        }

        public string SQL_ADD_ROW_Value_Datatype(string DB_PLATFORM, string connAuth, string ValueDataType)
        {
            List<DBParameters> ParamList = new List<DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_VALUE_DATATYPE",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Varchar2,
                ParamSize = 50,
                ParamValue = ValueDataType
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_VALUE_DATATYPES_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_VALUE_DATATYPES_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, "VALUE_DATATYPES_ADD_RW", ParamList));
        }

        public string SQL_ADD_ROW_OBJECT_DATA_ENTRY(string DB_PLATFORM, string connAuth, string _ObjectsPropertySetsID, string _Destination, string _Destination_ID, string _ValueDataType, string _Value)
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

            List<DBParameters> ParamList = new List<DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_" + _Destination + "_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _Destination_ID
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_OBJ_PROP_SETS_ID",
                ParamDirection = ParameterDirection.Input,
                ParamDBType = OracleDbType.Int32,
                ParamValue = _ObjectsPropertySetsID
            });

            switch (_ValueDataType)
            {
                case "Number":
                case "number":
                case "numb":
                    ParamList.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        ParamDBType = OracleDbType.Int32,
                        ParamValue = _Value
                    });
                    break;
                case "Var":
                case "Character":
                case "Characters":
                case "CHAR":
                case "CHARS":
                case "Varchar2":
                    ParamList.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        ParamDBType = OracleDbType.Varchar2,
                        ParamSize = 4000,
                        ParamValue = _Value
                    });
                    break;
                case "Date":
                case "date":
                    ParamList.Add(new DB_Toolbox.DBParameters
                    {
                        ParamName = "P_VALUE",
                        ParamDirection = ParameterDirection.Input,
                        ParamDBType = OracleDbType.Date,
                        ParamValue = _Value
                    });
                    break;
            }

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_" + sourceDestination + "_ID",
                ParamDirection = ParameterDirection.Output,
                ParamDBType = OracleDbType.Int32
            });


            return SQL_PROCEDURE_GET_VALUE("R_" + sourceDestination + "_ID", SQL_PROCEDURE_PARAMS("ORACLE", constr1, sourceDestination + "_ADD_RW", ParamList));

        }

        public string SQL_Add_Sequence(string DB_Platform, string ConnAuth, string Name)
        {

            string tempstringNAME = MaxNameLength(Name, 27) + "_SQ";

            return EP_RUN_NONE_QUERY("ORACLE", constr1, "Create Sequence " + tempstringNAME + " INCREMENT BY 1 START WITH 1000", "Sequence &quot;" + MaxNameLength(Name, 30) + "&quot; Created");
        }

        public string SQL_Add_Table(string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27);

            return EP_RUN_NONE_QUERY("ORACLE", constr1, "CREATE TABLE " + tempstringNAME + "(" + tempstringNAME + "_ID number not null, enabled char(1) default 'N', TM_CREATED timestamp default sysdate not null,  DT_CREATED date default sysdate not null, DT_UPDATED date default sysdate not null, DT_AVAILABLE date, DT_END date )", "Table &quot;" + Name + "&quot; created.");
        }

        public string SQL_Add_Tablespace(string Name)
        {
            string tempstringNAME = MaxNameLength(Name, 27) + "_TS";

            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("CREATE SMALLFILE TABLESPACE " + tempstringNAME + " ");
            sqlStatement.Append("DATAFILE '" + ConfigurationManager.AppSettings["OracleOraDataFile"].ToString() + "\\EMIN_CMS_DF_1.dbf' ");
            sqlStatement.Append("SIZE 50M REUSE LOGGING ");
            sqlStatement.Append("EXTENT MANAGEMENT LOCAL SEGMENT SPACE MANAGEMENT AUTO ");
            //sqlStatement.Append("DEFAULT STORAGE(ENCRYPT) ENCRYPTION USING 'AES192'");

            return EP_RUN_NONE_QUERY("ORACLE", constr, sqlStatement.ToString(), "Tablespace " + tempstringNAME + " created.");

        }

        public string SQL_Add_Trigger_For_Each_Row(string DB_Platform, string ConnAuth, string TableName, string BeforeOrAfter, string Action, string SequenceName, string OneColumnName)
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

        public string EP_ADD_MSSQL_LOGIN(string ConnAuth, string LoginName, string Password, string DefaultDatabase)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();

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

        public string EP_ADD_MSSQL_USER(string ConnAuth, string UserName, string LoginName, string DatabaseName, string DefaultSchema)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();

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
            return EP_RUN_NONE_QUERY("MICROSOFT", ConnAuth, "GRANT " + Privilege + " ON " + SourceDB + " TO " + Grantee, "Grant Successfully issued");
        }

        public string EP_DROP_MSSQL_USER(string ConnAuth, string Name)
        {
            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();

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

        //Create Database in SqlServer
        public string EP_ADD_MSSQL_DB(string ConnAuth, string Name)
        {

                try
                {
                    String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLServerDBConnection"].ToString();
                    
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
                catch(Exception e)
                {
                    return e.ToString();
                }
            

            
        }

        //Drop Database in SqlServer
        public string EP_DROP_MSSQL_DB(string ConnAuth, string Name)
        {

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

        public ArrayList SQL_CREATE_SCAFFOLD(string DB_PLATFORM, string connAuth, string Name, List<ColumnStructure> ColumnsList)
        {
            ArrayList HoldResult = new ArrayList();

            HoldResult.Add("--Start Scaffold " + Name + "");
            ArrayList ColumnsList2 = new ArrayList();
            ColumnsList2.Add(Name + "_ID");
            HoldResult.Add(SQL_Add_Table(Name));
            HoldResult.AddRange(SQL_Add_Columns(DB_PLATFORM, connAuth, Name, ColumnsList));
            HoldResult.Add(SQL_Add_Key_Unique(DB_PLATFORM, connAuth, Name + "_ID", Name, ColumnsList2));
            HoldResult.Add(SQL_Add_Sequence(DB_PLATFORM, connAuth, Name));
            HoldResult.Add(SQL_Add_Trigger_For_Each_Row(DB_PLATFORM, connAuth, Name, "Before", "INSERT", (Name + "_SQ"), (Name + "_ID")));
            HoldResult.Add(SQL_Add_PROC_INSERT(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(SQL_Add_PROC_UPDATE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(SQL_Add_PROC_DELETE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add("--EndScaffold " + Name + "");

            return HoldResult;
        }

        public ArrayList SQL_CREATE_DATA_SCAFFOLD(string DB_PLATFORM, string connAuth, string TableName)
        {
            ArrayList HoldResult = new ArrayList();

            string newTableName = MaxNameLength(TableName, 14);
            string seq_Name = MaxNameLength(TableName, 14) + "_data";
            string Char_Table = newTableName + "_Dat_Char";
            string Date_Table = newTableName + "_Dat_Date";
            string Number_Table = newTableName + "_Dat_Numb";

            HoldResult.Add("---Start Data Scaffold " + TableName + "");
            List<ColumnStructure> ColumnsList = new List<ColumnStructure>();
            //ColumnsList2.Add(TableName + "_ID");
            HoldResult.Add(SQL_Add_Table(Char_Table));
            HoldResult.Add(SQL_Add_Table(Number_Table));
            HoldResult.Add(SQL_Add_Table(Date_Table));

            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "varchar2(4000)", _IsNull = false, _DefaultValue = "" });

            HoldResult.AddRange(SQL_Add_Columns(DB_PLATFORM, connAuth, Char_Table, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.AddRange(SQL_Add_Columns(DB_PLATFORM, connAuth, Number_Table, ColumnsList));

            ColumnsList.Clear();
            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });
            ColumnsList.Add(new ColumnStructure { _Name = "Value", _DataType = "date", _IsNull = false, _DefaultValue = "" });

            HoldResult.AddRange(SQL_Add_Columns(DB_PLATFORM, connAuth, Date_Table, ColumnsList));


            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Char_Table + "_1", Char_Table, TableName, ColumnsList, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Char_Table + "_2", Char_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Number_Table + "_1", Number_Table, TableName, ColumnsList, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Number_Table + "_2", Number_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = TableName + "_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Date_Table + "_1", Date_Table, TableName, ColumnsList, ColumnsList));

            ColumnsList.Clear();

            ColumnsList.Add(new ColumnStructure { _Name = "OBJ_PROP_SETS_ID", _DataType = "number", _IsNull = false, _DefaultValue = "" });

            HoldResult.Add(SQL_Add_Key_Foreign(DB_PLATFORM, connAuth, Date_Table + "_2", Date_Table, "OBJ_PROP_SETS", ColumnsList, ColumnsList));

            HoldResult.Add(SQL_Add_Sequence(DB_PLATFORM, connAuth, seq_Name));
            //HoldResult.Add(SQL_Add_Sequence(DB_PLATFORM, connAuth, Number_Table));
            //HoldResult.Add(SQL_Add_Sequence(DB_PLATFORM, connAuth, Date_Table));

            HoldResult.Add(SQL_Add_Trigger_For_Each_Row(DB_PLATFORM, connAuth, Char_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Char_Table + "_ID")));
            HoldResult.Add(SQL_Add_Trigger_For_Each_Row(DB_PLATFORM, connAuth, Number_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Number_Table + "_ID")));
            HoldResult.Add(SQL_Add_Trigger_For_Each_Row(DB_PLATFORM, connAuth, Date_Table, "Before", "INSERT", (seq_Name + "_SQ"), (Date_Table + "_ID")));

            HoldResult.Add(SQL_Add_PROC_INSERT(DB_PLATFORM, connAuth, Char_Table, ""));
            HoldResult.Add(SQL_Add_PROC_INSERT(DB_PLATFORM, connAuth, Number_Table, ""));
            HoldResult.Add(SQL_Add_PROC_INSERT(DB_PLATFORM, connAuth, Date_Table, ""));

            //HoldResult.Add(SQL_Add_PROC_UPDATE(DB_PLATFORM, connAuth, TableName, ""));
            //HoldResult.Add(SQL_Add_PROC_DELETE(DB_PLATFORM, connAuth, TableName, ""));
            HoldResult.Add("---End Data Scaffold " + TableName + "");

            return HoldResult;
        }

        public ArrayList SQL_CREATE_EXTENDED_SCAFFOLD(string DB_PLATFORM, string connAuth, string Name)
        {

            ArrayList HoldResult = new ArrayList();

            HoldResult.Add("Scaffold " + Name + "");
            ArrayList ColumnsList2 = new ArrayList();
            ColumnsList2.Add(Name + "_ID");
            //Add(SQL_Add_Table(Name));
            //HoldResult.AddRange(SQL_Add_Columns(DB_PLATFORM, connAuth, Name, ColumnsList));
            //HoldResult.Add(SQL_Add_Key_Unique(DB_PLATFORM, connAuth, Name, ColumnsList2));
            HoldResult.Add(SQL_Add_Sequence(DB_PLATFORM, connAuth, Name));
            HoldResult.Add(SQL_Add_Trigger_For_Each_Row(DB_PLATFORM, connAuth, Name, "Before", "INSERT", (Name + "_SQ"), (Name + "_ID")));
            HoldResult.Add(SQL_Add_PROC_INSERT(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(SQL_Add_PROC_UPDATE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add(SQL_Add_PROC_DELETE(DB_PLATFORM, connAuth, Name, ""));
            HoldResult.Add("Scaffold " + Name + "");

            return HoldResult;
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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "All_Objects_Set_Props", sqlStruct));

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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "All_Objects", sqlStruct));

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

            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "Organizations", sqlStruct));

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

            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "OBJECTS", sqlStruct));

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

            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "USERS", sqlStruct));

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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "Cases", sqlStruct));

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

            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "APPLICATIONS", sqlStruct));

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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "FORMS", sqlStruct));

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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "FORM_TEMPLATES", sqlStruct));

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


            Logger.Add(SQL_Add_View(DB_PLATFORM, connAuth, "FORM_TEMPLATE_DETAILS", sqlStruct));

            return Logger;
        }

        public ArrayList SQL_DATA_DEFAULT(string DB_PLATFORM, string connAuth, string SystemName)
        {
            ArrayList Logger = new ArrayList();

            string Organizations_ID;

            string Users_ID; 

            string Cores_ID;

            string APP_ID;

            string Cases_ID;

            string Forms_ID, Stages_ID, GripsID, ObjectSetsID, ObjectPropertySetID, ObjectPropertyOptionSetID;

            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Front-End"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Design"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Framework"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Architecture"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Application"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Presentation"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Session"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Transport"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Network"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Data Link"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Physical"));         
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "Universal"));
            Logger.Add(SQL_ADD_ROW_Object_Layer(DB_PLATFORM, connAuth, "System"));

            Logger.Add(SQL_ADD_ROW_Value_Datatype(DB_PLATFORM, connAuth, "Characters"));
            Logger.Add(SQL_ADD_ROW_Value_Datatype(DB_PLATFORM, connAuth, "Number"));
            Logger.Add(SQL_ADD_ROW_Value_Datatype(DB_PLATFORM, connAuth, "Date"));

            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Core", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Application", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Stage", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Grip", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Object Set", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Case", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Form", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Notification", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Log", "System"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Flow", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Organization", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "User", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Role", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Privelege", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Notes", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Pages", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Comment", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Form Field", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Demographic", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Conditional_Logic", "Application"));

            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "ID", "Framework"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Value", "Framework"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Text_Box", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Paragraph_Text", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Rich_Text", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Check_Box", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Radio_Button", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Drop_Down", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Section", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Page_Break", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Number", "Application"));

            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Email", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Url", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Date", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Time", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Phone", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "Credit_Card", "Application"));
            Logger.Add(SQL_ADD_ROW_Objects(DB_PLATFORM, connAuth, "File_Upload", "Application"));

            Organizations_ID = SQL_ADD_ROW_Organizations(DB_PLATFORM, connAuth);
            Users_ID = SQL_ADD_ROW_Users(DB_PLATFORM, connAuth);
            Cores_ID = SQL_ADD_ROW_Cores(DB_PLATFORM, connAuth, Users_ID, Organizations_ID);
            APP_ID = SQL_ADD_ROW_Applications(DB_PLATFORM, connAuth, Cores_ID, Users_ID, Organizations_ID);
            Cases_ID = SQL_ADD_ROW_Cases(DB_PLATFORM, connAuth, APP_ID, Users_ID);
            Forms_ID = SQL_ADD_ROW_Forms(DB_PLATFORM, connAuth, Organizations_ID, Users_ID);

            Stages_ID = SQL_ADD_ROW_Stage(DB_PLATFORM, connAuth, "Core", "Base Core", Organizations_ID, Users_ID);
            GripsID = SQL_ADD_ROW_Grip(DB_PLATFORM, connAuth, "Core", "Base Core", "Core Settings", Organizations_ID, Users_ID);
            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Core", "Base Core", "Core Settings", "Core", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Enabled", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Version", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");

            Stages_ID = SQL_ADD_ROW_Stage(DB_PLATFORM, connAuth, "Application", "Base Application", Organizations_ID, Users_ID);
            GripsID = SQL_ADD_ROW_Grip(DB_PLATFORM, connAuth, "Application", "Base Application", "Application Settings", Organizations_ID, Users_ID);
            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Application", "Base Application", "Application Settings", "Application", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Enabled", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Version", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");

            Stages_ID = SQL_ADD_ROW_Stage(DB_PLATFORM, connAuth, "User", "Base User", Organizations_ID, Users_ID);
            GripsID = SQL_ADD_ROW_Grip(DB_PLATFORM, connAuth, "User", "Base User", "User Settings", Organizations_ID, Users_ID);
            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "User", "Base User", "User Settings", "Demographic", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "First Name", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Last Name", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Date", "Date", "DOB", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Email", "Characters", "Email", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "SSN", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Sex", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Male");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Female");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Unknown");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Clearance Level", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Top Secret /SCI");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Top Secret");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Secret");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Classified");

            Stages_ID = SQL_ADD_ROW_Stage(DB_PLATFORM, connAuth, "Form", "Default Form", Organizations_ID, Users_ID);
            GripsID = SQL_ADD_ROW_Grip(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", Organizations_ID, Users_ID);
            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");

            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Settings", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Drop_Down", "Characters", "Form Style", "false", "false", "", "Test");
            SQL_ADD_ROW_OBJ_OPT_SETS(DB_PLATFORM, connAuth, ObjectSetsID, "Style 1");
            SQL_ADD_ROW_OBJ_OPT_SETS(DB_PLATFORM, connAuth, ObjectSetsID, "Style 2");

            Stages_ID = SQL_ADD_ROW_Stage(DB_PLATFORM, connAuth, "Form", "Default Form", Organizations_ID, Users_ID);
            GripsID = SQL_ADD_ROW_Grip(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", Organizations_ID, Users_ID);
            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "False");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "2");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "5");

            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "2");

            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Text_Box", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "2");

            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Drop_Down", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "20");

            SQL_ADD_ROW_OBJ_OPT_SETS(DB_PLATFORM, connAuth, ObjectSetsID, "One");
            SQL_ADD_ROW_OBJ_OPT_SETS(DB_PLATFORM, connAuth, ObjectSetsID, "Two");

            ObjectSetsID = SQL_ADD_ROW_Object_Sets(DB_PLATFORM, connAuth, "Form", "Default Form", "Form Fields", "Paragraph_Text", Organizations_ID, Users_ID);
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "Label", "false", "false", "", "Test");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Radio_Button", "Characters", "Required", "false", "false", "", "Test");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "Yes");
            ObjectPropertyOptionSetID = SQL_ADD_ROW_Object_Property_Option_Sets(DB_PLATFORM, connAuth, ObjectPropertySetID, "No");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Min", "false", "false", "", "1");
            ObjectPropertySetID = SQL_ADD_ROW_Object_Property_Sets(DB_PLATFORM, connAuth, ObjectSetsID, "Number", "Number", "Max", "false", "false", "", "10");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, ObjectSetsID, "Forms", Forms_ID, "Characters", "alsjdhflakjsdhf"); 

           //SQL_ADD_ROW_Object_Data(DB_PLATFORM, connAuth, "FORMS", "Characters", "FORMS_ID", ObjectSetsID, "Test");

            
            return Logger;
        }

        public ArrayList SQL_DATA_EXAMPLE(string DB_PLATFORM, string connAuth, string SystemName)
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
            
            

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1017", "Forms", Forms_ID, "Characters", "Example 101091");         

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1018", "Forms", Forms_ID, "Characters", "Example Organization");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1019", "Forms", Forms_ID, "Characters", "One");
           
            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1020", "Forms", Forms_ID, "Characters", "alsjdhflakjsdhf");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1004", "Applications", APP_ID, "Characters", "First Application");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1005", "Applications", APP_ID, "Characters", "First_Application");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1006", "Applications", APP_ID, "Characters", "True");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1007", "Applications", APP_ID, "Characters", "1.0");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1000", "Cores", Forms_ID, "Characters", "First Core");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1001", "Cores", Forms_ID, "Characters", "First_Core");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1002", "Cores", Forms_ID, "Characters", "True");

            SQL_ADD_ROW_OBJECT_DATA_ENTRY(DB_PLATFORM, connAuth, "1003", "Cores", Forms_ID, "Characters", "1.0");

            //SQL_ADD_ROW_Object_Data(DB_PLATFORM, connAuth, "FORMS", "Characters", "FORMS_ID", ObjectSetsID, "Test");


            return Logger;
        }

        //Oracle Way of Dropping Perception DB
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

            return  SQLBuffer.ToString();
            
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
                    
                    if(i._Parameters.Count < iNumber)
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

            rows.Add(SQL_Add_Tablespace(SystemName));

            rows.Add(SQL_Add_USER(SystemName));

            rows.Add(EP_ADD_ORACLE_ROLE(SystemName + "_ROLE"));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE TABLE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE PROCEDURE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("CREATE SEQUENCE", SystemName));

            rows.Add(EP_GRANT_ORACLE_PRIVILEGE("SELECT ANY DICTIONARY", SystemName));

            //1st Stage of Core TABLES
            //
            rows.Add(SQL_Add_Table("Applications"));
            //Application_ID, NAME

            rows.Add(SQL_Add_Table("Events"));  //Has Applications_ID
            //Events_ID, Date_Created, Application_ID, Event_Type_ID

            rows.Add(SQL_Add_Table("Logs"));
            //LOG_ID

            rows.Add(SQL_Add_Table("Event_Types"));
            //Event_Type_ID, Value

            rows.Add(SQL_Add_Table("Errors")); //Has Application_ID
            //Error_ID, Error_Number, Error_Description, User, Program, Procedure_Name, 

            //2nd Stage of Core Tables
            //
            rows.Add(SQL_Add_Table("Organizations"));
            //Organization_ID

            rows.Add(SQL_Add_Table("Users"));
            //User_ID

            rows.Add(SQL_Add_Table("Transactions"));
            //Transaction_ID, Application_ID, Organization_ID, User_ID 

            rows.Add(SQL_Add_Table("Sessions"));
            //SESSION_ID, USER_ID, Application_ID

            rows.Add(SQL_Add_Table("Tokens"));
            //TOKEN_ID, SESSION_ID

            rows.Add(SQL_Add_Table("HTML_ELEMENTS"));
            //HTML_ELEMENT_ID, Name, Start Tag, End Tag, Empty, Depr., DTD, Description
            //http;//www.w3.org/TR/html4/index/elements.html

            rows.Add(SQL_Add_Table("Cases"));
            //CASE_ID, CASE_TYPE, ORGANIZATION_ID, 

            rows.Add(SQL_Add_Table("Case_Types"));
            //Case_Type_ID, NAME

            rows.Add(SQL_Add_Table("Forms"));
            //Form_ID, FIELD_NAME, FIELD_TYPE, VALUE, TEXT

            rows.Add(SQL_Add_Table("Forms_HTML_Objects")); //Foreign Keys TO Forms(FormID) & HTML_Objects(HTML_Object_ID)


            rows.Add(SQL_Add_Table("WorkFlows"));
            //Workflow_ID

            rows.Add(SQL_Add_Table("Roles"));
            //Role_ID, Applications_ID, Privelege_ID

            rows.Add(SQL_Add_Table("Roles_Priveleges"));

            rows.Add(SQL_Add_Table("USERS_ROLES_PRIVELEGES"));

            rows.Add(SQL_Add_Table("Priveleges"));
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
        
        public string SQL_Add_View(string DB_Platform, string ConnAuth, string ViewName, List<sqlSelectStructure> QueryStructure)
        {
            
            string _ViewName = MaxNameLength( ViewName, 27) + "_VW";
            StringBuilder SQLBuffer = new StringBuilder();

            SQLBuffer.Append("CREATE OR REPLACE VIEW " + _ViewName + " AS ");
            SQLBuffer.Append(EP_GENERATE_QUERY(DB_Platform, QueryStructure));

            return EP_RUN_NONE_QUERY(DB_Platform, ConnAuth, SQLBuffer.ToString(), "View " + _ViewName + " created.").ToString();
            //return SQLBuffer.ToString();
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

        public string SQL_EXTEND_TABLE_VIA_COLUMN(string DB_Platform, string ConnAuth, string OneColumn, string ColumnType, string SourceTable)
        {
            ArrayList ResultHolder = new ArrayList();

            string tempstringNAME = MaxNameLength(SourceTable + "_" + OneColumn, 27);
            //SQLCreate Table

            //Create Extending Table 
            ResultHolder.Add(SQL_Add_Table(tempstringNAME));

            ArrayList ColumnList = new ArrayList();
            ColumnList.Add(tempstringNAME + "_ID");

            //Use Table ID as Unique Key
            ResultHolder.Add(SQL_Add_Key_Unique("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, ColumnList));

            //Add Distinguishing Column
            ResultHolder.Add(SQL_Add_Column(tempstringNAME, OneColumn, ColumnType, "", false));

            ColumnList.Clear();
            ColumnList.Add(SourceTable + "_ID");
            ResultHolder.Add(SQL_Add_Column(tempstringNAME, (SourceTable + "_ID"), "Number", "", false));
            ResultHolder.Add(SQL_Add_Column(SourceTable, OneColumn, ColumnType, "", false));


            ColumnList.Clear();
            ColumnList.Add(SourceTable + "_ID");
            ResultHolder.Add(SQL_Add_Key_Foreign("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, SourceTable, ColumnList, ColumnList));

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
            ResultHolder.Add(SQL_Add_Table(tempstringNAME));

            //This Class has to assign the Unique Key of the ID number.
            ArrayList ColumnList = new ArrayList();
            ColumnList.Add(tempstringNAME + "_ID");

            //Use Table ID as Unique Key
            ResultHolder.Add(SQL_Add_Key_Unique("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, ColumnList));

            //Add Distinguishing Column
            foreach (ColumnStructure i in NamingColumns)
            {
                //Add Parameters to DBProcedure
                ResultHolder.Add(SQL_Add_Column(tempstringNAME, i._Name, i._DataType.ToString(), i._DefaultValue, i._IsNull));
            }

            //Add Columns from Source 
            ResultHolder.Add(SQL_Add_Column(tempstringNAME, (SourceTable + "_ID"), "Number", "", false));

            List<ColumnStructure> ColumnStruct1 = new List<ColumnStructure>();

            ColumnStruct1.Add(new ColumnStructure { _Name = (SourceTable + "_ID") });

            //Foreign Key
            ResultHolder.Add(SQL_Add_Key_Foreign("ORACLE", constr1, tempstringNAME + "_ID", tempstringNAME, SourceTable, ColumnStruct1, ColumnStruct1));

            //Add New Columns for New Table
            foreach (ColumnStructure i in NewColumns)
            {
                //Add Parameters to DBProcedure
                ResultHolder.Add(SQL_Add_Column(tempstringNAME, i._Name, i._DataType.ToString(), i._DefaultValue, i._IsNull));
            }

            return "Table Extension Completed.";
        }

        public string EP_GRANT_ORACLE_PRIVILEGE(string ConnAuth, string PrivType, string Grantee)
        {
            return EP_RUN_NONE_QUERY("ORACLE", ConnAuth, "Grant " + PrivType + " to " + Grantee, PrivType + " granted to &quot;" + Grantee + "&quot;");
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
                    DBProcedure.Parameters.Add(i.ParamName, i.ParamDBType, i.ParamSize, i.ParamValue, i.ParamDirection);
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

            return table;
        }

        public DataTable SQL_PROCEDURE_PARAMS(string DB_Platform, string ConnAuth, string ProcedureName, List<DBParameters> _dbParameters)
        {

            DBParameters[] dbParameters = new DBParameters[_dbParameters.Count];

            List<DBParameters> infoList = new List<DBParameters>();

            int iNumber = 0;

            foreach (DBParameters i in _dbParameters)
            {
                dbParameters[iNumber] = new DB_Toolbox.DBParameters();
                dbParameters[iNumber].ParamName = i.ParamName;
                dbParameters[iNumber].ParamDBType = i.ParamDBType;
                dbParameters[iNumber].ParamDirection = i.ParamDirection;
                dbParameters[iNumber].ParamValue = i.ParamValue;
                dbParameters[iNumber].ParamSize = i.ParamSize;
                dbParameters[iNumber].ParamReturn = i.ParamReturn;

                infoList.Add(dbParameters[iNumber]);

                iNumber++; //++ Increments by 1.
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
        public DataTable SQL_SELECT_COLUMNS_FROM_USER_TABLES(string DB_Platform, string ConnAuth, string TableName, Boolean _isSeqID)
        {
            StringBuilder SQLin = new StringBuilder();

            // SQLin.Append("Select COLUMN_NAME \"COLUMNS\", DATA_TYPE");
            SQLin.Append("Select COLUMN_NAME, DATA_TYPE ");
            SQLin.Append("from USER_TAB_COLUMNS ");
            SQLin.Append("where TABLE_NAME = upper('" + TableName + "') ");
            if (_isSeqID == false)
            {
                SQLin.Append("and column_id > 7  ");
            }
            else
            {
                SQLin.Append("and column_id = 1  ");
            }
            SQLin.Append("order by COLUMN_ID ASC ");

            return EP_RUN_QUERY("Oracle", constr1, SQLin.ToString());
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
            var startTag = tag;
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            var startTag2 = tag2;
            int startIndex2 = s.IndexOf(startTag2) - startTag.Length;

            return s.Substring(startIndex, startIndex2);
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
    }
}