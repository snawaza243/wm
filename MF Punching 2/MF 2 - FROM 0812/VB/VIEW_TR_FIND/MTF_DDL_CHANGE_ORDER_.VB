Private Sub CmbOrder_Click()
MyOrder = ""
If CmbOrder.ListIndex = 1 Then
   MyOrder = "Rm_Name"
ElseIf CmbOrder.ListIndex = 2 Then
    MyOrder = "Branch_Name"
ElseIf CmbOrder.ListIndex = 3 Then
    MyOrder = "Amc_Name"
ElseIf CmbOrder.ListIndex = 4 Then
    MyOrder = "Scheme_Name"
ElseIf CmbOrder.ListIndex = 5 Then
    MyOrder = "PAYMENT_MODE"
ElseIf CmbOrder.ListIndex = 6 Then
    MyOrder = "TR_DATE"
End If
End Sub

