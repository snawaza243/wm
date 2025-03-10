SELECT CT.GUEST_CODE, CT.CLIENT_CODE, CM.GUEST_CD, IM.PAN, IM.EMAIL, im.aadhar_card_no, im.g_pan, IM.MOBILE, ct.source_code, IM.INV_CODE, CM.CLIENT_CODE FROM CLIENT_TEST ct
    LEFT JOIN client_master cm ON cm.client_code = SUBSTR(ct.client_codekyc, 1, 8)
    LEFT JOIN employee_master em ON em.payroll_id = CT.business_code
    left join investor_master im on im.inv_code = ct.client_codekyc
WHERE CM.GUEST_CD IS NOT NULL 
AND im.aadhar_card_no IS NOT NULL

AND CT.GUEST_CODE IS NOT NULL
AND IM.PAN IS NOT NULL
AND IM.EMAIL IS NOT NULL
AND IM.G_PAN IS NOT NULL
AND IM.MOBILE IS NOT NULL;





 SELECT NVL(COUNT(PAN),0) --INTO VG_PAN 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE --UPPER(G_PAN) = UPPER('APQPJ3222C') and
                'APQPJ3222C'  NOT in (SELECT PAN FROM INVESTOR_MASTER WHERE PAN = 'APQPJ3222C' AND ROWNUM = 1)
                AND PAN = 'APQPJ3222C'
                ;



 SELECT *
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE 'APQPJ3222C'  IN (SELECT PAN FROM INVESTOR_MASTER WHERE PAN = 'APQPJ3222C')




