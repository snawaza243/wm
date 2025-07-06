CREATE OR REPLACE PROCEDURE PSM_NPS_CALC2 (
    P_REQ_CODE          IN NUMBER,
    P_SCHEME            IN VARCHAR2,
    P_AMOUNT_T1         IN NUMBER,
    P_AMOUNT_T2         IN NUMBER,
    P_COLLECTION        IN NUMBER,
    P_DATE              IN DATE,
    P_TYPE              IN NUMBER DEFAULT 0, -- 0=Inv, 1=Cor
    P_AR_NO             IN NUMBER DEFAULT 0,
    P_POP_REG_CHARGE_OT OUT NUMBER,
    P_POP_REG_CHARGE    OUT NUMBER,
    P_GST               OUT NUMBER,
    P_INVESTED          OUT NUMBER,
    P_STATUS            OUT NUMBER, -- 0=Success, 1=Error
    P_ERROR_MSG         OUT VARCHAR2
)
IS
    V_SERVICETAX        NUMBER := 0;
    V_FRESH_CONTRI      NUMBER := 0;
    V_SCHEME_TYPE       VARCHAR2(10);
    V_IS_TIRE1          BOOLEAN := FALSE;
    V_IS_TIRE2          BOOLEAN := FALSE;
    V_IS_TIRE12         BOOLEAN := FALSE;
    V_MY_REQ_TYPE       NUMBER := 0;
    V_TRAN_AMOUNT       NUMBER := 0;
BEGIN
    -- Initialize output parameters
    P_STATUS := 0;
    P_ERROR_MSG := NULL;
    P_POP_REG_CHARGE_OT := 0;
    P_POP_REG_CHARGE := 0;
    P_GST := 0;
    P_INVESTED := 0;

    -- Get app_no and amount from transaction_st if ar_no is provided
    IF P_AR_NO > 0 THEN
        BEGIN
            -- Query for app_no 
            SELECT NVL(APP_NO, 0) 
            INTO V_MY_REQ_TYPE
            FROM TRANSACTION_ST 
            WHERE TRAN_CODE = TO_CHAR(P_AR_NO);
            
            -- Query for amount
            SELECT NVL(AMOUNT, 0)
            INTO V_TRAN_AMOUNT
            FROM TRANSACTION_ST
            WHERE TRAN_CODE = TO_CHAR(P_AR_NO);
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                V_MY_REQ_TYPE := 0;
                V_TRAN_AMOUNT := 0;
            WHEN OTHERS THEN
                P_STATUS := 1;
                P_ERROR_MSG := 'Error fetching transaction details: ' || SQLERRM;
                RETURN;
        END;
    END IF;

    -- Special case for AR_NO > 0 and app_no = 12 (Contribution) and tran_amount = 0
    IF P_AR_NO > 0 AND V_MY_REQ_TYPE = 12 AND V_TRAN_AMOUNT = 0 THEN
        -- No calculation needed for this case
        RETURN;
    END IF;

    -- Determine scheme type
    IF P_SCHEME = 'OP#09971' THEN
        V_SCHEME_TYPE := 'TIRE1';
        V_IS_TIRE1 := TRUE;
    ELSIF P_SCHEME = 'OP#09972' THEN
        V_SCHEME_TYPE := 'TIRE2';
        V_IS_TIRE2 := TRUE;
    ELSIF P_SCHEME = 'OP#09973' THEN
        V_SCHEME_TYPE := 'TIRE12';
        V_IS_TIRE12 := TRUE;
    ELSE
        V_SCHEME_TYPE := P_SCHEME;
    END IF;

    -- Determine service tax rate based on date
    IF P_DATE < TO_DATE('01/06/2015', 'DD/MM/YYYY') THEN
        V_SERVICETAX := 0.1236;
    ELSIF P_DATE >= TO_DATE('01/06/2015', 'DD/MM/YYYY') AND P_DATE < TO_DATE('15/11/2015', 'DD/MM/YYYY') THEN
        V_SERVICETAX := 0.14;
    ELSIF P_DATE >= TO_DATE('15/11/2015', 'DD/MM/YYYY') AND P_DATE < TO_DATE('01/06/2016', 'DD/MM/YYYY') THEN
        V_SERVICETAX := 0.145;
    ELSIF P_DATE >= TO_DATE('01/06/2017', 'DD/MM/YYYY') AND P_DATE < TO_DATE('01/07/2017', 'DD/MM/YYYY') THEN
        V_SERVICETAX := 0.15;
    ELSIF P_DATE >= TO_DATE('01/07/2017', 'DD/MM/YYYY') THEN
        V_SERVICETAX := 0.18;
    END IF;

    -- SUBSCRIBER REGISTRATION (11) or CONTRIBUTION (12) logic
    IF P_REQ_CODE IN (11, 12) THEN
        -- For SUBSCRIBER REGISTRATION (11) with zero amount
        IF P_REQ_CODE = 11 AND NVL(P_AMOUNT_T1, 0) = 0 THEN
            P_POP_REG_CHARGE_OT := 0;
        ELSE
            -- Calculate Fresh Contribution for Tier 1
            IF P_AMOUNT_T1 > 0 THEN
                V_FRESH_CONTRI := P_AMOUNT_T1 * 0.0025;
                
                -- Apply minimum and maximum limits
                IF V_FRESH_CONTRI < 20 THEN
                    V_FRESH_CONTRI := 20;
                ELSIF V_FRESH_CONTRI >= 25000 THEN
                    V_FRESH_CONTRI := 25000;
                END IF;
                
                -- Set one-time registration charge
                IF P_DATE >= TO_DATE('01/11/2017', 'DD/MM/YYYY') THEN
                    P_POP_REG_CHARGE_OT := 200 + V_FRESH_CONTRI;
                ELSE
                    P_POP_REG_CHARGE_OT := 100 + V_FRESH_CONTRI;
                END IF;
            END IF;
            
            -- For TIRE2 or TIRE12 schemes
            IF V_SCHEME_TYPE IN ('TIRE2', 'TIRE12') AND P_AMOUNT_T2 > 0 THEN
                P_POP_REG_CHARGE := 0; -- Special case in original code
            END IF;
        END IF;
        
        -- For non-SUBSCRIBER REGISTRATION requests with Tier 2 amount
        IF P_REQ_CODE != 11 AND P_AMOUNT_T2 != 0 THEN
            P_POP_REG_CHARGE := P_AMOUNT_T2 * 0.0025;
            IF P_POP_REG_CHARGE < 20 THEN
                P_POP_REG_CHARGE := 20;
            END IF;
        END IF;
        
        -- For non-SUBSCRIBER REGISTRATION requests with Tier 1 amount
        IF P_REQ_CODE != 11 AND P_AMOUNT_T1 != 0 THEN
            P_POP_REG_CHARGE_OT := P_AMOUNT_T1 * 0.0025;
            IF P_POP_REG_CHARGE_OT < 20 THEN
                P_POP_REG_CHARGE_OT := 20;
            ELSIF P_POP_REG_CHARGE_OT >= 25000 THEN
                P_POP_REG_CHARGE_OT := 25000;
            END IF;
        END IF;
        
        -- For non-SUBSCRIBER REGISTRATION requests with Tier 2 amount
        IF P_REQ_CODE != 11 AND P_AMOUNT_T2 != 0 THEN
            P_POP_REG_CHARGE := P_AMOUNT_T2 * 0.0025;
            IF P_POP_REG_CHARGE < 20 THEN
                P_POP_REG_CHARGE := 20;
            ELSIF P_POP_REG_CHARGE >= 25000 THEN
                P_POP_REG_CHARGE := 25000;
            END IF;
        END IF;
        
        -- Calculate GST
        IF P_REQ_CODE != 11 THEN
            P_GST := ROUND((P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE) * V_SERVICETAX, 2);
        ELSE
            P_GST := ROUND((P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE) * V_SERVICETAX, 2);
        END IF;
        
        -- Calculate invested amount
        P_INVESTED := ROUND((P_AMOUNT_T1 + P_AMOUNT_T2) - (P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE + P_GST), 2);
        
        -- Special case for type=1 (Cor) and req_code=11
        IF P_TYPE = 1 AND P_REQ_CODE = 11 THEN
            P_POP_REG_CHARGE_OT := ROUND((P_AMOUNT_T1 / (1 + V_SERVICETAX)), 2);
            P_GST := ROUND(P_POP_REG_CHARGE_OT * V_SERVICETAX, 2);
            P_INVESTED := 0;
        END IF;
    ELSE
        -- For other request types (not 11 or 12)
        IF P_COLLECTION > 0 THEN
            P_GST := ROUND((P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE + P_COLLECTION) * V_SERVICETAX, 2);
            P_INVESTED := ROUND(P_COLLECTION - (P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE + P_GST), 2);
        ELSE
            P_GST := ROUND(P_AMOUNT_T1 * V_SERVICETAX, 2);
            P_INVESTED := ROUND((P_AMOUNT_T1 + P_AMOUNT_T2) - (P_POP_REG_CHARGE_OT + P_POP_REG_CHARGE + P_GST), 2);
        END IF;
    END IF;
    
    -- Final adjustments for negative values
    P_INVESTED := ABS(P_INVESTED);
    
EXCEPTION
    WHEN OTHERS THEN
        P_STATUS := 1;
        P_ERROR_MSG := 'Error in PSM_NPS_CALC: ' || SQLERRM;
        P_POP_REG_CHARGE_OT := 0;
        P_POP_REG_CHARGE := 0;
        P_GST := 0;
        P_INVESTED := 0;
END;
/