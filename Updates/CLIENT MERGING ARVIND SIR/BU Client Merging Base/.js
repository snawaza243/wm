$(document).ready(function () {

    var panUpdateModal = document.getElementById('panUpdateModal');
    var searchModal = document.getElementById('searchModal');
    let $totalClientCount = $(".total-client-count");
    let $selectedClientCount = $(".selected-client-count");
    let $clientSheetTable = $("#client_sheet");
    let $mainSheet = $("#main_sheet");
    let $mainSheetCount = $("#main_sheet_count");

    let $otherSheet = $("#other_sheet");
    let $otherSheetCount = $("#other_sheet_count");

    panUpdateModal.addEventListener('hide.bs.modal', function () {
        $("#panUpdateTable tbody").html('');
    })

    searchModal.addEventListener('hide.bs.modal', function () {

        $(".total-client-count").html($("#client_sheet tbody tr").length);
        resetSearch();
    })

    searchModal.addEventListener('show.bs.modal', function () {

        let dropdown = $("[id$='Branch_Dropdown']");
        if (dropdown.val()) {
            $("[id$='BranchSearch_Dropdown']").val(dropdown.val())
        }
    })

    $clientSheetTable.on("click", "tbody .delete", function () {
        $(this).closest("tr").remove();
        let rowCount = $("#client_sheet tbody tr").length;
        $totalClientCount.html(rowCount);
        $selectedClientCount.html($("#client_sheet tbody .selected").length);
    })

    $mainSheet.on("click", "tbody .delete", function () {
        $(this).closest("tr").remove();
        let rowCount = $(this).closest("tbody").find("tr").length;
        $mainSheetCount.html(rowCount);
    })

    $otherSheet.on("click", "tbody .delete", function () {
        $(this).closest("tr").remove();
        let rowCount = $("#other_sheet tbody tr").length;
        console.log(rowCount)
        $otherSheetCount.html(rowCount);
    })

    $(".branch-search").change(function () {
        let branchCode = $(this).val();
        if (branchCode) {
            $.ajax({
                url: "/masters/clientmerge.aspx/getrmlist",
                method: "post",
                data: JSON.stringify({ branchCode: branchCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {

                    let { data } = JSON.parse(res.d);

                    //console.log(data);
                    let text = data.length > 0 ? "Select RM" : "RM Not Found";
                    data.unshift({ text: text, value: "" })

                    let rmOptions = data.map(r => `<option value="${r.value}">${r.text}</option>`)
                    $("#RMSearch_Dropdown").html(rmOptions.join(''));

                },
                error: function () {

                },
            })
        }

    })

    $("#btnSearh").click(function () {

        let $this = $(this);
        let searchInputs = {
            category: $("#categorySearch").val(),
            branchCode: $(".branch-search").val(),
            cityCode: $(".city-search").val(),
            rmCode: $("#RMSearch_Dropdown").val(),
            clientCode: $("#clientCodeSearch").val(),
            clientName: $("#clientNameSearch").val(),
            panNo: $("#panNoSearch").val(),
            phone: $("#phoneSearch").val(),
            mobile: $("#mobileSearch").val(),
            address1: $("#address1Search").val(),
            address2: $("#address2Search").val(),
            sortBy: $("#sortBy2Search").val()
        }

        $this.attr("disabled", "disabled").html("<i class='fa fa-spinner ms-0'></i> Wait..");
        $.ajax({
            url: "/masters/clientmerge.aspx/getsearchdata",
            method: "post",
            data: JSON.stringify(searchInputs),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {

                let { data } = JSON.parse(res.d);
                console.log(data);
                let tableRow = createTable(data);
                $("#searchTable tbody").html(tableRow);
                $this.removeAttr("disabled", "disabled").html("<i class='fa fa-search'></i> Search");
            },
            error: function () {
                $this.removeAttr("disabled", "disabled").html("<i class='fa fa-search'></i> Search");
            },
        })

    })

    $("#btnSearhReset").click(function () {
        resetSearch();
    })

    $(".modal table tbody").on("dblclick", "tr", function () {
        let clientTable = $("#client_sheet tbody")
        $(this).toggleClass("selected");

        let clientCode = $(this).find("td:nth-child(1)").data('clientCode');

        if ($(this).hasClass('selected') == false) {
            clientTable.find(`tr[data-row-id='${clientCode}']`).remove();
            return false;
        }

        if (clientTable.find(`tr[data-row-id='${clientCode}']`).length == 0) {

            let tableRow = `
            <tr data-row-id='${clientCode}'>
            <td data-name='Create Date'>${$(this).find("td:nth-child(10)").text()}</td>  
            <td data-name='Client Name'>${$(this).find("td:nth-child(1)").text()}</td>
            <td data-name='Client Code' data-client-code="${clientCode}">${$(this).find("td:nth-child(1)").data('clientCode')}</td>
            <td data-name='Address1'>${$(this).find("td:nth-child(2)").text()}</td>
            <td data-name='Address2'>${$(this).find("td:nth-child(3)").text()}</td>
            <td data-name='city'>${$(this).find("td:nth-child(4)").text()}</td>
            <td data-name='Branch Code' data-branch-code='${$(this).find("td:nth-child(5)").data('branchCode')}'>${$(this).find("td:nth-child(5)").text()}</td>
            <td data-name='Phone'>${$(this).find("td:nth-child(6)").text()}</td>
            <td data-name='Mobile'>${$(this).find("td:nth-child(7)").text()}</td>
            <td data-name='Rm Code' data-rm-code='${$(this).find("td:nth-child(8)").data('rmCode')}'>${$(this).find("td:nth-child(8)").text()}</td>
            <td data-name='KYC'>${$(this).find("td:nth-child(9)").text()}</td> 
            <td data-name='Last Tran Date'>${$(this).find("td:nth-child(11)").text()}</td> 
            <td data-name='Approved'>${$(this).find("td:nth-child(12)").text()}</td> 
            <td data-name='Action'><i class="fa fa-trash fs-5 text-danger mb-0 delete"></i></td>             
            </tr>
            `
            clientTable.append(tableRow);

        }
        
    })

    $clientSheetTable.on("dblclick", "tbody tr", function () {
        $(this).toggleClass("selected");
        $selectedClientCount.html($("#client_sheet tbody .selected").length);
    })

    $("#btnSetAsMain").click(function () {
        let sheetRow = $clientSheetTable.find("tbody tr");
        let selectedClient = $clientSheetTable.find("tbody .selected");

        if (sheetRow.length == 0) {
            alert("Please search client");
            return false;
        }

        if (selectedClient.length == 0) {
            alert("Please select client to set main client.");
            return false;
        }

        if (selectedClient.length > 1) {
            alert("The main client can't be more than one.");
            return false;
        }

        let branchName = selectedClient.find("td[data-name='Branch Code']").text()?.trim().toLowerCase();
        if (branchName == 'insurance data import' || branchName == 'mf data import') {
            alert(`Client of ${branchName} Branch Cannot be Set as Main Client`);
            return false;
        }

        let $tbody = $mainSheet.find("tbody");

        if ($tbody.find("tr").length > 0) {
            let isConfirm = confirm("Main Investor Already Selected. Sure to Proceed ?");
            if (!isConfirm) return false;
        }

        
        $tbody.html(selectedClient.clone());
        $tbody.find('tr').removeClass("selected");
        $mainSheetCount.html($tbody.find('tr').length);
        sheetRow.removeClass("selected");
    })

    $("#btnSelectToMerge").click(function () {
        let mainSheetRow = $mainSheet.find("tbody tr");
        let otherSheetRow = $clientSheetTable.find("tbody tr");
        let selectedClient = $clientSheetTable.find("tbody .selected");

        if (otherSheetRow.length == 0) {
            alert("Please search client");
            return false;
        }

        if (selectedClient.length == 0) {
            alert("Please select client");
            return false;
        }

        if (mainSheetRow.length == 0) {
            alert("Please set the main client first.");
            return false;
        }
        let isMain = false,mainClientInfo = {};
        let selectedClientRows = [];
        let kycAccountClients = [];
        let mainClientKyc = mainSheetRow.find("td[data-name='KYC']").text()?.trim()?.toLowerCase();
        let mainClientCode = mainSheetRow.find("td:nth-child(3)").data("clientCode")
        $.each(selectedClient, function (index, row) {

            let clientCode = row.querySelector('td:nth-child(3)').dataset.clientCode;            
            let kyc = $(row).find("td[data-name='KYC']").text()?.trim().toLowerCase();
            let clientName = row.querySelector('td:nth-child(2)').innerText

            if (clientCode == mainClientCode) {
                isMain = true;
                //let clientName = row.querySelector('td:nth-child(2)').innerText
                mainClientInfo.clientCode = clientCode;
                mainClientInfo.clientName = clientName;
            }

            if ((mainClientKyc != 'yes' && mainClientKyc != 'yesp') &&  (kyc == "yes" || kyc == "yesp")) {
                isKyc = true;
                kycAccountClients.push({
                    clientCode: clientCode,
                    clientName: clientName,
                })
            }

            selectedClientRows.push(`<tr>${row.innerHTML}</tr>`);
        })

        if (isMain) {
            alert(`${mainClientInfo.clientName} (${mainClientInfo.clientCode}) is already set as the main client.`);
            return false;
        }

        if (kycAccountClients.length > 0) {
            let clientName = kycAccountClients.map(x => `${x.clientName}`).join(', ')
            alert(`The Client which has KYC Account can not be set as Sub Client. (${clientName})`);
            return false;
        }

        $otherSheet.find("tbody").html(selectedClientRows.join(''));
        $otherSheetCount.html($otherSheet.find("tbody tr").length);
    })

    $("#btnClientMerge").click(function () {
        let data = [];
        let $this = $(this);
        let otherTableRoes = $otherSheet.find("tbody tr");

        let mainSheetRows = $("#main_sheet tbody tr");
        let otherSheetRows = $("#other_sheet tbody tr");

        if (mainSheetRows.length == 0) {
            alert("Please set main client first.");
            return false;
        }

        if (otherSheetRows.length == 0) {
            alert("Please select clients to merge.");
            return false;
        }

        $.each(otherTableRoes, function (index, row) {
            data.push({
                ClientCode: row.querySelector("td:nth-child(3)").dataset.clientCode
            })
        })

        let mainClient = {
            ClientCode: document.querySelector("#main_sheet tbody tr td:nth-child(3)").dataset.clientCode
        }

        $this.removeAttr("disabled", "disabled").html("<i class='fa fa-spinner'></i> Wait..");

        $.ajax({
            url: "/masters/clientmerge.aspx/mergeclient",
            method: "post",
            data: JSON.stringify({ clients: data, mainClient: mainClient }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {

                let { message, data } = JSON.parse(res.d);

                console.log(data);

                if (message.includes("Error")) {
                    alert(message);
                    $this.removeAttr("disabled", "disabled").html("Merge");
                    return false;
                }

                alert(message);

                if (data != null && data.length > 0) {

                    let tableRoes = data.map(x => {
                        return (`<tr>
                        <td>${x.INV_CODE}</td>
                        <td>${x.INVESTOR_NAME}</td>
                        <td>${x.PAN ?? ''}</td>
                        </tr>`)
                    })

                    $("#panUpdateTable tbody").html(tableRoes.join(''));
                    var panModel = new bootstrap.Modal(panUpdateModal, {
                        keyboard: false
                    })

                    panModel.show();

                    $("#client_sheet tbody").html('');
                    $("#main_sheet tbody").html('');
                    $("#other_sheet tbody").html('');
                }

                $this.removeAttr("disabled", "disabled").html("Merge");
            },
            error: function () {
                $this.removeAttr("disabled", "disabled").html("Merge");
            },
        })


    })


    function createTable(data = []) {

        let tableRow = "";
        if (data == null || data.length == 0) {
            tableRow = "<tr><td colspan='9'><p class='mb-0 text-danger'>Data not found</td></tr>";
            return tableRow;
        }

        tableRow = data.map(employee => {
            let creationDate = employee.CREATION_DATE != null ? new Date(employee.CREATION_DATE) : null;
            let cdate = creationDate ? `${creationDate.getDate()}-${creationDate.getMonth() + 1}-${creationDate.getFullYear()}` : "";

            return (`<tr>
            <td data-client-code="${employee.CLIENT_CODE}" title='${employee.CLIENT_NAME}'>${employee.CLIENT_NAME}</td>
            <td title='${(employee.ADDRESS1 == null ? "" : employee.ADDRESS1)}'>${(employee.ADDRESS1 == null ? "" : employee.ADDRESS1)}</td>
            <td title='${(employee.ADDRESS2 == null ? "" : employee.ADDRESS2)}'>${(employee.ADDRESS2 == null ? "" : employee.ADDRESS2)}</td>
            <td title='${employee.CITY_NAME}'>${employee.CITY_NAME}</td>
            <td data-branch-code='${employee.BRANCH_CODE}' title='${employee.BRANCH_NAME}'>${employee.BRANCH_NAME}</td>
            <td title='${employee.PHONE}'>${employee.PHONE}</td>
            <td title='${employee.MOBILE}'>${(employee.KYC == null ? "" : employee.MOBILE)}</td>
            <td data-rm-code='${employee.RM_CODE}' title='${employee.RM_NAME}'>${employee.RM_NAME}</td>
            <td title='${(employee.KYC == null ? "NO" : employee.KYC)}'>${(employee.KYC == null ? "NO" : employee.KYC)}</td>
            <td>${cdate}</td>
            <td>${employee.LAST_TRAN_DT1 ?? ''}</td>
            <td>${employee.APPROVED ?? ''}</td>
            </tr>`)
        })

        return tableRow.join('');
    }

    function resetSearch() {
        $(".client-search-section input:not(#categorySearch)").val('');
        $(".client-search-section select").val('');
        $("#searchTable tbody").html('');
    }

}); // Ready closed here