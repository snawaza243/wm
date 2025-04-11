'***********************Pankaj Pundeer***********************
Option Explicit
Dim Count_Loop As Integer
Dim Already_Exist As Integer
Dim importExcelcon As New ADODB.Connection
Dim TxtFile_Name As String
Public sheetName As String
Public filepath As String
Dim XL As New Excel.Application
Dim RsImport As New ADODB.Recordset
Public VChkZeroCommission As String
Public Nps_Importing_flag As String
Dim XLW As Excel.Workbook
Dim Total_records As Integer
Dim i As Long
Dim importFile As String
Dim Value1 As String
Public Comp_Cd As String
Dim rschk As New ADODB.Recordset
Dim delHash() As String
Dim delComma() As String
Dim SqlChk As String
Dim Xls_Fld As String
Dim Exc_Clent_FldName As String
Dim countNull As Integer
Dim Rs_Status As New ADODB.Recordset
Dim fso As filesystemobject
Public Status_Chk As String
Dim Chk_OPENFORM As Boolean
Dim Split_Company() As String
Dim XLWM As Excel.Workbook
Private Sub GIAPPMAP()
    Dim Rs_GIApp As ADODB.Recordset
    Dim MyAR As String
    Dim MyDt As String
    Dim sql As String
    Dim MyApp As String
    sql = "select app_no,login_dt,policy_no from Bajaj_PolicyInfo_Data_gen "
    Set Rs_GIApp = New ADODB.Recordset
    Rs_GIApp.open sql, MyConn, adOpenForwardOnly
    If Not Rs_GIApp.EOF Then
        While Not Rs_GIApp.EOF
            MyApp = ""
            MyApp = Rs_GIApp.Fields(0)
            If MyApp <> "" Then
                If SqlRet("select count(*) from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and app_no='" & Rs_GIApp.Fields(0) & "'") > 1 Then
                ElseIf SqlRet("select count(*) from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "'") = 1 Then
                    MyAR = ""
                    MyDt = ""
                    MyAR = SqlRet("select sys_ar_no from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "' ")
                    MyDt = SqlRet("select sys_ar_dt from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "' ")
                    
                    MyConn.Execute "Insert into gicovernoteimport select * from bajaj_ar_head  where sys_ar_no='" & MyAR & "'"
                    MyConn.Execute "Update bajaj_ar_head set status_cd ='A', login_dt=to_date('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR'),ply_issue_dt=add_months(to_date('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR'),12)-1 , policy_no='" & Rs_GIApp.Fields(2) & "',modified_user='AutoI' where company_cd= '" & Comp_Cd & "' and app_no='" & Rs_GIApp.Fields(0) & "' and ptype<>0 and modified_user<>'AutoI'"
                    
                    sql = ""
    '                sql = " UPDATE BAJAJ_AR_DETAILS SET STATUS_CD='A'  "
    '                sql = sql & " WHERE SYS_AR_NO = '" & MyAR & "' AND STATUS_DT = (SELECT MAX (STATUS_DT) "
    '                sql = sql & "                      FROM BAJAJ_AR_DETAILS "
    '                sql = sql & "                     WHERE SYS_AR_NO = '" & MyAR & "' AND STATUS_CD not in( 'A','F') ) "
                    sql = "insert into bajaj_ar_details (sys_ar_no,status_dt,status_cd,remarks,sys_ar_dt,userid,status_update_on,reason_cd) values"
                    sql = sql & "('" & MyAR & "',to_date(sysdate,'DD/MM/RRRR'),'A','Imported data',"
                    sql = sql & "to_date('" & MyDt & "','DD/MM/RRRR'),'AutoI',sysdate,'A001'"
                    sql = sql & ")"
                    MyConn.Execute (sql)
                Else
                    'MsgBox "This cover Note No. does not Exist in Our Database:" & MyApp
                    Call MarkinExcel(MyApp)
                End If
            End If
            Rs_GIApp.MoveNext
        Wend
    End If
    Rs_GIApp.Close
    Set Rs_GIApp = Nothing
End Sub

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

Private Sub NPSECSIMP()
Dim Rs_GIApp As ADODB.Recordset
    Dim MyAR As String
    Dim MyDt As String
    Dim sql As String
    Dim MyApp As String
    sql = ""
    sql = "select REF_TRAN_CODE,TR_DATE,ECS_AMT,CONSUMER_CODE from NPS_ECS_TBL_IMP "
    Set Rs_GIApp = New ADODB.Recordset
    Rs_GIApp.open sql, MyConn, adOpenForwardOnly
    If Not Rs_GIApp.EOF Then
        While Not Rs_GIApp.EOF
            MyApp = ""
            MyApp = Rs_GIApp.Fields(0)
            If MyApp <> "" Then
                If SqlRet("select count(*) from TRANSACTION_ST where MANUAL_ARNO= '" & MyApp & "' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR')") = 0 Then
                    Dim MyComIsuue As New ADODB.Command
                            MyComIsuue.ActiveConnection = MyConn
                            MyComIsuue.CommandType = adCmdStoredProc
                            MyComIsuue.CommandTimeout = 99999
                            MyComIsuue.CommandText = "NPS_ECSTRAN_GENERATE_IMP"
                            MyComIsuue.Parameters.Append MyComIsuue.CreateParameter("CONSUMERCODE", adChar, adParamInput, 50, MyApp)
                            MyComIsuue.Execute
                            Set MyComIsuue = Nothing
                            DoEvents
                Else
                    Call MarkinExcel(MyApp)
                End If
            End If
            Rs_GIApp.MoveNext
        Wend
    End If
    Rs_GIApp.Close
    Set Rs_GIApp = Nothing
End Sub
Private Sub NPSNONECSIMP()
Dim Rs_GIApp As ADODB.Recordset
    Dim MyAR As String
    Dim MyDt As String
    Dim sql As String
    Dim MyApp As String
    sql = ""
    sql = "select REF_TRAN_CODE,TR_DATE,ECS_AMT,CONSUMER_CODE from NPS_NONECS_TBL_IMP "
    Set Rs_GIApp = New ADODB.Recordset
    Rs_GIApp.open sql, MyConn, adOpenForwardOnly
    If Not Rs_GIApp.EOF Then
        While Not Rs_GIApp.EOF
            MyApp = ""
            MyApp = Rs_GIApp.Fields(0)
            If MyApp <> "" Then
                If SqlRet("select count(*) from TRANSACTION_ST where MANUAL_ARNO= '" & MyApp & "' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR')") = 0 Then
                            Dim MyComIsuue As New ADODB.Command
                            MyComIsuue.ActiveConnection = MyConn
                            MyComIsuue.CommandType = adCmdStoredProc
                            MyComIsuue.CommandTimeout = 99999
                            MyComIsuue.CommandText = "NPS_NONECSTRAN_GENERATE_IMP"
                            MyComIsuue.Parameters.Append MyComIsuue.CreateParameter("CONSUMERCODE", adChar, adParamInput, 50, MyApp)
                            MyComIsuue.Parameters.Append MyComIsuue.CreateParameter("PFLAG", adChar, adParamInput, 1, VChkZeroCommission)
                            MyComIsuue.Execute
                            Set MyComIsuue = Nothing
                            DoEvents
                Else
                    Call MarkinExcel(MyApp)
                End If
            End If
            Rs_GIApp.MoveNext
        Wend
    End If
    Rs_GIApp.Close
    Set Rs_GIApp = Nothing
End Sub


Private Sub CmdBrowse_Click()
Split_Company = Split(CmbCompany.Text, "#")
Comp_Cd = Trim(Split_Company(1))
Status_Chk = Mid(cmbstatus.List(cmbstatus.ListIndex), InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#") + 1, Len(cmbstatus.List(cmbstatus.ListIndex)) - InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#"))
TxtFileName.Text = ""
Cdilog.Filename = ""
Cdilog.Filter = "Excel Files|*.xls"
Cdilog.ShowOpen
TxtFileName.Text = Cdilog.Filename
lstsheet.Clear
If TxtFileName.Text <> "" Then
    Set XLW = XL.Workbooks.open(TxtFileName.Text)
    For Count_Loop = 1 To XLW.Worksheets.count
        lstsheet.AddItem XLW.Worksheets(Count_Loop).Name
    Next
    XLW.Close
    XL.Quit
End If
Set XLW = Nothing
Set XL = Nothing
End Sub

Private Sub CmdCreateNEw_Click()
If lstsheet.ListCount = 0 Then
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
If lstsheet.Text <> "" Then
  For i = 0 To lstsheet.ListCount - 1
    If lstsheet.Selected(i) = True Then
       sheetName = lstsheet.List(i)
       filepath = TxtFileName.Text
       Exit For
    End If
 Next
Else
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If

Split_Company = Split(CmbCompany.Text, "#")
Comp_Cd = Trim(Split_Company(1))
Call Get_Comp_TextFilePath(UCase(Trim(Split_Company(0))))
If filepath = "" Then MsgBox "Select Valid File", vbInformation: Exit Sub
Set importExcelcon = New ADODB.Connection
If importExcelcon.State = adStateOpen Then Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & filepath & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
If RsImport.State = 1 Then RsImport.Close
RsImport.CursorLocation = adUseClient
RsImport.open "Select * from [" & sheetName & "$] ", importExcelcon, adOpenForwardOnly, adLockOptimistic
Set Rs_Map_Exl_Fld = RsImport
If InStr(1, TxtFile_Name, ":") > 0 Then Map_Exl_Path = TxtFile_Name Else Map_Exl_Path = App.Path & "\life\INSUFLD\PolicyInfo\" & TxtFile_Name
Set fso = New Scripting.filesystemobject
If fso.FileExists(Map_Exl_Path) = False Then fso.CreateTextFile (Map_Exl_Path)
Map_DataBAse_FldName = "Application No***APP_NO/Policy No***POLICY_NO/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Login Date***Login_DT/Policy Issue Date***Policy_Issue_Dt/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Premium***PREMIUM/ChequeNo***CHEQUE_NO/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Date of commencement***DOC/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "BranchName***Branch_Name/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Status ***Status/SubStatus***REASON/Remarks***REMARKS/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Policy Term ***ply_term/Premium Term***prem_term/Premium Frequency***frequency/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Plan ***PRODUCT_NAME/Sum Assured***sum_assured"
'Load FrmMapPolicyField
RsImport.Close
Set RsImport = Nothing
importExcelcon.Close
Set importExcelcon = Nothing
If Rs_Map_Exl_Fld.State = 1 Then Rs_Map_Exl_Fld.Close
Set Rs_Map_Exl_Fld = Nothing
'FrmMapPolicyField.Show vbModal
End Sub


Private Sub Command1_Click()
'frmtabIns_Policyform.Show
End Sub

Private Sub Form_Load()
CmbCompany.Clear
OptNPS.Value = True
'OptLife.Value = False
OptGeneral.Value = False
Dim Rs_company As New ADODB.Recordset
'OptLife.Value = True
Rs_company.open "select iss_name,iss_code from iss_master where iss_code='IS02520'", MyConn, adOpenDynamic, adLockOptimistic
CmbCompany.Clear

Do While Not Rs_company.EOF
    CmbCompany.AddItem Rs_company!iss_name & Space(60) & "#" & Rs_company!iss_code
    Rs_company.MoveNext
Loop
CmbCompany.ListIndex = 0
Rs_company.Close
Set Rs_Status = New ADODB.Recordset
Rs_Status.open " select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status", MyConn, adOpenDynamic, adLockReadOnly
cmbstatus.Clear
Do While Not Rs_Status.EOF
   cmbstatus.AddItem Rs_Status!status & Space(60) & "#" & Rs_Status!status_cd
   Rs_Status.MoveNext
Loop
If cmbstatus.ListCount > 0 Then cmbstatus.ListIndex = 0
Rs_Status.Close

End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
On Error Resume Next
RsImport.Close
Set RsImport = Nothing
rschk.Close
Set rschk = Nothing

End Sub

Private Sub Form_Unload(Cancel As Integer)
On Error Resume Next
RsImport.Close
Set RsImport = Nothing
rschk.Close
Set rschk = Nothing
Set XL = Nothing
Set XLW = Nothing
Set importExcelcon = Nothing
End Sub

Private Sub OptGeneral_Click()
If OptGeneral.Value = True Then
    Dim rsCompny As New ADODB.Recordset
    rsCompny.open "select company_cd,company_name from bajaj_company_master where catagory='G' order by company_name", MyConn
    i = 0
    CmbCompany.Clear
    While Not rsCompny.EOF
        CmbCompany.AddItem rsCompny!company_name & Space(60) & "#" & rsCompny!COMPANY_CD
        rsCompny.MoveNext
    Wend
    rsCompny.Close
End If
End Sub

Private Sub OptLife_Click()
If OptLife.Value = True Then
Dim rsCompny As New ADODB.Recordset
        
    rsCompny.open "select company_cd,company_name from bajaj_company_master where catagory='L' order by company_name", MyConn
    i = 0
    CmbCompany.Clear
    While Not rsCompny.EOF
        CmbCompany.AddItem rsCompny!company_name & Space(60) & "#" & rsCompny!COMPANY_CD
        rsCompny.MoveNext
    Wend
    rsCompny.Close
End If
End Sub

Private Sub pic_import_Click()
'On Error GoTo BOError
Chk_OPENFORM = True
If lstsheet.ListCount = 0 Then
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
If lstsheet.Text <> "" Then
  For i = 0 To lstsheet.ListCount - 1
    If lstsheet.Selected(i) = True Then
      GoTo mm
    End If
  Next
  MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
mm:
filepath = TxtFileName.Text   'lblfile.Caption
Me.MousePointer = vbHourglass
For i = 0 To lstsheet.ListCount - 1
     If lstsheet.Selected(i) = True Then
        sheetName = lstsheet.List(i)
     End If
Next
Me.MousePointer = vbHourglass
If Nps_Importing_flag = "ECS" Then
    Call ImportTransaction
Else
    Call ImportNonECSTransaction
End If
Me.MousePointer = vbNormal
If OptLife.Value = True Then
ElseIf OptGeneral.Value = True Then
    Call GIAPPMAP
    MsgBox "All Records Updated Successfully", vbInformation
ElseIf OptNPS.Value = True Then
    If Nps_Importing_flag = "ECS" Then
        Call NPSECSIMP
    Else
        Call NPSNONECSIMP
    End If
    MsgBox "All Records Inserted Successfully", vbInformation
End If
MousePointer = vbNormal
Exit Sub
End Sub

Private Sub cmdExit_Click()
Unload Me
End Sub
Private Sub ImportTransaction()
'On Error GoTo err1
'On Error resume Next
Dim Rsunmatch As New ADODB.Recordset
Dim sql As String
Dim FileFields As String
Dim dataDaseField As String
Dim selectedFileField As String
Dim str1 As String
Status_Chk = Mid(cmbstatus.List(cmbstatus.ListIndex), InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#") + 1, Len(cmbstatus.List(cmbstatus.ListIndex)) - InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#"))



Split_Company = Split(CmbCompany.Text, "#")
Comp_Cd = Trim(Split_Company(1))
Set importExcelcon = New ADODB.Connection
importFile = filepath
Set XLW = XL.Workbooks.open(filepath)
i = 1
While XLW.Worksheets(sheetName).Cells(1, i) <> ""
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), ".", "")
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), "/", "")
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), "-", "")
    i = i + 1
Wend
XLW.save
XLW.Close
Set XLW = Nothing
XL.Quit
Set XL = Nothing
Set XLW = XL.Workbooks.open(filepath)
XLW.Close
Set XLW = Nothing
XL.Quit
Set XL = Nothing
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & importFile & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
RsImport.CursorLocation = adUseClient
If RsImport.State = 1 Then RsImport.Close
RsImport.open "Select * from [" & sheetName & "$] ", importExcelcon, adOpenForwardOnly
Set Rs_Map_Exl_Fld = RsImport
If RsImport.Fields.count > 0 Then
   FileFields = ""
   For i = 0 To RsImport.Fields.count - 1
       FileFields = FileFields & RsImport.Fields(i).Name & ","
   Next
   FileFields = Left(FileFields, Len(FileFields) - 1)
End If
Call Get_Comp_TextFilePath(Trim(UCase(Split_Company(0))))
str1 = FieldsParametersName(TxtFile_Name) 'Maping Fileds Are Shown In This Str1
If Trim(str1) = "" Then
    Chk_OPENFORM = False
    RsImport.Close
    Set RsImport = Nothing
    importExcelcon.Close
    Set importExcelcon = Nothing
    XLW.Close
    Set XLW = Nothing
    XL.Quit
    Set XL = Nothing
    MsgBox "File Format Is Wrong", vbInformation
    Exit Sub
End If
delComma = Split(str1, ","): selectedFileField = "": dataDaseField = ""
For i = 0 To UBound(delComma)
    delHash = Split(delComma(i), "#")
    selectedFileField = selectedFileField & delHash(1) & ","
    dataDaseField = dataDaseField & delHash(0) & ","
Next
selectedFileField = Left(selectedFileField, Len(selectedFileField) - 1)
dataDaseField = Left(dataDaseField, Len(dataDaseField) - 1)
'RsImport.Close
'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub
If CheckFieldMapping(UCase(dataDaseField), UCase(Status_Chk)) = False Then Chk_OPENFORM = False: Exit Sub
If OptLife.Value = True Then
    MyConn.Execute " delete from  Bajaj_PolicyInfo_Data " ' and already_import is null"
    Set RsImport = New ADODB.Recordset
    Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
    Total_records = 0
    Do While Not RsImport.EOF
        sql = ""
        SqlChk = ""
        Xls_Fld = ""
        SqlChk = " select * from  Bajaj_PolicyInfo_Data where  "
        For i = 0 To UBound(delComma)
            delHash = Split(delComma(i), "#")
            Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
            If InStr(1, Xls_Fld, "&") > 0 Then
                Xls_Fld = Exc_Clent_FldName
            End If
            If RsImport("" & Xls_Fld & "").Type = 7 Then
                Xls_Fld = Format(RsImport("" & Xls_Fld & "").Value, "dd-mmm-yyyy")
                Xls_Fld = Replace(Xls_Fld, "'", "")
                SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
            ElseIf (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Format(Xls_Fld, "dd-mmm-yyyy") & "' "
                End If
            ElseIf RsImport("" & Xls_Fld & "").Type = 202 Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & " (" & delHash(0) & "='" & UCase(Trim(Xls_Fld)) & "' or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
                End If
            Else
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "0" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & "=" & Xls_Fld & " or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "=" & Format(Xls_Fld, "######0") & " "
                End If
            End If
            SqlChk = SqlChk & " and "
        Next i
        SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
        SqlChk = newString(SqlChk)
        Set rschk = MyConn.Execute(SqlChk)
        'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
        'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
        If rschk.EOF Then
                countNull = 0
                Already_Exist = 0
                sql = "Insert into  Bajaj_PolicyInfo_Data (" & dataDaseField & ",Company_cd,Import_dt,UserID)  Values("
                For i = 0 To RsImport.Fields.count - 1
                    If InStr(1, RsImport(i), "'") > 0 Then
                        Value1 = Replace(RsImport(i), "'", "")
                        sql = sql & "'" & Value1 & "',"
                    Else
                       If RsImport(i).Type = 7 Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                            sql = sql & "'" & Trim(Format(RsImport(i), "dd-mmm-yyyy")) & "',"
                       Else
                            sql = sql & "'" & Trim(RsImport(i)) & "',"
                       End If
                    End If
                Next
                sql = sql & "'" & Comp_Cd & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & Glbloginid & "'"
                sql = sql & ")"
                sql = Replace(sql, "''", "Null")
                sql = newString(sql)
                INSERT_RECORD sql
            If countNull >= 50 Then Exit Do
            Else
                Already_Exist = Already_Exist + 1
            End If
        RsImport.MoveNext
        Total_records = Total_records + 1
       
        Label5.Caption = Total_records
        DoEvents
    Loop
ElseIf OptGeneral.Value = True Then
    MyConn.Execute " delete from  Bajaj_PolicyInfo_Data_gen " ' and already_import is null"
    Set RsImport = New ADODB.Recordset
    Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
    Total_records = 0
    Do While Not RsImport.EOF
        sql = ""
        SqlChk = ""
        Xls_Fld = ""
        SqlChk = " select * from  Bajaj_PolicyInfo_Data_gen where  "
        For i = 0 To UBound(delComma)
            delHash = Split(delComma(i), "#")
            Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
            If InStr(1, Xls_Fld, "&") > 0 Then
                Xls_Fld = Exc_Clent_FldName
            End If
            If RsImport("" & Xls_Fld & "").Type = 7 Then
                Xls_Fld = Format(RsImport("" & Xls_Fld & "").Value, "dd-mmm-yyyy")
                Xls_Fld = Replace(Xls_Fld, "'", "")
                SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
            ElseIf (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Format(Xls_Fld, "dd-mmm-yyyy") & "' "
                End If
            ElseIf RsImport("" & Xls_Fld & "").Type = 202 Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & " (" & delHash(0) & "='" & UCase(Trim(Xls_Fld)) & "' or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
                End If
            Else
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "0" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & "=" & Xls_Fld & " or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "=" & Format(Xls_Fld, "######0") & " "
                End If
            End If
            SqlChk = SqlChk & " and "
        Next i
        SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
        SqlChk = newString(SqlChk)
        Set rschk = MyConn.Execute(SqlChk)
        'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
        'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
        If rschk.EOF Then
                countNull = 0
                Already_Exist = 0
                sql = "Insert into  Bajaj_PolicyInfo_Data_gen (" & dataDaseField & ",Company_cd,Import_dt,UserID)  Values("
                For i = 0 To RsImport.Fields.count - 1
                    If InStr(1, RsImport(i), "'") > 0 Then
                        Value1 = Replace(RsImport(i), "'", "")
                        sql = sql & "'" & Value1 & "',"
                    Else
                       If RsImport(i).Type = 7 Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                            sql = sql & "'" & Trim(Format(RsImport(i), "dd-mmm-yyyy")) & "',"
                       Else
                            sql = sql & "'" & Trim(RsImport(i)) & "',"
                       End If
                    End If
                Next
                sql = sql & "'" & Comp_Cd & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & Glbloginid & "'"
                sql = sql & ")"
                sql = Replace(sql, "''", "Null")
                sql = newString(sql)
                INSERT_RECORD sql
            If countNull >= 50 Then Exit Do
            Else
                Already_Exist = Already_Exist + 1
            End If
        RsImport.MoveNext
        Total_records = Total_records + 1
       
        Label5.Caption = Total_records
        DoEvents
    Loop
ElseIf OptNPS.Value = True Then
    MyConn.Execute " insert into nps_ecs_tbl_imp_bk select * from  nps_ecs_tbl_imp" ' and already_import is null"
    MyConn.Execute " delete from  nps_ecs_tbl_imp " ' and already_import is null"
    Set RsImport = New ADODB.Recordset
    If RsImport.State = 1 Then RsImport.Close
    Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
    Total_records = 0
    Do While Not RsImport.EOF
        sql = ""
        SqlChk = ""
        Xls_Fld = ""
        SqlChk = " select * from  nps_ecs_tbl_imp where "
        For i = 0 To UBound(delComma)
            delHash = Split(delComma(i), "#")
            Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
            If InStr(1, Xls_Fld, "&") > 0 Then
                Xls_Fld = Exc_Clent_FldName
            End If
            If RsImport("" & Xls_Fld & "").Type = 7 Then
                Xls_Fld = Format(RsImport("" & Xls_Fld & "").Value, "dd-mmm-yyyy")
                Xls_Fld = Replace(Xls_Fld, "'", "")
                SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
            ElseIf (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Format(Xls_Fld, "dd-mmm-yyyy") & "' "
                End If
            ElseIf RsImport("" & Xls_Fld & "").Type = 202 Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & " (" & delHash(0) & "='" & UCase(Trim(Xls_Fld)) & "' or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
                End If
            Else
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "0" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & "=" & Xls_Fld & " or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "=" & Format(Xls_Fld, "######0") & " "
                End If
            End If
            SqlChk = SqlChk & " and "
        Next i
        'SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
        SqlChk = SqlChk & " 1=1 "
        SqlChk = newString(SqlChk)
        Set rschk = MyConn.Execute(SqlChk)
        'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
        'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
        If rschk.EOF Then
                countNull = 0
                Already_Exist = 0
                sql = "Insert into  nps_ecs_tbl_imp (" & dataDaseField & ",Import_dt,LOGGEDUserID)  Values("
                For i = 0 To RsImport.Fields.count - 1
                    If InStr(1, RsImport(i), "'") > 0 Then
                        Value1 = Replace(RsImport(i), "'", "")
                        sql = sql & "'" & Value1 & "',"
                    Else
                       If RsImport(i).Type = 7 Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                            sql = sql & "'" & Trim(Format(RsImport(i), "dd-mmm-yyyy")) & "',"
                       Else
                            sql = sql & "'" & Trim(RsImport(i)) & "',"
                       End If
                    End If
                Next
                sql = sql & "'" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & Glbloginid & "'"
                sql = sql & ")"
                sql = Replace(sql, "''", "Null")
                sql = newString(sql)
                INSERT_RECORD sql
            If countNull >= 50 Then Exit Do
            Else
                Already_Exist = Already_Exist + 1
            End If
        RsImport.MoveNext
        Total_records = Total_records + 1
       
        Label5.Caption = Total_records
        DoEvents
    Loop
End If
sql = "delete nps_ecs_tbl_imp where ref_tran_code is null"
INSERT_RECORD sql
Label5.Caption = Label5.Caption - 1
MsgBox " Out OF " & Str(Total_records - 1) & " Records " & Str(Already_Exist) & " Are Already Exist "
RsImport.Close
Set RsImport = Nothing
rschk.Close
Set rschk = Nothing
importExcelcon.Close
Set importExcelcon = Nothing
'XLW.Close
'Set XLW = Nothing
'XL.Quit
'Set XL = Nothing
Exit Sub
'err1:
'    Chk_OPENFORM = False
'    MsgBox err.Description
'    Set RsImport = Nothing
'    Set importExcelcon = Nothing
'    Set XLW = Nothing
'    Set XL = Nothing
'    Resume
End Sub
Private Sub ImportNonECSTransaction()
'On Error GoTo err1
'On Error resume Next
Dim Rsunmatch As New ADODB.Recordset
Dim sql As String
Dim FileFields As String
Dim dataDaseField As String
Dim selectedFileField As String
Dim str1 As String
Status_Chk = Mid(cmbstatus.List(cmbstatus.ListIndex), InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#") + 1, Len(cmbstatus.List(cmbstatus.ListIndex)) - InStr(1, cmbstatus.List(cmbstatus.ListIndex), "#"))
Split_Company = Split(CmbCompany.Text, "#")
Comp_Cd = Trim(Split_Company(1))
Set importExcelcon = New ADODB.Connection
importFile = filepath
Set XLW = XL.Workbooks.open(filepath)
i = 1
While XLW.Worksheets(sheetName).Cells(1, i) <> ""
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), ".", "")
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), "/", "")
    XLW.Worksheets(sheetName).Cells(1, i) = Replace(XLW.Worksheets(sheetName).Cells(1, i), "-", "")
    i = i + 1
Wend
XLW.save
XLW.Close
Set XLW = Nothing
XL.Quit
Set XL = Nothing
Set XLW = XL.Workbooks.open(filepath)
XLW.Close
Set XLW = Nothing
XL.Quit
Set XL = Nothing
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & importFile & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
RsImport.CursorLocation = adUseClient
If RsImport.State = 1 Then RsImport.Close
RsImport.open "Select * from [" & sheetName & "$] ", importExcelcon, adOpenForwardOnly
Set Rs_Map_Exl_Fld = RsImport
If RsImport.Fields.count > 0 Then
   FileFields = ""
   For i = 0 To RsImport.Fields.count - 1
       FileFields = FileFields & RsImport.Fields(i).Name & ","
   Next
   FileFields = Left(FileFields, Len(FileFields) - 1)
End If
Call Get_Comp_TextFilePath(Trim(UCase(Split_Company(0))))
str1 = FieldsParametersName(TxtFile_Name) 'Maping Fileds Are Shown In This Str1
If Trim(str1) = "" Then
    Chk_OPENFORM = False
    RsImport.Close
    Set RsImport = Nothing
    importExcelcon.Close
    Set importExcelcon = Nothing
    XLW.Close
    Set XLW = Nothing
    XL.Quit
    Set XL = Nothing
    MsgBox "File Format Is Wrong", vbInformation
    Exit Sub
End If
delComma = Split(str1, ","): selectedFileField = "": dataDaseField = ""
For i = 0 To UBound(delComma)
    delHash = Split(delComma(i), "#")
    selectedFileField = selectedFileField & delHash(1) & ","
    dataDaseField = dataDaseField & delHash(0) & ","
Next
selectedFileField = Left(selectedFileField, Len(selectedFileField) - 1)
dataDaseField = Left(dataDaseField, Len(dataDaseField) - 1)
'RsImport.Close
'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub
If CheckFieldMapping(UCase(dataDaseField), UCase(Status_Chk)) = False Then Chk_OPENFORM = False: Exit Sub
If OptNPS.Value = True Then
    MyConn.Execute " insert into nps_nonecs_tbl_imp_bk select * from  nps_nonecs_tbl_imp"
    MyConn.Execute " delete from  nps_nonecs_tbl_imp "
    Set RsImport = New ADODB.Recordset
    If RsImport.State = 1 Then RsImport.Close
    Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
    Total_records = 0
    Do While Not RsImport.EOF
        sql = ""
        SqlChk = ""
        Xls_Fld = ""
        SqlChk = " select * from  nps_nonecs_tbl_imp where "
        For i = 0 To UBound(delComma)
            delHash = Split(delComma(i), "#")
            Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
            If InStr(1, Xls_Fld, "&") > 0 Then
                Xls_Fld = Exc_Clent_FldName
            End If
            If RsImport("" & Xls_Fld & "").Type = 7 Then
                Xls_Fld = Format(RsImport("" & Xls_Fld & "").Value, "dd-mmm-yyyy")
                Xls_Fld = Replace(Xls_Fld, "'", "")
                SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
            ElseIf (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Format(Xls_Fld, "dd-mmm-yyyy") & "' "
                End If
            ElseIf RsImport("" & Xls_Fld & "").Type = 202 Then
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & " (" & delHash(0) & "='" & UCase(Trim(Xls_Fld)) & "' or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "='" & Xls_Fld & "' "
                End If
            Else
                If IsNull(RsImport("" & Xls_Fld & "").Value) = True Then
                    Xls_Fld = "0" ' RsImport("" & Xls_Fld & "").Value
                    SqlChk = SqlChk & "(" & delHash(0) & "=" & Xls_Fld & " or " & delHash(0) & " is null  )  "
                Else
                    Xls_Fld = RsImport("" & Xls_Fld & "").Value
                    Xls_Fld = Replace(Xls_Fld, "'", "")
                    SqlChk = SqlChk & delHash(0) & "=" & Format(Xls_Fld, "######0") & " "
                End If
            End If
            SqlChk = SqlChk & " and "
        Next i
        'SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
        SqlChk = SqlChk & " 1=1 "
        SqlChk = newString(SqlChk)
        Set rschk = MyConn.Execute(SqlChk)
        'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
        'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
        If rschk.EOF Then
                countNull = 0
                Already_Exist = 0
                sql = "Insert into  nps_nonecs_tbl_imp (" & dataDaseField & ",Import_dt,LOGGEDUserID)  Values("
                For i = 0 To RsImport.Fields.count - 1
                    If InStr(1, RsImport(i), "'") > 0 Then
                        Value1 = Replace(RsImport(i), "'", "")
                        sql = sql & "'" & Value1 & "',"
                    Else
                       If RsImport(i).Type = 7 Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 10 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 6) Or (RsImport(i).Type = 202 And Len(RsImport(i)) = 9 And InStr(1, RsImport(i), "/") = 3 And InStr(4, RsImport(i), "/") = 5) Then
                            sql = sql & "'" & Trim(Format(RsImport(i), "dd-mmm-yyyy")) & "',"
                       Else
                            sql = sql & "'" & Trim(RsImport(i)) & "',"
                       End If
                    End If
                Next
                sql = sql & "'" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & Glbloginid & "'"
                sql = sql & ")"
                sql = Replace(sql, "''", "Null")
                sql = newString(sql)
                INSERT_RECORD sql
            If countNull >= 50 Then Exit Do
            Else
                Already_Exist = Already_Exist + 1
            End If
        RsImport.MoveNext
        Total_records = Total_records + 1
       
        Label5.Caption = Total_records
        DoEvents
    Loop
End If
sql = "delete nps_nonecs_tbl_imp where ref_tran_code is null"
INSERT_RECORD sql
Label5.Caption = Label5.Caption - 1
MsgBox " Out OF " & Str(Total_records - 1) & " Records " & Str(Already_Exist) & " Are Already Exist "
RsImport.Close
Set RsImport = Nothing
rschk.Close
Set rschk = Nothing
importExcelcon.Close
Set importExcelcon = Nothing
Exit Sub
End Sub
Private Function newString(Str As String)
    Dim newstr As String
    Dim validchar As String
    Dim i As Integer
    newstr = ""
    validchar = "abcdfeghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/\*':;,.+-_) (@#!&{}[]~`|="
    For i = 1 To Len(Str)
        If InStr(1, validchar, Mid(Str, i, 1)) Then
            newstr = newstr & Mid(Str, i, 1)
        End If
    Next
    newString = newstr
End Function

Private Function CheckFieldsVis(StrField As String, selectFilefield) As Boolean
Dim selectFF() As String
Dim flag As Boolean
    flag = False
    selectFF = Split(selectFilefield, ",")
    For i = 0 To UBound(selectFF)
        If InStr(1, UCase(selectFF(i)), "&") = 0 Then
            If InStr(1, UCase(StrField), UCase(Replace(Replace(Trim(selectFF(i)), "[", ""), "]", ""))) = 0 Then
                MsgBox "Excel File is not Match on " & selectFF(i) & "  Parameter .", vbInformation
                Chk_OPENFORM = False
                XLW.Close
                Set XLW = Nothing
                Set XL = Nothing
                RsImport.Close
                Set RsImport = Nothing
                flag = True
                CheckFieldsVis = False
                Exit Function
            End If
        End If
    Next
    If flag = True Then
        CheckFieldsVis = False
    Else
        CheckFieldsVis = True
    End If
End Function
Private Function CheckFields(StrField As String) As Boolean
Dim StrFields As Variant
Dim StrFieldExcel As Variant
Dim str1 As String
If str1 = "" Then
        MsgBox "PLease Enter Fields Parameters into File" & App.Path & "FieldParameters"
        Chk_OPENFORM = False
        CheckFields = False
        Exit Function
Else
        StrFields = Split(str1, ",")
        StrFieldExcel = Split(StrField, ",")
        For i = 0 To UBound(StrFields)
            If StrFields(i) <> StrFieldExcel(i) Then
                Chk_OPENFORM = False
                MsgBox "Excel File and Fiels Parameters are not Same.", vbInformation
                Exit Function
                CheckFields = False
            End If
        Next
End If
CheckFields = True
End Function
Public Function FieldsParametersName(txtFile As String) As String
Dim FileObj As filesystemobject
Dim FS As Object
Set FileObj = CreateObject("Scripting.Filesystemobject")
Set fso = New filesystemobject
Set FS = FileObj.OpenTextFile(App.Path & "\life\insufld\PolicyInfo\" & txtFile)
FieldsParametersName = FS.ReadLine
Set FileObj = Nothing
End Function
Private Sub INSERT_RECORD(sql As String)
''On Error GoTo err1
MyConn.Execute (sql)
Exit Sub
err1:
MsgBox err.Description
End Sub
Private Sub Get_Comp_TextFilePath(companyname As String)
If companyname = UCase("ALL INSURANCE CO. (SERVICE)") Then
    TxtFile_Name = "FieldParametersALL1_Comm.txt"
ElseIf companyname = UCase("AEGON RELIGARE LIFE INSURANCE") Then
    TxtFile_Name = "FieldParametersAEGON_comm.txt"
ElseIf companyname = UCase("METLIFE INSURANCE COMPANY") Then
    TxtFile_Name = "FieldParametersMET_comm.txt"
ElseIf companyname = UCase("AVIVA LIFE INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersAVIVA1_comm.txt"
ElseIf companyname = UCase("BAJAJ ALLIANZ GENERAL INURANCE") Then
    TxtFile_Name = "FieldParametersBAGI1.txt"
ElseIf companyname = UCase("BAJAJ ALLIANZ LIFE INURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersBAJAJ1_Comm.txt"
ElseIf companyname = UCase("BHARTI AXA") Then
    TxtFile_Name = "FieldParametersBHTA1_Comm.txt"
ElseIf companyname = UCase("BIRLA SUN LIFE INSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersBIRLA1_Comm.txt"
ElseIf companyname = UCase("CHOLAMANDALAM MS GENERAL INSURALCE CO.LTD.") Then
    TxtFile_Name = "FieldParametersMSCHOLA1.txt"
ElseIf companyname = UCase("HDFC CHUB GENERAL INSURANCE CO.LTD.") Then
    TxtFile_Name = "FieldParametersHDFCCHUBB1.txt"
ElseIf companyname = UCase("HDFC STANDARD LIFE INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersHDFC1_Comm.txt"
ElseIf companyname = UCase("ICICI Lombard General Insurance  LTD") Then
    TxtFile_Name = "FieldParametersICICIL1.txt"
ElseIf companyname = UCase("ICICI PRUDENTIAL LIFE INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersICICI1_Comm.txt"
ElseIf companyname = UCase("IFFCO-TOKIO GENERAL INSURANCE CO.LTD.") Then
    TxtFile_Name = "FieldParametersITGI1.txt"
ElseIf companyname = UCase("ING Vysaya Life Insurance") Then
    TxtFile_Name = "FieldParametersINGVYS1_Comm.txt"
ElseIf companyname = UCase("LIFE INSURANCE CORPORATION") Then
    TxtFile_Name = "FieldParametersLIC11_Comm.txt"
ElseIf companyname = UCase("MAX NEW YORK LIFE INSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersMNYL1_Comm.txt"
ElseIf companyname = UCase("NATIONAL INSURANCE COMPANY LTD") Then
    TxtFile_Name = "FieldParametersNIC1_Comm.txt"
ElseIf companyname = UCase("NEW INDIA ASSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersNIAC1_COmm.txt"
ElseIf companyname = UCase("KOTAK LIFE INSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersKOTAK1_Comm.txt"
ElseIf companyname = UCase("ORIENTAL INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersOIC1_Comm.txt"
ElseIf companyname = UCase("RELIANCE LIFE INSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersRELLIFE1_Comm.txt"
ElseIf companyname = UCase("RELIENCE GENERAL INSURANCE CO. LTD.") Then
    TxtFile_Name = "FieldParametersRELGEN1_Comm.txt"
ElseIf companyname = UCase("ROYAL SUNDARAM ALLIANCE INSURANCE CO.LTD.") Then
    TxtFile_Name = "FieldParametersRSA1_Comm.txt"
ElseIf companyname = UCase("SBI LIFE INSURANCE") Then
    TxtFile_Name = "FieldParametersSBI1_Comm.txt"
ElseIf companyname = UCase("STAR HEALTH") Then
    TxtFile_Name = "FieldParameters5501_Comm.txt"
ElseIf companyname = UCase("TATA AIG GENERAL INSURANCE CO.LTD.") Then
    TxtFile_Name = "FieldParametersTATAAIG1.txt"
ElseIf companyname = UCase("TATA-AIG LIFE INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersTATA1_Comm.txt"
ElseIf companyname = UCase("UNITED INDIA INSURANCE CO. LTD") Then
    TxtFile_Name = "FieldParametersUIIC1_Comm.txt"
    '------------------mayank gi status update-------------------'
ElseIf OptGeneral.Value = True Then
    TxtFile_Name = "FieldParametersGIBHARTI_Comm.txt"
ElseIf OptNPS.Value = True Then
    TxtFile_Name = "FieldParametersNPS.txt"
End If
End Sub
Private Function CheckFieldMapping(data_Base_Fld As String, statusCode As String) As Boolean
CheckFieldMapping = False
If Comp_Cd = "D" Or Comp_Cd = "A" Or Comp_Cd = "B" Then
    If InStr(1, UCase(data_Base_Fld), "STATUS") <= 0 Then
        MsgBox "Status is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "REASON") <= 0 Then
        MsgBox "Reason is Not Mapped", vbCritical
        Exit Function
    End If
    If Comp_Cd = "A" Then
        If InStr(1, UCase(data_Base_Fld), "POLICY_ISSUE_DT") <= 0 Then
            MsgBox "POLICY ISSUE DATE is Not Mapped", vbCritical
            Exit Function
        End If
        If InStr(1, UCase(data_Base_Fld), "DOC") Then
            MsgBox "DOC is Not Mapped", vbCritical
            Exit Function
        End If
    ElseIf Comp_Cd = "D" Then
        If InStr(1, UCase(data_Base_Fld), "Login_DT") <= 0 Then
            MsgBox "LoginDate is Not Mapped", vbCritical
            Exit Function
        End If
    End If
End If
If Comp_Cd = "OM KOTAK" Or Comp_Cd = "SBI LIFE" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "CHEQUE_NO") <= 0 Then
        'MsgBox "Cheque_No is Not Mapped", vbCritical
       ' Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "PREMIUM") <= 0 Then
        MsgBox "PREMIUM is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "TATAAIG" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "RELLIFE" Or Comp_Cd = "ICICI" Or Comp_Cd = "AVIVA" Or Comp_Cd = "BIRLA SUN" Or Comp_Cd = "BHT A" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf Comp_Cd = "BAJAJ A" Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "CHEQUE_NO") <= 0 Then
       ' MsgBox "Cheque_No is Not Mapped", vbCritical
        'Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
    '------------------mayank gi status update--------------------
ElseIf OptGeneral.Value = True Then
    If InStr(1, UCase(data_Base_Fld), "POLICY_NO") <= 0 Then
        MsgBox "POLICY_NO is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "LOGIN_DT") <= 0 Then
        MsgBox "Policy Issue Dt is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "APP_NO") <= 0 Then
        MsgBox "Application No is Not Mapped", vbCritical
        Exit Function
    End If
ElseIf OptNPS.Value = True Then
    If InStr(1, UCase(data_Base_Fld), "REF_TRAN_CODE") <= 0 Then
        MsgBox "Consumer Code is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "TR_DATE") <= 0 Then
        MsgBox "Transaction Date is Not Mapped", vbCritical
        Exit Function
    End If
    If InStr(1, UCase(data_Base_Fld), "ECS_AMT") <= 0 Then
        MsgBox "Amount is Not Mapped", vbCritical
        Exit Function
    End If
End If
CheckFieldMapping = True
End Function


