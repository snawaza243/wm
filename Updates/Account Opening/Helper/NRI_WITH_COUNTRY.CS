
    if (ddlMailingCountryList.SelectedValue == "1") // Condition for numeric-only PIN
    {
        txtMailingPin.Text = string.Empty; // Clear existing text
        txtMailingPin.MaxLength = 6;
        txtMailingPin.TextMode = TextBoxMode.SingleLine;

        // Inline JavaScript for numeric-only input
        txtMailingPin.Attributes["oninput"] = "this.value = this.value.replace(/[^0-9]/g, '').substring(0, 6);";
    }
    else if (!string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue) && 
             ddlMailingCountryList.SelectedValue != "1") // Condition for 20-character string input
    {
        txtMailingPin.Text = string.Empty; // Clear existing text
        txtMailingPin.MaxLength = 20;
        txtMailingPin.TextMode = TextBoxMode.SingleLine;

        // Allow any characters (no input restriction)
        txtMailingPin.Attributes.Remove("oninput");
    }
    else
    {
        // Default state (optional)
        txtMailingPin.Text = string.Empty;
        txtMailingPin.MaxLength = 6;
        txtMailingPin.TextMode = TextBoxMode.SingleLine;
        txtMailingPin.Attributes["oninput"] = "this.value = this.value.replace(/[^0-9]/g, '').substring(0, 6);";
    }