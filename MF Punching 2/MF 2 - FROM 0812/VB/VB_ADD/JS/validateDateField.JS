function validateDateField(fieldSelector, autoFormat = false, defaultToToday = false) {
    const datePattern = /^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/\d{4}$/;

    // Set default date if enabled
    if (defaultToToday && $(fieldSelector).val().trim() === '') {
        const today = new Date();
        const formatted = ('0' + today.getDate()).slice(-2) + '/' +
                          ('0' + (today.getMonth() + 1)).slice(-2) + '/' +
                          today.getFullYear();
        $(fieldSelector).val(formatted);
    }

    // Validate on blur
    $(fieldSelector).on('blur', function () {
        const value = $(this).val().trim();
        if (!datePattern.test(value)) {
            $(this).css('border', '2px solid red');
            $(this).attr('title', 'Please enter date in DD/MM/YYYY format');
        } else {
            $(this).css('border', '');
            $(this).removeAttr('title');
        }
    });

    // Auto-format on input
    if (autoFormat) {
        $(fieldSelector).on('input', function () {
            let val = $(this).val().replace(/[^\d]/g, '');
            if (val.length >= 2 && val.length < 4) {
                val = val.slice(0, 2) + '/' + val.slice(2);
            } else if (val.length >= 4 && val.length < 8) {
                val = val.slice(0, 2) + '/' + val.slice(2, 4) + '/' + val.slice(4);
            } else if (val.length >= 8) {
                val = val.slice(0, 2) + '/' + val.slice(2, 4) + '/' + val.slice(4, 8);
            }
            $(this).val(val);
        });
    }
}



$(document).ready(function () {
    // Auto-format enabled, default to today enabled
    validateDateField('#ImSipStartDtA', true, true);

    // Only validation, no auto-format or default
    validateDateField('#ImSipEndDtA');
});