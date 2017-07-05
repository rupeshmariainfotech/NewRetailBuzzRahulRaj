using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using CodeFirstServices.Interfaces;
using CodeFirstServices.Services;


namespace MvcRetailApp.ReportEngine
{
    public partial class SalesBillLetterHeadWithSP : System.Web.UI.Page
    {
        public int Decode(string decodeMe)
        {
            byte[] decoded = Convert.FromBase64String(decodeMe);
            string decodedvalue = System.Text.Encoding.UTF8.GetString(decoded);
            return Convert.ToInt32(decodedvalue);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.Reset();
                string id = Request.QueryString["id"];
                int SalesBillId = Decode(id);
                ReportDataSource rds1 = new ReportDataSource("DataSet2", GetDs1(SalesBillId));
                ReportDataSource rds = new ReportDataSource("DataSet1", GetDs(SalesBillId));
                ReportViewer1.LocalReport.DataSources.Add(rds1);
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.ReportPath = "ReportEngine/SalesBillLetterHeadWithSP.rdlc";
                ReportViewer1.LocalReport.Refresh();
            }
        }

        private DataTable GetDs(int SBId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagementConnectionString"].ConnectionString);
            //SqlConnection con = new SqlConnection("Data Source=LEENA-PC;Initial Catalog=A To Z DADAR T.T. 01-04-2015 To 31-03-2016;Integrated Security=True");
            string SalesBillNo = Session["SalesBillNo"].ToString();
            SqlDataAdapter adp1 = new SqlDataAdapter("select * from SalesBillItems where SalesBillNo='" + SalesBillNo + "'", con);
            DataSet ds1 = new DataSet();
            con.Open();
            adp1.Fill(ds1);
            return ds1.Tables[1];
        }

        private DataTable GetDs1(int SBId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagementConnectionString"].ConnectionString);
            //SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["A_To_Z_DADAR_T_T__01_04_2015_To_31_03_2016ConnectionString"].ConnectionString);
           // SqlConnection con = new SqlConnection("Data Source=LEENA-PC;Initial Catalog=A To Z DADAR T.T. 01-04-2015 To 31-03-2016;Integrated Security=True");
            SqlDataAdapter adp2 = new SqlDataAdapter("select * from SalesBills where Id=" + SBId, con);
            DataSet ds2 = new DataSet();
            con.Open();
            adp2.Fill(ds2);

            //find sales order no using primary key
            string SalesBillNo = ds2.Tables[1].Rows[0]["SalesBillNo"].ToString();
            Session["SalesBillNo"] = SalesBillNo;
            return ds2.Tables[1];
        }
    }
}
