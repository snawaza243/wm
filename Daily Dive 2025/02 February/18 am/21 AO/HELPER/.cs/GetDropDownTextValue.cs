private string GetDropDownValueIgnoreCase(string valueToCheck, DropDownList dropdown)
{
    if (dropdown != null)
    {
        foreach (ListItem item in dropdown.Items)
        {
            if (item.Value.Equals(valueToCheck, StringComparison.OrdinalIgnoreCase))
            {
                return item.Value; // Return matched value
            }
        }
    }
    return string.Empty; // Return empty string if not found
}

private string GetDropDownTextIgnoreCase(string textToCheck, DropDownList dropdown)
{
    if (dropdown != null)
    {
        foreach (ListItem item in dropdown.Items)
        {
            if (item.Text.Equals(textToCheck, StringComparison.OrdinalIgnoreCase))
            {
                return item.Text; // Return matched text
            }
        }
    }
    return string.Empty; // Return empty string if not found
}



string matchedValue = GetDropDownValueIgnoreCase("INVESTOR", ddlSalutation);
if (!string.IsNullOrEmpty(matchedValue))
{
    ddlSalutation.SelectedValue = matchedValue;
}

string matchedText = GetDropDownTextIgnoreCase("Mr.", ddlSalutation);
if (!string.IsNullOrEmpty(matchedText))
{
    ddlSalutation.SelectedItem.Text = matchedText;
}
