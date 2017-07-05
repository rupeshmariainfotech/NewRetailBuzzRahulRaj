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
    public partial class RetailBillPrePrintedWithMRP : System.Web.UI.Page
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
                int RetailBillId = Decode(id);
                SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
                SqlDataAdapter adp2 = new SqlDataAdapter("select * from RetailBills where RetailMasterId=" + RetailBillId, con);
                RetailManagementDataSet5 ds2 = new RetailManagementDataSet5();
                con.Open();
                adp2.Fill(ds2);
                ReportDataSource rds1 = new ReportDataSource("DataSet2", GetDs1(RetailBillId));
                ReportDataSource rds = new ReportDataSource("DataSet1", GetDs(RetailBillId));
                ReportDataSource rds2 = new ReportDataSource("DataSet3", GetDs2());
                ReportDataSource rds3 = new ReportDataSource("DataSet4", GetDs3());
                ReportViewer1.LocalReport.DataSources.Add(rds1);
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.DataSources.Add(rds2);
                ReportViewer1.LocalReport.DataSources.Add(rds3);
                ReportViewer1.LocalReport.ReportPath = "ReportEngine/RetailBillPrePrintedWithMRP.rdlc";
                double grandtotal = Convert.ToDouble(ds2.Tables[0].Rows[0]["GrandTotal"]);
                string Words = NumberToWords(grandtotal);
                ReportParameter parameter = new ReportParameter("AmountInWords", Words);
                ReportViewer1.LocalReport.SetParameters(parameter);
                ReportViewer1.LocalReport.Refresh();
            }
        }

        private DataTable GetDs(int RBId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            string RetailBillNo = Session["RetailBillNo"].ToString();
            SqlDataAdapter adp1 = new SqlDataAdapter("select * from RetailBillItems where RetailBillNo='" + RetailBillNo + "'", con);
            RetailManagementDataSet4 ds1 = new RetailManagementDataSet4();
            con.Open();
            adp1.Fill(ds1);
            //Session["Barcode"] = @"file:///D:\Shraddha\RetailApp\ATOZ\MvcRetailApp\Images\" + ds1.Tables[1].Rows[0]["Barcode"].ToString();
            //for (int i = 0; i < ds1.Tables[1].Rows.Count; i++)
            //{
            //    ds1.Tables[1].Rows[i]["Barcode"] = @"file:///D:\Shraddha\RetailApp\ATOZ\MvcRetailApp\Images\" + ds1.Tables[1].Rows[i]["Barcode"].ToString();
            //}
            return ds1.Tables[1];
        }

        private DataTable GetDs1(int RBId)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            SqlDataAdapter adp2 = new SqlDataAdapter("select * from RetailBills where RetailMasterId=" + RBId, con);
            RetailManagementDataSet5 ds2 = new RetailManagementDataSet5();
            con.Open();
            adp2.Fill(ds2);

            //find retail bill no using primary key
            string RetailBillNo = ds2.Tables[1].Rows[0]["RetailBillNo"].ToString();
            Session["RetailBillNo"] = RetailBillNo;
            return ds2.Tables[1];
        }


        private DataTable GetDs2()
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["RetailManagement3ConnectionString"].ConnectionString);
            string rbno = Session["RetailBillNo"].ToString();
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
            string rbno = Session["RetailBillNo"].ToString();
            SqlDataAdapter adp3 = new SqlDataAdapter("select * from SalesReturns where BillNo='" + rbno + "'", con);
            SalesReturnsDataset ds4 = new SalesReturnsDataset();
            con.Open();
            adp3.Fill(ds4);

            // find retail bill no using primary key
            //string Code = ds3.Tables[1].Rows[0]["Code"].ToString();
            //Session["Code"] = Code;
            return ds4.Tables[1];
        }

        public string NumberToWords(double grandtotal)
        {
            int number = Convert.ToInt32(grandtotal);
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[(int)number];
                else
                {
                    words += tensMap[(int)number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[(int)number % 10];
                }
            }

            return words + " Only";
        }


    }
    }
