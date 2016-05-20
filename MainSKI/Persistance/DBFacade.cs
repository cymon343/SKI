using DomainLayer;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Persistance
{
    class DBFacade
    {
        #region Fields
        private string _userName;
        private string _userConnectionString;
        private static DBFacade instance;
        #endregion

        #region Properties
        public static DBFacade Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBFacade();
                return instance;
            }

        }

        public string UserName
        {
            get
            {
                return _userName;
            }

            private set
            {
                _userName = value;
            }
        }
        #endregion

        #region Constructor
        private DBFacade()
        {
            //Setting the correct connectionString:
            UserName = Environment.UserName;
            if (UserName.ToUpper() == "CYMON343")
                _userConnectionString = "SimonDesktop";
            else if (UserName.ToUpper() == "SIMON")
                _userConnectionString = "SimonLaptop";            
            else if (UserName.ToUpper() == "TM_MA")
                _userConnectionString = "MortenLaptop";
            else if (UserName.ToUpper() == "BRUGER")
                _userConnectionString = "MortenDesktop";
        }
        #endregion

        #region Methods

        public bool CreateOrder(Order o)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("createOrder", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = o.Id;
                        
                        

                        success = (command.ExecuteNonQuery() > 0); //TODO: REVISE THIS!!

                        if (success)
                            Console.WriteLine("Order created in DB...");
                        else
                            Console.WriteLine("Failed to create Order in DB...");

                        connection.Close();
                        Console.WriteLine(UserName + " disconnected from DB...");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return success;
        }

        public List<Order> RetrieveOrdersByDate(DateTime fromDate)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderProgressStateBegun(string orderID, bool begun)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderProgressStateDone(string orderID, bool done)
        {
            throw new NotImplementedException();
        }

        public bool UpdateOrderProgressStateComment(string orderID, string comment)
        {
            throw new NotImplementedException();
        }

        public bool UpdateElementProgressStateBegun(string orderID, string elementID, bool begun)
        {
            throw new NotImplementedException();
        }

        public bool UpdateElementProgressStateDone(string orderID, string elementID, bool done)
        {
            throw new NotImplementedException();
        }

        public bool UpdateElementProgressStateComment(string orderID, string elementID, string comment)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
