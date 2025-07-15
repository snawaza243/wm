CREATE OR REPLACE PROCEDURE WEALTHMAKER.ClientMerge_v1 (
    p_xml IN CLOB,
    MainClientCode IN NUMBER,
    ModifyDate IN date,
    LoggedUserId IN VARCHAR2,
    p_message OUT VARCHAR2,
    inv_cursor OUT SYS_REFCURSOR
)
IS
    ClientCode NUMBER;
    MainCode VARCHAR2(100) default 'AH2251863';
    NewInvCode NUMBER;
    BranchCode NUMBER;
    RmCode NUMBER;
    BusRmCode NUMBER;
    InvestorCount NUMBER;
    KYC VARCHAR2(50);
    PAN VARCHAR2(50);
    FpInvestorCount NUMBER;
    MemberCount NUMBER;

    CURSOR c IS
    SELECT 
        TO_NUMBER(EXTRACTVALUE(VALUE(x), '/Record/ClientCode')) AS ClientCode
    FROM TABLE(XMLSEQUENCE(EXTRACT(XMLTYPE(p_xml), '/Records/Record'))) x;

BEGIN
    -- Start the transaction
    SAVEPOINT start_transaction;

    select SourceId, rm_code into BranchCode, RmCode from client_master where client_code = MainClientCode;
    select Payroll_Id into BusRmCode from Employee_Master where rm_code = RmCode; 

    -- OUTER LOOP START
    FOR rec IN c LOOP

        ClientCode := rec.ClientCode;        
        -- RS CLIENTSelect * from client_master where client_code=" & msfgMain.TextMatrix(1, 2), MyConn, adOpenDynamic, adLockPessimistic
        -- RS CLINT 1Select * from client_master where client_code=" & msfgMergedInvestors.TextMatrix(i, 2), MyConn, adOpenForwardOnly
        -- RS DATA select inv_code,investor_name,PAN,MANDATE_FLAG from investor_master where source_id=" & msfgMergedInvestors.TextMatrix(i, 2), MyConn, adOpenForwardOnly

        -- INNER LOOP START
        FOR rec IN (select inv_code,investor_name,PAN,MANDATE_FLAG from investor_master where source_id = ClientCode) LOOP
            Select count(*) into InvestorCount from investor_master where source_id = MainClientCode;
            Select kyc, PAN into KYC, PAN from investor_master where source_id = MainClientCode AND ROWNUM = 1;
            if InvestorCount = 0 then
            begin
                Select inv_code into NewInvCode from investor_master where source_id = MainClientCode;
            end;
            else 
                --select count(*) into MemberCount from investor_master where source_id = MainClientCode;
                select max(to_number(substr(inv_code,9,3))) into MemberCount from investor_master where source_id = MainClientCode;
                
                NewInvCode := TO_NUMBER( MainClientCode || TO_CHAR((MemberCount + 1), 'FM000'));
                update INVESTOR_MASTER set SOURCE_ID = MainClientCode, BRANCH_CODE = BranchCode, RM_CODE = RmCode, INV_CODE = NewInvCode where INV_CODE = rec.inv_code; 


                update client_test set source_code = MainClientCode, BRANCH_CODE = BranchCode, business_code = BusRmCode, client_codekyc = NewInvCode, main_code = MainCode 
                where client_codekyc = rec.inv_code;
                /* table or view does not exist */
                update INVESTOR_MASTER set SOURCE_ID = MainClientCode, BRANCH_CODE = BranchCode, RM_CODE = RmCode, INV_CODE = NewInvCode 
                where INV_CODE = rec.inv_code;

           end if;

            update fp_investor set familyhead_code=NewInvCode where familyhead_code=rec.inv_code;
            update fp_investor set fam_mem1=replace(fam_mem1,rec.inv_code,NewInvCode) WHERE familyhead_code LIKE SUBSTR(rec.inv_code, 1, 8) || '%' OR familyhead_code LIKE SUBSTR(NewInvCode, 1, 8) || '%';
            update fp_investor set fam_mem2=replace(fam_mem2,rec.inv_code,NewInvCode) WHERE familyhead_code LIKE SUBSTR(rec.inv_code, 1, 8) || '%' OR familyhead_code LIKE SUBSTR(NewInvCode, 1, 8) || '%';
            update fp_investor set fam_mem3=replace(fam_mem3,rec.inv_code,NewInvCode) WHERE familyhead_code LIKE SUBSTR(rec.inv_code, 1, 8) || '%' OR familyhead_code LIKE SUBSTR(NewInvCode, 1, 8) || '%';
            update WEALTHMAKER.RP_DETAIL set INV_CODE=NewInvCode,RM_BRANCH_CODE=BranchCode,RM_BUSINESS_CODE=BusRmCode,MODIFIED_ON=ModifyDate where INV_CODE=rec.inv_code;

            update TRANSACTION_ST set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode,modify_TALISMA=ModifyDate where client_code=rec.inv_code;

            update TRANSACTION_mf_temp1 set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode where client_code=rec.inv_code;

            /* table or view does not exist*/
            update TRANSACTION_ST set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode where client_code=rec.inv_code;

            update TRANSACTION_STTEMP set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode,modify_TALISMA=ModifyDate where client_code=rec.inv_code;

            /table or view does not exist/
            update REDEM set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode where client_code=rec.inv_code;

            /* table or view does not exist */
            update INVESTOR_FOLIO@mf.bajajcapital set INVESTOR_CODE=NewInvCode where INVESTOR_CODE=rec.inv_code;

            update INVESTOR_MASTER_IPO set inv_code=NewInvCode,AGENT_CODE=MainClientCode where inv_code=rec.inv_code;
            update REVERTAL_TRANSACTION set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode where client_code=rec.inv_code;
            update TRANSACTION_IPO set inv_code=NewInvCode,AGENT_CODE=MainClientCode where inv_code=rec.inv_code;

            /table or view does not exist/
            update TRAN_PAYOUT set inv_code=NewInvCode where inv_code=rec.inv_code;

            /* ORA-04098: trigger 'WEALTHMAKER.TRG_WEBENGAGE_DATA' is invalid and failed re-validation*/ 
            update BAJAJ_AR_HEAD set CLIENT_CD=NewInvCode,modify_TALISMA=ModifyDate where CLIENT_CD=rec.inv_code;


            /* table or view does not exist*/
            update TRAN_NET_BALANCE6@mf.bajajcapital set CLIENT_CODE=NewInvCode where CLIENT_CODE=rec.inv_code;

            update TRAN_IPO set inv_code=NewInvCode,CLIENT_CODE=MainClientCode where inv_code=rec.inv_code;
            update TRAN_LEAD set inv_code=NewInvCode where inv_code=rec.inv_code;
            update LEADS.LEAD_DETAIL set inv_code=NewInvCode where inv_code=rec.inv_code;
            /table or view does not exist/
            update port_TRANSACTION_ST set client_code=NewInvCode,branch_code=BranchCode,source_code=MainClientCode,rmcode=RmCode where client_code=rec.inv_code;


            /* Error: Invalid Number  */  
            update tb_doc_upload set inv_code = to_char(NewInvCode) where inv_code = to_char(rec.inv_code);


            update WEALTHMAKER.CLIENT_VOUCHER_DETAILS set inv_code=NewInvCode where inv_code=rec.inv_code;

            /* table or view does not exist */
            update portfolio_trans@mf.bajajcapital set client_code=NewInvCode,source_code=MainClientCode where client_code=rec.inv_code;

            --Online Just Trade
            update transaction_st_online set client_code=NewInvCode where client_code=rec.inv_code;
            update online_client_request set inv_code=NewInvCode where inv_code=rec.inv_code;
            update online_client_request_hist set inv_code=NewInvCode where inv_code=rec.inv_code;
            update online_business_summary  set client_codewm=NewInvCode where client_codewm=rec.inv_code;
            update offline_business_summary set client_codewm=NewInvCode where client_codewm=rec.inv_code;



            if InvestorCount = 0 then
                insert into client_inv_merge_log values(NewInvCode,rec.inv_code,LoggedUserId,sysdate);
                /* too many values */
                insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=rec.inv_code;

                /table or view does not exist/
                Delete from INVESTOR_MASTER where inv_code=rec.inv_code;


                If KYC = 'YES' Or KYC = 'YESP' Then
                    insert into account_merge_log values(NewInvCode,rec.inv_code,LoggedUserId,sysdate);
                end if;

                If Trim(PAN) = '' Or PAN is null Then
                    If Trim(rec.PAN) <> '' and rec.PAN is not null Then
                        UPDATE client_test SET CLIENT_pan=rec.PAN where client_codekyc=NewInvCode and CLIENT_pan is null;
                    End If;
                End If;

                If rec.MANDATE_FLAG = 'Y' Then
                    UPDATE client_test SET mandate_flag='Y' where client_codekyc=NewInvCode and nvl(mandate_flag,'N')='N';
                    UPDATE Investor_Master SET mandate_flag='Y' where Inv_Code=NewInvCode and nvl(mandate_flag,'N')='N';
                End If;

            end if;


        END LOOP;
        -- INNER LOOP END HERE

        update INVESTOR_MASTER set BRANCH_CODE=BranchCode,RM_CODE=RmCode,modify_date=ModifyDate where source_id=MainClientCode;
        /* table or view does not exist */
        update INVESTOR_MASTER set BRANCH_CODE=BranchCode,RM_CODE=RmCode,modify_date=ModifyDate where source_id=MainClientCode;

        ----------------------CLIENT MERGING-----------------------
        update client_MASTER set sourceid=BranchCode,RM_CODE=RmCode,modify_date=ModifyDate where client_code=MainClientCode;
        /table or view does not exist/
        update client_MASTER set sourceid=BranchCode,RM_CODE=RmCode,modify_date=ModifyDate where client_code=MainClientCode;

        /* Error: ORA-04098: trigger 'WEALTHMAKER_MF.TRANTEMP_TR3_NEW1' is invalid and failed re-validation*/
        update TRANSACTION_ST set branch_code=BranchCode,rmcode=RmCode,modify_TALISMA=ModiFyDate where source_code=MainClientCode;


        update TRANSACTION_MF_TEMP1 set branch_code=BranchCode,rmcode=RmCode where source_code=MainClientCode;
        /* table or view does not exist */
        update TRANSACTION_ST set branch_code=BranchCode,rmcode=RmCode where source_code=MainClientCode;

        update TRANSACTION_STTEMP set branch_code=BranchCode,rmcode=RmCode,modify_TALISMA=ModifyDate where source_code=MainClientCode;
        /* table or view does not exist */
        update REDEM set branch_code=BranchCode,rmcode=RmCode where source_code=MainClientCode;

        update REVERTAL_TRANSACTION set branch_code=BranchCode,rmcode=RmCode where source_code=MainClientCode;

        update CLIENT_VEHICLE set client_code=MainClientCode where client_code=ClientCode;
        update REFERENCE_MASTER set client_code=MainClientCode where client_code=ClientCode;
        update RELATION_MASTER set client_code=MainClientCode where client_code=ClientCode;
        update BILL_DETAIL_PAID set AGENT_code=MainClientCode where AGENT_code=ClientCode;
        update PAYMENT_DETAIL set agent_code=MainClientCode where agent_code=ClientCode;
        update LEDGER set AGENT_code=MainClientCode where AGENT_code=ClientCode;
        /* table or view does not exist*/
        update port_TRANSACTION_ST set branch_code=BranchCode,rmcode=RmCode where source_code=ClientCode;

        insert into client_inv_merge_log values(MainClientCode,ClientCode,LOggedUserId,sysdate);
        /*too many values
        insert into client_del select * from client_master where client_code=ClientCode;
        */
        Delete from client_master where client_code = ClientCode;
        /* Error: ORA-04098: trigger 'WEALTHMAKER.CLIENTDEL' is invalid and failed re-validation*/
        Delete from client_test where substr(client_codekyc,1,8) = ClientCode;

        /* table or view does not exist*/
        Delete from client_master where client_code = ClientCode;
        Delete from client_master_ext4 where client_code = ClientCode;

        declare 
        FpInvestorCount number;
        FamilyHead varchar2(100);
        FamilyMember1 varchar2(100);
        FamilyMember2 varchar2(100);
        FamilyMember3 varchar2(100);
        begin
            Select count(*) into FpInvestorCount from fp_investor where substr(familyhead_code,1,8)=MainClientCode and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc ;
            if FpInvestorCount > 1 then
                Select familyhead_code, fam_mem1 into FamilyHead, FamilyMember1 
                from fp_investor where substr(familyhead_code,1,8 )= MainClientCode and (fp_type='Snapshot' or Fp_type='Comprehensive') and rownum = 1
                order by familyhead_code desc ;
                /* table or view does not exist */
                insert into dup_fp_investor select * from fp_investor where familyhead_code=FamilyHead;

                update fp_investor set fam_mem1=fam_mem1 || '#' || FamilyMember1 where substr(familyhead_code,1,8) = MainClientCode and (fp_type='Snapshot' or Fp_type='Comprehensive');
            end if;
        end;


    END LOOP; 
    -- OUTER LOOP END
    -- Commit the transaction if all updates are successful
    COMMIT;

    OPEN inv_cursor FOR
        select inv_code,investor_name,pan,source_id from investor_master where source_id = MainClientCode;

    p_message := 'Success: client merge successfully';

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO start_transaction;
        p_message := 'Error: ' || SQLERRM ;
END;
/