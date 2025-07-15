Private Sub cmdMain_Click()
    If msfgInvestors.Rows >= 2 Then
        If msfgInvestors.TextMatrix(msfgInvestors.Row, 6) = "Insurance Data Import" Or msfgInvestors.TextMatrix(msfgInvestors.Row, 6) = "MF Data Import" Then
            MsgBox " Client of " & msfgInvestors.TextMatrix(1, 6) & " Branch Cannot be Set as Main Client ", vbInformation
        Else
            If msfgMain.TextMatrix(1, 1) <> "" Then
                If MsgBox("Main Investor Already Selected. Sure to Proceed ?", vbQuestion + vbYesNo) = vbYes Then
                    msfgMain.Rows = 1
                    msfgMain.Rows = 2
                    msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
                    msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
                    msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
                    msfgMain.TextMatrix(1, 3) = msfgInvestors.TextMatrix(msfgInvestors.Row, 12)
                    
                    msfgMergedInvestors.Rows = 1
                End If
            Else
                msfgMain.Rows = 1
                msfgMain.Rows = 2
                msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
                msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
                msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
                msfgMain.TextMatrix(1, 3) = msfgInvestors.TextMatrix(msfgInvestors.Row, 12)
            End If
        End If
    End If
End Sub