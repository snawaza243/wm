Private Sub ImExpenses1_Change()
If TxtAmountM.Text <> "" Then
   If TxtAmountM.Text <> 0 Then ImnExp_Per1.Text = Round(Val(ImExpenses1.Text) / Val(TxtAmountM.Text) * 100, 4)
End If
End Sub

