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