Private Sub ChkClose_Click()
If TxtSchemeA.Text <> "" Then
    str1 = Split(TxtSchemeA.Text, "=")
    MySchCode = str1(1)
Else
    MySchCode = " "
End If

If ChkClose.Value = 1 Then
    If CmbSipStpA = "SIP" Or CmbSipStpA = "STP" Or SqlRet("select get_sch_status_close_end('" & MySchCode & "') from dual") = 0 Then
        MsgBox "Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction", vbInformation, "Wealthmaker"
        TxtCloseSch.Text = ""
        ChkClose.Value = 0
        Exit Sub
    End If
Else
    TxtCloseSch.Text = " "
End If

End Sub