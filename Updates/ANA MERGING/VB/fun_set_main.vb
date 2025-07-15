Private Sub cmdMain_Click()
    If msfgInvestors.Rows >= 2 Then
        If msfgMain.TextMatrix(1, 1) <> "" Then
            If MsgBox("Main Investor Already Selected. Sure to Proceed ?", vbQuestion + vbYesNo) = vbYes Then
                msfgMain.Rows = 1
                msfgMain.Rows = 2
                msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
                msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
                msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
                
                msfgMergedInvestors.Rows = 1
            End If
        Else
            msfgMain.Rows = 1
            msfgMain.Rows = 2
            msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
            msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
            msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
        
        End If
    End If
End Sub