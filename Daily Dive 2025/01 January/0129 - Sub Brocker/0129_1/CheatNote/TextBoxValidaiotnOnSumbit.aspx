<asp:TextBox ID="panNosbl" 
    CssClass="form-control" 
    runat="server" 
    MaxLength="10" 
    placeholder="Enter PAN (ABCDE1234F)" 
    pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}" 
    title="PAN format: 5 uppercase letters, 4 digits, 1 uppercase letter (e.g., ABCDE1234F)" 
     
    oninput="validatePanInput2(this)">
</asp:TextBox>

<script>
    function validatePanInput2(input) {
        let panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/; // PAN format

        // Convert to uppercase and remove invalid characters
        input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

        // Show error message as a tooltip if invalid
        if (input.value.length > 0 && !panPattern.test(input.value)) {
            input.setCustomValidity("Invalid PAN format (ABCDE1234F)");
        } else {
            input.setCustomValidity(""); // Clear tooltip if valid
        }
    }
</script>