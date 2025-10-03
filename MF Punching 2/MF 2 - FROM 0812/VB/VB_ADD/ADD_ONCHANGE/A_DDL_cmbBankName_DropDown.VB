Private Sub cmbBankName_DropDown()
Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
cmbBankName.Clear
While Not rsAmc.EOF
  cmbBankName.AddItem rsAmc(0)
  rsAmc.MoveNext
Wend
rsAmc.Close
End Sub

