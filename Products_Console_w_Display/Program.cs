// See https://aka.ms/new-console-template for more information


using System;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:5432;User Id=postgres; " +
           "Password=####;Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM product", conn);
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
        PrintSeven(dt);
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

