Private Sub cmdSelectSchm_Click()
If OptChkSwitchM = 0 Then
    frmSchemeSearch.searchschflag = 3
    
Else
    frmSchemeSearch.searchschflag = 4
End If
 call_frmschemesearchM
End Sub

