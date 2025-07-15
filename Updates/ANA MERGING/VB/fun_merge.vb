Private Sub cmdMerge_Click()
Dim i As Long
On Error GoTo errortrap
Dim RsData As New ADODB.Recordset
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
            RsData.open "select inv_code,investor_name from investor_master where source_id=" & msfgMergedInvestors.TextMatrix(i, 2), MyConn, adOpenForwardOnly
            MyConn.BeginTrans
            flag = True
            While Not RsData.EOF
                'rsInv_check.open "Select inv_code from investor_master where source_id=" & msfgMain.TextMatrix(i, 2) & " and substr(trim(upper(investor_name)),1,6)='" & Left(Trim(UCase(rsData("investor_name"))), 6) & "'", myconn, adOpenForwardOnly

                
                rsInv_check.open "Select inv_code from investor_master where source_id=" & msfgMain.TextMatrix(1, 2) & " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '%" & Left(Replace(Replace(Trim(UCase(RsData("investor_name"))), ".", ""), " ", ""), 8) & "%' and instr(trim(upper(investor_name)),'HUF')=0  ", MyConn, adOpenForwardOnly
                If rsInv_check.EOF = False Then
                    New_Inv_Code = rsInv_check("inv_code")
                Else
                    mCount = mCount + 1
                    If mCount >= 999 Then
                        New_Inv_Code = msfgMain.TextMatrix(1, 2) & Format(mCount, "00000")
                    Else
                        New_Inv_Code = msfgMain.TextMatrix(1, 2) & Format(mCount, "000")
                    End If
                    MyConn.Execute "update INVESTOR_MASTER       set   SOURCE_ID=" & msfgMain.TextMatrix(1, 2) & ",BRANCH_CODE=" & branch_cd & ",RM_CODE=" & Rm_cd & ",INV_CODE='" & New_Inv_Code & "' where INV_CODE=" & RsData("INV_CODE")
                End If
                MyConn.Execute "update fp_investor set familyhead_code='" & New_Inv_Code & "' where familyhead_code='" & RsData("inv_code") & "'"
                MyConn.Execute "update fp_investor set fam_mem1=replace(fam_mem1," & RsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(RsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                MyConn.Execute "update fp_investor set fam_mem2=replace(fam_mem2," & RsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(RsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                MyConn.Execute "update fp_investor set fam_mem3=replace(fam_mem3," & RsData("inv_code") & "," & New_Inv_Code & ") where familyhead_code like '" & Left(RsData("inv_code"), 8) & "%' or familyhead_code like '" & Left(New_Inv_Code, 8) & "%'"
                
                MyConn.Execute "update TRANSACTION_ST        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where client_code=" & RsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_MF_TEMP1  set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                
                
                MyConn.Execute "update TRANSACTION_ST@mf.bajajcapital        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_STTEMP    set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where client_code=" & RsData("INV_CODE")
                MyConn.Execute "update REDEM@mf.bajajcapital                 set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                'myconn.Execute "update IMPORT_FINAL_TRAN     set      inv_code=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update INVESTOR_FOLIO@mf.bajajcapital        set INVESTOR_CODE=" & New_Inv_Code & " where INVESTOR_CODE=" & RsData("INV_CODE")
                'myconn.Execute "update INVESTOR_HISTORY      set      inv_code=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update INVESTOR_MASTER_IPO   set      inv_code=" & New_Inv_Code & ",AGENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & RsData("INV_CODE")
                'myconn.Execute "update MAPPEDCAMS            set   client_code=" & msfgMain.TextMatrix(1, 2) & " where client_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                MyConn.Execute "update REVERTAL_TRANSACTION  set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                MyConn.Execute "update TRANSACTION_IPO       set      inv_code=" & New_Inv_Code & ",AGENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & RsData("INV_CODE")
                MyConn.Execute "update TRAN_PAYOUT@mf.bajajcapital           set      inv_code=" & New_Inv_Code & " where inv_code=" & RsData("INV_CODE")
                'myconn.Execute "update BAJAJ_AR_HEAD         set     CLIENT_CD=" & New_Inv_Code & ",BRANCH_CD=" & Branch_cd & " where CLIENT_CD=" & rsData("INV_CODE")
                MyConn.Execute "update BAJAJ_AR_HEAD         set     CLIENT_CD=" & New_Inv_Code & ",modify_TALISMA=TO_DATE('" & Format(ServerDateTime, "dd/mm/yyyy") & "','DD/MM/RRRR') where CLIENT_CD=" & RsData("INV_CODE")
                MyConn.Execute "update TRAN_NET_BALANCE6@mf.bajajcapital      set   CLIENT_CODE=" & New_Inv_Code & " where CLIENT_CODE=" & RsData("INV_CODE")
                MyConn.Execute "update TRAN_IPO              set      inv_code=" & New_Inv_Code & ",CLIENT_CODE=" & msfgMain.TextMatrix(1, 2) & " where inv_code=" & RsData("INV_CODE")
                MyConn.Execute "update TRAN_LEAD             set      inv_code=" & New_Inv_Code & " where inv_code=" & RsData("INV_CODE")
                MyConn.Execute "update LEADS.LEAD_DETAIL     set      inv_code=" & New_Inv_Code & " where inv_code=" & RsData("INV_CODE")
                MyConn.Execute "update port_TRANSACTION_ST@mf.bajajcapital        set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                '######################33     By Vinay Hatwal      ####################################3
                                
                 MyConn.Execute "update online_transaction_st   set   client_code=" & New_Inv_Code & ",branch_code=" & branch_cd & ",source_code=" & msfgMain.TextMatrix(1, 2) & ",rmcode=" & Rm_cd & " where client_code=" & RsData("INV_CODE")
                'History of updations (investor wise)
                 MyConn.Execute "insert into Inv_Del_Hist_Agent_Merge (inv_code,new_inv_code,UpdateOn,UpdatedBy) values ('" & RsData("INV_CODE") & "','" & New_Inv_Code & "',to_date('" & Format(Date, "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & Glbloginid & "')"

                '######################################################################################3
                
                
                '----------------------------------
                'By Vinay Hatwal   dt.  26-jun-2008
                'Online Just Trade
                MyConn.Execute "update transaction_st_online    set client_code='" & New_Inv_Code & "'      where client_code='" & RsData("INV_CODE") & "'"
                MyConn.Execute "update online_client_request    set inv_code='" & New_Inv_Code & "'         where inv_code='" & RsData("INV_CODE") & "'"
                MyConn.Execute "update online_client_request_hist    set inv_code='" & New_Inv_Code & "'         where inv_code='" & RsData("INV_CODE") & "'"
                MyConn.Execute "update online_business_summary  set client_codewm='" & New_Inv_Code & "'    where client_codewm='" & RsData("INV_CODE") & "'"
                MyConn.Execute "update offline_business_summary set client_codewm='" & New_Inv_Code & "'    where client_codewm='" & RsData("INV_CODE") & "'"
                '----------------------------------
                
                
                If rsInv_check.EOF = False Then
                    'write code here if fpl_date is not null and implemented date is not null for this investor
                    ' then first update before deletion
                    MyConn.Execute "insert into client_inv_merge_log values('" & New_Inv_Code & "','" & RsData("INV_CODE") & "','" & Glbloginid & "',sysdate)"
                    MyConn.Execute "insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=" & RsData("INV_CODE")
                    MyConn.Execute "Delete from INVESTOR_MASTER  where inv_code=" & RsData("INV_CODE")
                    MyConn.Execute "Delete from INVESTOR_MASTER@mf.bajajcapital  where inv_code=" & RsData("INV_CODE")
                End If
                rsInv_check.Close                                                                                                                                ''myconn.Execute "delete from INVESTOR_MASTER  where inv_code=" & msfgMergedInvestors.TextMatrix(i, 2)
                RsData.MoveNext
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
            RsData.Close
        Next i
        
        Me.MousePointer = 0
        MsgBox "Client(s) Merged Successfully", vbInformation
                
        Set RsData = Nothing
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