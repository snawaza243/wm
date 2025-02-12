  select 'Validity: Transaction data exist in st' AS message,

                        ST.CORPORATE_NAME as CORPORATE_NAME,
                        ST.DOC_ID as DOC_ID,
                        nvl((select investor_name from investor_master where inv_code = (select inv_code from tb_doc_upload where ar_code = st.tran_code)),'nope') as INV_NAME,
                        --ST.AMOUNT1 as AMOUNT1,
                        nvl((select amount2 from NPS_TRANSACTION where tran_code = '050104226247'),'0')as AMOUNT2,
                        nvl((select REG_CHARGE from NPS_TRANSACTION where tran_code = '050104226247'),'0') as REG_CHARGE,
                        nvl((select Tran_CHARGE from NPS_TRANSACTION where tran_code = '050104226247'),'0') as Tran_CHARGE,
                        nvl((select SERVICETAX from NPS_TRANSACTION where tran_code = '050104226247'),'0')  as SERVICETAX,    
                        --'tm1' as AMOUNT2,
                        --'tm2'  as REG_CHARGE,
                        --'tm3'  as Tran_CHARGE,
                        'tm4'   as SERVICETAX, 
                        ST.TRAN_CODE as TRAN_CODE,
                        ST.CLIENT_CODE as CLIENT_CODE,
                        ST.SCH_CODE as SCH_CODE,
                        ST.FOLIO_NO as FOLIO_NO,
                        ST.BUSINESS_RMCODE as BUSINESS_RMCODE,
                        ST.BUSI_BRANCH_CODE as BUSI_BRANCH_CODE,
                        ST.UNIQUE_ID as UNIQUE_ID,
                        ST.PAYMENT_MODE as PAYMENT_MODE,
                        ST.CHEQUE_NO as CHEQUE_NO,
                        ST.CHEQUE_DATE as CHEQUE_DATE,
                        ST.BANK_NAME as BANK_NAME,
                        ST.APP_NO as APP_NO,
                        ST.TR_DATE as TR_DATE,    
                        ST.manual_arno as manual_arno,
                        ST.AMOUNT as AMOUNT,
                        ST.REMARK as REMARK,
                        ST.*
                    FROM
                        transaction_st st
                    WHERE
                        st.tran_code = '050104226247';
                        
                        
select  AMOUNT1 , AMOUNT2 , REG_CHARGE , TRAN_CHARGE , SERVICETAX , REMARK from nps_transaction WHERE TRAN_CODE = '050104226247';


select * from nps_transaction;
insertio

