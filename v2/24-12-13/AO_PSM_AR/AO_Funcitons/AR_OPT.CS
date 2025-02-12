Private Sub optcash_Click()
cmbBankName.Clear
cmbBankName.AddItem "Cash"
cmbBankName.Enabled = False
txtChqNo.Enabled = False
dtChqDate.Enabled = False
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub

Private Sub optcheque_Click()
Label111.Caption = "Cheque No."
Label222.Caption = "Cheque Dated."
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub


Private Sub optdraft_Click()
Label111.Caption = "Draft No."
Label222.Caption = "Draft Dated."
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub