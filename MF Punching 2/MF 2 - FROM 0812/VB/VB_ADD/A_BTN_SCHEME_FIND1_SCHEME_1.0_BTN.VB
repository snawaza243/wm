Private Sub cmdSelectSch_Click()
If cmbTranTypeA.Text = "" Then
    MsgBox "Please Select Transaction Type First"
    cmbTranTypeA.SetFocus
    Exit Sub
End If
  frmSchemeSearch.searchschflag = 0
  call_frmschemesearch
End Sub

