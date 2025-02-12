create or replace PROCEDURE PSM_IR_GET_COM_L_CAT (
    p_company_list OUT SYS_REFCURSOR
) 
AS
BEGIN
    OPEN p_company_list FOR
        SELECT company_name, company_cd
        FROM bajaj_company_master
        WHERE catagory = 'L'
        ORDER BY company_name;
END;