CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_AR_PRINT (

    MyTranCode       IN VARCHAR2,
    MyPrintSourceId  IN VARCHAR2,
    MyTrDate         IN DATE,
    txtbusicode      IN VARCHAR2,
    MySourceId       IN VARCHAR2,  
    result_cursor    OUT SYS_REFCURSOR  
)
AS
    effective_date   DATE; -- New variable to hold the date
BEGIN

    -- Determine the effective date based on MyPrintSourceId
    IF MyPrintSourceId IS NOT NULL AND MyPrintSourceId <> '' THEN
        effective_date := MyTrDate;
    ELSE
        effective_date := SYSDATE;
    END IF;

    -- Validate required input parameters
    IF MyTranCode IS NULL OR MyTranCode = '' THEN
        RAISE_APPLICATION_ERROR(-20001, 'Transaction Code (MyTranCode) is missing. AR cannot be printed.');
        RETURN;
    END IF;

    IF txtbusicode IS NULL OR txtbusicode = '' THEN
        RAISE_APPLICATION_ERROR(-20002, 'Business Code (txtbusicode) is missing. AR cannot be printed.');
        RETURN;
    END IF;

    IF MySourceId IS NULL OR MySourceId = '' THEN
        RAISE_APPLICATION_ERROR(-20003, 'Source ID (MySourceId) is missing. AR cannot be printed.');
        RETURN;
    END IF;

    -- Construct and open the result cursor
    OPEN result_cursor FOR


    SELECT b.client_code, 'P' ar_type, t.tran_code, tr_date, cheque_date cheque_date, cheque_no cheque_no, t.bank_name, amount, 
        b.client_code source_code, app_no, NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, 
        NVL ((SELECT SUM (NVL (amt, 0)) FROM payment_detail WHERE tran_code = t.tran_code), 0) paidamt, '' asr, payment_mode, 
        (select investor_name from investor_master where inv_code=t.client_code)inv, (SELECT MAX (client_name) FROM client_master WHERE client_code = t.source_code) client, 
        exist_code AS existcode, address1 add1, address2 add2, '' loc, pincode pin, (SELECT MAX (city_name) FROM city_master WHERE city_id = (SELECT MAX (city_id) FROM client_master WHERE client_code = t.source_code)) ccity, 
        mobile ph, email, 0 arn, '' subbroker, (SELECT rm_name FROM employee_master WHERE payroll_id = TO_CHAR (t.business_rmcode) AND (TYPE = 'A' OR TYPE IS NULL)) rname, (SELECT payroll_id FROM employee_master WHERE payroll_id = TO_CHAR (t.business_rmcode) AND (TYPE = 'A' OR TYPE IS NULL)) rcode, (SELECT branch_name FROM branch_master WHERE branch_code = t.busi_branch_code) bname, (SELECT address1 FROM branch_master 
        WHERE branch_code = t.busi_branch_code) badd1, (SELECT address2 FROM branch_master WHERE branch_code = t.busi_branch_code) badd2, (SELECT phone FROM branch_master WHERE branch_code = t.busi_branch_code) bphone, (SELECT location_name FROM location_master 
        WHERE location_id = (SELECT location_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bloc, (SELECT city_name FROM city_master WHERE city_id = (SELECT city_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bcity, (SELECT iss_name 
        FROM iss_master WHERE iss_code = t.mut_code AND iss_code NOT IN ( SELECT DISTINCT iss_code FROM iss_master WHERE prod_code IN ( SELECT prod_code FROM product_master WHERE nature_code = 'NT004'))) compmf, 'Bajaj Capital Limited' compgroup, (SELECT longname 
        FROM other_product WHERE osch_code = t.sch_code) schmf, (SELECT short_name FROM scheme_info WHERE sch_code = t.sch_code) sschmf, ('38387') userid 
        FROM transaction_ST t, client_master b WHERE t.source_code = b.client_code 
        AND TRUNC(tr_date) = TO_DATE(tr_date, 'MM-dd-YYYY')  -- TEST effective_date  = 29-OCT-2012
        AND tran_code = MyTranCode    -- TEST MyTranCode = 050438002093
        AND source_code =  MySourceId  -- TEST MySourceId = 41414480    

         -- AND business_rmcode = txtbusicode --'135218' -- txtbusicode

          ;

END PSM_AO_AR_PRINT;
/
