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

        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        DB_Toolbox EMIN_DBK = new DB_Toolbox();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod] 
        public DataTable Applications()
        {
            DataTable dt = new DataTable();

            dt = EMIN_DBK.EP_RUN_QUERY("ORACLE", constr1, "Select * from APPLICATIONS_VW");



            return dt;
        }

    }
}
