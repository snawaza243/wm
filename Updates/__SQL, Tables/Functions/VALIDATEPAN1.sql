CREATE OR REPLACE Function WEALTHMAKER.ValidatePan1(str varchar2) RETURN NUMBER IS
StrTemp VARCHAR2(50);
BEGIN
    strTemp := Trim(str);
    IF Trim(strTemp) IS NULL THEN
        RETURN 0;
    END IF;
    IF (Ascii(substr(strTemp,length(strTemp),length(strTemp))) < 65 Or Ascii(substr(strTemp,length(strTemp),length(strTemp))) > 90)  Or (Ascii(substr(strTemp, 1, 1)) < 65 Or Ascii(substr(strTemp, 1, 1)) > 90)  Or (Ascii(substr(strTemp, 2, 1)) < 65 Or Ascii(substr(strTemp, 2, 1)) > 90)  Or (Ascii(substr(strTemp, 3, 1)) < 65 Or Ascii(substr(strTemp, 3, 1)) > 90)  Or (Ascii(substr(strTemp, 4, 1)) < 65 Or Ascii(substr(strTemp, 4, 1)) > 90)  Or (Ascii(substr(strTemp, 5, 1)) < 65 Or Ascii(substr(strTemp, 5, 1)) > 90)  Or (IsNumber(substr(upper(strTemp), 6, 4)) = 0)  Or (Length(strTemp) <> 10) Then
            RETURN 0;
    END IF;
    RETURN 1;

END;

-------------------------------------------------------------------------
/
