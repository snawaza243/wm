/*

        'SUCCESS:' AS MSG,
        V_LOG_USER_ID AS V_LOG_USER_ID,
        V_MAIN_CODE AS V_MAIN_CODE,
        V_UPD_PROC AS V_UPD_PROC,
        V_MFA_INV_CODE AS V_MFA_INV_CODE,
        V_MFA_CLIENT_CODE AS V_MFA_CLIENT_CODE,
        V_MFA_INVESTOR_NAME AS V_MFA_INVESTOR_NAME,
        V_MFA_PAN AS V_MFA_PAN,
        V_MFA_AH_CODE AS V_MFA_AH_CODE,
        V_MFA_BUSI_CODE AS V_MFA_BUSI_CODE,
        V_MFM_INV_CODE AS V_MFM_INV_CODE,
        V_MFM_CLIENT_CODE AS V_MFM_CLIENT_CODE,
        V_MFM_INVESTOR_NAME AS V_MFM_INVESTOR_NAME,
        V_MFM_PAN AS V_MFM_PAN,
        V_MFM_AH_CODE AS V_MFM_AH_CODE,
        V_MFM_BUSI_CODE AS V_MFM_BUSI_CODE,
        V_MF_LABEL42 AS V_MF_LABEL42,
        PX_INV_CODE AS PX_INV_CODE,
        PX_INDEX AS PX_INDEX,
        PX_CUR_FORM AS PX_CUR_FORM
*/ 

const inv_data = {
    msg             : MSG,
    log_user        : V_LOG_USER_ID,
    main_code       : V_MAIN_CODE,
    upd_proc        : V_UPD_PROC,
    add_inv_code    : V_MFA_INV_CODE,
    add_client_code : V_MFA_CLIENT_CODE,
    add_inv_name    : V_MFA_INVESTOR_NAME,
    add_pan         : V_MFA_PAN,
    add_ah_code     : V_MFA_AH_CODE,
    add_busi_code   : V_MFA_BUSI_CODE,
    mod_inv_code    : V_MFM_INV_CODE,
    mod_clinet_code : V_MFM_CLIENT_CODE,
    mod_inv_name    : V_MFM_INVESTOR_NAME,
    mod_pan         : V_MFM_PAN,
    mod_ad_code     : V_MFM_AH_CODE,
    mod_busi_code   : V_MFM_BUSI_CODE,
    mod_inv_code    : V_MF_LABEL42,
    px_inv_code     : PX_INV_CODE,
    px_index        : PX_INDEX,
    px_form         : PX_CUR_FORM

}


 function setAddSetByInvAfterDT(data = null) {
     if (data) {
        //$("#txtAddTrDate").val(formatDateToDMY(data.VA_IM_ENTRY_DT) || '');
        //$("#txtAddBssCode").val(data.V_MFA_BUSI_CODE || '');
        $("#txtAddInvCode").val(data.V_MFA_INV_CODE || '');
        $("#txtAddAHName").val(data.V_MFA_INVESTOR_NAME || '');
        $("#txtAddAHCode").val(data.V_MAIN_CODE || '');
        //$("#lblAddExpPer").html(data.VA_EXPENSE_PER || '');
        //$("#lblAddExpRs").html(data.VA_EXPENSE_RS || '');
        //$("#txtAddExpPer").val(data.VA_IM_N_EXP_PER || '');
        //$("#ddlAddBranch").val(data.VA_BUSI_BRANCH_CODE || '');
        //$("#ddlAddAMC").val(data.VA_AMC_CODE || '');
        //$("#txtAddScheme1").val(data.VA_SCH_NAME || '');
        //$("#hdnAddScheme1").val(data.VA_SCH_CODE || '');
        //$("#txtAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH || '');
        //$("#hdnAddScheme2_fromSwitch").val(data.VA_SWITCH_SCH_CODE || '');
        $("#hdnAddClientCode").val(data.V_MFA_CLIENT_CODE || '');
        $("#hdnAddPan1").val(data.V_MFA_PAN || '');
        $("#txtAddTrDate").prop('disabled', false);
        $("#ddlAddAMC").prop('disabled', false);
        $("#txtAddScheme1").prop('disabled', false);

         //alert(ReturnMessage);
        var retMsg = (data.V_RETURN_MESSAGE || '');
        var curDT = $('#txtAddDtNumber').val();
        var curBsi = $("#txtAddBssCode").val();
        var curInv = $("#txtAddInvCode").val();

        inv_add_up_load_data(curInv);
        $("#psmModalInvestorAddUpdate").modal("show");
        //BusiCodeLostFocusA(curBsi, curInv);

    } else {
        /*
        $("#txtAddTrDate").val('');
        $("#txtAddBssCode").val('');
        $("#txtAddInvCode").val('');
        $("#txtAddAHName").val('');
        $("#txtAddAHCode").val('');
        $("#lblAddExpPer").html('');
        $("#lblAddExpRs").html('');
        $("#txtAddExpPer").val('');
        $("#ddlAddBranch").val('');
        $("#ddlAddAMC").val('');
        $("#txtAddScheme1").val('');
        $("#hdnAddScheme1").val('');
        $("#txtAddScheme2_fromSwitch").val('');
        $("#hdnAddScheme2_fromSwitch").val('');
        $("#hdnAddClientCode").val('');
        $("#hdnAddPan1").val('');
        $("#txtAddTrDate").prop('disabled', true);
        $("#ddlAddAMC").prop('disabled', true);
        $("#txtAddScheme1").prop('disabled', true);
        populateBusiCodeDataAdd(null);
         */
            alert('No record found for this Investor');
    }
}
