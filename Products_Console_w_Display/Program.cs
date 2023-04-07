// See https://aka.ms/new-console-template for more information


using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=simpleps;Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
        //NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM product", conn);
		NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM customer", conn);
		//NpgsqlCommand quantity_command = new NpgsqlCommand("SELECT prod_id, prod_desc, prod_quantity from product WHERE prod_quantity <=30 and prod_quantity >= 12", conn);
		//NpgsqlCommand repids_command = new NpgsqlCommand("SELECT rep_id, SUM(cust_balance) AS rep_total_bal FROM customer GROUP BY rep_id", conn);
		// Execute the query and obtain the value of the first column of the first row
		//Int64 count = (Int64)command.ExecuteScalar();
		NpgsqlDataReader reader = command.ExecuteReader();

        //NpgsqlDataReader quantity = quantity_command.ExecuteReader();
		//NpgsqlDataReader repids = repids_command.ExecuteReader();
        
		DataTable dt = new DataTable();
		dt.Load(reader);

		//DataTable dt2 = new DataTable();
        //dt2.Load(quantity);

		//DataTable dt3 = new DataTable();
		//dt3.Load(repids);

		//print_results(dt);
        //PrintSeven(dt);
        printreps(dt);
		//print_results(dt2);
        //print_results(dt3);

        conn.Close();
    }

    static void PrintSeven(DataTable data)
        
    {
		DataTable sorted_data = new DataTable();
        foreach (DataColumn col in data.Columns)
        {
			if (col.ColumnName.Contains("prod_id") || col.ColumnName.Contains("prod_desc") || col.ColumnName.Contains("prod_quantity"))
            {
                sorted_data.Columns.Add(col.ColumnName, col.DataType);
                
            }

            
		}
        foreach (DataRow row in data.Rows)
        {
         int rowdata = Convert.ToInt32(row["prod_quantity"]);
         if  (rowdata >= 12 && rowdata<= 30){
            DataRow sorted_row = sorted_data.NewRow();
            foreach (DataColumn col in sorted_data.Columns)
            {
                
                sorted_row[col.ColumnName] = row[col.ColumnName];
            }
			sorted_data.Rows.Add(sorted_row); 
           };
			
		}
        Console.Write(sorted_data.Columns);
        Console.Write(sorted_data.Rows);
		Console.WriteLine();
        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        foreach (DataColumn col in sorted_data.Columns)
        {
            
			
			Console.Write(col.ColumnName);
            var maxLabelSize = data.Rows.OfType<DataRow>()
                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                    .OrderByDescending(m => m).FirstOrDefault();

            colWidths.Add(col.ColumnName, maxLabelSize);
            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
       
            
        }

        Console.WriteLine();

       foreach (DataRow dataRow in sorted_data.Rows)
        {
            for (int j = 0; j < dataRow.ItemArray.Length; j++)
            {
               Console.Write(dataRow.ItemArray[j]);
                for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
            }
            Console.WriteLine();
        }

    }
	static void printreps(DataTable data)
	{
		DataTable sorted_data = new DataTable();

		
		sorted_data.Columns.Add("rep_id", typeof(string));
		sorted_data.Columns.Add("total_bal",typeof(double));

		foreach (DataRow row in data.Rows)
		{
			string repIdValue = row["rep_id"].ToString();
			DataRow[] matchingRows = sorted_data.Select("rep_id = '" + repIdValue + "'");
		

			
			if (matchingRows.Length == 0)
			{
				// The current row's rep_id is not present in the sorted_data table
				// Create a new row with the same schema as sorted_data
				DataRow newRow = sorted_data.NewRow();
				newRow["rep_id"] = row["rep_id"];
				//newRow["total_bal"] = row["total_bal"];
				// Repeat for all columns in sorted_data...

				// Add the new row to sorted_data
				sorted_data.Rows.Add(newRow);
				//add the balance to the rep row

				newRow["total_bal"] = row["cust_balance"];
			}
			else
			{
				
				double value_to_add = Convert.ToDouble(row["cust_balance"]);
				foreach (DataRow RepRow in sorted_data.Rows)
				{
					
					string mainrow = Convert.ToString(row["rep_id"]);
					string grouprow = Convert.ToString(RepRow["rep_id"]);
					if (mainrow == grouprow) // im not understanding something about the object types of these two tables
					//if (row["rep_id"] == RepRow["rep_id"])
					{
						RepRow["total_bal"] = (double)RepRow["total_bal"] + value_to_add;
					}
				}
			}
		}

		DataRow[] rowstoremove = sorted_data.Select("total_bal < '12000'");// filters the data from the table that fits the argument
		//define the rows to remove outside of a loop otherwise it will raise and exception if data is removed mid loop
		foreach (DataRow row in rowstoremove)
		{
			sorted_data.Rows.Remove(row);
		}
		
		data = sorted_data;

		Console.Write(sorted_data.Columns);

		Console.Write(sorted_data) ;
		Console.WriteLine();
		Dictionary<string, int> colWidths = new Dictionary<string, int>();

		foreach (DataColumn col in data.Columns)
		{
			Console.Write(col.ColumnName);
			var maxLabelSize = data.Rows.OfType<DataRow>()
					.Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
					.OrderByDescending(m => m).FirstOrDefault();

			colWidths.Add(col.ColumnName, maxLabelSize);
			for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
		}

		Console.WriteLine();

		foreach (DataRow dataRow in data.Rows)
		{
			for (int j = 0; j < dataRow.ItemArray.Length; j++)
			{
				Console.Write(dataRow.ItemArray[j]);
				for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
			}
			Console.WriteLine();
		}

	}
	static void print_results(DataTable data)
	{
		Console.WriteLine();
		Dictionary<string, int> colWidths = new Dictionary<string, int>();

		foreach (DataColumn col in data.Columns)
		{
			Console.Write(col.ColumnName);
			var maxLabelSize = data.Rows.OfType<DataRow>()
					.Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
					.OrderByDescending(m => m).FirstOrDefault();

			colWidths.Add(col.ColumnName, maxLabelSize);
			for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
		}

		Console.WriteLine();

		foreach (DataRow dataRow in data.Rows)
		{
			for (int j = 0; j < dataRow.ItemArray.Length; j++)
			{
				Console.Write(dataRow.ItemArray[j]);
				for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
			}
			Console.WriteLine();
		}

	}
}

