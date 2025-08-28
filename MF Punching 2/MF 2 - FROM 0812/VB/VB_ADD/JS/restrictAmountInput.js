restrictAmountInput('TxtAmountA');

function restrictAmountInput(fieldName) {
  $('#' + fieldName).on('keypress', function (e) {
    const key = e.which || e.keyCode;
    const char = String.fromCharCode(key);
    const value = $(this).val();

    // Allow digits, one dot, and backspace
    if (
      (key >= 48 && key <= 57) || // 0–9
      (key === 46 && value.indexOf('.') === -1) || // one dot
      key === 8 // backspace
    ) {
      // allowed
    } else if (key === 13) {
      // Enter key → move to next field
      e.preventDefault();
      const inputs = $('input:visible');
      const idx = inputs.index(this);
      if (idx > -1 && idx < inputs.length - 1) {
        inputs.eq(idx + 1).focus();
      }
    } else {
      e.preventDefault(); // block everything else
    }
  });
}