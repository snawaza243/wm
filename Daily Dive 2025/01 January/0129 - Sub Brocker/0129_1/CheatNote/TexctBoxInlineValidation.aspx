<asp:TextBox ID="panNosbl" 
CssClass="form-control" 
runat="server" 
MaxLength="10" 
placeholder="Enter PAN (ABCDE1234F)" 
pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}" 
title="PAN format: 5 uppercase letters, 4 digits, 1 uppercase letter (e.g., ABCDE1234F)" 
 
oninput="validatePanInput(this)">
</asp:TextBox>
<span id="panNosblError" style="color: red; font-size: 12px;"></span>

<script>
function validatePanInput(input) {
    let panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/; // PAN format
    let errorSpan = document.getElementById("panNosblError");

    // Convert to uppercase and remove invalid characters
    input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

    // Inline validation message
    if (input.value.length > 0 && !panPattern.test(input.value)) {
        errorSpan.textContent = "Invalid PAN format (ABCDE1234F)";
    } else {
        errorSpan.textContent = ""; // Clear error if valid
    }
}
</script>