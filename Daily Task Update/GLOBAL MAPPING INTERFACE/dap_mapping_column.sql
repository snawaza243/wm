--due
SELECT
    policy_no, 
    company_cd, 
    status_cd, 
    location, 
    cl_name, 
    prem_amt, 
    pay_mode, 
    sa, 
    due_date, 
    cl_add1, 
    cl_add2, 
    cl_add3, 
    cl_add4, 
    cl_add5, 
    cl_city, 
    cl_pin, 
    cl_phone1, 
    cl_phone2,
    cl_mobile, 
    plan_name, 
    doc, 
    prem_freq, 
    ply_term, 
    ppt, 
    net_amount
FROM bajaj_due_data;


-- paid
select
policy_no, company_cd, prem_amt, paid_date, net_amount, due_date, next_due_date,
prem_freq, pay_mode, doc, plan_name
from bajaj_paid_data;


-- lapsed
select 
policy_no, company_cd, status_cd, location, cl_name, prem_amt
pay_mode, sa, cl_add1, cl_add2, cl_add3, cl_add4, cl_add5, cl_city, 
cl_pin, cl_phone1, cl_phone2, cl_mobile, due_date, plan_name, doc, 
prem_freq, ply_term, ppt, net_amount, fup_date
from bajaj_due_data;

select * from bajaj_due_data;

