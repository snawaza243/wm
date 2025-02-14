<asp:TextBox ID="panNosbl"
CssClass="form-control"
runat="server"
MaxLength="10"
placeholder="Enter PAN (ABCDE1234F)"
pattern="[A-Z]{5}[0-9]{4}[A-Z]{1}"
title="Enter PAN in format: ABCDE1234F"
oninput="validatePanInput2(this)">
</asp:TextBox>

<script>
function validatePanInput2(input) {
    let panPattern = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/; // PAN format

    // Convert to uppercase and remove invalid characters
    input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');

    // Show error message as a tooltip if invalid
    if (input.value.length > 0 && !panPattern.test(input.value)) {
        input.setAttribute("title", "Invalid PAN format (ABCDE1234F)");
        input.classList.add("invalid-input"); // Add red border
    } else {
        input.setAttribute("title", "Enter PAN in format: ABCDE1234F");
        input.classList.remove("invalid-input"); // Remove red border if valid
    }
}
</script>
<style>
    /* Optional: Add red border for invalid input */
    .invalid-input {
        border: 2px solid red;
    }
</style>