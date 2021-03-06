﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.classes;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
using System.Text;

namespace CMS
{
    public partial class Data : System.Web.UI.Page
    {
        string SystemName = "CMS"; //Referred to as name in subcalls.
        string constr = System.Configuration.ConfigurationManager.ConnectionStrings["EP_OracleRootConn"].ToString();
        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_OracleUserConn"].ToString();
        string constrSQLServer = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConn"].ToString();
        string constrSQLServer1 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLRootConntoDB"].ToString();
        string constrSQLServer2 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_MSSQLUserConn"].ToString();

        DB_Toolbox EMIN_DBK = new DB_Toolbox();

        protected void Page_Load(object sender, EventArgs e)
        {
            Environment_Tools environment_tools = new Environment_Tools();

            //QUERY-STEP 1-Check if the Database is Available
            string _CurrentDbStatus = environment_tools.CheckConnection();
    
            //HTML-Send Status to HTML
            currentDBStatus.Controls.Add(new LiteralControl("<h1>" + _CurrentDbStatus + "</h1>"));         
            
            //HTML-Line Break
            currentDBStatus.Controls.Add(new LiteralControl("<br />"));

            //Checks if CMS User Is Installed in Database.
            if (environment_tools.CheckIfCMSUserIsInstalled(SystemName) == "no")
            {
                //DBStatusHolder.Controls.Add(new LiteralControl("Please <a href=#tips>Press Here</a> to Install")); 
                createCmsDiv.Visible = true;
            }
            else
            {
                createCmsDiv.Visible = true;
            }
            
        }

        protected void submitOracleServer_Click(object sender, EventArgs e)
        {
            ArrayList Results = new ArrayList();

            Results = EMIN_DBK.EP_CREATE_DB("ORACLE", constr, constr1, constr1, "PerceptionDB", "ChangeYourPassWord");

            DGResults.DataSource = Results;
            DGResults.DataBind();

            //Add_Data();

            //SelectFromAny();
            //createView();
            //CallProcedure_Min();
            //CreateDB();
           // SQLQUERY();
            //SQLQUERY2();
            //createProcedure();
            //GET_COLUMNS_FOR_TABLE();

           /* 
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_INSERT_PROCEDURE("ORACLE", constr1, "APPLICATION","")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_INSERT_PROCEDURE("ORACLE", constr1, "LOG", "")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_INSERT_PROCEDURE("ORACLE", constr1, "ROLES", "")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_UPDATE_PROCEDURE("ORACLE", constr1, "APPLICATION", "")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_DELETE_PROCEDURE("ORACLE", constr1, "APPLICATION", "")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));

            ArrayList columnlist = new ArrayList();
            columnlist.Add("NAME");
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_UNIQUE_KEY("ORACLE", constr1, "APPLICATION", columnlist)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));

            columnlist.Clear();
            columnlist.Add("APPLICATION_ID");
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_FOREIGN_KEY("ORACLE", constr1, "ERRORS", "APPLICATION", columnlist, columnlist)));

            columnlist.Clear();
            columnlist.Add("Error_ID");
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_PRIMARY_KEY("ORACLE", constr1, "ERRORS", columnlist)));


            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_ADD_INSERT_PROCEDURE("ORACLE", constr1, "APPLICATION", "")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));

            */

            InstallerHolder.Visible = true;
            submitCmsName.Enabled = false;     
        }

        protected void submitSqlServer_Click(object sender, EventArgs e)
        {
            /*string temp, temp2;
            temp = EMIN_DBK.EP_ADD_MSSQL_DB(constrSQLServer, "Perception123");

            temp2 = temp;

            temp = EMIN_DBK.EP_DROP_MSSQL_DB(constrSQLServer, "Perception123");

            temp2 = temp;*/

            ArrayList Results = new ArrayList();

            Results = EMIN_DBK.EP_CREATE_DB("Microsoft", constrSQLServer, constrSQLServer1, constrSQLServer2, "PerceptionDB", "ChangeYourPassWord");

            DGResults.DataSource = Results;
            DGResults.DataBind();

            InstallerHolder.Visible = true;
            submitSqlServer.Enabled = false; 
        }

        protected void submitQuery_Click(object sender, EventArgs e) {
            DataGrid dg1 = new DataGrid();
            
            string tempSQLTEXT = SQLText.Text.ToString();

            if (DBTypeddl.SelectedValue.ToString() == "ORACLE")
                dg1.DataSource = EMIN_DBK.EP_RUN_QUERY(DBTypeddl.SelectedValue.ToString(), constr1, tempSQLTEXT);
            else
                dg1.DataSource = EMIN_DBK.EP_RUN_QUERY(DBTypeddl.SelectedValue.ToString(), constrSQLServer, tempSQLTEXT);

            dg1.DataBind();
            InstallerHolder.Controls.Add(dg1);
            InstallerHolder.Visible = true;
        }


        protected void submitNoneQuery_Click(object sender, EventArgs e) {
            DataGrid dg1 = new DataGrid();

            string tempSQLTEXT = SQLText.Text.ToString();

            if (DBTypeddl.SelectedValue.ToString() == "ORACLE")
                dg1.DataSource = EMIN_DBK.EP_RUN_NONE_QUERY(DBTypeddl.SelectedValue.ToString(), constr1, tempSQLTEXT, "Test Query Successful");
            else
                dg1.DataSource = EMIN_DBK.EP_RUN_NONE_QUERY(DBTypeddl.SelectedValue.ToString(), constrSQLServer, tempSQLTEXT, "Test Query Successful");
            
            dg1.DataBind();
            InstallerHolder.Controls.Add(dg1);
            InstallerHolder.Visible = true;
        }

        protected void CreateDB()
        {
            DB_Toolbox environment_Tools_db = new DB_Toolbox();

            /* InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLE("Application")));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
             InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLE("Log")));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
             InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLE("Roles")));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
             InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLE("Priveleges")));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
             InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.SQL_KILL_SESSIONS(SystemName)));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
                InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLE("Users")));
             InstallerHolder.Controls.Add(new LiteralControl("<hr />"));*/
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_USER(constr, SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_TABLESPACE(constr, SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_PROFILE(constr, SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_DROP_ORACLE_ROLE(constr, SystemName + "_ROLE")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_ORACLE_PROFILE(constr, SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_ORACLE_TABLESPACE(SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_ORACLE_USER(constrSQLServer, SystemName, "ChangeYourPassWord")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_ORACLE_ROLE(constr, SystemName + "_ROLE")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_GRANT_ORACLE_PRIVILEGE(constr, "CREATE TABLE", SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_GRANT_ORACLE_PRIVILEGE(constr, "CREATE PROCEDURE", SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_GRANT_ORACLE_PRIVILEGE(constr, "CREATE SEQUENCE", SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_GRANT_ORACLE_PRIVILEGE(constr, "SELECT ANY DICTIONARY", SystemName)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Application")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Errors")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Log")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Roles")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Priveleges")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_TABLE("ORACLE", constr, "Users")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Users", "Name", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Errors", "Application_ID", "Number", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Roles", "Name", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Priveleges", "Name", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Application", "Name", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Application", "Version", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_COLUMN("ORACLE", constr, "Application", "CodeName", "varchar2(50)", "", false)));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(environment_Tools_db.EP_ADD_ORACLE_SEQUENCE("ORACLE", constr1, "APPLICATION")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            
        }

        protected void CallProcedure() //Calls Procedure and loads data to dataset.
        {


            DB_Toolbox environment_Tools_db = new DB_Toolbox();
            DB_Toolbox.DBParameters[] dbParameters = new DB_Toolbox.DBParameters[3];
            List<DB_Toolbox.DBParameters> infoList = new List<DB_Toolbox.DBParameters>();

            dbParameters[0] = new DB_Toolbox.DBParameters();
            dbParameters[0].ParamName = "PARAM1";
            dbParameters[0].OracleParamDataType = OracleDbType.Varchar2;
            dbParameters[0].ParamDirection = ParameterDirection.Input;
            dbParameters[0].ParamValue = "123";
            dbParameters[0].ParamSize = 100;
            dbParameters[0].ParamReturn = false;

            infoList.Add(dbParameters[0]);

            dbParameters[1] = new DB_Toolbox.DBParameters();
            dbParameters[1].ParamName = "PARAM2";
            dbParameters[1].OracleParamDataType = OracleDbType.Varchar2;
            dbParameters[1].ParamDirection = ParameterDirection.Output;
            dbParameters[1].ParamSize = 100;
            dbParameters[1].ParamReturn = true;

            infoList.Add(dbParameters[1]);

            dbParameters[2] = new DB_Toolbox.DBParameters();
            dbParameters[2].ParamName = "PARAM3";
            dbParameters[2].OracleParamDataType = OracleDbType.Varchar2;
            dbParameters[2].ParamDirection = ParameterDirection.Output;
            dbParameters[2].ParamSize = 100;
            dbParameters[2].ParamReturn = true;

            infoList.Add(dbParameters[2]);

            DataTable dt = new DataTable();

            dt = environment_Tools_db.SQL_PROCEDURE("Oracle", constr1, "TEST123", infoList);

            dg.DataSource = dt;
            dg.DataBind();

            InstallerHolder.Controls.Add(dg);

            InstallerHolder.Visible = true;
            submitCmsName.Enabled = false;      
        }

        protected void CallProcedure_Min() //EXAMPLE ON HOW TO PASS PARAMETERS TO DB.
        {
            List<DB_Toolbox.DBParameters> paramList = new List<DB_Toolbox.DBParameters>();

            paramList.Add(new DB_Toolbox.DBParameters 
                { 
                    ParamName = "PARAM1", 
                    ParamDirection = ParameterDirection.Input,
                    ParamValue = "123",
                    OracleParamDataType = OracleDbType.Varchar2,
                    ParamSize = 100
                });

            paramList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "PARAM2",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100
            });

            paramList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "PARAM3",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100
            });


            DB_Toolbox environment_Tools_db = new DB_Toolbox();

            //SQL_PROCEDURE_PARAMS ( DB TYPE, CONNECTION STRING, PROCEDURE NAME, PARAMLIST
            dg.DataSource = environment_Tools_db.SQL_PROCEDURE_PARAMS("ORACLE", constr1, "TEST123", paramList);

            dg.DataBind();

            InstallerHolder.Controls.Add(dg);

        }

        protected void SQLQUERY()
        {
            dg.DataSource = EMIN_DBK.EP_RUN_QUERY("Oracle", constr1, "Select * from User_tables");
            dg.DataBind();

            InstallerHolder.Controls.Add(dg);
        }

        protected void SQLQUERY2()
        {
            DataGrid dg1 = new DataGrid();

            dg1.DataSource = EMIN_DBK.EP_RUN_QUERY("Oracle", constr1, "Select * from USER_TAB_COLUMNS");
            dg1.DataBind();
            InstallerHolder.Controls.Add(dg1);
        }

        protected void createProcedure()
        {

            StringBuilder SQLin = new StringBuilder();

            SQLin.Append("create or replace ");
            SQLin.AppendLine("PROCEDURE TEST1234 ");
            SQLin.AppendLine("( ");
            SQLin.AppendLine(" PARAM1 IN VARCHAR2, ");
            SQLin.AppendLine(" PARAM2 OUT VARCHAR2, ");
            SQLin.AppendLine("  PARAM3 OUT VARCHAR2 ");
            SQLin.AppendLine(") AS ");
            SQLin.AppendLine("BEGIN ");
            SQLin.AppendLine("  PARAM2 := 'This is a test'; ");
            SQLin.AppendLine("  PARAM3 := 'YO YOUYO'; ");
            SQLin.AppendLine("END TEST1234; ");

            InstallerHolder.Controls.Add(new LiteralControl(SQLin.ToString()));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            InstallerHolder.Controls.Add(new LiteralControl(EMIN_DBK.EP_RUN_NONE_QUERY("ORACLE", constr1, SQLin.ToString(), "Procedure Created")));
            InstallerHolder.Controls.Add(new LiteralControl("<hr />"));
            
        }

        protected void GET_COLUMNS_FOR_TABLE()
        {
            DataGrid dg2 = new DataGrid();

            dg2.DataSource = EMIN_DBK.EP_GET_COLUMNS_VIA_TABLENAME("Oracle", constr1, "Application", false);
            dg2.DataBind();
            InstallerHolder.Controls.Add(dg2);
        }

        protected void createView()
        {
            List<DB_Toolbox.sqlSelectStructure> ListTV = new List<DB_Toolbox.sqlSelectStructure>();

            List<DB_Toolbox.ColumnStructure> CS = new List<DB_Toolbox.ColumnStructure>();

            CS.Add(new DB_Toolbox.ColumnStructure { _Name = "NAME", _Alias="AppName" });

            //ListTV.Add(new DB_Toolbox.sqlSelectStructure { _TableName = "Applications", _TableAlias = "App", _isFROM = true, _HasJoin = false, _HasWhere = false, _HasAggregateFunc = false, _HasGroupBy = false, _HasHaving = false });
            //ListTV.Add(new DB_Toolbox.sqlSelectStructure { _JoinClause = "INNER JOIN", _TableName = "Applications_Name", _TableAlias = "AppName", _IncludeColumns = CS, _JoinOn = "App.Applications_ID = AppName.Applications_ID", _isFROM = false, _HasJoin = true, _HasWhere = false, _HasAggregateFunc = false, _HasGroupBy = false, _HasHaving = false });


            ListTV.Add(new DB_Toolbox.sqlSelectStructure { _TableName = "Applications", _TableAlias = "App", _HasFrom = true });
            ListTV.Add(new DB_Toolbox.sqlSelectStructure { _JoinClause = "INNER JOIN", _TableName = "Applications_Name", _TableAlias = "AppName", _IncludeColumns = CS, _JoinOn = "App.Applications_ID = AppName.Applications_ID", _HasJoin = true});


            string test = EMIN_DBK.EP_ADD_VIEW("ORACLE", constr1, "Applications", ListTV);
        }

        protected void SelectFromAny()
        {
            DataTable dt = new DataTable();

            dt = EMIN_DBK.EP_RUN_QUERY("ORACLE", constr1, "Select * from APPLICATIONS_VW");

            DGResults.DataSource = dt;
            DGResults.DataBind();
        }

        protected void Add_Data()
        {
            List<DB_Toolbox.DBParameters> ParamList = new List<DB_Toolbox.DBParameters>();

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERNAME",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100,
                ParamValue = "JohnDoes"
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_FIRSTNAME",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100,
                ParamValue = "John"
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_LASTNAME",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100,
                ParamValue = "Does"
            });

            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_DOB",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Date,
                ParamValue = DateTime.Now.ToString("dd-MMM-yyyy")
            });


            ParamList.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_USERS_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32
            });

            string USER_ID = EMIN_DBK.SQL_PROCEDURE_GET_VALUE("R_USERS_ID", EMIN_DBK.SQL_PROCEDURE_PARAMS("ORACLE", constr1, "USERS_INSERT_ROW", ParamList));


            List<DB_Toolbox.DBParameters> ParamList2 = new List<DB_Toolbox.DBParameters>();

            ParamList2.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_NAME",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Varchar2,
                ParamSize = 100,
                ParamValue = "2nd Application"
            });

            ParamList2.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_USERS_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                ParamValue = "1000",

            });

            ParamList2.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "P_PARENT_ID",
                ParamDirection = ParameterDirection.Input,
                OracleParamDataType = OracleDbType.Int32,
                ParamValue = "1000",

            });

            ParamList2.Add(new DB_Toolbox.DBParameters
            {
                ParamName = "R_APPLICATION_ID",
                ParamDirection = ParameterDirection.Output,
                OracleParamDataType = OracleDbType.Int32
            });

            string APP_ID = EMIN_DBK.SQL_PROCEDURE_GET_VALUE("R_APPLICATION_ID", EMIN_DBK.SQL_PROCEDURE_PARAMS("ORACLE", constr1, "APPLICATIONS_INSERT_ROW", ParamList2));

            

        }
    }
}