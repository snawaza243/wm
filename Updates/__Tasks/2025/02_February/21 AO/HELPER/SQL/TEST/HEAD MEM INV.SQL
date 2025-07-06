SELECT 
    im.inv_code,
    CASE 
        WHEN ct.client_code IS NOT NULL AND ct.client_code = ct.main_code THEN 'ct head'
        WHEN ct.client_code IS NOT NULL AND ct.client_code <> ct.main_code THEN 'ct member'
        WHEN im.inv_code IS NOT NULL AND SUBSTR(im.inv_code, -3) = '001' AND cm.client_code IS NOT NULL THEN 'inv head'
        WHEN im.inv_code IS NOT NULL AND SUBSTR(im.inv_code, -3) = '001' AND cm.client_code IS NULL THEN 'Invalid: Investor exists in investor_master but not in client_master'
        ELSE 'Invalid: Investor data not found!'
    END AS case_result
FROM investor_master im
LEFT JOIN client_test ct ON ct.client_codekyc = im.inv_code
LEFT JOIN client_master cm ON cm.client_code = SUBSTR(im.inv_code, 1, 8)
WHERE ROWNUM <= 200;  -- Change 20 to the number of test cases you want
