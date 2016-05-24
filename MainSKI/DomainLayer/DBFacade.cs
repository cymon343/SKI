using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer;


namespace Persistence
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
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = o.Id;
                        //TODO 1) CREATE CUSTOMER 
                        command.Parameters.Add("@customerID", SqlDbType.VarChar).Value = o.Customer.Id; //Cannot be null
                                                
                        if (o.MainOrder != null)
                            command.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = o.MainOrder.Id;//This guarantees that we never need to handle subOrders on order (DB) creation
                        else
                            command.Parameters.Add("@mainOrderID", SqlDbType.VarChar).Value = null;

                        command.Parameters.Add("@orderNumber", SqlDbType.Int).Value = o.OrderNumber;
                        command.Parameters.Add("@deliveryDate", SqlDbType.Date).Value = o.DeliveryDate.Date;
                        command.Parameters.Add("@productionDate", SqlDbType.Date).Value = o.ProductionDate.Date;
                        command.Parameters.Add("@cubicMeters", SqlDbType.Float).Value = o.CubicMeters;
                        command.Parameters.Add("@numberOfElements", SqlDbType.Float).Value = o.NumOfElements;

                        if (CreateCustomer(o.Customer, connection))
                        {
                            if (command.ExecuteNonQuery() > 0) //IF order is created
                            {
                                if (CreateLink(o.AppendixLinks, connection))
                                {
                                    if (CreateOPS(o.ProgressInfo, connection))
                                    {
                                        if (CreateProductionData(o.ProdData, connection))
                                        {
                                            if (CreateElements(o.Elements, connection))
                                            {
                                                success = true;                                               
                                            }
                                        }
                                    }
                                }
                            }
                        }

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

        private bool CreateCustomer(Customer customer, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateLink(List<string> appendixLinks, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateOPS(ProgressState[] progressInfo, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateProductionData(List<ProductionData> prodData, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        private bool CreateElements(List<Element> elements, SqlConnection connection)
        {
            bool success = false;
            //Foreach element:
            for (int i = 0; i < elements.Count; i++)
            {
                //Create the Element...
                if (CreateEPS(elements[i].ProgressInfo, connection))
                {
                    success = true;
                }
            }
            
            throw new NotImplementedException();
        }

        private bool CreateEPS(ProgressState[] progressInfo, SqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public List<Order> RetrieveAllOrders()
        {
            /*
            ORDER OF BUSINESS:
            EXTRACT EACH TABLE SEPERATLY.
            LOOP TO COMBINE IN THE ORDER DESCRIBED BELOW.

            "ORDER DESCRIBED BELOW":
            1) ElementProgressState
            2) Element
            3) OrderProgressState
            4) Links
            5) ProductionData
            6) Customer
            7) Order
            */



            throw new NotImplementedException();
        }
        
        public bool UpdateOrderProgressStateBegun(string orderID, int stationNumber, bool begun)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateBegun", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@begun", SqlDbType.Bit).Value = begun;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateBegun has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateBegun in DB...");

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

        public bool UpdateOrderProgressStateDone(string orderID, int stationNumber, bool done)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateDone", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@done", SqlDbType.Bit).Value = done;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateDone has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateDone in DB...");

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

        public bool UpdateOrderProgressStateComment(string orderID, int stationNumber, string comment)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateOrderProgressStateComment", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@orderID", SqlDbType.VarChar).Value = orderID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("OrderProgressStateComment has been set in DB...");
                        else
                            Console.WriteLine("Failed to set OrderProgressStateComment in DB...");

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

        public bool UpdateElementProgressStateBegun(string elementID, int stationNumber, bool begun)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateBegun", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@begun", SqlDbType.Bit).Value = begun;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateBegun has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateBegun in DB...");

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

        public bool UpdateElementProgressStateDone(string elementID, int stationNumber, bool done)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateDone", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@done", SqlDbType.Bit).Value = done;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateDone has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateDone in DB...");

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

        public bool UpdateElementProgressStateComment(string elementID, int stationNumber, string comment)
        {
            bool success = false;
            try
            {
                Console.WriteLine(UserName + " trying to access DB...");

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[_userConnectionString].ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine(UserName + " connected to DB...");
                    using (SqlCommand command = new SqlCommand("updateElementProgressStateComment", connection) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Add("@elementID", SqlDbType.VarChar).Value = elementID;
                        command.Parameters.Add("@stationNumber", SqlDbType.Int).Value = stationNumber;
                        command.Parameters.Add("@comment", SqlDbType.VarChar).Value = comment;

                        success = (command.ExecuteNonQuery() > 0);

                        if (success)
                            Console.WriteLine("ElementProgressStateComment has been set in DB...");
                        else
                            Console.WriteLine("Failed to set ElementProgressStateComment in DB...");

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

        #endregion
    }
}
