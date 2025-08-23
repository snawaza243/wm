Private Sub ChkCloseM_Click()
If TxtSchemeM.Text <> "" Then
    str1 = Split(TxtSchemeM.Text, "=")
    MySchCode = str1(1)
Else
    MySchCode = " "
End If

If ChkCloseM.Value = 1 Then
    If CmbSipStpM = "SIP" Or CmbSipStpM = "STP" Or SqlRet("select get_sch_status_close_end('" & MySchCode & "') from dual") = 0 Then
        MsgBox "Auto Switch on Maturity can be selected on Close Ended and NON-SIP transactions", vbInformation, "Wealthmaker"
        ChkCloseM.Value = 0
        TxtCloseSchM.Text = " "
        Exit Sub
    End If
Else
    TxtCloseSchM.Text = " "
End If
End Sub



