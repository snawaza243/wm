
//#regoin Dynamic Table Generation by Data
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
