function populateMfTransactionTable(data, tableElem) {
    const $tbody = $(tableElem).find('tbody');
    $tbody.empty();

    if (!Array.isArray(data) || data.length === 0) {
        alert('No MF Transaction Data Found! 🙁');
        return;
    }

    $.each(data, function (index, row) {
        const tr = $('<tr>');

        tr.append($('<td>').text(row.investor_name || ''));
        tr.append($('<td>').text(row.bank_name || ''));
        tr.append($('<td>').text(row.SCH_CODE || ''));
        tr.append($('<td>').text(row.rm_name || ''));
        tr.append($('<td>').text(row.sip_type || ''));
        tr.append($('<td>').text(row.branch_name || ''));
        tr.append($('<td>').text(row.PANNO || ''));
        tr.append($('<td>').text(row.Amc_Name || ''));
        tr.append($('<td>').text(row.Scheme_Name || ''));
        tr.append($('<td>').text(formatDateToDMY(row.TR_DATE) || ''));
        tr.append($('<td>').text(row.TRAN_TYPE || ''));
        tr.append($('<td>').text(row.App_No || ''));
        tr.append($('<td>').text(row.PAYMENT_MODE || ''));
        tr.append($('<td>').text(row.CHEQUE_NO || ''));
        tr.append($('<td>').text(formatDateToDMY(row.CHEQUE_DATE) || ''));
        tr.append($('<td>').text(row.Amount || ''));
        tr.append($('<td>').text(row.lEAD_nO || ''));
        tr.append($('<td>').text(row.LEAD_NAME || ''));
        tr.append($('<td>').text(row.TRAN_code || ''));
        tr.append($('<td>').text(row.branch_code || ''));
        tr.append($('<td>').text(row.BUSINESS_RMCODE || ''));
        tr.append($('<td>').text(row.frequency || ''));
        tr.append($('<td>').text(row.installments_no || ''));
        tr.append($('<td>').text(row.micro_investment || ''));
        tr.append($('<td>').text(row.target_switch_scheme || ''));

        $tbody.append(tr);
    });
}