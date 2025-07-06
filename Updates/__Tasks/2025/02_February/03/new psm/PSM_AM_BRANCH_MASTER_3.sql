create or replace PROCEDURE PSM_AM_BRANCH_MASTER_3 (
    p_login         IN VARCHAR2,
    p_mark1         in VARCHAR2,
    p_mark2         in VARCHAR2,
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN

    if p_mark1 is null then
            OPEN p_branch_cursor FOR
        SELECT *
        FROM BRANCH_MASTER
        where branch_tar_cat in (186, 615, 308)
        --(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615) -- branch_tar_cat - 186, 615, 308 corporate partner , -- only ang : 189, 231, 186
        and branch_code is not null 
        and branch_tar_cat is not null
        ORDER BY BRANCH_NAME; 
    else
        OPEN p_branch_cursor FOR
        SELECT *
        FROM BRANCH_MASTER
        where branch_tar_cat in  (186, 615, 308)
        --(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615)
        and branch_code is not null 
        and branch_tar_cat is not null
        ORDER BY BRANCH_NAME; 
    end if;
END PSM_AM_BRANCH_MASTER_3;