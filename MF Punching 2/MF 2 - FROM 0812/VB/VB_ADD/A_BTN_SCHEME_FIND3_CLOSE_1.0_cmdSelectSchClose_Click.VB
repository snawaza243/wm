Private Sub cmdSelectSchClose_Click()
If ChkClose.Value = 1 Then
    frmSchemeSearch.searchschflag = 2
    Call call_frmschemesearch
End If
End Sub

