
-- cmbPtroduct()

Select mut_code from mut_fund where mut_name = 'New Pension Scheme';

Select iss_code from iss_master a,product_class_issuer_mf b where trim(iss_name)='New Pension Scheme' 
and a.iss_code=b.issuermf_code and b.prod_code='New Pension Scheme';


select mut_name from (
    Select mut_name from mut_fund where mut_code = 'IS02520'
    union all
    Select iss_name mut_name from iss_master a,product_class_issuer_mf b where trim(iss_code)= 'IS02520'
    and a.iss_code=b.issuermf_code
) where rownum = 1;




Select nature_code,prod_code from product_master where name like '%'|| (select mut_name from (
    Select mut_name from mut_fund where mut_code = 'IS02520'
    union all
    Select iss_name mut_name from iss_master a,product_class_issuer_mf b where trim(iss_code)= 'IS02520'
    and a.iss_code=b.issuermf_code
) where rownum = 1) ||'%';

 and b.prod_code='New Pension Scheme';



Select nature_code,prod_code from product_master where name like '%New Pension Scheme%';


-- IS02520