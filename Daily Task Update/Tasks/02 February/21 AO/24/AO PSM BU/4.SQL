SELECT_BY_DT

20222011


SELECT
    'Valid Data: DT Exist - Open New Account' AS message,
    NVL(tdu.BUSI_RM_CODE, '') AS BUSI_RM_CODE,
    NVL(tdu.BUSI_BRANCH_CODE, '') AS BUSI_BRANCH_CODE,
    NVL(tdu.common_id, '') AS COMMON_ID,
    NVL(tdu.common_id, '') AS DOC_ID,            
    NVL(em.payroll_id, 'No code') AS business_code,
    NVL(em.rm_name, 'No RM') AS rm_name,
    NVL(bm.branch_name, 'No Branch') AS BRANCH_CODE,  
    NVL(tdu.guest_cd, '') AS guest_code
FROM tb_doc_upload tdu
LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code
LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code
WHERE tdu.common_id = TRIM('20222011')AND ROWNUM = 1;