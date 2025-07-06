using System;
using System.IO;
using System.Web.UI.WebControls;

public class YourClass
{
    public string SaveMapped(DropDownList ddl, string mapExlPath)
    {
        int ddlCount = ddl.Items.Count;

        if (ddlCount <= 0)
        {
            // Message for no mappings
            return "Mapping Is Not Done, Do You Want To Save the Blank File?";
        }

        string maskedStr = "";
        for (int i = 0; i < ddlCount; i++)
        {
            string[] splitStr = ddl.Items[i].Value.ToString().Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);

            if (splitStr.Length != 2)
            {
                // If the split does not yield the expected number of parts, skip this entry or handle it appropriately
                continue;
            }

            string excelFld = splitStr[0];
            string backFld = splitStr[1];

            if (CountBrackets(excelFld) > 0)
            {
                maskedStr += backFld + "#" + excelFld + ",";
            }
            else
            {
                maskedStr += backFld + "#[" + excelFld + "],";
            }
        }

        if (!string.IsNullOrEmpty(maskedStr))
        {
            maskedStr = maskedStr.TrimEnd(',');
        }

        // Write the data to a file
        try
        {
            File.WriteAllText(mapExlPath, maskedStr);
            string sql = $"INSERT INTO DUEPAID_MAPPEDDATA VALUES("DUE", '{maskedStr}', "ModifiedUser", "LoginUser", "MODIFIEDTIME")"

            pc.ExecuteCurrentQuery()
            return "Mapped Successfully";
        }
        catch (Exception ex)
        {
            // Log the exception or handle error appropriately
            return "Error occurred while saving the file: " + ex.Message;
        }
    }

    private int CountBrackets(string input)
    {
        // Counts the braces in the input string
        int count = 0;
        foreach (char c in input)
        {
            if (c == '[' || c == ']')
            {
                count++;
            }
        }
        return count;
    }




}