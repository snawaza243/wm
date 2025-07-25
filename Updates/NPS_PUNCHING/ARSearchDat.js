$(document).ready(function () {
    $('#<%= arPopToDatePicker.ClientID %>').datepicker({
        format: 'dd/mm/yyyy',
        autoclose: true,
        todayHighlight: true,
        endDate: '0d'  // Prevents selecting future dates
    });

    // Input handling to format the date
    $('#<%= txtArToDate.ClientID %>').on('input', function () {
        let value = $(this).val().replace(/[^0-9/]/g, ''); // Allow only numbers and '/'
        if (value.length === 2 || value.length === 5) {
            $(this).val(value + '/');
        }
        if (value.length > 10) {
            $(this).val(value.substring(0, 10));
        }
    });
});
