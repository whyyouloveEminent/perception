﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CMS.classes;
using Oracle.DataAccess.Client;

namespace CMS
{
    public partial class Forms : System.Web.UI.Page
    {
        string constr1 = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
        DB_Toolbox EMIN_DBK = new DB_Toolbox();
        DB_Toolbox DB_Kit = new DB_Toolbox();

        DB_Toolbox.DBParameters ProcParams = new DB_Toolbox.DBParameters();

        List<DB_Toolbox.DBParameters> ParamList0 = new List<DB_Toolbox.DBParameters>();
        List<DB_Toolbox.DBParameters> ParamList1 = new List<DB_Toolbox.DBParameters>();

        protected void Page_Load(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();

            dt = EMIN_DBK.EP_RUN_QUERY("ORACLE", constr1, "Select * from FORMS_VW");

            

            DataSet ds = new DataSet();

            ds.Tables.Add(dt);
            
            Repeater1.DataSourceID = null;
            Repeater1.DataSource = ds.Tables[0].DefaultView;
            Repeater1.DataBind();
            
        }

      
    }
}