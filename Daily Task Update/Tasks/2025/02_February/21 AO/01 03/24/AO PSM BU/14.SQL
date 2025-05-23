select * from client_test;

select COMMON_ID, EXIST_CODE from tb_doc_upload where 
tran_type = 'ANA' and verification_flag = '1' and punching_flag = '0' and rejection_status= '0';



SELECT COMMON_ID, GUEST_CD, INV_CODE 
FROM TB_DOC_UPLOAD T
WHERE TRAN_TYPE = 'AC' 
AND VERIFICATION_FLAG = '1' 
AND PUNCHING_FLAG = '0' 
AND REJECTION_STATUS = '0'
AND GUEST_CD IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM CLIENT_MASTER C WHERE C.GUEST_CD = T.GUEST_CD
);


SELECT GUEST_CD 
FROM BAJAJ_VENUE_BOOKING B
WHERE GUEST_CD IS NOT NULL
AND not EXISTS (
    SELECT 1 FROM CLIENT_MASTER C WHERE C.GUEST_CD = B.GUEST_CD
)
AND EXISTS (
    SELECT 1 FROM TB_DOC_UPLOAD T WHERE T.GUEST_CD = B.GUEST_CD
);


SELECT * FROM CLIENT_MASTER WHERE GUEST_CD = '1000000138';

UPDATE CLIENT_MASTER 
SET GUEST_CD = NULL
 WHERE GUEST_CD = '1000000138'; COMMIT;

