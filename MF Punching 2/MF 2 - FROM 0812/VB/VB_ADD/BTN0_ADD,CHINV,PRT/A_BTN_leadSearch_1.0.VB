Private Sub cmdExit_Click(Index As Integer)
If frmtransactionmf.TxtClientCodeA <> "" Then
        Lead_Form = "All"
        Load frm_leadtran_search
        frm_leadtran_search.Show
        'frm_leadtran_search.Visible = True
        'frm_leadtran_search.cmdSearch_Click
    Else
        MsgBox "Please Select Investor First", vbInformation
    End If
End Sub

