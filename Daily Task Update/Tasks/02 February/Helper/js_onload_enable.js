<script type="text/javascript">
// Function to check fields and disable/enable buttons
function checkFields() {
    var associateCode = document.getElementById('<%= associateCode.ClientID %>').value;
    var subBrokerExistCode = document.getElementById('<%= subBrokerExistCode.ClientID %>').value;
    var saveButton = document.getElementById('<%= saveButton.ClientID %>');  // Add Button
    var updateButton = document.getElementById('<%= upateButton.ClientID %>');  // Update Button

    // If either or both fields have value, disable Add and enable Update
    if (associateCode || subBrokerExistCode) {
        saveButton.disabled = true;   // Disable Add button
        updateButton.disabled = false;  // Enable Update button
    }
    // If both fields are empty, enable Add and disable Update
    else {
        saveButton.disabled = false;  // Enable Add button
        updateButton.disabled = true;  // Disable Update button
    }
}

// Run the check on page load
window.onload = function () {
    checkFields();  // Call the function on page load
};

// Add event listeners to check the fields whenever the values change
document.getElementById('<%= associateCode.ClientID %>').addEventListener('input', checkFields);
document.getElementById('<%= subBrokerExistCode.ClientID %>').addEventListener('input', checkFields);
</script>
