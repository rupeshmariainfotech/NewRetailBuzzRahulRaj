﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using Microsoft.Reporting.WebForms;
using CodeFirstServices.Interfaces;
using CodeFirstServices.Services;

namespace MvcRetailApp.ReportEngine
{
    public partial class RetailCreditNoteLetterHeadWithMRP : System.Web.UI.Page
    {


        private string UserEmail
        {
            get { return (string)HttpContext.Current.Session["UserEmail"]; }
            set { HttpContext.Current.Session["UserEmail"] = value; }
        }
        private string CompanyCode
        {
            get { return (string)HttpContext.Current.Session["CompanyCode"]; }
            set { HttpContext.Current.Session["CompanyCode"] = value; }
        }
        private string CompanyName
        {
            get { return (string)HttpContext.Current.Session["CompanyName"]; }
            set { HttpContext.Current.Session["CompanyName"] = value; }
        }
        private string FinancialYear
        {
            get { return (string)HttpContext.Current.Session["FinancialYear"]; }
            set { HttpContext.Current.Session["FinancialYear"] = value; }
        }
        private string DatabaseName
        {
            get { return (string)HttpContext.Current.Session["DatabaseName"]; }
            set { HttpContext.Current.Session["DatabaseName"] = value; }
        }


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
                int CreditNoteId = Decode(id);
                ReportDataSource rds1 = new ReportDataSource("DataSet2", GetDs1(CreditNoteId));
                ReportDataSource rds = new ReportDataSource("DataSet1", GetDs(CreditNoteId));
                ReportDataSource rds2 = new ReportDataSource("DataSet3", GetDs2());
                ReportDataSource rds3 = new ReportDataSource("DataSet4", GetDs3());
                ReportViewer1.LocalReport.DataSources.Add(rds1);
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.DataSources.Add(rds2);
                ReportViewer1.LocalReport.DataSources.Add(rds3);
                ReportViewer1.LocalReport.ReportPath = "ReportEngine/RetailCreditNoteLetterHeadWithMRP.rdlc";
                ReportViewer1.LocalReport.Refresh();



                Warning[] warnings;
                string[] streamIds;
                string mimetype = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string title = "Retail Bill";
                byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimetype, out encoding, out extension, out streamIds, out warnings);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.End();
                string filename = "RetailCreditNoteLetterHeadWithMRP.pdf";
                string path = Server.MapPath("C");
                FileStream file = new FileStream(path + "/" + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                file.Write(bytes, 0, bytes.Length);
                file.Dispose();

                Response.Write(string.Format("<script>window.open('{0}','_blank');</script>", "RetailCreditNoteLetterHeadWithMRP.aspx?file=" + filename));
            }
        }

        private DataTable GetDs(int DId)
        {
            // SqlConnection con = new SqlConnection("Data Source=MARY-PC;Initial Catalog=A To Z Life Style(India) Pvt Ltd Retail 01-04-2016 To 31-03-2017;Integrated Security=True");
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            string CreditNoteNo = Session["SalesReturnNo"].ToString();
            SqlDataAdapter adp1 = new SqlDataAdapter("select * from SalesReturnItems  where SalesReturnNo='" + CreditNoteNo + "'", con);
            SalesReturnItems ds1 = new SalesReturnItems();
            con.Open();
            adp1.Fill(ds1);
            return ds1.Tables[1];
        }

        private DataTable GetDs1(int DId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            //SqlConnection con = new SqlConnection("Data Source=MARY-PC;Initial Catalog=A To Z Life Style(India) Pvt Ltd Retail 01-04-2016 To 31-03-2017;Integrated Security=True");
            SqlDataAdapter adp2 = new SqlDataAdapter("select * from SalesReturns where Id=" + DId, con);
            SalesReturns ds2 = new SalesReturns();
            con.Open();
            adp2.Fill(ds2);

            //find Challan no using primary key
            string CreditNoteNo = ds2.Tables[1].Rows[0]["SalesReturnNo"].ToString();
            Session["SalesReturnNo"] = CreditNoteNo;
            return ds2.Tables[0];
        }

        private DataTable GetDs2()
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            //SqlConnection con = new SqlConnection("Data Source=MARY-PC;Initial Catalog=A To Z Life Style(India) Pvt Ltd Retail 01-04-2016 To 31-03-2017;Integrated Security=True");
            string rbno = Session["SalesReturnNo"].ToString();
            SqlDataAdapter adp3 = new SqlDataAdapter("select * from InventoryTaxes where Code='" + rbno + "'", con);
            InventoryTaxesDataSet ds3 = new InventoryTaxesDataSet();
            con.Open();
            adp3.Fill(ds3);

            // find retail bill no using primary key
            //string Code = ds3.Tables[1].Rows[0]["Code"].ToString();
            //Session["Code"] = Code;
            return ds3.Tables[1];
        }

        private DataTable GetDs3()
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            //  SqlConnection con = new SqlConnection("Data Source=MARY-PC;Initial Catalog=A To Z Life Style(India) Pvt Ltd Retail 01-04-2016 To 31-03-2017;Integrated Security=True");
            string rbno = Session["SalesReturnNo"].ToString();
            SqlDataAdapter adp3 = new SqlDataAdapter("select * from SalesReturns where BillNo='" + rbno + "'", con);
            SalesReturnsDataset ds4 = new SalesReturnsDataset();
            con.Open();
            adp3.Fill(ds4);

            // find retail bill no using primary key
            //string Code = ds3.Tables[1].Rows[0]["Code"].ToString();
            //Session["Code"] = Code;
            return ds4.Tables[1];
        }


    }
}