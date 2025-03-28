'*******************************************************
' Subroutine: Picture1_Click
' Description: Handles the click event of the Picture1 control.
'              This subroutine performs the following tasks:
'              - Validates if a sheet is selected from the list.
'              - Retrieves the selected sheet name.
'              - Validates the selected file path.
'              - Opens a connection to the selected Excel file.
'              - Reads data from the specified sheet in the Excel file.
'              - Prepares a mapping of database field names.
'              - Loads and displays a mapping form for user interaction.
'              - Cleans up resources after execution.
'
' Parameters: None
'
' Returns: None
'
' Error Handling: Displays message boxes for user guidance in case of:
'                 - No sheet selected.
'                 - Invalid file path.
'
' Dependencies:


' - Requires Microsoft ActiveX Data Objects (ADODB) library.
' - Requires Microsoft Scripting Runtime library.
' - Relies on external forms and controls such as FrmMapField_JPM, lstsheet, and TxtFileName.
'
' Notes:
' - The subroutine assumes the presence of a valid Excel file and sheet.
' - The mapping form (FrmMapField_JPM) is displayed modally for user interaction.
' - The database field names are hardcoded and concatenated into a single string.
'
'*******************************************************
Private Sub Picture1_Click()
If lstsheet.ListCount = 0 Then
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
If lstsheet.Text <> "" Then
    For KOUNT_Loop = 0 To lstsheet.ListCount - 1
        If lstsheet.Selected(KOUNT_Loop) = True Then
           sheetName = lstsheet.List(KOUNT_Loop)
           Exit For
        End If
    Next
Else
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
Call Get_Comp_TextFilePath
If TxtFileName.Text = "" Then MsgBox "Select Valid File", vbInformation: Exit Sub
Set importExcelcon = New ADODB.Connection
If importExcelcon.State = adStateOpen Then Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & TxtFileName.Text & " ;Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set RsImport = importExcelcon.Execute("Select * from [" & sheetName & "$] ")
Set Rs_Map_Exl_Fld = RsImport
If InStr(1, TxtFile_Name, ":") > 0 Then Map_Exl_Path = TxtFile_Name Else Map_Exl_Path = App.Path & "\Life\insufld\" & TxtFile_Name 'App.Path & "\Life\insufld\" & "Field_Parameter_due.txt"
Set fso = New Scripting.filesystemobject
If fso.FileExists(Map_Exl_Path) = False Then fso.CreateTextFile (Map_Exl_Path)
Map_DataBAse_FldName = " "
Map_DataBAse_FldName = "Company_Cd/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Status_cd/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Location/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Policy_No/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "CL_Name/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Prem_Amt/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Pay_Mode/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Due_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "SA/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_Add1/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add2/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add3/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add4/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_add5/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_City/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_pin/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_Phone1/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_phone2/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Cl_mobile/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "plan_name/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "prem_freq/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "Doc/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ply_term/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ecs_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "next_due_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "paid_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "net_amount/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "fup_date/"
Map_DataBAse_FldName = Map_DataBAse_FldName & "ppt/"
Load FrmMapField_JPM
FrmMapField_JPM.lblmap = "** Status,Scheme,Frequency Field Are Mandatory "
FrmMapField_JPM.Frame3.Caption = "Excel File Fields"
FrmMapField_JPM.Show vbModal
RsImport.Close
Set RsImport = Nothing
Set Rs_Map_Exl_Fld = Nothing
End Sub