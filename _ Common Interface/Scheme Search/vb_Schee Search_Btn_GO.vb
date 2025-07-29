Private Sub cmdGo_Click()
Dim rsFind As New ADODB.Recordset
Dim strMF As String
Dim CodeMF As String
Dim strSearch As String
Dim i As Integer
        msfgSchemes.Rows = 1
        msfgSchemes.Rows = 2
        If txtFind.Text <> "" Then
            strSearch = ConvertString(UCase(Trim(txtFind.Text)))
            rsFind.open "Select name,PROD_CODE from product_master where nature_code='NT001'", MyConn, adOpenForwardOnly
            strMF = rsFind("Name")
            CodeMF = rsFind("prod_code")
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
            Else
                rsFind.open "select sch_code,sch_name,Short_name,mut_name from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%" & strSearch & "%' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') and exists (SELECT sch_code FROM SCH_CITY_MAPPING WHERE sch_code=s.sch_code and (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and iss_code like 'MF%' ) ORDER BY sch_name,Short_name", MyConn, adOpenForwardOnly
            End If
            i = 1
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = strMF
                msfgSchemes.TextMatrix(i, 2) = rsFind("Short_name")
                msfgSchemes.TextMatrix(i, 3) = rsFind("sch_name")
                msfgSchemes.TextMatrix(i, 4) = rsFind("mut_name")
                msfgSchemes.TextMatrix(i, 5) = CodeMF
                msfgSchemes.TextMatrix(i, 6) = rsFind("sch_code")
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If Trim(GlbroleId) = 1 Then
                rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            Else
                 rsFind.open "select osch_code,osch_name,longname,name,iss_name from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%" & strSearch & "%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) OR CITY_ID='ALL') and (iss_code like 'IS%' or iss_code like 'MFIS%')) ORDER BY longname,osch_name", MyConn, adOpenForwardOnly
            End If
            While Not rsFind.EOF
                msfgSchemes.TextMatrix(i, 1) = rsFind("name")
                msfgSchemes.TextMatrix(i, 2) = rsFind("osch_name")
                msfgSchemes.TextMatrix(i, 3) = rsFind("longname")
                msfgSchemes.TextMatrix(i, 4) = rsFind("iss_name")
                msfgSchemes.TextMatrix(i, 6) = rsFind("osch_code")
                i = i + 1
                msfgSchemes.Rows = msfgSchemes.Rows + 1
                rsFind.MoveNext
            Wend
            rsFind.Close
            If msfgSchemes.Rows > 2 Then
                msfgSchemes.Rows = msfgSchemes.Rows - 1
                msfgSchemes.SetFocus
            End If
            AlignCols
        Else
            txtFind.SetFocus
        End If
        Set rsFind = Nothing
End Sub