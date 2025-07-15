create or replace PROCEDURE             search_client_v1 (
    p_BranchCode  VARCHAR2,
    p_CityId      VARCHAR2,
    p_RmCode      VARCHAR2,
    p_ClientCode  VARCHAR2,
    p_ClientName  VARCHAR2,
    p_Phone       VARCHAR2,
    p_Mobile      VARCHAR2,
    p_Pan         VARCHAR2,
    p_Address1    VARCHAR2,
    p_Address2    VARCHAR2,
    p_OrderBy     VARCHAR2,
    p_Category    VARCHAR2,
    p_cursor  OUT SYS_REFCURSOR
) AS
v_query VARCHAR2(30000);
BEGIN

    v_query := '
    Select c.client_name, c.client_CODE, c.address1, c.address2, C.CITY_NAME, b.Branch_name, b.Branch_Code, c.phone, c.mobile, 
    e.rm_name, e.rm_Code, c.kyc, c.CREATION_DATE, TO_CHAR(c.last_tran_dt1,''DD-MM-YYYY'') AS last_tran_dt1,
    nvl(approved_flag,''NO'') as Approved
    FROM client_master c
    join EMPLOYEE_MASTER e on c.rm_code = e.rm_code
    join Branch_master b on e.source = b.branch_code
    join city_master cm on c.city_id = cm.city_id
    where rownum < 300
    ';

    if p_BranchCode is not null then
        v_query := v_query || ' and b.BRANCH_CODE = '''|| p_BranchCode ||''' ';
    end if;

    if p_CityId is not null then
        v_query := v_query || ' and c.CITY_ID = '''|| p_CityId ||''' ';
    end if;

    if p_RmCode is not null then
        v_query := v_query || ' and c.RM_CODE = '''|| p_RmCode ||''' ';
    end if;

    if p_ClientCode is not null then
        v_query := v_query || ' and c.Client_Code = '''|| p_ClientCode ||''' ';
    end if;

    if p_ClientName is not null then
        v_query := v_query || ' and upper(c.Client_Name) = '''|| upper(p_ClientName) ||''' ';
    end if;

    if p_Phone is not null then
        v_query := v_query || ' and c.phone = '''|| p_Phone ||''' ';
    end if;

    if p_Mobile is not null then
        v_query := v_query || ' and c.mobile = '''|| p_Mobile ||''' ';
    end if;

    if p_Pan is not null then
        v_query := v_query || ' and upper(c.PAN) = '''|| upper(p_Pan) ||''' ';
    end if;

     if p_Address1 is not null then
        v_query := v_query || ' and upper(c.address1) = '''|| upper(p_Address1) ||''' ';
    end if;

    if p_Address2 is not null then
        v_query := v_query || ' and upper(c.address2) = '''|| upper(p_Address2) ||''' ';
    end if;

    if p_OrderBy is not null then
        v_query := v_query || ' order by '|| p_OrderBy ||' ';
    end if;

    OPEN p_cursor FOR v_query;

END;