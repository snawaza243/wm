$(document).ready(function () {
    calculatePercentageOnBlur('#ImExpenses', '#TxtAmountA', '#ImnExp_Per');
});


function calculatePercentageOnBlur(expenseField, amountField, outputField) {
    $(expenseField).on('blur', function () {
        const expenseVal = parseFloat($(expenseField).val()) || 0;
        const amountVal = parseFloat($(amountField).val()) || 0;
        if (amountVal !== 0) {
            const percentage = Math.round((expenseVal / amountVal) * 100 * 10000) / 10000; // 4 decimal places
            $(outputField).val(percentage);
        } else {
            $(outputField).val('');
        }
    });
}