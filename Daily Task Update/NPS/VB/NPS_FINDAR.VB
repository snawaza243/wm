Private Sub Picture3_Click()
 'cmdSave.Item(0).Enabled = False
    treeselected = "N"
    Set frmTransactionfind.currentForm = Nothing
    Set frmTransactionfind.currentForm = frmNPS
    frmTransactionfind.Label4.Caption = "PRAN No."
    EPF = False
    frmTransactionfind.Show
    frmTransactionfind.ZOrder
End Sub