// Add this to your JS file or <script> block
function attachAlphanumericFilter(selector) {
    $(selector).on('keypress', function(e) {
        var key = e.which || e.keyCode;
        // Allow: backspace (8)
        if (key === 8) return true;
        // Allow: A-Z (65-90), a-z (97-122), 0-9 (48-57)
        if ((key >= 65 && key <= 90) ||
            (key >= 97 && key <= 122) ||
            (key >= 48 && key <= 57)) {
            return true;
        }
        e.preventDefault();
        return false;
    });
}

// Usage examples:
// attachAlphanumericFilter('#myInput');   // for id
// attachAlphanumericFilter('.myClass');   // for class