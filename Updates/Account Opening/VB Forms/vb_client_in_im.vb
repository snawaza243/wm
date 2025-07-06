'------------------------------------------------------INSERT INTO INVESTOR MASTER SELF-------------------------------------
        Ky = "insert into investor_master (occupation_id,INVESTOR_CODE,investor_title,branch_code,investor_name,address1,address2,fax,email,pan,dob,mobile,pincode,gender,phone1,phone2,status,city_id,kyc,source_id,rm_code,AADHAR_CARD_NO)"
        Ky = Ky & " values(" & MyOccCode & ",1,'" & cmbtitle.Text & "','" & MyBranch & "', '" & txtclientname & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "',"
        If TxtFax <> "" Then
            Ky = Ky & " " & TxtFax & ","
        Else
            Ky = Ky & " 0 ,"
        End If
        Ky = Ky & " '" & TxtEmail & "','" & txtclientpan & "',"
        If dob.Text <> "__/__/____" Then
            Ky = Ky & " '" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        Else
            Ky = Ky & " null,"
        End If
        If TxtMobile.Text <> "" Then
            Ky = Ky & " '" & TxtMobile & "',"
        Else
            Ky = Ky & " '',"
        End If
        If txtpin.Text <> "" Then
            Ky = Ky & " '" & txtpin & "',"
        Else
            Ky = Ky & " ' ',"
        End If
        Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "',"
        If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & TxtStd.Text & "-" & TxtTel1.Text & "', "
        Else
            Ky = Ky & " '', "
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " '" & txtstd1.Text & "-" & TxtTel2.Text & "', "
        Else
            Ky = Ky & " '', "
        End If
        Ky = Ky & " '" & cmbstatus.Text & "','" & MyCityCode & "',"
        Ky = Ky & " 'YESP'," & MySourceId & "," & MyRmCode & ","
        If txtaddharCardNo.Text <> "" Then
            Ky = Ky & " " & txtaddharCardNo.Text & " "
        Else
            Ky = Ky & " '' "
        End If
        Ky = Ky & " )"
        MyConn.Execute (Ky)
        