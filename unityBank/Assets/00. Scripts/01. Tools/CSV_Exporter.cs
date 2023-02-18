using System.IO;
using UnityEngine;

public class CSV_Exporter
{
    public void ExportToCSV(string fileName, SimControllerSO simControllerSO)
    {
        // Create a string array with the headers of the data you want to export
        string[] headers = { "TotalCustomerCount", "BankingDoneCount", "BankingFailCount" };

        // Create a 2D string array with the data you want to export
        string[,] data = {
            {simControllerSO.TotalCustomerCount.ToString(), simControllerSO.BankingDoneCount.ToString(), simControllerSO.BankingFailCount.ToString()},
        };

        // Create a new file in the project's root directory
        string filePath = Application.dataPath + "/" + fileName + ".csv";
        File.WriteAllText(filePath, "");

        // Write the headers to the file
        File.AppendAllText(filePath, string.Join(",", headers) + "\n");

        // Write the data to the file
        for (int i = 0; i < data.GetLength(0); i++)
        {
            string[] row = new string[data.GetLength(1)];
            for (int j = 0; j < data.GetLength(1); j++)
            {
                row[j] = data[i, j];
            }
            File.AppendAllText(filePath, string.Join(",", row) + "\n");
        }
        Debug.Log("CSV exported to: " + filePath);
    }
}
