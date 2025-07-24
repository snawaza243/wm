//javascript used only for insvestor merge controls
$(document).ready(function () {
    $("#ContentPlaceHolder1_ddlBranch").change(function () {
        let branchCode = $(this).val();
        if (branchCode) {
            $.ajax({
                url: "/masters/investormerge.aspx/GetRMData",
                method: "post",
                data: JSON.stringify({ code: branchCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    let text = data.length > 0 ? "Select RM" : "RM Not Found";
                    data.unshift({ text: text, value: "" })
                    let rmOptions = data.map(r => `<option value="${r.value}">${r.text}</option>`)
                    $("#ContentPlaceHolder1_ddlRM").html(rmOptions.join(''));
                },
                error: function () {

                },
            })
        }
    });

    $("#ContentPlaceHolder1_ddlRM").change(function () {
        let rmCode = $(this).val();
        if (rmCode) {
            $.ajax({
                url: "/masters/investormerge.aspx/GetClientList",
                method: "post",
                data: JSON.stringify({ code: rmCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    let text = data.length > 0 ? "Select Client" : "Client Not Found";
                    data.unshift({ text: text, value: "" })
                    let clientOptions = data.map(r => `<option value="${r.value}">${r.text}</option>`)
                    $("#ContentPlaceHolder1_ddlClient").html(clientOptions.join(''));
                },
                error: function () {

                },
            })
        }
    });

    $("#ContentPlaceHolder1_ddlClient").change(function () {
        let clientCode = $(this).val();
        if (clientCode) {

            $.ajax({
                url: "/masters/investormerge.aspx/GetInvestorForMerge",
                method: "post",
                data: JSON.stringify({ code: clientCode }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let { data } = JSON.parse(res.d);
                    if (data == null || data.length == 0) {
                        let rmOptions = "<tr><td colspan='4'><p class='mb-0 text-danger'>Investors not found</td></tr>";
                        $(".table-investor-selection tbody").html(rmOptions);
                    }
                    else {
                        let rmOptions = data.map(investor => `<tr>
                            <td data-client-code="${investor.TOTALCOUNT}" title='${investor.TOTALCOUNT}'>${investor.TOTALCOUNT}</td>
                            <td data-client-code="${investor.INVESTOR_NAME}" title='${investor.INVESTOR_NAME}'>${investor.INVESTOR_NAME}</td>
                            <td data-client-code="${investor.INV_CODE}" title='${investor.INV_CODE}'>${investor.INV_CODE}</td>                            
                            <td title='${(investor.ADDRESS1 == null ? "" : investor.ADDRESS1)}'>${(investor.ADDRESS1 == null ? "" : investor.ADDRESS1)}</td>                            
                            <td title='${(investor.ADDRESS2 == null ? "" : investor.ADDRESS2)}'>${(investor.ADDRESS2 == null ? "" : investor.ADDRESS2)}</td>
                            <td title='${(investor.CITY_NAME == null ? "" : investor.CITY_NAME)}'>${(investor.CITY_NAME == null ? "" : investor.CITY_NAME)}</td>
                            <td title='${(investor.PINCODE == null ? "" : investor.PINCODE)}'>${(investor.PINCODE == null ? "" : investor.PINCODE)}</td>
                            <td title='${(investor.KYC == null ? "" : investor.KYC)}'>${(investor.KYC == null ? "" : investor.KYC)}</td>

                            </tr>`)
                        $(".table-investor-selection tbody").html(rmOptions.join(''));
                    }

                },
                error: function () {

                },
            })
        }
    });


    
    $("#investor_selection").on("dblclick", "tbody tr", function () {
        $(this).toggleClass("selected");
    })

    $("#merging_investor").on("dblclick", "tbody tr", function () {
        $(this).toggleClass("selected");
        let mainSheetRow = $("#merging_investor").find("tbody tr");
        let selectedInvestor = $("#merging_investor").find("tbody .selected");

        // Delete selected rows
        selectedInvestor.each(function () {
            $(this).remove(); // Remove the selected row from the DOM
        });

        alert("Selected client(s) have been removed.");
    })

    $("#btnSetAsMain").click(function () {
        let selectedInvestor = $("#investor_selection").find("tbody .selected");
    
        if (selectedInvestor.length == 0) {
            alert("Please select investor to set main investor.");
            return false;
        }

        if (selectedInvestor.length > 1) {
            alert("The main investor can't be more than one.");
            return false;
        }

        let $tbody = $("#main_investor").find("tbody");
        $tbody.html(selectedInvestor.clone());
        $tbody.find('tr').removeClass("selected");
        sheetRow.removeClass("selected");
    })

    $("#btnSelectToMerge").click(function () {
        let mainSheetRow = $("#main_investor").find("tbody tr");
        let selectedInvestor = $("#investor_selection").find("tbody .selected");

        if (selectedInvestor.length == 0) {
            alert("Please select client");
            return false;
        }

        if (mainSheetRow.length == 0) {
            alert("Please set the main client first.");
            return false;
        }
        let isMain = false, mainClientInfo = {};
        let selectedClientRows = [];
        $.each(selectedInvestor, function (index, row) {

            let clientCode = row.querySelector('td:nth-child(2)').dataset.clientCode;
            if (clientCode == mainSheetRow.find("td:nth-child(2)").data("clientCode")) {
                isMain = true;
                let clientName = row.querySelector('td:nth-child(2)').innerText
                mainClientInfo.clientCode = clientCode;
                mainClientInfo.clientName = clientName;
            }

            selectedClientRows.push(`<tr>${row.innerHTML}</tr>`);
        })

        //if (isMain) {
        //    alert(`${mainClientInfo.clientName} is already set as the main investor.`);
        //    return false;
        //}

        $("#merging_investor").find("tbody").html(selectedClientRows.join(''));
    })

    $("#btnInvestorMerge").click(function () {      

        //checking if any investor present to merge
        hasRecordToMerge = true;
        $("#investor_selection tbody tr").each(function () {
            // Get the value of the first cell (td) in the row
            let invCode = $(this).find("td").eq(2).text(); // 0 for the first column          
            if (invCode == "") {
                alert("No Record To Merge.");
                hasRecordToMerge = false;
                return false;
            }
                      
        });

        // Check if we found any valid records
        if (!hasRecordToMerge) {
            return; // Exit the function if no records were found
        }

        $("#merging_investor tbody tr").each(function () {
            // Get the value of the first cell (td) in the row
            let invCode = $(this).find("td").eq(2).text(); // 0 for the first column          
            if (invCode == "") {
                alert("No Record To Merge.");
                hasRecordToMerge = false;
                return false;
            }

        });

        // Check if we found any valid records
        if (!hasRecordToMerge) {
            return; // Exit the function if no records were found
        }

        let invCodeForMerge= []; // Array to hold the names
        // Iterate over each row in the tbody
        let invCodeMain;
        $("#merging_investor tbody tr").each(function () {
            // Get the value of the first cell (td) in the row
            let invCode = $(this).find("td").eq(2).text(); // 0 for the first column          
                invCodeForMerge.push(invCode); // Add to the names array           
        });
        
        const separatedStringForMerge = invCodeForMerge.join('#');
        console.log(separatedStringForMerge);

        
        $("#main_investor tbody tr").each(function () {
            // Get the value of the first cell (td) in the row
             invCodeMain = $(this).find("td").eq(2).text(); // 0 for the first column
            console.log(invCodeMain);
        });

        // checking if main investor and merge investor is same

        // Check if invCodeMain is present in the separatedStringForMerge
        if (separatedStringForMerge.split('#').includes(invCodeMain)) {
            alert("Main And Merged Code Can't Be Same.");
            return; // Exit the function
        }

        $('#blockUI').fadeIn();
        $.ajax({
            url: "/masters/investormerge.aspx/UpdateInvestorMerge",
            method: "post",
            data: JSON.stringify({ Mcode: invCodeMain, MergedCode: separatedStringForMerge }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                debugger;
                let { data } = JSON.parse(res.d);
                if (data == "SUCCESS") {
                    alert("Investor Merged Successfully.");
                    $('#blockUI').fadeOut();
                    window.location.href = 'investorMerge.aspx';
                }
                else {
                    alert(data);
                    $('#blockUI').fadeOut();
                    window.location.href = 'investorMerge.aspx';
                }

            },
            error: function () {

            },
        })
       
    })
   
})