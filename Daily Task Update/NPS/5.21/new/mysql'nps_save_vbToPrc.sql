CREATE PROCEDURE [dbo].[NPS_SaveTransaction]
    @Index INT,
    @ProductName NVARCHAR(100) = NULL,
    @DocID NVARCHAR(50) = NULL,
    @PRAN NVARCHAR(50) = NULL,
    @CorporateName NVARCHAR(100) = NULL,
    @IsCorporate BIT = 0,
    @IsUnfreeze BIT = 0,
    @ChequeNo NVARCHAR(50) = NULL,
    @TranCode NVARCHAR(50) = NULL,
    @ReqCode NVARCHAR(10) = NULL,
    @RequestType NVARCHAR(100) = NULL,
    @BusiBranch NVARCHAR(100) = NULL,
    @RmBusiCode NVARCHAR(50) = NULL,
    @InvCode NVARCHAR(50) = NULL,
    @RegistrationNo NVARCHAR(50) = NULL,
    @AmountInvest DECIMAL(18,2) = 0,
    @PaymentMode CHAR(1) = NULL,
    @ChqDate DATETIME = NULL,
    @BankName NVARCHAR(100) = NULL,
    @Tire1Amount DECIMAL(18,2) = 0,
    @Tire2Amount DECIMAL(18,2) = 0,
    @RegCharge DECIMAL(18,2) = 0,
    @TranCharge DECIMAL(18,2) = 0,
    @ServiceTax DECIMAL(18,2) = 0,
    @Remark NVARCHAR(500) = NULL,
    @LoginID NVARCHAR(50),
    @TransactionDate DATETIME,
    @IsIndividual BIT = 1,
    @OutputTranCode NVARCHAR(50) OUTPUT,
    @OutputReceiptNo NVARCHAR(50) OUTPUT,
    @Success BIT OUTPUT,
    @Message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Busi_Branch_cd NVARCHAR(50) = '';
    DECLARE @Busi_Rm_Cd NVARCHAR(50) = '';
    DECLARE @ClientBranchCode NVARCHAR(50) = '';
    DECLARE @ClientRmCode NVARCHAR(50) = '';
    DECLARE @Paymode CHAR(1) = '';
    DECLARE @InvCodePrefix NVARCHAR(10) = '';
    DECLARE @MyTranCode NVARCHAR(50) = '';
    DECLARE @MyGSTNO NVARCHAR(50) = '';
    DECLARE @MyTranCode1 NVARCHAR(50) = '';
    DECLARE @MySecReq NVARCHAR(50) = '';
    DECLARE @Vclientcategory NVARCHAR(10) = '';
    DECLARE @MutCode NVARCHAR(10) = 'IS02520';
    DECLARE @SCHCODE NVARCHAR(10) = '';
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @Count INT = 0;
    
    SET @Success = 0;
    SET @Message = '';
    
    BEGIN TRY
        -- Validate required fields
        IF @ProductName IS NULL OR @ProductName = ''
        BEGIN
            SET @Message = 'Product cannot be left blank';
            RETURN;
        END
        
        IF @DocID IS NULL OR @DocID = ''
        BEGIN
            SET @Message = 'DT No cannot be left blank';
            RETURN;
        END
        
        -- Corporate validation
        IF @Index = 0 AND @IsCorporate = 1
        BEGIN
            IF @CorporateName IS NULL OR @CorporateName = ''
            BEGIN
                SET @Message = 'Corporate name cannot be left blank.';
                RETURN;
            END
        END
        
        -- FATCA validation
        IF @Index = 0
        BEGIN
            IF @IsUnfreeze = 0
            BEGIN
                IF @PRAN IS NOT NULL AND @PRAN <> ''
                BEGIN
                    SELECT @Count = COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO = @PRAN;
                    IF @Count >= 1
                    BEGIN
                        SET @Message = 'FATCA for this PRAN is non compliant. Please contact product team for the same';
                        RETURN;
                    END
                END
            END
            ELSE
            BEGIN
                DELETE FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO = RTRIM(LTRIM(@PRAN));
            END
        END
        
        -- Duplicate cheque validation
        IF @Index = 0 OR @Index = 4
        BEGIN
            SELECT @Vclientcategory = category_id FROM client_master WHERE client_code = SUBSTRING(@InvCode, 1, 8);
            
            IF @Vclientcategory <> '4004'
            BEGIN
                IF @TranCode = '0'
                BEGIN
                    SELECT @Count = COUNT(*) FROM (
                        SELECT TRAN_CODE FROM transaction_st 
                        WHERE mut_code = @MutCode AND cheque_no = RTRIM(LTRIM(@ChequeNo)) t;
                        
                    IF @Count > 0
                    BEGIN
                        SET @Message = 'Duplicate Cheque Number!';
                        RETURN;
                    END
                END
                ELSE
                BEGIN
                    IF @ReqCode = '11'
                    BEGIN
                        SELECT @Count = COUNT(*) FROM (
                            SELECT TRAN_CODE FROM transaction_st 
                            WHERE TRAN_CODE <> @TranCode AND mut_code = @MutCode 
                            AND cheque_no = RTRIM(LTRIM(@ChequeNo) AND REF_TRAN_CODE IS NULL) t;
                            
                        IF @Count > 0
                        BEGIN
                            SET @Message = 'Duplicate Cheque Number!';
                            RETURN;
                        END
                    END
                    ELSE
                    BEGIN
                        SELECT @Count = COUNT(*) FROM (
                            SELECT TRAN_CODE FROM transaction_st 
                            WHERE TRAN_CODE <> @TranCode AND mut_code = @MutCode 
                            AND cheque_no = RTRIM(LTRIM(@ChequeNo)) t;
                            
                        IF @Count > 0
                        BEGIN
                            SET @Message = 'Duplicate Cheque Number!';
                            RETURN;
                        END
                    END
                END
            END
        END
        
        -- NSDL Branch validation
        IF @Index <> 3 AND @Index <> 1
        BEGIN
            IF @RegistrationNo IS NULL OR @RegistrationNo = ''
            BEGIN
                SET @Message = 'Please Select NSDL Branch First';
                RETURN;
            END
        END
        
        -- Get business branch code
        IF @BusiBranch IS NOT NULL AND @BusiBranch <> ''
        BEGIN
            SET @Busi_Branch_cd = SUBSTRING(@BusiBranch, CHARINDEX('#', @BusiBranch) + 1, LEN(@BusiBranch));
        END
        
        -- Get business RM code
        SELECT @Busi_Rm_Cd = payroll_id FROM employee_master WHERE payroll_id = @RmBusiCode;
        
        -- Get client branch and RM code
        IF @InvCode IS NOT NULL AND @InvCode <> ''
        BEGIN
            SELECT @ClientRmCode = rm_code, @ClientBranchCode = branch_code 
            FROM investor_master 
            WHERE inv_code = @InvCode;
        END
        
        -- Modification logic
        IF @Index = 4
        BEGIN
            -- Corporate validation for modification
            IF @IsCorporate = 1
            BEGIN
                IF @CorporateName IS NULL OR @CorporateName = ''
                BEGIN
                    SET @Message = 'Corporate name cannot be left blank.';
                    RETURN;
                END
            END
            
            -- Transaction selection validation
            IF @TranCode = '0'
            BEGIN
                SET @Message = 'Please Select a Transaction to Modify';
                RETURN;
            END
            
            -- Set payment mode
            SET @Paymode = @PaymentMode;
            
            -- Update transaction
            UPDATE transaction_st SET
                tr_date = @TransactionDate,
                client_Code = RTRIM(LTRIM(@InvCode)),
                source_code = LEFT(RTRIM(LTRIM(@InvCode)), 8),
                BUSI_BRANCH_CODE = @Busi_Branch_cd,
                BUSINESS_RMCODE = @Busi_Rm_Cd,
                mut_code = @MutCode,
                sch_code = @SCHCODE,
                amount = RTRIM(LTRIM(@AmountInvest)),
                folio_no = RTRIM(LTRIM(@RegistrationNo)),
                app_no = @ReqCode,
                PAYMENT_MODE = @Paymode,
                CHEQUE_DATE = @ChqDate,
                cheque_no = RTRIM(LTRIM(@ChequeNo)),
                BANK_NAME = @BankName,
                manual_arno = RTRIM(LTRIM(@PRAN)),
                corporate_name = RTRIM(LTRIM(@CorporateName)),
                MODIFY_USER = @LoginID,
                MODIFY_DATE = GETDATE()
            WHERE tran_code = @TranCode;
            
            -- Update NPS transaction
            UPDATE nps_transaction SET
                amount1 = @Tire1Amount,
                amount2 = @Tire2Amount,
                REG_CHARGE = @RegCharge,
                Tran_CHARGE = @TranCharge,
                SERVICETAX = @ServiceTax,
                remark = @Remark
            WHERE tran_code = RTRIM(LTRIM(@TranCode));
            
            SET @Message = 'Transaction Updated Successfully';
            SET @Success = 1;
            SET @OutputTranCode = @TranCode;
            
            SELECT @OutputReceiptNo = unique_id FROM transaction_st WHERE tran_code = @TranCode;
        END
        
        -- Save logic
        IF @Index = 0
        BEGIN
            -- Duplicate cheque validation
            IF @Vclientcategory <> '4004'
            BEGIN
                SELECT @Count = COUNT(*) FROM transaction_st 
                WHERE cheque_no = RTRIM(LTRIM(@ChequeNo)) 
                AND RTRIM(LTRIM(bank_name)) = RTRIM(LTRIM(@BankName)) 
                AND tran_type IN ('PURCHASE','REINVESTMENT','SWITCH IN');
                
                IF @Count > 0
                BEGIN
                    SET @Message = 'Duplicate Cheque Number!';
                    RETURN;
                END
            END
            
            -- Duplicate transaction validation
            SELECT @Count = COUNT(*) FROM transaction_st 
            WHERE CLIENT_code = RTRIM(LTRIM(@InvCode)) 
            AND sch_code = @SCHCODE 
            AND app_no = @ReqCode 
            AND amount = RTRIM(LTRIM(@AmountInvest)) 
            AND cheque_no = RTRIM(LTRIM(@ChequeNo)) 
            AND RTRIM(LTRIM(bank_name)) = RTRIM(LTRIM(@BankName)) 
            AND tran_type IN ('PURCHASE','REINVESTMENT','SWITCH IN');
            
            IF @Count > 0
            BEGIN
                SET @Message = 'Duplicate Transaction!';
                RETURN;
            END
            
            -- Set payment mode
            SET @Paymode = @PaymentMode;
            
            -- Insert into transaction_sttemp
            IF @Paymode IN ('C', 'D') -- Cheque or Draft
            BEGIN
                SET @SQL = 'INSERT INTO transaction_sttemp (CORPORATE_NAME, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, doc_id) VALUES (''' + 
                    @CorporateName + ''', ''' + @PRAN + ''', ''' + @BankName + ''', ''' + @RegistrationNo + ''', ''' + @ReqCode + ''', ''' + @Paymode + ''', ''1'', ''' + 
                    CONVERT(NVARCHAR, @TransactionDate, 106) + ''', ' + @InvCode + ', ''' + @MutCode + ''', ''' + @SCHCODE + ''', ''PURCHASE'', ' + 
                    CAST(@AmountInvest AS NVARCHAR) + ', ' + @ClientBranchCode + ', ' + SUBSTRING(@InvCode, 1, 8) + ', ' + @ClientRmCode + ', ' + 
                    RTRIM(LTRIM(@RmBusiCode)) + ', ' + @Busi_Branch_cd + ', ' + RTRIM(LTRIM(@ChequeNo)) + ', ''' + CONVERT(NVARCHAR, @ChqDate, 103) + 
                    ''', ''NPS'', ''' + RTRIM(LTRIM(@DocID)) + ''')';
            END
            ELSE
            BEGIN
                SET @SQL = 'INSERT INTO transaction_sttemp (CORPORATE_NAME, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, remark, doc_id) VALUES (''' + 
                    @CorporateName + ''', ''' + @PRAN + ''', ''' + @BankName + ''', ''' + @RegistrationNo + ''', ''' + @ReqCode + ''', ''' + @Paymode + ''', ''1'', ''' + 
                    CONVERT(NVARCHAR, @TransactionDate, 106) + ''', ' + @InvCode + ', ''' + @MutCode + ''', ''' + @SCHCODE + ''', ''PURCHASE'', ' + 
                    CAST(@AmountInvest AS NVARCHAR) + ', ' + @ClientBranchCode + ', ' + SUBSTRING(@InvCode, 1, 8) + ', ' + @ClientRmCode + ', ' + 
                    RTRIM(LTRIM(@RmBusiCode)) + ', ' + @Busi_Branch_cd + ', ''NPS'', ''' + RTRIM(LTRIM(@DocID)) + ''')';
            END
            
            EXEC sp_executesql @SQL;
            
            -- Get the new transaction code
            SELECT @MyTranCode = MAX(tran_code) FROM temp_tran 
            WHERE branch_code = @ClientBranchCode AND SUBSTRING(tran_code, 1, 2) = '07';
            
            -- Get GST number
            SELECT @MyGSTNO = invoice_no FROM transaction_sttemp WHERE tran_code = @MyTranCode;
            
            -- Insert into transaction_st
            IF @Paymode IN ('C', 'D') -- Cheque or Draft
            BEGIN
                SET @SQL = 'INSERT INTO transaction_st (invoice_no, CORPORATE_NAME, manual_arno, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, LOGGEDUSERID, doc_id) VALUES (''' + 
                    @MyGSTNO + ''', ''' + @CorporateName + ''', ''' + @PRAN + ''', ''' + @BankName + ''', ''' + @RegistrationNo + ''', ''' + @ReqCode + ''', ''' + @Paymode + ''', ''' + 
                    RTRIM(LTRIM(@MyTranCode)) + ''', ''1'', ''' + CONVERT(NVARCHAR, @TransactionDate, 106) + ''', ' + @InvCode + ', ''' + @MutCode + ''', ''' + @SCHCODE + 
                    ''', ''PURCHASE'', ' + CAST(@AmountInvest AS NVARCHAR) + ', ' + @ClientBranchCode + ', ' + SUBSTRING(@InvCode, 1, 8) + ', ' + @ClientRmCode + ', ' + 
                    RTRIM(LTRIM(@RmBusiCode)) + ', ' + @Busi_Branch_cd + ', ' + RTRIM(LTRIM(@ChequeNo)) + ', ''' + CONVERT(NVARCHAR, @ChqDate, 103) + 
                    ''', ''NPS'', ''' + @LoginID + ''', ''' + RTRIM(LTRIM(@DocID)) + ''')';
            END
            ELSE
            BEGIN
                SET @SQL = 'INSERT INTO transaction_st (invoice_no, CORPORATE_NAME, manual_arno, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, remark, LOGGEDUSERID, doc_id) VALUES (''' + 
                    @MyGSTNO + ''', ''' + @CorporateName + ''', ''' + @PRAN + ''', ''' + @BankName + ''', ''' + @RegistrationNo + ''', ''' + @ReqCode + ''', ''' + @Paymode + ''', ''' + 
                    RTRIM(LTRIM(@MyTranCode)) + ''', ''1'', ''' + CONVERT(NVARCHAR, @TransactionDate, 106) + ''', ' + @InvCode + ', ''' + @MutCode + ''', ''' + @SCHCODE + 
                    ''', ''PURCHASE'', ' + CAST(@AmountInvest AS NVARCHAR) + ', ' + @ClientBranchCode + ', ' + SUBSTRING(@InvCode, 1, 8) + ', ' + @ClientRmCode + ', ' + 
                    RTRIM(LTRIM(@RmBusiCode)) + ', ' + @Busi_Branch_cd + ', ''NPS'', ''' + @LoginID + ''', ''' + RTRIM(LTRIM(@DocID)) + ''')';
            END
            
            EXEC sp_executesql @SQL;
            
            -- Insert into nps_transaction
            SET @SQL = 'INSERT INTO nps_transaction(tran_code, amount1, amount2, reg_charge, tran_charge, SERVICETAX, remark) VALUES(''' + 
                RTRIM(LTRIM(@MyTranCode)) + ''', ' + CAST(@Tire1Amount AS NVARCHAR) + ', ' + CAST(@Tire2Amount AS NVARCHAR) + ', ' + 
                CAST(@RegCharge AS NVARCHAR) + ', ' + CAST(@TranCharge AS NVARCHAR) + ', ' + CAST(@ServiceTax AS NVARCHAR) + ', ''' + @Remark + ''')';
                
            EXEC sp_executesql @SQL;
            
            -- Get receipt number
            SELECT @OutputReceiptNo = unique_id FROM transaction_st WHERE tran_code = @MyTranCode;
            
            -- Double transaction for registration
            IF @ReqCode = '11' AND @IsIndividual = 1
            BEGIN
                -- Insert into transaction_sttemp
                SET @SQL = 'INSERT INTO transaction_sttemp (CORPORATE_NAME, ref_tran_code, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, doc_id) SELECT CORPORATE_NAME, tran_code, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, 0, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, '''' FROM transaction_sttemp WHERE tran_code=''' + @MyTranCode + '''';
                EXEC sp_executesql @SQL;
                
                -- Get the second transaction code
                SELECT @MyTranCode1 = MAX(tran_code) FROM temp_tran 
                WHERE branch_code = @ClientBranchCode AND SUBSTRING(tran_code, 1, 2) = '07';
                
                -- Update document upload
                SET @SQL = 'UPDATE tb_doc_upload SET ar_code=''' + @MyTranCode1 + ''' WHERE common_id=''' + RTRIM(LTRIM(@DocID)) + '''';
                EXEC sp_executesql @SQL;
                
                -- Insert into transaction_st
                SET @SQL = 'INSERT INTO transaction_st (ref_tran_code, manual_arno, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, LOGGEDUSERID, doc_id) SELECT ref_tran_code, manual_arno, BANK_NAME, FOLIO_NO, 12, PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, LOGGEDUSERID, doc_id FROM transaction_sttemp WHERE tran_code=''' + @MyTranCode1 + '''';
                EXEC sp_executesql @SQL;
                
                -- Insert into nps_transaction for second transaction
                SET @SQL = 'INSERT INTO nps_transaction(tran_code, amount1, amount2, reg_charge, tran_charge, SERVICETAX, remark) VALUES(''' + 
                    RTRIM(LTRIM(@MyTranCode1)) + ''', 0, 0, 0, 0, 0, ''' + @Remark + ''')';
                EXEC sp_executesql @SQL;
                
                -- Delete from transaction_sttemp
                SET @SQL = 'DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE=''' + @MyTranCode1 + '''';
                EXEC sp_executesql @SQL;
                
                -- Get second receipt number
                SELECT @MySecReq = unique_id FROM transaction_st WHERE tran_code = @MyTranCode1;
                
                SET @Message = 'Your Transaction No Is ' + @MyTranCode + ' and Your Recpt No IS ' + @OutputReceiptNo + '. Your Duplicate Transaction No Is ' + @MyTranCode1 + ' and Your Recpt No IS ' + @MySecReq;
            END
            ELSE
            BEGIN
                SET @Message = 'Your Transaction No Is ' + @MyTranCode + ' and Your Recpt No IS ' + @OutputReceiptNo;
            END
            
            -- Delete from transaction_sttemp
            SET @SQL = 'DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE=''' + @MyTranCode + '''';
            EXEC sp_executesql @SQL;
            
            SET @Success = 1;
            SET @OutputTranCode = @MyTranCode;
        END
        
        -- Call Recd_paid_update procedure
        IF @Index = 0 OR @Index = 4
        BEGIN
            EXEC Recd_paid_update @tr_code = @OutputTranCode;
        END
    END TRY
    BEGIN CATCH
        SET @Message = ERROR_MESSAGE();
        SET @Success = 0;
    END CATCH
END