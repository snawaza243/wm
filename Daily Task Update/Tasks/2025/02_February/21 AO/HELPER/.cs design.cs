 if (selected_inv == txtInvestorCode.Text.Trim() && !string.IsNullOrEmpty(txtInvestorCode.Text))
 {
     txtInvestorName.Text = "VALID INVESTOR";
     txtInvestorName.ForeColor = System.Drawing.Color.Green; // Highlight in green
 }
 else
 {
     txtInvestorName.Text = "INVALID INVESTOR";
     txtInvestorName.ForeColor = System.Drawing.Color.Red; // Highlight in red
 }