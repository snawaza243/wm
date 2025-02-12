Dim mCount As Integer
Dim branch_cd As String
Dim Rm_cd As String
Private Sub cmbbranch_Change()
Dim rsData As New ADODB.Recordset

    cmbOldRM.Clear
    If SRmCode = "" Then
        If cmbbranch.ListIndex <> -1 Then
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where source=" & cmbbranch.List(cmbbranch.ListIndex, 1) & " and (type='A' or type is null) order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where source in(" & Branches & ") and (type='A' or type is null) order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    Else
        rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code=" & SRmCode & " order by RM_NAME", MyConn, adOpenForwardOnly
    End If
    
    j = 0
    While Not rsData.EOF
        cmbOldRM.AddItem rsData("RM_NAME")
        cmbOldRM.List(j, 1) = rsData("PAYROLL_ID")
        cmbOldRM.List(j, 2) = rsData("RM_CODE")
        j = j + 1
        rsData.MoveNext
    Wend
    rsData.Close
    Set rsData = Nothing

End Sub

Private Sub cmbBranch_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbbranch.MatchFound = False Then
        cmbbranch.ListIndex = -1
    End If

End Sub

Public Sub cmbClient_Change()
On Error Resume Next
Dim RsInvestor As New ADODB.Recordset
Dim rsCount As New ADODB.Recordset
Dim i As Long

    msfgInvestors.Rows = 1
    msfgInvestors.Rows = 2
    If cmbClient.ListIndex <> -1 Then
        i = 1
        RsInvestor.open "select i.investor_name,i.address1,i.address2,c.city_name,i.PINCODE,i.inv_code from investor_master i,city_master c where i.city_id=c.city_id(+) and source_id=" & cmbClient.List(cmbClient.ListIndex, 1) & " order by i.investor_name", MyConn, adOpenForwardOnly
        While Not RsInvestor.EOF
            rsCount.open "select count(*) from transaction_st where client_code=" & RsInvestor("inv_code"), MyConn, adOpenForwardOnly
            msfgInvestors.TextMatrix(i, 0) = rsCount(0)
            msfgInvestors.TextMatrix(i, 1) = StrConv(RsInvestor("investor_name"), vbProperCase)
            msfgInvestors.TextMatrix(i, 2) = StrConv(RsInvestor("address1"), vbProperCase)
            msfgInvestors.TextMatrix(i, 3) = StrConv(RsInvestor("address2"), vbProperCase)
            msfgInvestors.TextMatrix(i, 4) = StrConv(RsInvestor("city_name"), vbProperCase)
            msfgInvestors.TextMatrix(i, 5) = RsInvestor("PINCODE")
            msfgInvestors.TextMatrix(i, 6) = RsInvestor("inv_code")
            msfgInvestors.Rows = msfgInvestors.Rows + 1
            i = i + 1
            RsInvestor.MoveNext
            rsCount.Close
        Wend
        RsInvestor.Close
        If msfgInvestors.Rows > 2 Then
            msfgInvestors.Rows = msfgInvestors.Rows - 1
        End If
    End If
    Set RsInvestor = Nothing
End Sub

Private Sub cmbClient_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbClient.MatchFound = False Then
        cmbClient.ListIndex = -1
    End If
End Sub

Private Sub cmbOldRM_Change()
'Dim rsclient As New ADODB.Recordset
'Dim i As Long
'    cmbClient.Clear
'    If cmbOldRM.ListIndex <> -1 Then
'        i = 0
'        'rsclient.open "select client_name,client_code from client_master where rm_code=" & cmbOldRM.List(cmbOldRM.ListIndex, 2) & " order by client_name", myconn, adOpenForwardOnly
'        rsclient.open "select max(client_name),i.source_id from client_master c,investor_master i where c.client_code=i.source_id and c.rm_code=" & cmbOldRM.List(cmbOldRM.ListIndex, 2) & " group by i.source_id having count(*) >1 order by max(client_name)", myconn, adOpenForwardOnly
'        While Not rsclient.EOF
'            cmbClient.AddItem rsclient(0)
'            cmbClient.List(i, 1) = rsclient(1)
'            rsclient.MoveNext
'            i = i + 1
'        Wend
'        rsclient.Close
'    End If
'    Set rsclient = Nothing
End Sub

Private Sub cmbOldRM_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbOldRM.MatchFound = False Then
        cmbOldRM.ListIndex = -1
    End If

End Sub

Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub cmdMain_Click()
    If msfgInvestors.Rows >= 2 Then
        If msfgMain.TextMatrix(1, 1) <> "" Then
            If MsgBox("Main Investor Already Selected. Sure to Proceed ?", vbQuestion + vbYesNo) = vbYes Then
                msfgMain.Rows = 1
                msfgMain.Rows = 2
                msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
                msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
                msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
                
                msfgMergedInvestors.Rows = 1
            End If
        Else
            msfgMain.Rows = 1
            msfgMain.Rows = 2
            msfgMain.TextMatrix(1, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
            msfgMain.TextMatrix(1, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
            msfgMain.TextMatrix(1, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
        
        End If
    End If
End Sub

Private Sub cmdMerge_Click()
Dim i As Long
On Error GoTo errortrap
Dim rsData As New ADODB.Recordset
Dim New_Inv_Code As String
Dim flag As Boolean
Dim rsClient As New ADODB.Recordset
Dim rsclient1 As New ADODB.Recordset
Dim rsInv_check As New ADODB.Recordset
Dim rsEnroll As New ADODB.Recordset
Dim rsHead As New ADODB.Recordset
Dim Fam_Head As String
Dim Members1 As String
Dim Members2 As String
Dim Members3 As String

    If Validate = False Then
        Exit Sub
    End If
    If MsgBox("Are you Sure to Prcoceed ?", vbQuestion + vbYesNo) = vbYes Then
        Me.MousePointer = 11
        Frame1.Enabled = False
        Frame2.Enabled = False
        Frame3.Enabled = False
        Frame4.Enabled = False
        
        branch_cd = "": Rm_cd = ""
        Find_RM
        
        Fam_Head = ""
'        rsHead.open "SELECT FAMILY_HEAD FROM INVESTOR_MASTER  WHERE SOURCE_ID=" & msfgMain.TextMatrix(1, 2) & " AND substr(inv_Code, 1, 8) = substr(family_head, 1, 8) and fpl_date is not null", myconn, adOpenForwardOnly
'        If Not rsHead.EOF Then
'            If Not (IsNull(rsHead("family_head"))) Then
'                Fam_Head = rsHead("family_head")
'            End If
'        End If
'        rsHead.Close
'        Set rsHead = Nothing
        
        For i = 1 To msfgMergedInvestors.Rows - 1
            rsClient.open "Select * from agent_master where agent_code=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenDynamic, adLockPessimistic
            rsclient1.open "Select * from agent_master where agent_code=" & msfgMergedInvestors.TextMatrix(i, 2), MyConn, adOpenForwardOnly
             'rsEnroll.open "Select min(creation_date) from client_master where client_code=" & msfgMain.TextMatrix(1, 2) & " or client_code=" & msfgMergedInvestors.TextMatrix(i, 2), myconn, adOpenForwardOnly
            rsData.open "select inv_code,investor_name from investor_master where source_id=" & msfgMergedInvestors.TextMatrix(i, 2), MyConn, adOpenForwardOnly
            MyConn.BeginTrans
            flag = True
            While Not rsData.EOF
                'rsInv_check.open "Select inv_code from investor_master where source_id=" & msfgMain.TextMatrix(i, 2) & " and substr(trim(upper(investor_name)),1,6)='" & Left(Trim(UCase(rsData("investor_name"))), 6) & "'", myconn, adOpenForwardOnly
                rsInv_check.open "Select inv_code from investor_master where source_id=" & msfgMain.TextMatrix(1, 2) & " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '%" & Left(Replace(Replace(Trim(UCase(rsData("investor_name"))), ".", ""), " ", ""), 8) & "%' and instr(trim(upper(investor_name)),'HUF')=0  ", MyConn, adOpenForwardOnly
                If rsInv_check.EOF = False Then
                    New_Inv_Code = rsInv_check("inv_code")
                Else
                    mCount = mCount + 1
                    If mCount >= 999 Then
                        New_Inv_Code = msfgMain.TextMatrix(1, 2) & Format(mCount, "00000")
                    Else
                        New_Inv_Code = msfgMain.TextMatrix(1, 2) & Format(mCount, "000")
                    End If
                    MyConn.Execute "update INVESTOR_MASTER       set   SOURCE_ID=" & msfgMain.TextMatrix(1, 2) & ",BRANCH_CODE=" & branch_cd & ",RM_CODE=" & Rm_cd & ",INV_CODE='" & New_Inv_Code & "' where INV_CODE=" & rsData("INV_CODE")
                End If
                MyConn.Execute "update fp_investor set familyhead_code='" & New_Inv_Code & "' where familyhead_code='" & rsData("inv_code") & "'"
                MyConn.Execute "update fp_investor set fam_mem1=replace(fam_mem1," & rsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(rsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                MyConn.Execute "update fp_investor set fam_mem2=replace(fam_mem2," & rsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(rsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                MyConn.Execute "update fp_investor set fam_mem3=replace(fam_mem3," & rsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(rsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                
                MyConn.Execute "update TRANSACTION_ST        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where client_code=" & rsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_MF_TEMP1  set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                
                
                MyConn.Execute "update TRANSACTION_ST@mf.bajajcapital        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_STTEMP    set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where client_code=" & rsData("INV_CODE")
                MyConn.Execute "update REDEM@mf.bajajcapital                 set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                'myconn.Execute "update IMPORT_FINAL_TRAN     set      inv_code=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update INVESTOR_FOLIO@mf.bajajcapital        set INVESTOR_CODE=" & New_Inv_Code & " where INVESTOR_CODE=" & rsData("INV_CODE")
                'myconn.Execute "update INVESTOR_HISTORY      set      inv_code=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update INVESTOR_MASTER_IPO   set      inv_code=" & New_Inv_Code & ",AGENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & rsData("INV_CODE")
                'myconn.Execute "update MAPPEDCAMS            set   client_code=" & msfgMain.TextMatrix(1, 2) & " where client_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update REVERTAL_TRANSACTION  set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_IPO       set      inv_code=" & New_Inv_Code & ",AGENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & rsData("INV_CODE")
                MyConn.Execute "update TRAN_PAYOUT@mf.bajajcapital           set      inv_code=" & New_Inv_Code & " where inv_code=" & rsData("INV_CODE")
                'myconn.Execute "update BAJAJ_AR_HEAD         set     CLIENT_CD=" & New_Inv_Code & ",BRANCH_CD=" & Branch_cd & " where CLIENT_CD=" & rsData("INV_CODE")
                MyConn.Execute "update BAJAJ_AR_HEAD         set     CLIENT_CD=" & New_Inv_Code & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where CLIENT_CD=" & rsData("INV_CODE")
                MyConn.Execute "update TRAN_NET_BALANCE6@mf.bajajcapital      set   CLIENT_CODE=" & New_Inv_Code & " where CLIENT_CODE=" & rsData("INV_CODE")
                MyConn.Execute "update TRAN_IPO              set      inv_code=" & New_Inv_Code & ",CLIENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & rsData("INV_CODE")
                MyConn.Execute "update TRAN_LEAD             set      inv_code=" & New_Inv_Code & " where inv_code=" & rsData("INV_CODE")
                MyConn.Execute "update LEADS.LEAD_DETAIL     set      inv_code=" & New_Inv_Code & " where inv_code=" & rsData("INV_CODE")
                MyConn.Execute "update port_TRANSACTION_ST@mf.bajajcapital        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                '######################33     By Vinay Hatwal      ####################################3
                                
                 MyConn.Execute "update online_transaction_st   set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & rsData("INV_CODE")
                'History of updations (investor wise)
                 MyConn.Execute "insert into Inv_Del_Hist_Agent_Merge (inv_code,new_inv_code,UpdateOn,UpdatedBy) values ('" & rsData("INV_CODE") & "','" & New_Inv_Code & "',to_date('" & Format(Date, "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & Glbloginid & "')"

                '######################################################################################3
                
                
                '----------------------------------
                'By Vinay Hatwal   dt.  26-jun-2008
                'Online Just Trade
                MyConn.Execute "update transaction_st_online    set client_code='" & New_Inv_Code & "'      where client_code='" & rsData("INV_CODE") & "'"
                MyConn.Execute "update online_client_request    set inv_code='" & New_Inv_Code & "'         where inv_code='" & rsData("INV_CODE") & "'"
                MyConn.Execute "update online_client_request_hist    set inv_code='" & New_Inv_Code & "'         where inv_code='" & rsData("INV_CODE") & "'"
                MyConn.Execute "update online_business_summary  set client_codewm='" & New_Inv_Code & "'    where client_codewm='" & rsData("INV_CODE") & "'"
                MyConn.Execute "update offline_business_summary set client_codewm='" & New_Inv_Code & "'    where client_codewm='" & rsData("INV_CODE") & "'"
                '----------------------------------
                
                
                If rsInv_check.EOF = False Then
                    'write code here if fpl_date is not null and implemented date is not null for this investor
                    ' then first update before deletion
                    MyConn.Execute "insert into client_inv_merge_log values('" & New_Inv_Code & "','" & rsData("INV_CODE") & "','" & Glbloginid & "',sysdate)"
                    MyConn.Execute "insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=" & rsData("INV_CODE")
                    MyConn.Execute "Delete from INVESTOR_MASTER  where inv_code=" & rsData("INV_CODE")
                    MyConn.Execute "Delete from INVESTOR_MASTER@mf.bajajcapital  where inv_code=" & rsData("INV_CODE")
                End If
                rsInv_check.Close                                                                                                                                ''myconn.Execute "delete from INVESTOR_MASTER  where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                rsData.MoveNext
            Wend
            
            MyConn.Execute "update INVESTOR_MASTER           set BRANCH_CODE=" & branch_cd & ",RM_CODE=" & Rm_cd & ",modify_DATE=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where source_id=" & msfgMain.TextMatrix(1, 2)
            '******Agent MERGING*****
            MyConn.Execute "update agent_MASTER             set sourceid=" & branch_cd & ",RM_CODE=" & Rm_cd & ",modify_DATE=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where agent_code=" & msfgMain.TextMatrix(1, 2)
       
            MyConn.Execute "update TRANSACTION_ST            set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where source_code=" & msfgMain.TextMatrix(1, 2)
            MyConn.Execute "update TRANSACTION_MF_TEMP1      set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & " where source_code=" & msfgMain.TextMatrix(1, 2)
            
            MyConn.Execute "update TRANSACTION_ST@mf.bajajcapital            set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & " where source_code=" & msfgMain.TextMatrix(1, 2)
            MyConn.Execute "update port_TRANSACTION_ST@mf.bajajcapital            set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & " where source_code=" & msfgMain.TextMatrix(1, 2)
            MyConn.Execute "update TRANSACTION_STTEMP        set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where source_code=" & msfgMain.TextMatrix(1, 2)
            MyConn.Execute "update REDEM                     set   branch_code=" & branch_cd & ",rmcode=" & Rm_cd & " where source_code=" & msfgMain.TextMatrix(1, 2)
            '##########################3   Begin --- Previously Written But Empty Table   ###################################################
            'myconn.Execute "update REVERTAL_TRANSACTION      set   branch_code=" & Branch_Cd & ",rmcode=" & Rm_cd & " where source_code=" & msfgMain.TextMatrix(1, 2)
            'myconn.Execute "update CLIENT_VEHICLE            set client_code=" & msfgMain.TextMatrix(1, 2) & " where client_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            'myconn.Execute "update REFERENCE_MASTER          set client_code=" & msfgMain.TextMatrix(1, 2) & " where client_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            'myconn.Execute "update RELATION_MASTER           set client_code=" & msfgMain.TextMatrix(1, 2) & " where client_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            'myconn.Execute "update BILL_DETAIL_PAID          set AGENT_code=" & msfgMain.TextMatrix(1, 2) & " where AGENT_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            '##########################3   End --- Previously Written But Empty Table   ###################################################
            
            
            
            MyConn.Execute "update PAYMENT_DETAIL            set agent_code=" & msfgMain.TextMatrix(1, 2) & " where agent_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            MyConn.Execute "update LEDGER                    set AGENT_code=" & msfgMain.TextMatrix(1, 2) & " where AGENT_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            
            
            
            '######################33     By Vinay Hatwal      ####################################3
            MyConn.Execute "update upfront_paid set client_agent_code='" & msfgMain.TextMatrix(1, 2) & "' where client_agent_code='" & msfgMergedInvestors.TextMatrix(i, 2) & "'"
            MyConn.Execute "update ADD_INCENTIVE_PAID set client_agent_code='" & msfgMain.TextMatrix(1, 2) & "' where client_agent_code='" & msfgMergedInvestors.TextMatrix(i, 2) & "'"
            MyConn.Execute "update SIP_BROKER_BILLING1 set SOURCE_CODE='" & msfgMain.TextMatrix(1, 2) & "' where SOURCE_CODE='" & msfgMergedInvestors.TextMatrix(i, 2) & "'"
            MyConn.Execute "update STP_BROKER_BILLING1 set SOURCE_CODE='" & msfgMain.TextMatrix(1, 2) & "' where SOURCE_CODE='" & msfgMergedInvestors.TextMatrix(i, 2) & "'"
            
            
            ' Paid ANA Subscription table
            MyConn.Execute "update ADVISORSUBENTRY set anacode='" & msfgMain.TextMatrix(1, 2) & "' where anacode='" & msfgMergedInvestors.TextMatrix(i, 2) & "'"
            
            
            
            '######################################################################################3
            
            
            
            '**********************
'            If IsNull(rsClient("FP")) And Not (IsNull(rsclient1("FP"))) Then
'                If rsclient1("FP") = "Y" Then rsClient("FP") = rsclient1("FP")
'            End If
'            If IsNull(rsClient("FPF_DATE")) And Not (IsNull(rsclient1("FPF_DATE"))) Then
'                rsClient("FPF_DATE") = rsclient1("FPF_DATE")
'            End If
'            If IsNull(rsClient("FPL_DATE")) And Not (IsNull(rsclient1("FPL_DATE"))) Then
'                rsClient("FPL_DATE") = rsclient1("FPL_DATE")
'            End If
'            If IsNull(rsClient("BM_REASON")) And Not (IsNull(rsclient1("BM_REASON"))) Then
'                rsClient("BM_REASON") = rsclient1("BM_REASON")
'            End If
'            If IsNull(rsClient("BM_REMARK")) And Not (IsNull(rsclient1("BM_REMARK"))) Then
'                rsClient("BM_REMARK") = rsclient1("BM_REMARK")
'            End If
            If (IsNull(rsClient("PHONE")) Or Trim(rsClient("PHONE")) = "") And Not (IsNull(rsclient1("PHONE"))) Then
                rsClient("PHONE") = rsclient1("PHONE")
            End If
            If IsNull(rsClient("EMAIL")) And Not (IsNull(rsclient1("EMAIL"))) Then
                rsClient("EMAIL") = rsclient1("EMAIL")
            End If
            If IsNull(rsClient("MOBILE")) And (Not (IsNull(rsclient1("MOBILE"))) And rsclient1("MOBILE") <> "0") Then
                rsClient("MOBILE") = rsclient1("MOBILE")
            End If
            If IsNull(rsClient("PINCODE")) And Not (IsNull(rsclient1("PINCODE"))) Then
                rsClient("PINCODE") = rsclient1("PINCODE")
            End If
            If IsNull(rsClient("CITY_ID")) And Not (IsNull(rsclient1("CITY_ID"))) Then
                rsClient("CITY_ID") = rsclient1("CITY_ID")
            End If
            If IsNull(rsClient("DOB")) And Not (IsNull(rsclient1("DOB"))) Then
                rsClient("DOB") = rsclient1("DOB")
            End If
            If IsNull(rsClient("EXIST_CODE")) And Not (IsNull(rsclient1("EXIST_CODE"))) Then
                rsClient("EXIST_CODE") = rsclient1("EXIST_CODE")
            End If
'            If IsNull(rsClient("ANN_DT")) And Not (IsNull(rsclient1("ANN_DT"))) Then
'                rsClient("ANN_DT") = rsclient1("ANN_DT")
'            End If
'            If IsNull(rsClient("ANN_MM")) And Not (IsNull(rsclient1("ANN_MM"))) Then
'                rsClient("ANN_MM") = rsclient1("ANN_MM")
'            End If

            If IsNull(rsClient("TDS")) And Not (IsNull(rsclient1("TDS"))) Then
                rsClient("TDS") = rsclient1("TDS")
            End If
            If IsNull(rsClient("INTRODUCER")) And Not (IsNull(rsclient1("INTRODUCER"))) Then
                rsClient("INTRODUCER") = rsclient1("INTRODUCER")
            End If
            
'            If IsNull(rsClient("Phone1")) And Not (IsNull(rsclient1("phone1"))) Then
'                rsClient("phone1") = rsclient1("phone1")
'            End If
'            If IsNull(rsClient("Phone2")) And Not (IsNull(rsclient1("Phone2"))) Then
'                rsClient("Phone2") = rsclient1("Phone2")
'            End If
'            If IsNull(rsClient("Phone3")) And Not (IsNull(rsclient1("Phone3"))) Then
'                rsClient("Phone3") = rsclient1("Phone3")
'            End If
'

'            If IsNull(rsClient("INTRO_CODE")) And (Not (IsNull(rsclient1("INTRO_CODE"))) And rsclient1("INTRO_CODE") <> "0") Then
'                rsClient("INTRO_CODE") = rsclient1("INTRO_CODE")
'            End If
            
            If Not (IsNull(rsclient1("JOININGDATE"))) Then
                If CheckDate(Format(rsClient("JOININGDATE"), "dd/mm/yyyy"), Format(rsclient1("JOININGDATE"), "dd/mm/yyyy")) = False Then
                    rsClient("JOININGDATE") = rsclient1("creation_date")
                End If
            End If
            
            If IsNull(rsClient("JOININGDATE")) And Not (IsNull(rsclient1("JOININGDATE"))) Then
                rsClient("JOININGDATE") = rsclient1("JOININGDATE")
            End If
            
            If Not (IsNull(rsclient1("LAST_TRAN_DT1"))) Then
                If CheckDate(Format(rsclient1("LAST_TRAN_DT1"), "dd/mm/yyyy"), Format(rsClient("LAST_TRAN_DT1"), "dd/mm/yyyy")) = False Then
                    rsClient("LAST_TRAN_DT1") = rsclient1("LAST_TRAN_DT1")
                End If
            End If
            
            If IsNull(rsClient("LAST_TRAN_DT1")) And Not (IsNull(rsclient1("LAST_TRAN_DT1"))) Then
                rsClient("LAST_TRAN_DT1") = rsclient1("LAST_TRAN_DT1")
            End If
            
            rsClient.Update
            rsClient.Close
            rsclient1.Close
            '**********************
            'Pankaj
            MyConn.Execute "insert into client_inv_merge_log values('" & msfgMain.TextMatrix(1, 2) & "','" & msfgMergedInvestors.TextMatrix(i, 2) & "','" & Glbloginid & "',sysdate)"
            '------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            MyConn.Execute "insert into agent_del select * from agent_master where agent_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            MyConn.Execute "Delete from agent_master where agent_code=" & msfgMergedInvestors.TextMatrix(i, 2)
            MyConn.Execute "Delete from agent_master@mf.bajajcapital where agent_code=" & msfgMergedInvestors.TextMatrix(i, 2)
             
             
            '######################33     By Vinay Hatwal      ####################################3
                    'History of updations (investor wise)
                    MyConn.Execute "insert into Agent_Del_Hist_Agent_Merge (agent_code,new_agent_code,UpdateOn,UpdatedBy) values ('" & msfgMergedInvestors.TextMatrix(i, 2) & "','" & msfgMain.TextMatrix(1, 2) & "',to_date('" & Format(Date, "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & Glbloginid & "')"
            '######################33     By Vinay Hatwal      ####################################3
            
            
            'Committing Transactions
            MyConn.CommitTrans
            'myconn.RollbackTrans
            
            
            If rsHead.State = 1 Then rsHead.Close
            Members = ""
            Fam_Head = ""
            rsHead.open "Select * from fp_investor where substr(familyhead_code,1,8)=" & msfgMain.TextMatrix(1, 2) & " and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc ", MyConn, adOpenKeyset
            If rsHead.RecordCount > 1 Then
                Fam_Head = rsHead("familyhead_code")
                Members1 = rsHead("fam_mem1")
                'If Not (IsNull(rsHead("fam_mem2"))) Then Members2 = rsHead("fam_mem2")
                'If Not (IsNull(rsHead("fam_mem3"))) Then Members3 = rsHead("fam_mem3")
                MyConn.Execute "insert into dup_fp_investor select * from fp_investor where familyhead_code=" & Fam_Head
                MyConn.Execute "update fp_investor set fam_mem1=fam_mem1||'#'||'" & Members1 & "' where substr(familyhead_code,1,8)=" & msfgMain.TextMatrix(1, 2) & " and (fp_type='Snapshot' or Fp_type='Comprehensive')"
                'myconn.Execute "delete from fp_investor where familyhead_code=" & Fam_Head
                'myconn.Execute "insert into dup_fp_investor select * from fp_investor where familyhead_code=" & Fam_Head
            End If
            
            flag = False
            rsData.Close
        Next i
        
        Me.MousePointer = 0
        MsgBox "Client(s) Merged Successfully", vbInformation
                
        Set rsData = Nothing
        Set rsClient = Nothing
        Set rsclient1 = Nothing
        Set rsInv_check = Nothing
        
        Frame1.Enabled = True
        Frame2.Enabled = True
        Frame3.Enabled = True
        Frame4.Enabled = True
        
        msfgInvestors.Rows = 1
        msfgMain.Rows = 1
        msfgMergedInvestors.Rows = 1
        'msfgInvestors.Rows = 2
        msfgMain.Rows = 2
        SetGrid
        'cmbClient_Change
    End If
    Exit Sub
errortrap:
    'On Error resume Next
    '$$$$$$$$$$$$$$$$$$$$$$$$$$4
    MyConn.RollbackTrans
    
    
    
    
    Me.MousePointer = 0
    MsgBox err.Description
    If flag = True Then
        MyConn.RollbackTrans
    End If
    Frame1.Enabled = True
    Frame2.Enabled = True
    Frame3.Enabled = True
    Frame4.Enabled = True
    
End Sub

Private Sub cmdMergedInvestors_Click()
Dim i As Long
    If msfgMain.TextMatrix(1, 1) <> "" Then
        If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMain.TextMatrix(1, 2) Then
            MsgBox "This Agent is Already Choosen as Main Client", vbInformation
            Exit Sub
        End If
        For i = 1 To msfgMergedInvestors.Rows - 1
            If msfgInvestors.TextMatrix(msfgInvestors.Row, 9) = msfgMergedInvestors.TextMatrix(i, 2) Then
                MsgBox "This Agent is Already Exist in the List", vbInformation
                Exit Sub
            End If
        Next i
'        If msfgMergedInvestors.Rows > 2 Then
'        End If
        msfgMergedInvestors.Rows = msfgMergedInvestors.Rows + 1
        i = msfgMergedInvestors.Rows - 1
        msfgMergedInvestors.TextMatrix(i, 0) = msfgInvestors.TextMatrix(msfgInvestors.Row, 0)
        msfgMergedInvestors.TextMatrix(i, 1) = msfgInvestors.TextMatrix(msfgInvestors.Row, 2)
        msfgMergedInvestors.TextMatrix(i, 2) = msfgInvestors.TextMatrix(msfgInvestors.Row, 9)
    
    Else
        MsgBox "Please Select Main Investor First !", vbInformation
    End If

End Sub

Private Sub CMDRESET_Click()
        Frame1.Enabled = True
        Frame2.Enabled = True
        Frame3.Enabled = True
        Frame4.Enabled = True
        
        msfgInvestors.Rows = 1
        msfgMain.Rows = 1
        msfgMergedInvestors.Rows = 1
        'msfgInvestors.Rows = 2
        msfgMain.Rows = 2
        SetGrid
        cmbbranch.ListIndex = -1
        cmbOldRM.ListIndex = -1
End Sub

Private Sub Form_Activate()
Dim Rs_rm As New ADODB.Recordset
    cmbbranch.Clear
    If GlbDataFilter = "72" Then
        Rs_rm.open "Select branch_code,branch_name from branch_master where category_id not in('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly
    Else
        Rs_rm.open "Select branch_code,branch_name from branch_master where branch_code in (" & Branches & ") and category_id not in('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly
    End If
    
    j = 0
    While Not Rs_rm.EOF
        cmbbranch.AddItem Rs_rm("branch_name")
        cmbbranch.List(j, 1) = Rs_rm("branch_code")
        j = j + 1
        Rs_rm.MoveNext
    Wend
    Rs_rm.Close
    Set Rs_rm = Nothing
    'cmbbranch.ListIndex = 0

End Sub

Private Sub Form_KeyUp(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyF1 Then
        KeyCode = 0
        Picture1_Click
    End If
End Sub

Private Sub Form_Load()
    Me.Top = (main.Height - Me.Height) / 2 - 600
    Me.Left = (main.width - Me.width) / 2
    Me.Icon = LoadPicture(App.Path & "\W.ICO")
    
    Drawgrid
    

End Sub

Private Sub Drawgrid()

    msfgInvestors.TextMatrix(0, 0) = "Total Transactions"
    msfgInvestors.TextMatrix(0, 1) = "Create Date"
    msfgInvestors.TextMatrix(0, 2) = "Agent Name"
    msfgInvestors.TextMatrix(0, 3) = "Address1"
    msfgInvestors.TextMatrix(0, 4) = "Address2"
    msfgInvestors.TextMatrix(0, 5) = "City"
    msfgInvestors.TextMatrix(0, 6) = "Branch"
    msfgInvestors.TextMatrix(0, 7) = "Phone"
    msfgInvestors.TextMatrix(0, 8) = "PIN"
    msfgInvestors.TextMatrix(0, 9) = "Code"
    msfgInvestors.TextMatrix(0, 10) = "RM"
    msfgInvestors.TextMatrix(0, 11) = "Last Transaction"
    
    msfgMain.TextMatrix(0, 0) = "Tot Tr."
    msfgMain.TextMatrix(0, 1) = "Agent Name"
    msfgMain.TextMatrix(0, 2) = "Code"
    
    SetGrid
        
    msfgInvestors.RowHeight(0) = 400
    msfgInvestors.WordWrap = True
    
    msfgInvestors.ColAlignment(0) = 4
    msfgMain.ColAlignment(0) = 4
    msfgMergedInvestors.ColAlignment(0) = 4

    msfgInvestors.ColAlignment(1) = vbLeftJustify
    msfgInvestors.ColAlignment(2) = vbLeftJustify
    msfgInvestors.ColAlignment(3) = vbLeftJustify
    msfgInvestors.ColAlignment(4) = vbLeftJustify
    msfgInvestors.ColAlignment(5) = vbLeftJustify
    msfgInvestors.ColAlignment(6) = vbLeftJustify
    msfgInvestors.ColAlignment(7) = vbLeftJustify
    msfgInvestors.ColAlignment(8) = vbLeftJustify
    msfgInvestors.ColAlignment(9) = vbLeftJustify
    msfgInvestors.ColAlignment(10) = vbLeftJustify
    msfgInvestors.ColAlignment(11) = vbLeftJustify
    
    msfgInvestors.ColWidth(0) = 1000
    msfgInvestors.ColWidth(1) = 1000
    msfgInvestors.ColWidth(2) = 2500
    msfgInvestors.ColWidth(3) = 2500
    msfgInvestors.ColWidth(4) = 2500
    msfgInvestors.ColWidth(5) = 1500
    msfgInvestors.ColWidth(6) = 800
    msfgInvestors.ColWidth(7) = 1200
    msfgInvestors.ColWidth(10) = 2000
    msfgInvestors.ColWidth(11) = 1000
    
    msfgMain.ColWidth(0) = 800
    msfgMain.ColWidth(1) = 2500
    msfgMain.ColWidth(2) = 1200
    
    msfgMergedInvestors.ColWidth(0) = 800
    msfgMergedInvestors.ColWidth(1) = 2500
    msfgMergedInvestors.ColWidth(2) = 1200
    
    msfgInvestors.Rows = 1
End Sub

Private Sub msfgInvestors_DblClick()
    DeleteRowMerge msfgInvestors
End Sub

Private Sub msfgInvestors_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 46 Then
        DeleteRowMerge msfgInvestors
        'SetGrid
    End If

End Sub

Private Sub msfgMergedInvestors_DblClick()
    DeleteRowMerge msfgMergedInvestors
    'SetGrid
End Sub

Private Sub msfgMergedInvestors_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 46 Then
        DeleteRowMerge msfgMergedInvestors
        'SetGrid
    End If

End Sub

Private Function Validate() As Boolean
Dim rsData As New ADODB.Recordset
Dim sCount As Integer
Dim i As Integer
    Validate = False
    mCount = 0
    sCount = 0
    If msfgMain.TextMatrix(1, 1) <> "" Then
        If msfgMergedInvestors.Rows >= 2 Then
            Validate = True
        Else
            MsgBox "There is no Data to Merge ", vbInformation
            Validate = False
            Exit Function
        End If
    Else
        MsgBox "There is no Data to Merge ", vbInformation
        Validate = False
        Exit Function
    End If
    'prob solve ankit 19/10/2007
    rsData.open "select max(substr(inv_code,9,5)) cnt from investor_master where source_id=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenForwardOnly
    If rsData.EOF = False Then
        If Not IsNull(rsData("cnt")) Then
           mCount = Val(rsData("cnt"))
        End If
    End If
    rsData.Close
    
    For i = 1 To msfgMergedInvestors.Rows - 1
    'prob solve ankit 19/10/2007
    
  '  rsData.open "select lpad(max(substr(inv_code,9,5)),5,0) cnt from investor_master where source_id=" & msfgMergedInvestors.TextMatrix(1, 2), myconn, adOpenForwardOnly
        rsData.open "select max(substr(inv_code,9,5)) cnt from investor_master where source_id=" & msfgMergedInvestors.TextMatrix(1, 2), MyConn, adOpenForwardOnly
        If Not (IsNull(rsData("cnt"))) Then sCount = sCount + Val(rsData("cnt"))
        rsData.Close
        If mCount + sCount > 99999 Then
            MsgBox "Investors Exceeded 99999 Limit ", vbInformation
            Validate = False
            Exit For
        End If
    Next
End Function

Private Sub SetGrid()
   
    msfgMergedInvestors.TextMatrix(0, 0) = "Tot Tr."
    msfgMergedInvestors.TextMatrix(0, 1) = "Client Name"
    msfgMergedInvestors.TextMatrix(0, 2) = "Code"

End Sub

Private Sub Picture1_Click()
    'If cmbBranch.ListIndex <> -1 Then
        treeselected = "N"
        Set frmtree_search.currentForm = Nothing
        Set frmtree_search.currentForm = frmAgentMerging
        frmtree_search.treeName = "Treeclient"
        
        frmtree_search.Show
        frmtree_search.cmbbranch.Text = cmbbranch.Text
        frmtree_search.txtRM.Text = ""
        If cmbOldRM.ListIndex <> -1 Then
            frmtree_search.txtRM.Text = cmbOldRM.List(cmbOldRM.ListIndex, 2)
        End If
        'frmtree_search.cmbbranch.Enabled = False
        frmtree_search.Cmbcat.Text = "AGENT"
        frmtree_search.Cmbcat.Enabled = False
        frmtree_search.ZOrder
    '    frmtree_search.cmdSelect.Enabled = False
    '    frmtree_search.cmbOldRM.Enabled = False
    '    frmtree_search.Label6.Enabled = False
        'strForm = "frmInvestorMerge"
    'Else
    '    MsgBox "Please Select Branch First ", vbInformation
    '    cmbBranch.SetFocus
    'End If
End Sub


Private Sub Find_RM()
Dim rsTran As New ADODB.Recordset
Dim rsClient_CD As New ADODB.Recordset
Dim i As Integer
    CLIENT_CD = ""
    If msfgMain.TextMatrix(1, 0) <> "0" Then CLIENT_CD = msfgMain.TextMatrix(1, 2) & ","
    For i = 1 To msfgMergedInvestors.Rows - 1
        If msfgMergedInvestors.TextMatrix(i, 0) <> "0" Then CLIENT_CD = CLIENT_CD & msfgMergedInvestors.TextMatrix(i, 2) & ","
    Next i
    If CLIENT_CD <> "" Then
        CLIENT_CD = Left(CLIENT_CD, Len(CLIENT_CD) - 1)
    End If
    If CLIENT_CD <> "" Then
        rsTran.open "select max(tr_date) tr_date,source_code from( " _
                        & "select max(tr_date) tr_date,source_code from transaction_sttemp where source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & " Union All " _
                        & "select max(tr_date) tr_date,source_code from transaction_st where  source_code in(" & CLIENT_CD & ") " _
                           & "group by source_code " _
                        & ") " _
                    & "group by source_code " _
                    & "order by tr_date desc ", MyConn, adOpenForwardOnly
                    
'        If Not (rsTran.EOF) Then
'            rsTran.MoveFirst
'            rsClient_CD.open "Select rm_code,sourceid from client_master where client_code=" & rsTran("source_code"), myconn, adOpenForwardOnly
'            Branch_cd = rsClient_CD("sourceid")
'            Rm_cd = rsClient_CD("rm_code")
'            rsClient_CD.Close
'        End If

        If Not (rsTran.EOF) Then rsTran.MoveFirst
        branch_cd = "10010226"
        While Not (rsTran.EOF) And branch_cd = "10010226"
            rsClient_CD.open "Select rm_code,sourceid from agent_master  where agent_code=" & rsTran("source_code"), MyConn, adOpenForwardOnly
            branch_cd = rsClient_CD("sourceid")
            Rm_cd = rsClient_CD("rm_code")
            rsClient_CD.Close
            rsTran.MoveNext
        Wend


    End If
    If branch_cd = "" Or Rm_cd = "" Or branch_cd = "10010226" Then
        rsClient_CD.open "Select sourceid,rm_code from agent_master where agent_code=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenForwardOnly
        branch_cd = rsClient_CD("sourceid")
        Rm_cd = rsClient_CD("rm_code")
        rsClient_CD.Close
    End If
    
    If rsTran.State = 1 Then rsTran.Close
    Set rsTran = Nothing
    Set rsClient_CD = Nothing
    
End Sub




