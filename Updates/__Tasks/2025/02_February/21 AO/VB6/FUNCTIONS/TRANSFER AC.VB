Private Sub cmdTransfer_Click()
On Error GoTo err1
    
Dim flag As Boolean
Dim rsEmp As ADODB.Recordset
Dim b_cd() As String

    
    If Len(txtbusicode.Text) <> 0 And Trim(txtbusicode.Text) <> "95829" Then
        MyBranch = ""
        MyRmCode = ""
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            MsgBox "RM Should Be Sale Support and Marketing "
            Label32.Caption = ""
            Label55.Caption = ""
            txtbusicode = ""
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
        
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "select client_codekyc,main_code,business_code from client_test where client_code='" & Trim(txtclientcode.Text) & "'", MyConn, adOpenForwardOnly
        If Not rsEmp.EOF Then
            
            If rsEmp("BUSINESS_CODE") <> "95829" Then
                'MsgBox "Account Already Transferred", vbCritical
                rsEmp.Close
                Set rsEmp = Nothing
                Exit Sub
            End If
            
            MyConn.BeginTrans
            flag = True
            ''Account transfer log
            MyConn.Execute "insert into Account_transfer_log(CLIENT_CODE,OLD_BUSINESS_RM,NEW_BUSINESS_RM,LOGGEDUSERID) values('" & rsEmp("main_code") & "','" & rsEmp("business_code") & "','" & Trim(txtbusicode.Text) & "','" & Glbloginid & "')"
            '''''''''''''
            
            ''''''''''''''''''''''''''''''''''''''''
            MyConn.Execute "Update Client_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), rm_code=" & MyRmCode & ",Sourceid=" & MyBranch & " where Client_code=" & Left(rsEmp("client_codekyc"), 8)
            MyConn.Execute "Update Client_master@mf.bajajcapital set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), rm_code=" & MyRmCode & ",Sourceid=" & MyBranch & " where Client_code=" & Left(rsEmp("client_codekyc"), 8)
            MyConn.Execute "Update Investor_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),rm_code=" & MyRmCode & ",Branch_code=" & MyBranch & " where inv_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "Update Investor_master@mf.bajajcapital set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),rm_code=" & MyRmCode & ",Branch_code=" & MyBranch & " where inv_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "Update CLIENT_TEST set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), BUSINESS_CODE=(SELECT PAYROLL_ID FROM EMPLOYEE_MASTER WHERE RM_CODE=" & MyRmCode & "),BRANCH_CODE=" & MyBranch & " where main_code='" & rsEmp("main_code") & "'"
    
    
            MyConn.Execute "UPDATE TRANSACTION_ST SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE TRANSACTION_ST@mf.bajajcapital SET RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE port_TRANSACTION_ST@mf.bajajcapital SET RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE TRANSACTION_STTEMP SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE transaction_mf_temp1 SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            ''''''''''''''''''''''''''''''''''''''''
            MyConn.CommitTrans
            flag = False
        Else
            'MsgBox "Account Not Found", vbCritical
        End If
        
        rsEmp.Close
        Set rsEmp = Nothing
    End If
    Exit Sub
err1:
    MsgBox err.Description
    If flag = True Then MyConn.RollbackTrans
End Sub