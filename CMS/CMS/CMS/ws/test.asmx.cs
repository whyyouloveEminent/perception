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
    [WebService(Namespace = "http://tempuri.org/")]
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
        public String SaveGrid(string DB_PLATFORM, string connAuth, string gridName, dynamic [] GridTableElements)
        {
            Organizations_ID = "1000";
            Users_ID = "1000";

            connAuth = constr1;

            Stages_ID = EP_Toolkit.EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Grid", gridName, Organizations_ID, Users_ID);
            GripsID = EP_Toolkit.EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Grid", gridName, "Grid Settings", Organizations_ID, Users_ID);
            ObjectSetsID = EP_Toolkit.EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Grid", gridName, "Grid Settings", "Grid Widget", Organizations_ID, Users_ID);

            foreach (dynamic dc in GridTableElements)
            {
                
                ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-col", "false", "false", "", dc["_DataCol"].ToString());
                ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-row", "false", "false", "", dc["_DataRow"].ToString());
                ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizex", "false", "false", "", dc["_DataSizeX"].ToString());
                ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "data-sizey", "false", "false", "", dc["_DataSizeY"].ToString());
                ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "Text_Box", "Characters", "class", "false", "false", "", dc["_Class"].ToString());
            }

            return "Success";

        }

        public String SaveForm(string DB_PLATFORM, string connAuth, string formName, dynamic[] FormSettingsTable, dynamic[] FormFieldsTable)
        {
            Organizations_ID = "1000";
            Users_ID = "1000";

            connAuth = constr1;

            Stages_ID = EP_Toolkit.EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Form", formName, Organizations_ID, Users_ID);
            GripsID = EP_Toolkit.EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Form", formName, "Form Settings", Organizations_ID, Users_ID);
            
            //ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, "ID", "Characters", "ID", "false", "false", "", formName);
            
            foreach (dynamic dc in FormSettingsTable) //Object
            {
                ObjectSetsID = EP_Toolkit.EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", dc["_StageName"], "Form Settings", dc["_ObjectType"], Organizations_ID, Users_ID);
                
                foreach (dynamic sdc in dc["_Property"]) //Property
                {
                    ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, sdc["_ObjectType"], sdc["_ValueDatatype"], sdc["_PropertyName"], sdc["_HasParent"], sdc["_HasChild"], sdc["_ParentObjectPropID"], sdc["_PropertyValue"]);

                    foreach (dynamic ssdc in sdc["_PropertyOption"]) //PropertyOption
                    {
                        EP_Toolkit.EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, ssdc["_OptionValue"]);
                    }
                }
            }


            Stages_ID = EP_Toolkit.EP_ADD_ENTRY_Stage(DB_PLATFORM, connAuth, "Form", formName, Organizations_ID, Users_ID);
            GripsID = EP_Toolkit.EP_ADD_ENTRY_Grip(DB_PLATFORM, connAuth, "Form", formName, "Form Fields", Organizations_ID, Users_ID);
            ObjectSetsID = EP_Toolkit.EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", formName, "Form Fields", "Text_Box", Organizations_ID, Users_ID);

            foreach (dynamic dc in FormFieldsTable)
            {
                ObjectSetsID = EP_Toolkit.EP_ADD_ENTRY_Object_Set(DB_PLATFORM, connAuth, "Form", dc["_StageName"], "Form Fields", dc["_ObjectType"], Organizations_ID, Users_ID);

                foreach (dynamic sdc in dc["_Property"]) //Property
                {
                    ObjectPropertySetID = EP_Toolkit.EP_ADD_ENTRY_Object_Property_Set(DB_PLATFORM, connAuth, ObjectSetsID, sdc["_ObjectType"], sdc["_ValueDatatype"], sdc["_PropertyName"], sdc["_HasParent"], sdc["_HasChild"], sdc["_ParentObjectPropID"], sdc["_PropertyValue"]);

                    foreach (dynamic ssdc in sdc["_PropertyOption"]) //PropertyOption
                    {
                        EP_Toolkit.EP_ADD_ENTRY_Object_Property_Option_Set(DB_PLATFORM, connAuth, ObjectPropertySetID, ssdc["_OptionValue"]);
                    }
                }
            }
            
            return "Success";

        }

    }
}
