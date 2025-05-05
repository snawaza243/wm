Public Sub cboRequestType_Click()
    Dim MyReqType As Double
    Dim TranAmount As Double
    Dim v_servicetax As Double

    If CDate(DtDate) < CDate("01/06/2015") Then
        v_servicetax = 0.1236
    ElseIf CDate(DtDate) >= CDate("01/06/2015") And CDate(DtDate) < CDate("15/11/2015") Then
        v_servicetax = 0.14
    ElseIf CDate(DtDate) >= CDate("15/11/2015") And CDate(DtDate) < CDate("01/06/2016") Then
        v_servicetax = 0.145
    ElseIf CDate(DtDate) >= CDate("01/06/2017") And CDate(DtDate) < CDate("01/07/2017") Then
        v_servicetax = 0.15
    ElseIf CDate(DtDate) >= CDate("01/07/2017") Then
        v_servicetax = 0.18
    End If

    If cboRequestType.Text = "" Then
        Exit Sub
    End If

    If Val(lbtrancode.Caption) > 0 Then
        MyReqType = SqlRet("select nvl(app_no,0) from transaction_st where tran_code='" & lbtrancode.Caption & "'")
        TranAmount = SqlRet("select amount from transaction_st where tran_code='" & lbtrancode.Caption & "'")
    End If

    ' 11,12,21 ke alawa sab me
    If Val(lbtrancode.Caption) > 0 And MyReqType = 12 And TranAmount = 0 Then
        ' 12 Contribution
    Else
        If txtregistrationno.Text = "" Then
            MsgBox "Please Select NSDL Branch First", vbInformation
            Exit Sub
        End If

        Req = Split(cboRequestType.Text, "#")
        ReqCode = Req(1)

        If ReqCode = "11" Or ReqCode = "12" Then
            ' 11 SUBSCRIBER REGISTRATION
            If VarScheme = "TIRE1" Then
                TxtTire1.Enabled = True
                TxtTire2.Enabled = False
            ElseIf VarScheme = "TIRE2" Then
                TxtTire2.Enabled = True
                TxtTire1.Enabled = False
            End If
        End If

        If ReqCode = "11" And Val(TxtTire1.Text) = 0 Then
            ' 11 SUBSCRIBER REGISTRATION
            txtpopregistration1.Text = "0"
        Else
            If ReqCode = "11" Or ReqCode = "12" Then
                ' We do not have to calculate pop registration charge other than 11,12,21
                FreshContri = Val(TxtTire1.Text) * 0.0025
                If FreshContri < 20 Then
                    FreshContri = 20
                End If

                If FreshContri >= 25000 Then
                    FreshContri = 25000
                End If

                If Val(TxtTire1.Text) > 0 Then
                    If CDate(DtDate) >= CDate("01/11/2017") Then
                        txtpopregistration1.Text = 200 + FreshContri
                    Else
                        txtpopregistration1.Text = 100 + FreshContri
                    End If
                Else
                    txtpopregistration1.Text = 0
                End If

                If VarScheme = "TIRE2" Or VarScheme = "TIRE1-2" Then
                    TxtTire2.Enabled = True
                    If Val(TxtTire2.Text) = 0 Then
                        txtpopregistration2.Text = "0"
                    Else
                        txtpopregistration2.Text = "0"
                    End If
                End If
            End If
        End If

        If Val(TxtTire2) <> 0 And ReqCode = "11" Then
            ' 11 SUBSCRIBER REGISTRATION
            txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
            If Val(txtpopregistration2.Text) < 20 Then
                txtpopregistration2.Text = 20
            End If
        End If

        If (ReqCode <> "11" And Val(TxtTire1) <> 0) Then
            ' 11 SUBSCRIBER REGISTRATION
            If ReqCode = "11" Or ReqCode = "12" Then
                ' We do not have to calculate pop registration charge other than 11,12,21
                txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
                If Val(txtpopregistration1.Text) < 20 Then
                    txtpopregistration1.Text = 20
                End If
                If Val(txtpopregistration1.Text) >= 25000 Then
                    txtpopregistration1.Text = 25000
                End If
            End If
        End If

        If (ReqCode <> "11" And Val(TxtTire2) <> 0) Then
            ' 11 SUBSCRIBER REGISTRATION
            txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
            If Val(txtpopregistration2.Text) < 20 Then
                txtpopregistration2.Text = 20
            End If
            If Val(txtpopregistration2.Text) >= 25000 Then
                txtpopregistration2.Text = 25000
            End If
        End If

        If (ReqCode <> "11" And Val(TxtTire2) <> 0) And VarScheme = "TIRE1-2" Then
            ' Do nothing
        End If

        If (ReqCode <> "11" And ReqCode <> "12") Then
            ' 11 SUBSCRIBER REGISTRATION, 12 Contribution
            TxtCollection.Visible = True
            Label22.Visible = True
            Label19.Caption = "Misclenious collection"
            Label22.Caption = "Collection Amount"
            TxtTire2.Text = "0"
            txtpopregistration1.Text = "0"
            txtpopregistration2.Text = "0"
            txtAmountInvest = "0"
            TxtTire2.Enabled = False
            txtServiceAmount.Text = Round((Val(TxtTire1.Text) * v_servicetax), 2)
            txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2)
        Else
            TxtCollection.Text = "0"
            TxtCollection.Visible = False
            Label22.Visible = False
            Label22.Caption = "Amount Invested"
            Label19.Caption = "Amount Invested"
            MiscAmt.Visible = False
            txtAmountInvest.Visible = True
            If VarScheme = "TIRE1" Or VarScheme = "TIRE1-2" Then
                TxtTire1.Enabled = True
            End If
            If (ReqCode <> "11") Then
                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
            Else
                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
            End If
            txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2)
        End If

        If (ReqCode <> "11") Then
            If ReqCode = "11" Or ReqCode = "12" Then
                If Val(TxtTire1.Text) > 0 Then
                    txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
                    If Val(txtpopregistration1.Text) < 20 Then
                        txtpopregistration1.Text = 20
                    End If
                    If Val(txtpopregistration1.Text) >= 25000 Then
                        txtpopregistration1.Text = 25000
                    End If
                Else
                    txtpopregistration1.Text = 0
                End If

                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
                txtAmountInvest.Text = Abs(Round((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2))
            End If
        End If

        If Val(TxtCollection.Text) > 0 Then
            txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(TxtCollection.Text)) * v_servicetax), 2)
            txtAmountInvest.Text = Round((Val(TxtCollection.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2)
        End If
    End If

    If OptCorporate.Value = True And ReqCode = 11 Then
        txtpopregistration1.Text = Round((Val(TxtTire1.Text) / (100 + v_servicetax * 100)) * 100, 2)
        txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text)) * v_servicetax), 2)
        txtAmountInvest.Text = 0
    End If
End Sub
