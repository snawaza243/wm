Private Sub ImExpenses_Change()
   If Val(TxtAmountA.Text) <> 0 Then ImnExp_Per.Text = Round(Val(ImExpenses.Text) / Val(TxtAmountA.Text) * 100, 4)
End Sub

