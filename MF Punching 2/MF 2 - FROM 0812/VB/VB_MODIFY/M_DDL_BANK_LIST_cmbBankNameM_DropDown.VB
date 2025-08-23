Private Sub cmbBankNameM_DropDown()
Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
cmbBankNameM.Clear
While Not rsAmc.EOF
  cmbBankNameM.AddItem rsAmc(0)
  rsAmc.MoveNext
Wend
rsAmc.Close
End Sub


