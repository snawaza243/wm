Option Explicit
Dim importExcelcon As New ADODB.Connection
Public sheetName As String
Public filepath As String
Dim XL As New Excel.Application
Dim RsImport As New ADODB.Recordset
Dim XLW As Excel.Workbook
Dim i As Integer
Dim Split_GenFld() As String
Dim MappedField As String
Dim Count_Loop As Integer
Dim Filename As String
Dim FieldMap(10) As String
Dim sql As String

'Dim Count As Integer
Private Sub CmdBrowse_Click()
TxtFileName.Text = ""
Cdilog.Filename = ""
Cdilog.Filter = "Excel Files|*.xls"
Cdilog.ShowOpen
TxtFileName.Text = Cdilog.Filename
lstsheet.Clear
If TxtFileName.Text <> "" Then
    Set XLW = XL.Workbooks.open(TxtFileName.Text)
    lstsheet.Clear
    For Count_Loop = 1 To XLW.Worksheets.count
        lstsheet.AddItem XLW.Worksheets(Count_Loop).Name
    Next
    XLW.Close
    Set XLW = Nothing
    XL.Quit
    Set XL = Nothing
End If
Filename = Cdilog.Filename
End Sub

Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub cmdImport_Click()
Dim query, branch As String
Dim RS As New ADODB.Recordset
Dim REF_NO, tot, Not_Imp As Integer
Dim flag As Boolean
'On Error GoTo err
Set XLW = XL.Workbooks.open(Filename)
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & Filename & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set RsImport = New ADODB.Recordset
Set RsImport = importExcelcon.Execute("Select * from [" & lstsheet.Text & "$] ")
MyConn.Execute "delete from POLICY_MAP_TEMP1"
If RS.State = 1 Then RS.Close
RS.open "select * from POLICY_MAP_TEMP1", MyConn, adOpenDynamic, adLockOptimistic
While RsImport.EOF = False
    tot = tot + 1
    If Trim(RsImport(0)) <> "" Then
        RS.AddNew
        RS(0) = Replace(Trim(RsImport(0)), "-", "")
        If Lst_Exc_Fld.ListCount > 1 Then
           RS(1) = Trim(RsImport(1))
        End If
        RS.Update
    End If
    RsImport.MoveNext
Label3.Caption = tot
DoEvents
Wend
If RS.State = 1 Then RS.Close

sql = "SELECT DISTINCT(P.POLICY_NO) AS POLICY_NO,max(a.PREM_AMT) max_amt,decode(max(a.PREM_FREQ),1,'Y',2,'HY',4,'Q',12,'M') PREM_FREQ,max(a.NEXT_DUE_DT) NEXT_DUE_DT,MAX(A.COMPANY_CD) COMPANY_CD,R.REGION_NAME REGION_NAME,Z.ZONE_NAME ZONE_NAME, E.RM_NAME RM_NAME, B.BRANCH_NAME BRANCH_NAME,I.INVESTOR_NAME INVESTOR_NAME, I.ADDRESS1 ADDRESS1, I.ADDRESS2 ADDRESS2,C.CITY_NAME CITY_NAME,S.STATE_NAME STATE_NAME ,I.MOBILE MOBILE, I.PHONE PHONE from branch_master b,zone_master z,POLICY_MAP_TEMP1 p,bajaj_ar_head a,investor_master i,employee_master e,region_master r,STATE_MASTER S, CITY_MASTER C Where a.FRESH_RENEWAL in ('1','5') and I.CITY_ID=C.CITY_ID AND C.STATE_ID=S.STATE_ID AND e.payroll_id=to_char(a.emp_no) and TRIMZERO(upper(Trim(p.POLICY_NO))) = upper(trim(a.POLICY_NO)) "
sql = "SELECT DISTINCT(P.POLICY_NO) AS POLICY_NO,max(a.PREM_AMT) max_amt,decode(max(a.PREM_FREQ),1,'Y',2,'HY',4,'Q',12,'M') PREM_FREQ,max(a.NEXT_DUE_DT) NEXT_DUE_DT,MAX(A.COMPANY_CD) COMPANY_CD,R.REGION_NAME REGION_NAME,Z.ZONE_NAME ZONE_NAME, E.RM_NAME RM_NAME, B.BRANCH_NAME BRANCH_NAME,I.INVESTOR_NAME INVESTOR_NAME, I.ADDRESS1 ADDRESS1, I.ADDRESS2 ADDRESS2,C.CITY_NAME CITY_NAME,S.STATE_NAME STATE_NAME ,I.MOBILE MOBILE, I.PHONE PHONE from branch_master b,zone_master z,POLICY_MAP_TEMP1 p,bajaj_ar_head a,investor_master i,employee_master e,region_master r,STATE_MASTER S, CITY_MASTER C Where I.CITY_ID=C.CITY_ID AND C.STATE_ID=S.STATE_ID AND e.RM_CODE=I.RM_CODE and TRIMZERO(upper(Trim(p.POLICY_NO))) = TRIMZERO(upper(trim(a.POLICY_NO))) "
If Lst_Exc_Fld.ListCount > 1 Then
   sql = sql & " and Trim(UPPER(p.COMPANY_CD)) = upper(Trim(a.COMPANY_CD))"
End If
If CmbCompany.Text <> "" Then
    sql = sql & " And a.company_CD ='" & Mid(CmbCompany.Text, 81) & "' "
End If
sql = sql & " And a.CLIENT_CD = i.inv_Code And I.BRANCH_CODE = B.BRANCH_CODE and  b.region_id=r.region_id and b.zone_id=z.zone_id   group by p.policy_no,p.company_cd,b.BRANCH_NAME,r.region_name,z.zone_name,i.INVESTOR_NAME,i.address1,i.address2,i.mobile,i.phone,e.rm_name,C.CITY_NAME,S.STATE_NAME"
RS.open sql, MyConn
If RS.EOF = False Then
    If Dir(App.Path & "\Reports\policymap.xls") <> "" Then
        Kill App.Path & "\Reports\policymap.xls"
        RS.save App.Path & "\Reports\policymap.xls", adPersistXML
    Else
        RS.save App.Path & "\Reports\policymap.xls", adPersistXML
    End If
    XLW.Close
    Set XLW = Nothing
    RsImport.Close
    importExcelcon.Close
    Set RsImport = Nothing
    Dim xx As Long
    xx = ShellExecute(Me.hwnd, "Open", App.Path & "\Reports\policymap.xls", 0&, 0&, 1)
Else
    MsgBox "No Record found against policy no."
End If


End Sub

Private Sub Form_Load()
Dim RS As New ADODB.Recordset
Dim query As String
query = "select company_name,company_cd from bajaj_company_master order by company_name"
RS.open query, MyConn
CmbCompany.Clear
While RS.EOF = False
    CmbCompany.AddItem (RS(0) & Space(80 - Len(RS(0))) & RS(1))
    RS.MoveNext
Wend
RS.Close
End Sub

Private Sub lstsheet_Click()
On Error GoTo err1
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & Filename & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set RsImport = New ADODB.Recordset
Set RsImport = importExcelcon.Execute("Select * from [" & lstsheet.Text & "$] ")
If RsImport.Fields.count > 0 Then
   'FileFields = ""
   Lst_Exc_Fld.Clear
   'Lst_Map_Fld.Clear
   For i = 0 To RsImport.Fields.count - 1
      Lst_Exc_Fld.AddItem RsImport.Fields(i).Name
   Next
   'FileFields = Left(FileFields, Len(FileFields) - 1)
End If
importExcelcon.Close
'XLW.Close
'Set XLW = Nothing
'XL.Quit
'Set XL = Nothing
RsImport.Close
Set RsImport = Nothing
Exit Sub
err1:
   If err.Number = 62 Then
    'GoTo next_Stat
   End If
End Sub
Private Sub pic_add_fld_Click()
Dim ExcelFiled As String
Dim BAckField As String
If Lst_Exc_Fld.SelCount = 0 Then
    MsgBox "Please Select Excel Field Name", vbInformation
    Exit Sub
End If

'If Lst_Back_Fld.SelCount = 0 Then
'    MsgBox "Please Select BackOffice Field Name", vbInformation
'    Exit Sub
'End If
For i = 0 To Lst_Exc_Fld.ListCount - 1
        If Lst_Exc_Fld.Selected(i) = True Then
            ExcelFiled = Lst_Exc_Fld.List(i)
            Exit For
        End If
Next

'For i = 0 To Lst_Back_Fld.ListCount - 1
'    If Lst_Back_Fld.Selected(i) = True Then
'        BAckField = Lst_Back_Fld.List(i)
'        Exit For
'    End If
'Next
FieldMap(i) = ExcelFiled
End Sub
Public Sub export2excel()
On Error GoTo BOError
Dim XL As New Excel.Application
Dim wbXL As New Excel.Workbook
Dim wsXL As New Excel.Worksheet
Dim lRow As Integer, lRows As Integer
Dim lCol As Integer, lCols As Integer
Dim lHeader As String
Dim i As Integer, j As Integer
On Error Resume Next
If DataGrid1 Then If DataGrid1.Rows <= 1 Then Exit Sub
If Not IsObject(XL) Then
    MsgBox "You need Microsoft Excel to use this function", vbExclamation, "Print to Excel"
    Exit Sub
End If
MousePointer = vbHourglass
Set wbXL = XL.Workbooks.Add
Set wsXL = XL.ActiveSheet
'If DataGrid1 Then lHeader = "Top Channel Report For The Period " & Format(date, "dd mmmm, yyyy") & " To " & Format(dateto, "dd mmmm, yyyy")
wsXL.Name = lHeader
wsXL.Range("a1:f1").MergeCells = True
wsXL.Cells(1, 1) = lHeader
wsXL.Range("a1:f1").Font.Size = 12
wsXL.Range("a1:f1").Font.Bold = True
wsXL.Range("a1:f1").Font.Color = vbBlue
If DataGrid1 Then
    lCols = DataGrid1.Cols - 2
    lRows = DataGrid1.Rows
End If
    For i = 0 To DataGrid1.Cols - 1
        DataGrid1.Col = i
        For j = 0 To DataGrid1.Rows - 1
            DataGrid1.Row = j
            If IsDate(DataGrid1.Text) And InStr(DataGrid1.Text, "/") <> 0 Then
                wsXL.Cells(j + 3, i) = Format(DataGrid1.Text, "mm/dd/yyyy")
            ElseIf IsNumeric(DataGrid1.Text) Then
                wsXL.Cells(j + 3, i) = DataGrid1.Text
            Else
                wsXL.Cells(j + 3, i) = DataGrid1.Text
            End If
            If j = 0 Then
                wsXL.Range(wsXL.Cells(j + 3, i), wsXL.Cells(j + 3, i)).Font.Bold = True
            End If
            DoEvents
            Label3.Caption = j
        Next
    Next
    For lCol = 1 To lCols
        wsXL.Columns(lCol).AutoFit
    Next
    For lRow = 1 To lRows
        wsXL.Rows(lRow).AutoFit
    Next
    XL.Visible = True
    For lCol = 1 To lCols
        wsXL.Columns(lCol).AutoFit
    Next
    For lRow = 1 To lRows
        wsXL.Rows(lRow).AutoFit
    Next
    XL.Visible = True
MousePointer = vbNormal
Exit Sub
BOError:
PrintToTrace "Error : " & err.Description
PrintToTrace "Error in Module : frmPerformance   Error in Procedure : cmdExport_Click"
PrintToTrace "Error End"
MsgBox "Please Contact BackOffice Administrator.", vbCritical, "Error..."
End Sub
