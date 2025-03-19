Private Sub cmdApprove_Click()
Dim cmd As New ADODB.Command
Dim UPD_CL_INV As New ADODB.Command
Dim ClCat As String
Dim invCat As String
Dim Str() As String
ApproveFlag = 1
If cmbacatg.Text <> "" Then
    Str = Split(cmbacatg.Text, "#")
    ClCat = Str(1)
End If
If cmbinvestor.Text <> "" Then
    Str = Split(cmbinvestor.Text, "#")
    invCat = Str(1)
End If
If ClCat = "" Then
    MsgBox "Please Select Client category", vbInformation, "Wealthmaker"
    Exit Sub
End If
If invCat = "" Then
    MsgBox "Please Select Investor category", vbInformation, "Wealthmaker"
    Exit Sub
End If
Call cmdUpdate_Click
Set UPD_CL_INV.ActiveConnection = MyConn
UPD_CL_INV.CommandType = adCmdStoredProc
UPD_CL_INV.CommandText = "PRC_UPDATE_CLIENT_INVESTOR"
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("Main_Cd", adVarChar, adParamInput, 50, txtclientcode)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("login_id", adVarChar, adParamInput, 50, Glbloginid)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("client_cat", adVarChar, adParamInput, 50, ClCat)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("inv_Cd", adVarChar, adParamInput, 50, invCat)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("guest_Cd", adVarChar, adParamInput, 50, TguestCode)
UPD_CL_INV.Execute

'MyConn.Execute "update investor_master set approved='YES' where inv_code in (select client_codekyc from client_test where main_code='" & txtclientcode & "')"


MyConn.Execute "update client_test set APPROVED_FLAG='YES' , approved='YES',approve_dt= '" & Format(ServerDateTime, "DD-MMM-YYYY") & "',APPROVE_USERID='" & Glbloginid & "' where client_code=main_code AND main_code='" & txtclientcode & "'"
MyConn.Execute "update CLIENT_master set approved_flag='YES' where client_code in (select substr(client_codekyc,1,8) from client_test where main_code='" & txtclientcode & "')"

'To Approve Grid Investor
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
    '-----------------------------------Checking Duplicate pan------------------------------
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If SqlRet("select count(*) from client_test where client_codekyc <>'" & Trim(VSFCommGrdE.TextMatrix(i, 1)) & "' and upper(client_pan) = upper('" & Trim(VSFCommGrdE.TextMatrix(i, 5)) & "')") > 0 Then
                MsgBox "This Pan No is already Exist"
                Exit Sub
            End If
        End If
     '-------------------------------------------------------------------
        If VSFCommGrdE.TextMatrix(i, 0) <> "" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 15)) = "YES" Then
                MyConn.Execute "update investor_master set approved='YES' where inv_code='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
                MyConn.Execute "update client_test set APPROVED_FLAG='YES' ,approved='YES',approve_dt= '" & Format(ServerDateTime, "DD-MMM-YYYY") & "',APPROVE_USERID='" & Glbloginid & "' where  main_code='" & txtclientcode & "' and client_codekyc='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
            Else
                MyConn.Execute "update client_test set APPROVED_FLAG='NO' ,approved=NULL,approve_dt= NULL,APPROVE_USERID=NULL where  main_code='" & txtclientcode & "' and client_codekyc='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
                MyConn.Execute "update investor_master set approved=null where inv_code='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
            End If
        End If
     Next
End If
MsgBox "Main Account And All The Sub Account Has Been Approved", vbInformation, "Wealthmaker"
Call CMDRESET_Click
End Sub