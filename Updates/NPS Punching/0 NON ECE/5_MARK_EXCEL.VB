Private Sub MarkinExcel(AppNo As String)
Set XLWM = XL.Workbooks.open(filepath)
Dim j As Integer
j = 1
Dim MyCol As Integer
Dim MyColApp As Integer
MyCol = 0
MyColApp = 0
While XLWM.Worksheets(sheetName).Cells(1, j) <> ""
    If UCase(Trim(XLWM.Worksheets(sheetName).Cells(1, j))) = UCase("Remark") Then
        MyCol = j
    End If
    If UCase(Trim(XLWM.Worksheets(sheetName).Cells(1, j))) = UCase("consumer_code") Then
        MyColApp = j
    End If
    j = j + 1
Wend
i = 2
If AppNo <> "" Then
While XLWM.Worksheets(sheetName).Cells(i, 1) <> ""
    If Trim(XLWM.Worksheets(sheetName).Cells(i, MyColApp)) = AppNo Then
        XLWM.Worksheets(sheetName).Cells(i, MyCol) = "Not Inserted"
    End If
    i = i + 1
Wend
End If
XLWM.save
XLWM.Close
Set XLWM = Nothing
XL.Quit
Set XL = Nothing
Set XLW = XL.Workbooks.open(filepath)
XLW.Close
Set XLW = Nothing
'XL.Quit
'Set XL = Nothing
End Sub