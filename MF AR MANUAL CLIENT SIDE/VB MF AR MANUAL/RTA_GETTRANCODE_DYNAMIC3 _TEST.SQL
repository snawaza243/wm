-- Sample call to the function with demo data
SELECT WEALTHMAKER.GETTRANCODE_DYNAMIC(
    P_LOG_ID        => 'JOHN123',           -- Sample login ID
    P_ROLE_ID       => 'ADMIN',             -- Sample role ID
    P_BRANCH        => 'MUMBAI',            -- Sample branch
    P_AMC           => 'HDFC',              -- Sample AMC
    P_MUT_CODE      => 'MF001',             -- Sample mutual fund code
    P_RECONCILE     => 'Y',                 -- Reconcile status
    P_TRAN_TYPE     => 'REGULAR',           -- Transaction type
    P_FROM_DATE     => TO_DATE('2024-01-01', 'YYYY-MM-DD'),  -- From date
    P_TO_DATE       => TO_DATE('2024-12-31', 'YYYY-MM-DD'),  -- To date
    P_CHEQUE_TYPE   => '001',               -- Cheque type
    P_CHEQUE_SEARCH => 'CHQ123456',         -- Cheque number
    P_INVESTOR_NAME => 'RAJESH SHARMA',     -- Investor name
    P_AMOUNT        => 50000,               -- Amount
    P_SCH_CODE      => 'SCH123',            -- Scheme code
    P_FOLIO_NO      => 'FOL456789',         -- Folio number
    P_TR_DATE       => TO_DATE('2024-06-15', 'YYYY-MM-DD'),  -- Transaction date
    P_FLAG          => 'C'                  -- Flag (C for CAMS)
) AS TRAN_CODE_RESULT
FROM DUAL;





SELECT WEALTHMAKER.GETTRANCODE_DYNAMIC(
    P_LOG_ID        => 'SMITH456',
    P_ROLE_ID       => 'USER',
    P_BRANCH        => 'ALL',
    P_AMC           => 'ICICI',
    P_MUT_CODE      => 'MF002',
    P_RECONCILE     => 'N',
    P_TRAN_TYPE     => 'SIP',
    P_FROM_DATE     => TO_DATE('2024-03-01', 'YYYY-MM-DD'),
    P_TO_DATE       => TO_DATE('2024-09-30', 'YYYY-MM-DD'),
    P_CHEQUE_TYPE   => '002',
    P_CHEQUE_SEARCH => 'FOL987654',
    P_INVESTOR_NAME => 'PRIYA PATEL',
    P_AMOUNT        => 10000,
    P_SCH_CODE      => 'SCH456',
    P_FOLIO_NO      => 'FOL987654',
    P_TR_DATE       => TO_DATE('2024-07-20', 'YYYY-MM-DD'),
    P_FLAG          => 'K'
) AS TRAN_CODE_RESULT
FROM DUAL;







SELECT WEALTHMAKER.GETTRANCODE_DYNAMIC(
    P_LOG_ID        => 'TEST001',
    P_ROLE_ID       => 'VIEWER',
    P_BRANCH        => NULL,
    P_AMC           => NULL,
    P_MUT_CODE      => NULL,
    P_RECONCILE     => NULL,
    P_TRAN_TYPE     => NULL,
    P_FROM_DATE     => NULL,
    P_TO_DATE       => NULL,
    P_CHEQUE_TYPE   => NULL,
    P_CHEQUE_SEARCH => NULL,
    P_INVESTOR_NAME => NULL,
    P_AMOUNT        => NULL,
    P_SCH_CODE      => 'SCH789',
    P_FOLIO_NO      => 'FOL111222',
    P_TR_DATE       => TO_DATE('2024-08-10', 'YYYY-MM-DD'),
    P_FLAG          => 'C'
) AS TRAN_CODE_RESULT
FROM DUAL;







SELECT WEALTHMAKER.GETTRANCODE_DYNAMIC(
    P_LOG_ID        => 'ADMIN001',
    P_ROLE_ID       => 'ADMIN',
    P_BRANCH        => 'DELHI',
    P_AMC           => 'SBI',
    P_MUT_CODE      => 'MF003',
    P_RECONCILE     => 'Y',
    P_TRAN_TYPE     => 'REGULAR',
    P_FROM_DATE     => NULL,  -- Typically null for KCOB
    P_TO_DATE       => NULL,  -- Typically null for KCOB
    P_CHEQUE_TYPE   => '003',
    P_CHEQUE_SEARCH => 'APP888999',
    P_INVESTOR_NAME => 'AMIT KUMAR',
    P_AMOUNT        => 75000,
    P_SCH_CODE      => 'SCH999',
    P_FOLIO_NO      => 'FOL333444',
    P_TR_DATE       => TO_DATE('2024-09-05', 'YYYY-MM-DD'),
    P_FLAG          => 'KCOB'
) AS TRAN_CODE_RESULT
FROM DUAL;