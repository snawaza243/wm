Private Sub cmdview_Click()
CmdView.Enabled = False
FrmTransactionLog.Visible = False
Call gridfill(2)
CmdView.Enabled = True
ResetView_Click
Cmbsubinv.ListIndex = 0
Cmbsubinvu.ListIndex = 0


End Sub

