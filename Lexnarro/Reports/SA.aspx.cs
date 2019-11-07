using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lexnarro.Reports
{
    public partial class SA : System.Web.UI.Page
    {
        public DataSet data = new DataSet();
        public string yearFrom = "";
        public string yearTo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                decimal userid = Convert.ToDecimal(Request.QueryString["v"]);
                string finYear = Request.QueryString["f"];

                //int CurrentYear = DateTime.Today.Year;
                //int PreviousYear = DateTime.Today.Year - 1;
                //int NextYear = DateTime.Today.Year + 1;
                //string PreYear = PreviousYear.ToString();
                //string NexYear = NextYear.ToString();
                //string CurYear = CurrentYear.ToString();
                //string FinYear = null;

                //if (DateTime.Today.Month > 3)
                //{
                //    FinYear = CurYear + "-" + NexYear;
                //}
                //else
                //{
                //    FinYear = PreYear + "-" + CurYear;
                //}

                using (SqlCommand cmd = new SqlCommand())
                {
                    using (SqlDataAdapter adap = new SqlDataAdapter())
                    {
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString))
                        {
                            //   cmd.CommandText = @"SELECT User_Training_Transaction.Date, User_Training_Transaction.Hours, User_Training_Transaction.Provider, User_Training_Transaction.Financial_Year, User_Training_Transaction.Forwardable, 
                            //User_Training_Transaction.Has_been_Forwarded, User_Training_Transaction.Descrption, User_Training_Status.Received_By_Forwarding, User_Training_Status.Units_Done, 
                            //User_Training_Status.Min_Required_Category_Units, Activity_Master.Name AS 'Activity Name', Category_Master.Name AS 'Category Name', State_Master.Name AS 'State Name', User_Profile.FirstName, 
                            //User_Profile.LastName, User_Profile.OtherName, User_Profile.StreetNumber, User_Profile.StreetName, User_Profile.PostCode, User_Profile.Suburb, User_Profile.LawSocietyNumber, User_Profile.EmailAddress, 
                            //User_Profile.PhoneNumber, User_Profile.Address, Country_Master.Name AS 'Country Name', User_Training_Transaction.Your_Role, Activity_Master.ShortName AS 'ActivityShortName', Category_Master.ShortName
                            //FROM User_Training_Transaction INNER JOIN
                            //User_Training_Status ON User_Training_Transaction.Id = User_Training_Status.Training_Transaction_ID AND User_Training_Transaction.User_Id = User_Training_Status.User_Id INNER JOIN
                            //Activity_Master ON User_Training_Transaction.Activity_Id = Activity_Master.ID AND User_Training_Status.Activity_Id = Activity_Master.ID INNER JOIN
                            //Category_Master ON User_Training_Transaction.Category_Id = Category_Master.ID AND User_Training_Status.Category_Id = Category_Master.ID INNER JOIN
                            //State_Master ON User_Training_Transaction.State_Id = State_Master.ID INNER JOIN
                            //User_Profile ON User_Training_Transaction.User_Id = User_Profile.ID AND User_Training_Status.User_Id = User_Profile.ID AND State_Master.ID = User_Profile.StateID AND 
                            //State_Master.ID = User_Profile.StateEnrolled INNER JOIN
                            //Country_Master ON State_Master.Country_ID = Country_Master.ID AND User_Profile.CountryID = Country_Master.ID 
                            //where User_Training_Transaction.User_Id = " + userid + " and User_Training_Transaction.Financial_Year = '" + FinYear + "'";



                         cmd.CommandText = @"SELECT  User_Training_Transaction.Date, User_Training_Transaction.Hours, User_Training_Transaction.Provider, 
                         User_Training_Transaction.Financial_Year, User_Training_Transaction.Forwardable, 
                         User_Training_Transaction.Has_been_Forwarded, User_Training_Transaction.Descrption, User_Training_Status.Received_By_Forwarding, 
                         User_Training_Status.Units_Done, User_Profile.LawSocietyNumber,
                         User_Training_Status.Min_Required_Category_Units, User_Profile.FirstName, User_Profile.LastName, User_Profile.OtherName, 
                         User_Profile.StreetNumber, User_Profile.StreetName, User_Profile.PostCode, 
                         User_Profile.Suburb, User_Profile.LawSocietyNumber, User_Profile.EmailAddress, User_Profile.PhoneNumber, User_Profile.Address, 
                         User_Training_Transaction.Your_Role, Country_Master.Name AS 'Country Name', 
                         State_Master.Name AS  'State Name', State_Master.ShortName , Activity_Master.Name AS 'Activity Name', 
                         Activity_Master.ShortName AS 'ActivityShortName', Category_Master.Name  AS 'Category Name', Category_Master.ShortName as 'CategShortName',  
                         Sub_Activity_Master.Name AS 'SubActivity Name', Sub_Activity_Master.ShortName AS 'SubActivityShortName', User_Profile.Firm
                         FROM   User_Training_Transaction INNER JOIN
                         User_Training_Status ON User_Training_Transaction.Id = User_Training_Status.Training_Transaction_ID INNER JOIN
                         User_Profile ON User_Training_Transaction.User_Id = User_Profile.ID AND User_Training_Status.User_Id = User_Profile.ID INNER JOIN
                         Country_Master ON User_Profile.CountryID = Country_Master.ID INNER JOIN
                         State_Master ON User_Profile.StateID = State_Master.ID INNER JOIN
                         Activity_Master ON User_Training_Transaction.Activity_Id = Activity_Master.ID AND 
                         User_Training_Status.Activity_Id = Activity_Master.ID INNER JOIN
                         Category_Master ON User_Training_Transaction.Category_Id = Category_Master.ID AND 
                         User_Training_Status.Category_Id = Category_Master.ID LEFT OUTER JOIN
                         Sub_Activity_Master ON User_Training_Transaction.SubActivity_Id = Sub_Activity_Master.ID
                         where User_Training_Transaction.User_Id = " + userid + " and User_Training_Transaction.Financial_Year = '" + finYear + "'";


                            cmd.CommandType = CommandType.Text;
                            if (con.State == ConnectionState.Closed)
                            {
                                con.Open();
                            }
                            cmd.Connection = con;
                            adap.SelectCommand = cmd;
                            data.Clear();
                            adap.Fill(data, "Report_SA");
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }

                yearFrom = data.Tables[0].Rows[0][3].ToString().Split('-')[0];
                yearTo = data.Tables[0].Rows[0][3].ToString().Split('-')[1];
            }
            catch (Exception rr)
            {
                //throw;
            }


            //obj.SetDataSource(data.Tables["Report_ACT"]);
            //obj.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Response, false, "Report_ACT");
        }
    }
}