using Lexnarro.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexnarro.EmailTemplate
{
    public partial class Invoice : System.Web.UI.Page
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        public static int rowCount = 0;

        public DataTable dt = new DataTable();
        public string term = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                decimal userId = Convert.ToDecimal(Request.QueryString["v"]);
                decimal recordId = Convert.ToDecimal(Request.QueryString["r"]);
                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter adap = new SqlDataAdapter())
                    {
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString))
                        {
                            cmd.CommandText = @"SELECT Plan_Master.[Plan], User_Profile.FirstName, User_Profile.LastName, User_Profile.EmailAddress, 
                            User_Transaction_Master.Amount, User_Transaction_Master.Invoice_No, User_Transaction_Master.User_ID,
                            CONVERT(varchar(12), User_Transaction_Master.Payment_Date, 103) AS 'Payment_Date', 
                            (SELECT SUM(User_Transaction_Master.Amount) 
							FROM User_Transaction_Master WHERE User_ID=@userId) as sum
                            FROM            User_Transaction_Master INNER JOIN
                            Plan_Master ON User_Transaction_Master.PlanID = Plan_Master.Plan_ID INNER JOIN
                            User_Profile ON User_Transaction_Master.User_ID = User_Profile.ID
                            WHERE (User_Transaction_Master.Payment_Status = 'Paid') AND (User_Transaction_Master.Id = @recordId)";

                            cmd.CommandType = CommandType.Text;

                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@recordId", recordId);

                            if (con.State == ConnectionState.Closed)
                                con.Open();

                            cmd.Connection = con;

                            adap.SelectCommand = cmd;

                            adap.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[0]["Plan"].ToString() == "Yearly")
                                {
                                    term = "365";
                                }
                                if (dt.Rows[0]["Plan"].ToString() == "Two Yearly")
                                {
                                    term = "730";
                                }
                            }

                            if (con.State == ConnectionState.Open)
                                con.Close();
                        }
                    }
                }
            }
            catch (Exception rr)
            {
                //throw;
            }
        }
    }
}