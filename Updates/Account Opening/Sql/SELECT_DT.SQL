SELECT COMMON_ID, GUEST_CD, inv_code, PUNCHING_FLAG, rejection_status, tran_type, verification_flag
--INTO DT_FLAG 
FROM TB_DOC_UPLOAD  
WHERE COMMON_ID  = '20036704'
AND tran_type= 'AC' AND rejection_status = '0' AND verification_flag = '1' AND PUNCHING_FLAG = '0' 
AND GUEST_CD  is null ;

--NOT IN (SELECT GUEST_CD FROM CLIENT_MASTER WHERE GUEST_CD IS NOT NULL)

;
SELECT GUEST_CD, client_code FROM CLIENT_MASTER WHERE GUEST_CD IS NOT NULL;

UPDATE client_master set GUEST_CD = null where client_code = '41274036';



SELECT *
(select branch_name from branch_master where branch_Code = am.SOURCEID) as branch_name,
(select rm_name from employee_master where rm_code = am.RM_CODE) as rm_name,
FROM AGENT_MASTER am where sourceid is not null in ( SELECT BR.BRANCH_CODE FROM BRANCH_MASTER BR INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '38387' AND UJ.ROLE_ID = '261' );

-- WHERE am.AGENT_CODE = '30048090';



