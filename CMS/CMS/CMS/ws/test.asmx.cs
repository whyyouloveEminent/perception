﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using Oracle.DataAccess.Client;
using CMS.classes;


namespace CMS.ws
{
    /// <summary>
    /// Summary description for test
    /// </summary>
    [WebService(Namespace = "http://localhost:7723")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    
    
    
    public class test : System.Web.Services.WebService
    {

        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["EP_OracleUserConn"].ToString();
        DB_Toolbox EP_Toolkit = new DB_Toolbox();
        string Organizations_ID;

        string Users_ID;

        string Cores_ID;

        string APP_ID;

        string Cases_ID;

        string Forms_ID, Stages_ID, GripsID, ObjectSetsID, ObjectPropertySetID, ObjectPropertyOptionSetID;

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod] 
        public DataTable Applications()
        {
            DataTable dt = new DataTable();

            dt = EP_Toolkit.EP_RUN_QUERY("ORACLE", constr1, "Select * from APPLICATIONS_VW");



            return dt;
        }

        [WebMethod]
        public DataTable SaveGrid(string DB_PLATFORM, string connAuth, string gridName, DataTable jsontable)
        {
            Organizations_ID = "1000";
            Users_ID = "1000";

            Stages_ID = EP_Toolkit.EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Grid", "First Grid", Organizations_ID, Users_ID);
            GripsID = EP_Toolkit.EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Grid", "First Grid", "Grid Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_Toolkit.EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Grid", "First Grid", "Grid Settings", "Grid Widget", Organizations_ID, Users_ID);

            
            ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-col", "false", "false", "", "1");
            ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-row", "false", "false", "", "1");
            ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizex", "false", "false", "", "57");
            ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizey", "false", "false", "", "4");
            ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "class", "false", "false", "", "gs-w");

            return jsontable;

        }

    }
}
