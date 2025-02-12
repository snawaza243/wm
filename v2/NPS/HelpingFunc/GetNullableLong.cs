// Define the GetNullableLong function
public long? GetNullableLong(string inputText)
{
    // Try to parse the input text into a long value
    if (long.TryParse(inputText, out long parsedValue))
    {
        // Return the parsed long value if successful
        return parsedValue;
    }
    else
    {
        // Return null if parsing fails (invalid input)
        return null;
    }
}

// Example of how to use this function in your code:

// Get the input value from the TextBox (or any other source)
string mobileInput = anameringMobile.Text;  // Replace with your actual input source

// Use the GetNullableLong function to get the nullable long
long? mobile = GetNullableLong(mobileInput);

// Check if the mobile number is valid (not null)
if (mobile.HasValue)
{
    // Do something with the valid mobile number
    lblAgentCodeSearchedMasterInfo.Text = $"Mobile number is: {mobile.Value}";
}
else
{
    // Handle invalid input (e.g., display an error message)
    lblAgentCodeSearchedMasterInfo.Text = "Invalid or no mobile number entered.";
}
