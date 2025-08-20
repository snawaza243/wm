function loadTRListX(x) {
    let branchCat = $('#ddlChannel').val() || '';
    let region = $('#ddlRegion').val() || '';
    let zone = $('#ddlZone').val() || '';
    let branch = $('#ddlBranch').val() || '';
    let rm = $('#ddlRM').val() || '';
    let amc = $('#ddlAMC').val() || '';
    let tranType = $('input[name="tranType"]:checked').val() || '';
    let registrar = $('input[name="registrar"]:checked').val() || '';
    let statusType = $('input[name="reco"]:checked').val() || '';
    let pms = $('#chkPMS').is(':checked') ? 'Y' : 'N';
    let cob = $('#chkTrCOB').is(':checked') ? 'Y' : 'N';
    let dateFrom = $('#txtDateFrom').val() || '';
    let dateTo = $('#txtDateTo').val() || '';
    let arNum = ($('#txtARNo').val() || '').trim();
    let chequeType = $('#ddlChequeType').val() || '';
    let chequeSearch = ($('#txtChequeSearch').val() || '').trim();
    let investorName = ($('#txtInvestorName').val() || '').trim();
    let amount = ($('#txtAmount').val() || '').trim();
    let sipFolioNo = ($('#txtSIPFolioNo').val() || '').trim();
    let sipAmount = ($('#txtSIPAmount').val() || '').trim();
    let sipPan = ($('#txtSIPPan').val() || '').trim();
    let sipClientCode = ($('#txtSIPClientCode').val() || '').trim();
    let sipDate = $('#txtSIPDate').val() || '';

    // Handle different scenarios based on x value
    if (x == '1') {
        // For RTA (x=1) - override values with RTA-specific controls
        dateFrom = $('#txtRtaDateFrom').val() || '';
        dateTo = $('#txtRtaDateTo').val() || '';
        amc = $('#ddlRtaAMC').val() || '';
        branch = $('#ddlRtaBranch').val() || '';
        statusType = $('input[name="rtaRecoStatus"]:checked').val() || '';
        chequeType = $('#ddlRtaCheque').val() || '';
        chequeSearch = ($('#txtRtaChequeNo').val() || '').trim();
        investorName = ($('#txtRtaInvestorName').val() || '').trim();
        amount = ($('#txtRtaAmount').val() || '').trim();
        tranType = $('input[name="rtaTranType"]:checked').val() || '';
        
    }

    if (x == '2') {
        // For TR (x=2) - if AR number is provided, clear dates
        if (arNum) {
            dateFrom = '';
            dateTo = '';
        }
        
        // Validation for TR
        if (!arNum && (!dateFrom || !dateTo)) {
            alert("⚠️ Please enter AR No or both From Date and To Date.");
            return;
        }
    }

    // For SIP (x=3 or other values) - add additional validations if needed
    if (x == '3') {
        // SIP-specific validations can go here
        if (!sipFolioNo && !sipPan && !sipClientCode && !sipDate) {
            alert("⚠️ Please enter at least one SIP search criteria (Folio, PAN, Client Code, or Date).");
            return;
        }
    }

    // Debug alert (optional)
    alert(`Transaction Filter Values:\n\n` +
        `X: ${x}\n` + `Branch Category: ${branchCat}\n` + `Region: ${region}\n` +
        `Zone: ${zone}\n` + `Branch: ${branch}\n` + `RM: ${rm}\n` +
        `AMC: ${amc}\n` + `Transaction Type: ${tranType}\n` + `Registrar: ${registrar}\n` +
        `Status Type: ${statusType}\n` + `PMS: ${pms}\n` + `COB: ${cob}\n` +
        `Date From: ${dateFrom}\n` + `Date To: ${dateTo}\n` + `AR Number: ${arNum}\n` +
        `Cheque Type: ${chequeType}\n` + `Cheque Search: ${chequeSearch}\n` +
        `Investor Name: ${investorName}\n` + `Amount: ${amount}\n` +
        `SIP Folio No: ${sipFolioNo}\n` + `SIP Amount: ${sipAmount}\n` +
        `SIP PAN: ${sipPan}\n` + `SIP Client Code: ${sipClientCode}\n` +
        `SIP Date: ${sipDate}\n` + `Log ID: ${logId}\n` + `Role ID: ${roleId}`
    );

    $.ajax({
        url: "/masters/sip_master1.aspx/GetTRListX",
        method: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({
            x: x,
            branchCat: branchCat,
            region: region,
            zone: zone,
            branch: branch,
            rm: rm,
            amc: amc,
            tranType: tranType,
            registrar: registrar,
            statusType: statusType,
            pms: pms,
            cob: cob,
            dateFrom: dateFrom,
            dateTo: dateTo,
            arNum: arNum,
            chequeType: chequeType,
            chequeSearch: chequeSearch,
            investorName: investorName,
            amount: amount,
            sipFolioNo: sipFolioNo,
            sipAmount: sipAmount,
            sipPan: sipPan,
            sipClientCode: sipClientCode,
            sipDate: sipDate,
            logId: logId,
            roleId: roleId
        }),
        success: function (res) {
            let { data } = JSON.parse(res.d);

            if (!data || data.length === 0) {
                alert('No records found');
                return;
            } else {
                alert(data.length + ' Record(s) found, for x = ' + x);
                return;
            }
        },
        error: function (xhr, status, error) {                        
            alert("Error: " + xhr.responseText);
        }
    });
}