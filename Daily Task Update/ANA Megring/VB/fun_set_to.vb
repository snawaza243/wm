Private Sub cmdMergedInvestors_Click()
Dim i As Long
    If msfgMain.TextMatrix(1, 1) <> "" Then
        If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMain.TextMatrix(1, 2) Then
            MsgBox "This Agent is Already Choosen as Main Client", vbInformation
            Exit Sub
        End If
        For i = 1 To msfgMergedInvestors.Rows - 1
            If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMergedInvestors.TextMatrix(i, 2) Then
                MsgBox "This Agent is Already Exist in the List", vbInformation
                Exit Sub
            End If
        Next i
'        If msfgMergedInvestors.Rows > 2 Then
'        End If
        msfgMergedInvestors.Rows = msfgMergedInvestors.Rows + 1
        i = msfgMergedInvestors.Rows - 1
        msfgMergedInvestors.TextMatrix(i, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
        msfgMergedInvestors.TextMatrix(i, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
        msfgMergedInvestors.TextMatrix(i, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
    
    Else
        MsgBox "Please Select Main Investor First !", vbInformation
    End If

End Sub