    CREATE OR REPLACE PROCEDURE PSM_AR_FILLBYAH (
    P_CLIENT_CODE IN VARCHAR2,
    P_CLIENT_PAN IN VARCHAR2,
    result_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    -- Open a cursor for the result set
    OPEN result_cursor FOR
        SELECT 
            'Valid Data: Client Advisory Data' as message,
            
            sch_code, 
            amount, 
            bank_name, 
            payment_mode, 
            bank_ac_no, 
            cheque_no, 
            cheque_date, 
            tran_code, 
            source_code, 
            tr_date
        FROM 
            filladvisory
        WHERE 
            act_code = P_CLIENT_CODE
            AND (client_pan = P_CLIENT_PAN OR P_CLIENT_PAN IS NULL);
END;
/

 
 
 
 
    TxtRemark.Text = RsAdvisory.Fields("bank_ac_no")
    txtChqNo.Text = RsAdvisory.Fields("cheque_no")
    dtChqDate.Text = RsAdvisory.Fields("cheque_date")
    MyTranCode = RsAdvisory.Fields("tran_code")
    MyPrintSourceId = RsAdvisory.Fields("source_code")
    MyTrDate = RsAdvisory.Fields("tr_date")
Else
    OptRen.Value = False
    optFresh.Value = True
    TxtAmount.Text = ""
    TxtRemark.Text = ""
    txtChqNo.Text = ""
    dtChqDate.Text = "__/__/____"
    MyTranCode = ""
    MyPrintSourceId = ""
    MyTrDate = "__/__/____"
End If
RsAdvisory.Close
