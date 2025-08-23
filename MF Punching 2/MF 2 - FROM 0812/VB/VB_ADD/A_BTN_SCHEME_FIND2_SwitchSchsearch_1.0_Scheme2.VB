Private Sub CmdSwitchSchsearch_Click()
If cmbTranTypeA.Text = "" Then
    MsgBox "Please Select Transaction Type First"
    cmbTranTypeA.SetFocus
    Exit Sub
End If
frmSchemeSearch.searchschflag = 1
'OptChkSwitch.Value = 1
Call call_frmschemesearch
'OptChkSwitch.Value = 0
End Sub

