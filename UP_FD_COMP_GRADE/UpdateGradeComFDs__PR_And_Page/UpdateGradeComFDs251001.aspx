<%@ Page Title="Update Grade for FDs Company" Language="C#" MasterPageFile="~/vmSite.Master" AutoEventWireup="true" CodeBehind="update_grade_for_fd_com.aspx.cs" Inherits="WM.Masters.update_grade_for_fd_com" EnableViewState="true" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <div class="page-header mb-4">
            <h3 class="page-title">Update Grade for FDs Company</h3>
        </div>
        <div class="row">
            <div class="col-m1">
                <div class="card">
                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-sm-12   d-flex align-items-center gap-2 mt-0">
                                        <label for="ddlMonth" class="col-sm-4 col-form-label" style="width: 100px">Month/Year</label>
                                        <select id="ddlMonth" class="form-control me-1" style="width: 100px"></select>
                                        <span class="px-1">/</span>
                                        <select id="ddlYear" class="form-control ms-1" style="width: 100px"></select>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-sm-12  d-flex align-items-center gap-2 mt-2">
                                        <button id="btnCalculate" class="btn btn-primary btn-sm mx-1" type="button">Save</button>
                                        <button id="btnReset" class="btn btn-secondary btn-sm mx-1" type="button" onclick="location.reload();">Reset</button>
                                        <button id="btnExit" class="btn btn-outline-secondary btn-sm mx-1" type="button" onclick="window.close();">Exit</button>
                                        <span id="lblResultCount" class="text-danger fw-bold px-3">Count</span>

                                    </div>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group row">
                                    <div class="col-sm-12   d-flex align-items-center gap-2 mt-2">
                                        <div class="progress" style="height: 20px; margin-left: 10px; width: 200px; display:none;">
                                            <div id="saveProgressBar" class="progress-bar bg-success" role="progressbar" style="width: 0%;" aria-valuemin="0" aria-valuemax="100">
                                                0%
                                            </div>
                                        </div>
                                        <span id="lblSaveResultCount" class="text-success fw-bold px-3"></span>
                                    </div>
                                </div>
                            </div>


                        </div>

                        <div class="row g-3">
                            <div class="form-group row mt-2">
                                <div class="table-responsive" style="max-height: 300px">
                                    <table id="tbViewTrTrDetails" class="table table-bordered">
                                        <thead class="thead-dark">
                                        </thead>
                                        <tbody>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row d-none">

        <asp:HiddenField ID="hdnLoginId" runat="server" />
        <asp:HiddenField ID="hdnRoleId" runat="server" />

    </div>

    <style>
        .table th, .table td {
            white-space: nowrap;
        }

        td {
            padding: 0 !important;
            padding-left: 8px !important;
        }

        .no-results {
            text-align: center;
            font-weight: bold;
            color: red !important;
            padding: 10px !important;
        }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).ready(function () {

            //#region ONLOAD FUNCTIONS

            function checkUserSession() {
                const loginId = $('#' + '<%= hdnLoginId.ClientID %>').val();
                const roleId = $('#' + '<%= hdnRoleId.ClientID %>').val();

                if (!loginId || !roleId) {
                    alert("Session expired or invalid. Please Login");
                    window.location.href = 'https://www.wealthmaker.in/login_new.aspx';
                }
            }

            function showLoader() {
                if ($('#customLoader').length) {
                    $('#customLoader').show();
                    return;
                }

                const loaderHtml = `
    <div id="customLoader" style="
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0,0,0,0.2);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 9999;">
        <div style="
            background-color: white;
            color: black;
            padding: 20px 30px;
            width: 250px;
            text-align: center;
            border-radius: 5px;
            font-weight: bold;
            font-size: 16px;">
            Please wait...
        </div>
    </div>
`;
                $('body').append(loaderHtml);
            }

            function hideLoader() {
                $('#customLoader').fadeOut(300, function () {
                    $(this).remove();
                });
            }

            $(document).ajaxStart(function () {
                checkUserSession();
                showLoader();
            });

            $(document).ajaxStop(function () {
                hideLoader();
            });

            function preventDefaultActions(preventClick = false, preventEnter = false) {
                if (preventClick) {
                    $(document).on('click.preventBtn', 'button, input[type="submit"]', function (e) {
                        e.preventDefault();
                        console.log('Button click prevented');
                    });
                } else {
                    $(document).off('click.preventBtn');
                }

                if (preventEnter) {
                    $(document).on('keydown.preventEnter', function (e) {
                        if (e.key === "Enter" || e.keyCode === 13) {
                            e.preventDefault();
                            console.log('Enter key prevented');
                            return false;
                        }
                    });
                } else {
                    $(document).off('keydown.preventEnter');
                }
            }


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
            preventDefaultActions(true, true);


            //#endregion ON LAOD FUNCTIONS

            //#region Dynamic Table Generation by Data
            /* How to use:
            displaySearchResults(result, '#psmModelInvestorSearch_tbResult', {
                countElement: '#psmModelInvestorSearch_lblResultCount',
                defaultColumnWidth: '250px',
                adjustableColumns: true,
                displayNoneToTheseColumns: ''
            });
            */


            function displaySearchResults(data, targetTable, options = {}) {
                const {
                    countElement = null,
                    defaultColumnWidth = null,
                    adjustableColumns = false,
                    tableClass = 'table-striped table-bordered',
                    displayNoneToTheseColumns = ''

                } = options;

                // Get the target table element
                const table = $(targetTable);

                table.empty();

                if (data.data && data.data.length > 0) {
                    if (countElement) {
                        $(countElement).text(data.totalCount || data.data.length);
                    }

                    // Create table structure
                    let headerRow = $('<tr>');
                    let tableBody = $('<tbody>');

                    // Create table header
                    if (data.schema && data.schema.length > 0) {
                        data.schema.forEach(function (column) {
                            let th = $('<th>').text(column.columnName);
                            if (defaultColumnWidth) {
                                th.addClass('width', defaultColumnWidth);
                            }
                            headerRow.append(th);
                        });
                    } else if (data.data.length > 0) {
                        // Fallback: use keys from first data item
                        Object.keys(data.data[0]).forEach(function (key) {
                            let th = $('<th>').text(key);
                            if (defaultColumnWidth) {
                                th.addClass('width', defaultColumnWidth);
                            }
                            headerRow.append(th);
                        });
                    }

                    // Create table body rows
                    data.data.forEach(function (row) {
                        let tr = $('<tr>');
                        Object.values(row).forEach(function (value) {
                            tr.append($('<td>').text(value !== null ? value : ''));
                        });
                        tableBody.append(tr);
                    });

                    // Add header to thead and append to table
                    table.append($('<thead>').append(headerRow));
                    table.append(tableBody);
                    //table.addClass(tableClass);

                    // Make columns adjustable if enabled
                    if (adjustableColumns) {
                        var newName = targetTable;
                        makeTableColumnsResizable(targetTable);
                    }
                    applyDataTableStyles(targetTable);
                    applyColumnVisibility(targetTable, options);

                } else {
                    table.html('<tbody><tr><td colspan="100%" class="no-results">No records found matching your criteria</td></tr></tbody>');
                    $(countElement).text('');

                }
            }

            function makeTableColumnsResizable(tableId) {

                if (tableId.startsWith('#')) {
                    tableId = tableId.substring(1);
                }

                // Check if CSS for this tableId already exists
                if (!document.getElementById("resizableTableStyles_" + tableId)) {
                    const css = `
            #${tableId} {
                table-layout: fixed;
                width: 100%;
            }

            #${tableId} th {
                width: 150px;
                position: relative;
                overflow: hidden;
            }

            #${tableId} .resizer {
                position: absolute;
                right: 0;
                top: 0;
                width: 5px;
                height: 100%;
                cursor: col-resize;
                user-select: none;
                z-index: 1;
            }

            #${tableId} th:hover .resizer {
                background-color: rgba(0, 0, 0, 0.1);
            }
        `;

                    const style = document.createElement("style");
                    style.id = "resizableTableStyles_" + tableId;
                    style.innerHTML = css;
                    document.head.appendChild(style);
                }

                const table = document.getElementById(tableId);
                const ths = table.querySelectorAll("th");

                ths.forEach(th => {
                    if (!th.style.width) {
                        th.style.width = th.offsetWidth + "px";
                    }

                    const resizer = document.createElement("div");
                    resizer.classList.add("resizer");
                    th.appendChild(resizer);

                    let startX, startWidth;

                    resizer.addEventListener("mousedown", function (e) {
                        startX = e.pageX;
                        startWidth = th.offsetWidth;

                        function onMouseMove(e) {
                            const newWidth = startWidth + (e.pageX - startX);
                            th.style.width = newWidth + "px";
                        }

                        function onMouseUp() {
                            document.removeEventListener("mousemove", onMouseMove);
                            document.removeEventListener("mouseup", onMouseUp);
                        }

                        document.addEventListener("mousemove", onMouseMove);
                        document.addEventListener("mouseup", onMouseUp);

                        e.preventDefault();
                    });
                });
            }

            function applyDataTableStyles(targetId) {
                // targetId with hash like #psmModal_InvestorSearch

                const styleId = 'dynamic-custom-style';
                if (!document.getElementById(styleId)) {
                    const styles = `
                                .error-message {
                                    display: block;
                                    margin-top: 2px;
                                    font-size: 12px;
                                    /* Small and compact */
                                    color: red;
                                }


                                .draggable {
                                    cursor: move;
                                }

                                .print-table {
                                    width: 100%;
                                    border-collapse: collapse;
                                    border: 1px solid #000;
                                }

                                    .print-table th,
                                    .print-table td {
                                        border: 1px solid #000;
                                        padding: 8px;
                                    }

                                    .print-table th {
                                        background-color: #f2f2f2;
                                    }

                                .floating-panel {
                                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                                    border-radius: 5px;
                                }

                                .client-search-section {
                                    background-color: #f8f9fa;
                                    padding: 15px;
                                    border-radius: 5px;
                                    margin-bottom: 15px;
                                }

                                th {
                                    background-color: #B78939 !important;
                                    color: #fff !important;
                                    position: sticky;
                                    top: 0;
                                    z-index: 2;
                                }

                        .invalid-date {
                            border: 1px solid red !important;
                        }

                        .form-check-label {
                            margin-left: 2px !important;
                        }

                        td {
                            overflow: hidden;
                            text-overflow: ellipsis;
                            white-space: nowrap;
                            max-width: 150px;
                        }

                        ${targetId} tbody tr.row-selected > td {
                            background-color: lightblue !important;
                        }

                        ${targetId} thead tr th, #rtaGrid thead tr th {
                            position: sticky !important;
                            top: 0 !important;
                            background-color: #B78939 !important;
                            color: white !important;
                            z-index: 1;
                        }

                        .ctm_hidden {
                            display: none;
                        }
                    `;

                    // Inject into head
                    $('<style>', {
                        id: styleId,
                        type: 'text/css',
                        html: styles
                    }).appendTo('head');
                }
            }

            function applyColumnVisibility(targetTable, options = {}) {

                // var displayNoneToTheseColumns = '1,2,3,4'

                const { displayNoneToTheseColumns = '' } = options;

                // Parse column numbers into an array of integers
                const hiddenColumns = displayNoneToTheseColumns
                    .split(',')
                    .map(c => parseInt(c.trim(), 10))
                    .filter(c => !isNaN(c));

                if (hiddenColumns.length === 0) return; // nothing to hide

                const table = $(targetTable);

                // Hide headers
                table.find('thead tr th').each(function (index) {
                    if (hiddenColumns.includes(index + 1)) {
                        $(this).css('display', 'none');
                    }
                });

                // Hide body cells
                table.find('tbody tr').each(function () {
                    $(this).find('td').each(function (index) {
                        if (hiddenColumns.includes(index + 1)) {
                            $(this).css('display', 'none');
                        }
                    });
                });
            }

            //#endregion

            function getDataOnChange(month, year) {
                month = month || $('#ddlMonth').val();
                year = year || $('#ddlYear').val();

                if (!month) {
                    alert("Please select Month");
                    return;
                }
                if (!year) {
                    alert("Please select Year");
                    return;
                }


                $.ajax({
                    url: "/masters/update_grade_for_fd_com.aspx/GetDataOnChange",
                    method: "POST",
                    data: JSON.stringify({ mon: month, year: year }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            if (result.data && result.data.length > 0) {
                                displaySearchResults(result, '#tbViewTrTrDetails', {
                                    countElement: '#lblResultCount',
                                    defaultColumnWidth: '150px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: '1,2'
                                });
                                makeColumnDropdown('#tbViewTrTrDetails', 'GRADE');
                            } else {
                                alert("No records found.");
                                $('#lblResultCount').text('');
                                displaySearchResults(result, '#tbViewTrTrDetails', {
                                    countElement: 'lblResultCount',
                                    defaultColumnWidth: '150px',
                                    adjustableColumns: true,
                                    displayNoneToTheseColumns: '1,2'
                                });
                            }
                        } else {
                            alert("Error: " + result.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("AJAX Error: " + error);
                    }
                });
            }

            function makeColumnDropdown(tableId, targetColumn) {

                /* How to use:
                makeColumnDropdown('#tbViewTrTrDetails', 2);
                makeColumnDropdown('#tbViewTrTrDetails', 'GRADE'); // column header text


                */


                const $table = $(tableId);
                const $rows = $table.find('tbody tr');
                let columnIndex = -1;

                // If targetColumn is a number, use directly; else find index by column name
                if (typeof targetColumn === 'number') {
                    columnIndex = targetColumn;
                } else {
                    // Find column index by header name
                    const headers = $table.find('thead th');
                    headers.each(function (i) {
                        if ($(this).text().trim().toLowerCase() === targetColumn.trim().toLowerCase()) {
                            columnIndex = i;
                            return false; // break
                        }
                    });

                    if (columnIndex === -1) {
                        console.warn("Column not found:", targetColumn);
                        return;
                    }
                }

                // Loop through each row and convert cell to <select>
                $rows.each(function () {
                    const $td = $(this).find('td').eq(columnIndex);
                    const currentValue = $td.text().trim();

                    // Create dropdown
                    const $select = $('<select class="form-control form-select">');
                    ['', 'A', 'B'].forEach(function (option) {
                        const $opt = $('<option>').val(option).text(option);
                        if (option === currentValue) {
                            $opt.attr('selected', 'selected');
                        }
                        $select.append($opt);
                    });

                    $td.empty().append($select);
                });
            }

            function deleteGradeRecord(month, year) {
                month = month || $('#ddlMonth').val();
                year = year || $('#ddlYear').val();

                if (!month) {
                    alert("Please select Month");
                    return;
                }
                if (!year) {
                    alert("Please select Year");
                    return;
                }

                $.ajax({
                    url: "/masters/update_grade_for_fd_com.aspx/DeleteGradeRecord",
                    method: "POST",
                    data: JSON.stringify({ mon: month, year: year }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            processSave(month, year, '#tbViewTrTrDetails');
                        } else {
                            alert("Error: " + result.message);
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("AJAX Error: " + error);
                    }
                });
            }

            getDataOnChange();
            $('#ddlMonth').change(function () {
                getDataOnChange();
            });

            $('#ddlYear').change(function () {
                getDataOnChange();
            });

            $('#btnCalculate').click(function () {
                var cur_month = $('#ddlMonth').val();
                var cur_year = $('#ddlYear').val();
                var mon_year = cur_month + '-' + cur_year;
                //getDataOnChange();

                deleteGradeRecord(cur_month, cur_year);
            });

            function processSave(mon, year, target) {
                let totalRows = $(`${target} tbody tr`).length;
                let savedCount = 0;

                if (totalRows === 0) {
                    alert("No rows to save.");
                    return;
                }

                // if table hass not four columns then return
                if ($(`${target} tbody tr`).first().find('td').length < 4) {
                    alert("Data not found to save!");
                    return;
                }


                $('#lblSaveResultCount').text(`Saved 0 of ${totalRows}`);
                updateProgressBar(0);

                $(`${target} tbody tr`).each(function () {
                    var $cells = $(this).find('td');

                    var issTag = $cells.eq(0).text().trim();
                    var issCode = $cells.eq(1).text().trim();
                    var issName = $cells.eq(2).text().trim();
                    var grade = $cells.eq(3).find('select option:selected').val() || '';

                    //ajaxSaveGradeData(mon, year, issTag, issCode, issName, grade);

                    ajaxSaveGradeData0(mon, year, issTag, issCode, issName, grade, function (success) {
                        if (success) {
                            savedCount++;
                            $('#lblSaveResultCount').text(`Saved ${savedCount} of ${totalRows}`);
                            let percent = Math.round((savedCount / totalRows) * 100);
                            updateProgressBar(percent);
                        }
                    });
                });
            }

            function updateProgressBar(percent) {
                if (percent > 0) {
                    $('.progress').show();
                }
                $('#saveProgressBar').css('width', percent + '%').text(percent + '%');
                if (percent === 100) {
                    alert("All rows saved successfully!");
                    // RELOAD PAGE AFTER 3 SECOND 
                    //setTimeout(function () {
                    //    location.reload();
                    //}, 1000);
                }
            }

            function ajaxSaveGradeData(mon, year, issTag, issCode, issName, grade) {
                $.ajax({
                    url: "/masters/update_grade_for_fd_com.aspx/SaveGradeData",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        mon: mon,
                        year: year,
                        issTag: issTag,
                        issCode: issCode,
                        issName: issName,
                        grade: grade
                    }),
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            alert("Data saved successfully." + result.message);
                            console.log(result.message);

                        } else {
                            alert("Error: " + result.message);
                            console.log(result.message);

                            return;
                        }
                    },
                    error: function (xhr, status, error) {
                        alert("AJAX Error: " + error);
                        return;
                    }
                });
            }



            function ajaxSaveGradeData0(mon, year, issTag, issCode, issName, grade, callback) {
                $.ajax({
                    url: "/masters/update_grade_for_fd_com.aspx/SaveGradeData",
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify({
                        mon: mon,
                        year: year,
                        issTag: issTag,
                        issCode: issCode,
                        issName: issName,
                        grade: grade
                    }),
                    success: function (response) {
                        var result = JSON.parse(response.d);
                        if (result.success) {
                            callback(true);
                        } else {
                            alert("Error: " + result.message);
                            callback(false);
                        }
                    },
                    error: function () {
                        callback(false);
                    }
                });
            }


        });
    </script>

</asp:Content>

