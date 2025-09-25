    rsGetDocPath.open "select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
            vApprovalFlag = SqlRet("select wealthmaker.fn_check_for_approval_all('" & txtdocID.Text & "')  from dual")
            'vApprovalFlag 2  Approval request for this transaction has already been raised.
            'vApprovalFlag 4  Approval request for this transaction has rejected by Management.
            
            If vApprovalFlag = 2 Then
                MsgBox "Approval request for this transaction has already been raised.", vbInformation, strBajaj
                Exit Sub
            End If
            
            If vApprovalFlag = 4 Then
                MsgBox "Approval request for this transaction has been rejected by Management.", vbInformation, strBajaj
                Exit Sub
            End If
        
            If Label32.Caption = "" Then
                MsgBox "Information not present", vbInformation, strBajaj
                Exit Sub
            End If
            
            Dim cmd As New ADODB.Command
            Dim vResult As String
            Set cmd.ActiveConnection = MyConn
            cmd.CommandType = adCmdStoredProc
            cmd.CommandText = "WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO"
            cmd.Parameters.Append cmd.CreateParameter("PCOMMON_ID", adVarChar, adParamInput, 15, txtdocID.Text)
            cmd.Parameters.Append cmd.CreateParameter("PSUB_CLIENT_CD", adVarChar, adParamInput, 15, Label32.Caption)
            cmd.Parameters.Append cmd.CreateParameter("PLOGIN_ID", adVarChar, adParamInput, 15, Glbloginid)
            cmd.Parameters.Append cmd.CreateParameter("PCNT", adDouble, adParamOutput, 100)
            cmd.Execute
            vResult = IIf(IsNull(cmd("PCNT")), 0, cmd("PCNT"))
            Set cmd = Nothing
            
            If vResult > 0 Then
                frmCrossChannelValidate.formtype = "MF"
                frmCrossChannelValidate.CommonId = txtdocID.Text
                frmCrossChannelValidate.loggeduserid = Glbloginid
                frmCrossChannelValidate.Show
                frmCrossChannelValidate.ZOrder 0
            Else
                If Label32.Caption <> "" Then
                    Change_bt_Click (0)
                End If
            End If
    Else
        MsgBox "Information not present", vbInformation, strBajaj
    End If