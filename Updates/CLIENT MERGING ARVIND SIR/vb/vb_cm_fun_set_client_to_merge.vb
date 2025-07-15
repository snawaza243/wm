Private Sub cmdMergedInvestors_Click()
Dim i As Long
Dim sql As String
    If msfgMain.TextMatrix(1, 1) <> "" Then
        
        If UCase(msfgMain.TextMatrix(1, 3)) <> "YES" And UCase(msfgMain.TextMatrix(1, 3)) <> "YESP" Then
            If UCase(Trim(msfgInvestors.TextMatrix(msfgInvestors.Row, 12))) = UCase("YES") Or UCase(Trim(msfgInvestors.TextMatrix(msfgInvestors.Row, 12))) = UCase("YESP") Then
                MsgBox "The Client which has KYC Account can not be set as Sub Client", vbInformation
                Exit Sub
            End If
        End If
        
'        Sql = " SELECT COUNT(*) FROM CLIENT_MASTER WHERE CLIENT_CODE IN (SELECT SOURCE_CODE FROM TRANSACTION_STTEMP "
'        Sql = Sql & "WHERE SCH_CODE IN (SELECT OSCH_CODE FROM OTHER_PRODUCT WHERE PROD_CLASS_CODE='DT034')) and client_code= '" & msfgInvestors.TextMatrix(msfgInvestors.Row, 9) & "' "
'        If SqlRet(Sql) = 1 Then
'            MsgBox "This Client Can Not Be Merged", vbInformation
'            Exit Sub
'        End If
        
        If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMain.TextMatrix(1, 2) Then
            MsgBox "This Client is Already Choosen as Main Client", vbInformation
            Exit Sub
        End If
        
        For i = 1 To msfgMergedInvestors.Rows - 1
            If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMergedInvestors.TextMatrix(i, 2) Then
                MsgBox "This Client is Already Exist in the List", vbInformation
                Exit Sub
            End If
        Next i
        msfgMergedInvestors.Rows = msfgMergedInvestors.Rows + 1
        i = msfgMergedInvestors.Rows - 1
        msfgMergedInvestors.TextMatrix(i, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
        msfgMergedInvestors.TextMatrix(i, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
        msfgMergedInvestors.TextMatrix(i, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
    Else
        MsgBox "Please Select Main Investor First !", vbInformation
    End If
End Sub