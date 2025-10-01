
function fillMonthDropdown(targetId) {
    const months = [
        { text: "January", value: "01" },
        { text: "February", value: "02" },
        { text: "March", value: "03" },
        { text: "April", value: "04" },
        { text: "May", value: "05" },
        { text: "June", value: "06" },
        { text: "July", value: "07" },
        { text: "August", value: "08" },
        { text: "September", value: "09" },
        { text: "October", value: "10" },
        { text: "November", value: "11" },
        { text: "December", value: "12" }
    ];

    const $ddl = $(`#${targetId}`);
    $ddl.empty(); // Clear any existing items

    // Optional: Add default "Select Month"
    $ddl.append(`<option value="">Select Month</option>`);

    months.forEach(month => {
        $ddl.append(`<option value="${month.value}">${month.text}</option>`);
    });
}

function fillYearDropdown(targetId, startYear, endYear) {
    const $ddl = $(`#${targetId}`);
    $ddl.empty(); // Clear existing items

    // Optional: Add default "Select Year"
    $ddl.append(`<option value="">Select Year</option>`);

    for (let year = startYear; year <= endYear; year++) {
        $ddl.append(`<option value="${year}">${year}</option>`);
    }
}

function setCurrentMonthYear(monthDropdownId, yearDropdownId) {
    const now = new Date();

    const currentMonth = String(now.getMonth() + 1).padStart(2, '0');
    const currentYear = now.getFullYear();

    $(`#${monthDropdownId}`).val(currentMonth);
    $(`#${yearDropdownId}`).val(currentYear);
}

function formatDateToDMY(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')}/${d.getFullYear()}`;
}

// var curDt = new Date().getFullYear();
fillMonthDropdown('ddlMonth');
fillYearDropdown('ddlYear', 2020, 2030);
setCurrentMonthYear('ddlMonth', 'ddlYear');
