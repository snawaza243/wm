    function allowOnlyNumbers(fieldSelector) {
        $(fieldSelector).on("keypress", function (e) {
            var key = e.which || e.keyCode;

            // Allow: backspace (8), delete (46), tab (9), enter (13), arrows (37–40)
            if (key === 8 || key === 9 || key === 13 || key === 46 || (key >= 37 && key <= 40)) {
                return true;
            }

            // Allow only 0–9
            if (key < 48 || key > 57) {
                e.preventDefault();
            }
        });
    }