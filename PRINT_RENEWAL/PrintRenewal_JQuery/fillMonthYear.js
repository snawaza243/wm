function fillMonthYearDropdown(ddlId, startMonthYear, endMonthYear) {
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var [startMonth, startYear] = startMonthYear.split('-').map(Number);
    var [endMonth, endYear] = endMonthYear.split('-').map(Number);

    var options = '';
    var year = startYear, month = startMonth;
    while (year < endYear || (year === endYear && month <= endMonth)) {
        var monthName = months[month - 1];
        var value = (month < 10 ? '0' : '') + month + '-' + year;
        var text = monthName + '-' + year;
        options += `<option value="${value}">${text}</option>`;
        month++;
        if (month > 12) {
            month = 1;
            year++;
        }
    }
    $(ddlId).html(options);
}

/*

Example usage:
fillMonthYearDropdown('#CmbMonth', '01-2020', '12-2030');
This will generate options like:
<option value="01-2020">Jan-2020</option>
<option value="02-2020">Feb-2020</option>


<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="JQuery/fillMonthYear.js"></script>
<script>
$(document).ready(function() {
    fillMonthYearDropdown('#CmbMonth', '01-2020', '12-2030');
});
</script>

*/