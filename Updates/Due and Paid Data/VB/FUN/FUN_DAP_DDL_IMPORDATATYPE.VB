Private Sub CmbDataType_Click()
MyImportDataType = ""
MyImport = ""
'Due    0
'Lapsed 1
'Paid   2
'Reins  3
If CmbDataType.ListIndex = 0 Then 'DUE
    MyImportDataType = "DUEDATA"
    MyImport = "D"
    OptDue.Value = True
    OptPaid.Caption = "Paid Data"
    OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 1 Then 'LAPSED
    MyImportDataType = "LAPSEDDATA"
    MyImport = "L"
    OptDue.Value = True
    OptPaid.Caption = "Reinstate Data"
    OptDue.Caption = "Lappsed Data"
ElseIf CmbDataType.ListIndex = 2 Then 'PAID
    MyImportDataType = "DUEDATA"
    MyImport = "D"
    OptPaid.Value = True
    OptPaid.Caption = "Paid Data"
    OptDue.Caption = "Due Data"
ElseIf CmbDataType.ListIndex = 3 Then 'REINS
    MyImportDataType = "LAPSEDDATA"
    MyImport = "L"
    OptPaid.Value = True
    OptPaid.Caption = "Reinstate Data"
    OptDue.Caption = "Lappsed Data"
End If
End Sub