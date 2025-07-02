

function formatDateInput(inputField) {
    // oninput="formatDateInput(this)" placeholder="dd/mm/yyyy" MaxLength="10"
    inputField.addEventListener("input", function () {
        let input = this.value.replace(/\D/g, ''); // Remove non-numeric characters

        if (input.length > 2) input = input.substring(0, 2) + '/' + input.substring(2);
        if (input.length > 5) input = input.substring(0, 5) + '/' + input.substring(5, 10);

        this.value = input;
    });
}