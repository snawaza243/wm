UPDATE bajaj_due_data SET 
                              status_Cd = 'LAPSED',
                              last_update_dt = sysdate, 
                              last_update = '38387'
                              WHERE (policy_no, company_cd, due_date) IN (
                                  SELECT policy_no, company_cd, due_date 
                                  FROM TEMP_LAPSED_UPDATES
                              ) AND importdatatype = 'DUEDATA'