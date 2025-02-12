<asp:TextBox 
    ID="certRegNo" 
    CssClass="form-control" 
    runat="server" 
    ClientIDMode="Static" 
    MaxLength="20" 
    TextMode="Number" 
    min="1" 
    max="99999999999999999999"
    oninput="trimInput(this, 20); validateNumericInput(this);" 
    onpaste="validatePaste(event, this);"/>

<script type="text/javascript">
    // Function to trim the input if it exceeds the max length
    function trimInput(inputElement, maxLength) {
        if (inputElement.value.length > maxLength) {
            inputElement.value = inputElement.value.substring(0, maxLength);
        }
    }

    // Function to ensure only numeric values are allowed while typing
    function validateNumericInput(inputElement) {
        // Replace all non-numeric characters with empty string
        inputElement.value = inputElement.value.replace(/\D/g, '');
    }

    // Function to prevent pasting of non-numeric characters
    function validatePaste(event, inputElement) {
        // Get pasted data
        var pastedData = (event.clipboardData || window.clipboardData).getData('text');
        
        // Only allow numbers in the pasted data
        if (/\D/.test(pastedData)) {
            event.preventDefault(); // Prevent paste if it's not a number
        }
    }
</script>
