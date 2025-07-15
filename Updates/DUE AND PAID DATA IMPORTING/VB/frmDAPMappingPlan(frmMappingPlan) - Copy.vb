Dim MyConn As New ADODB.Connection
Dim RS As New ADODB.Recordset
Dim Str() As String
Dim CCODE As String
Public MonNo As Integer


Private Sub cmbCompany_Click()
If CmbCompany.ListIndex <> -1 Then
   Str = Split(CmbCompany.Text, "#")
   CCODE = Str(1)
End If
If CCODE = "" Then
    MsgBox "Please Select The Company", vbInformation, "Wealthmaker"
    Exit Sub
End If
sql = " SELECT DISTINCT PLAN_NAME FROM ( "
sql = sql & "SELECT POLICY_NO,COMPANY_CD,UPPER(TRIM(PLAN_NAME)) PLAN_NAME,(SELECT MAX(PLAN_CD) FROM SCH_MAPPING_INS WHERE COMPANY_CD=A.COMPANY_CD AND UPPER(INS_SCH_NAME)=UPPER(A.PLAN_NAME))PLAN_NO  FROM BAJAJ_DUE_DATA A WHERE  "
sql = sql & "  PLAN_NAME IS NOT NULL AND mon_no=" & MonNo & " and YEAR_NO=2009 and company_cd='" & CCODE & "'"
sql = sql & ") WHERE PLAN_NO IS NULL "
Set RS = MyConn.Execute(sql)
lst_add_scheme.Clear
lst_sch_sh.Clear
lst_sch.Clear
While Not RS.EOF
    MyItem = ""
    schCodeList = ""
    MyPlanNo = ""
    Set RS1 = MyConn.Execute("select plan_cd from sch_mapping_ins where upper(trim(ins_sch_name))= '" & UCase(Trim(RS(0))) & "' and company_cd='" & CCODE & "' ")
    If Not RS1.EOF Then
        MyPlan = Split(RS1.Fields("plan_cd"), ",")
        For K = LBound(MyPlan) To UBound(MyPlan)
            MyPlanNo = MyPlanNo & MyPlan(K) & ","
        Next
        MyPlanNo = Mid(MyPlanNo, 1, Len(MyPlanNo) - 1)
        MyItem = ""
        schCodeList = ""
    End If
    If Not RS1.EOF Then
         Set S_name = MyConn.Execute("select plan,plan_no from bajaj_plan_master where  plan_no in (" & MyPlanNo & " )")
         If Not S_name.EOF Then
            While Not S_name.EOF
                MyItem = MyItem & "'" & S_name(0) & "',"
                schCodeList = schCodeList & "'" & S_name(1) & "',"
                S_name.MoveNext
            Wend
            lst_add_scheme.AddItem RS(0) & "->" & MyItem
         End If
    Else
        lst_sch_sh.AddItem RS(0)
    End If
    DoEvents
    RS.MoveNext
 Wend
 Set RS = MyConn.Execute("select distinct plan,plan_no from bajaj_plan_master where company_cd='" & CCODE & "' order by plan")
 lst_sch.Clear
 While Not RS.EOF
        lst_sch.AddItem RS(0) & "#" & RS(1)
        RS.MoveNext
 Wend
End Sub

Private Sub Form_Load()
Dim sql As String
If MyConn.State = 1 Then MyConn.Close
'MyConn.open ("dsn=test;user id=wealthmaker;password=DB$JuL#baJ#2016")
MyConn.open ("dsn=test;user id=wealthmaker;password=" & DataBasePassword & "")

MyPlanNo = ""
Set RS = MyConn.Execute("select company_cd ,company_name from bajaj_company_master where catagory='L'")
CmbCompany.Clear
While Not RS.EOF
    CmbCompany.AddItem RS.Fields("company_name") & Space(100) & "#" & RS.Fields("company_Cd")
    RS.MoveNext
Wend
RS.Close
Set RS = Nothing
End Sub
Private Sub Pic_de_sch_Click()
Dim Par() As String
 If lst_add_scheme.ListCount = 0 Then
    MsgBox "No Records Found", vbInformation, "Final Import"
  Exit Sub
 End If
For i = 0 To lst_add_scheme.ListCount - 1
    If lst_add_scheme.Selected(i) = True Then
        Par = Split(lst_add_scheme.List(i), "->")
        MyConn.Execute ("delete from sch_mapping_ins where upper(trim(ins_sch_name))='" & UCase(Trim(Par(0))) & "' and company_cd ='" & CCODE & "' ")
        lst_sch_sh.Refresh
        lst_add_scheme.RemoveItem (i)
        Exit For
    End If
Next
End Sub

Private Sub pic_imp_sch_Click()
Dim Wealth_PlanNO As String
Dim Wealth_Plan1 As String
Dim MyTotPlan As String
Dim MyTotItem As String
Dim Excel_Plan As String
Dim Wealth_Plan() As String

If lst_sch_sh.SelCount = 0 Then
    MsgBox "Please Select Excel Scheme Name", vbInformation
    Exit Sub
End If

If lst_sch.SelCount = 0 Then
    MsgBox "Please Select BackOffice Scheme Name", vbInformation
    Exit Sub
End If
MyTotPlan = ""
MyTotItem = ""
For i = 0 To lst_sch.ListCount - 1
        If lst_sch.Selected(i) = True Then
            Wealth_Plan1 = lst_sch.List(i)
            Wealth_Plan = Split(Wealth_Plan1, "#")
            MyTotPlan = MyTotPlan & Wealth_Plan(1) & ","
            MyTotItem = MyTotItem & Wealth_Plan(0) & ","
            'Exit For
        End If
Next
MyTotPlan = Mid(MyTotPlan, 1, Len(MyTotPlan) - 1)
MyTotItem = Mid(MyTotItem, 1, Len(MyTotItem) - 1)
For i = 0 To lst_sch_sh.ListCount - 1
    If lst_sch_sh.Selected(i) = True Then
        Excel_Plan = lst_sch_sh.List(i)
        Exit For
    End If
Next
'Wealth_Plan = Split(Wealth_Plan1, "#")
MyConn.Execute "insert into sch_mapping_ins (company_cd,ins_sch_name,plan_cd) values('" & CCODE & "','" & UCase(Trim(Excel_Plan)) & "','" & MyTotPlan & "' )"
lst_add_scheme.AddItem Excel_Plan & "->" & MyTotItem
For i = 0 To lst_sch_sh.ListCount - 1
    If lst_sch_sh.Selected(i) = True Then
        lst_sch_sh.RemoveItem (i)
        Exit For
    End If
Next
For i = 0 To lst_sch.ListCount - 1
    lst_sch.Selected(i) = False
Next
End Sub



