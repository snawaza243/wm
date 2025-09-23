loadDropdownData(
    'PSM_LOG_RM_LIST',
    'MF_PUNCHING',
    'BRANCH',
    crtBranchCode,
    'RM_NAME',
    'RM_CODE',
    'psmModelInvestorSearch_ddlRM'
);


    function loadDropdownData(psm = null, for_x = null, by_y = null, y = null, get_name = null, get_code = null, targetElementId = null, includeAll = true) {
        $.ajax({
            url: "/masters/mf_punching_interface.aspx/GetDropdownData",
            method: "POST",
            data: JSON.stringify({
                psm: psm,
                for_x: for_x,
                by_y: by_y,
                y: y,
                get_name: get_name,
                get_code: get_code
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (res) {
                let response = JSON.parse(res.d);
                let data = response.data;

                if (includeAll) {
                    data.unshift({ text: "All", value: "" });
                }

                let options = data.map(x => `<option value="${x.value}">${x.text}</option>`);
                $(`#${targetElementId}`).html(options.join(''));
            },
            error: function (xhr, status, error) {
                alert("❌ Error loading dropdown data: " + xhr.responseText);
            }
        });
    }
