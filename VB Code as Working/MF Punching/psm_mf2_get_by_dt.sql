CREATE OR REPLACE PROCEDURE psm_mf2_get_by_dt(
    p_dt IN VARCHAR2,
    p_tab IN VARCHAR2,
    p_mf2 OUT SYS_REFCURSOR
) AS
-- Vertual variables to hold the following select columns data
-- select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'

v_dt_number1 VARCHAR2(50);
v_bssi_rm_code1 VARCHAR2(50);
v_busi_branch_code1 VARCHAR2(50);
v_busi_tr_date1 VARCHAR2(50);
v_sch_code1 VARCHAR2(50);
v_inv_code1 VARCHAR2(50);
v_branch_name1 VARCHAR2(100);
v_expense1 VARCHAR2(50);

-- v_varber names with varcahr2(50) for osch_code sch_code,osch_name sch_name,longname Short_name,name,iss_name mut_name and wiht _2 post fix
v_osch_code2 VARCHAR2(50);
v_osch_name2 VARCHAR2(100);
v_short_name2 VARCHAR2(100);
v_name2 VARCHAR2(100);
mut_name2 VARCHAR2(100);


v_sch_nature3 VARCHAR2(50);






-- return ui elements values
v_label2_16 VARCHAR2(100);
v_label2_17 VARCHAR2(100);
v_MyNat VARCHAR2(50);


BEGIN

    select a.common_id, a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense
    into v_dt_number1, v_bssi_rm_code1, v_busi_branch_code1, v_busi_tr_date1, v_sch_code1, v_inv_code1, v_branch_name1, v_expense1
    from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id=p_dt and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0';

    if(v_dt_number1 is not null) then
        if(v_sch_code1 is null) THEN
            select osch_code sch_code,osch_name sch_name,longname Short_name,name,iss_name mut_name 
            into v_osch_code2, v_osch_name2, v_short_name2, v_name2, mut_name2
            from other_product o, product_master p,iss_master i where osch_code=v_sch_code1 and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) 
            UNION all select sch_code,sch_name,Short_name,'MF' name,mut_name from scheme_info s,mut_fund m where s.sch_code=v_sch_code1 and s.mut_code=m.mut_code and m.mut_code not 
            in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') And s.sch_code in (select sch_code from mp_scheme_master sm where sm.active_status='Active');
            
            IF p_tab = '1' THEN

            select nvl(WEALTHMAKER.get_scheme_nature(v_sch_code1),'') into v_sch_nature3 from dual;

/*
rsnat.open "select nvl(get_scheme_nature('" & UCase(rsGetDocPath("sch_code")) & "'),'') a from dual", MyConn, adOpenForwardOnly
                If rsnat("a") = "O" Then
                    frmtransactionmf.Label2(16).Caption = "Expenses agnst. Trail%"
                    frmtransactionmf.Label2(17).Caption = "Expenses agnst. Trail(Rs.)"
                    frmtransactionmf.MyNat = "O"
                Else
                    frmtransactionmf.Label2(16).Caption = "Expenses%"
                    frmtransactionmf.Label2(17).Caption = "Expenses(Rs.)"
                    frmtransactionmf.MyNat = "C"
                End If
                rsnat.Close
*/
            if v_sch_nature3 = 'O' then
                v_label2_16 := 'Expenses agnst. Trail%';
                v_label2_17 := 'Expenses agnst. Trail(Rs.)';
                v_MyNat := 'O';
            else
                v_label2_16 := 'Expenses%';
                v_label2_17 := 'Expenses(Rs.)';
                v_MyNat := 'C';
            end if;



            END IF;

         end if;
    end if;



    OPEN p_mf2 FOR
    SELECT *
    FROM your_table
    WHERE date_column = p_dt;
END psm_mf2_get_by_dt;