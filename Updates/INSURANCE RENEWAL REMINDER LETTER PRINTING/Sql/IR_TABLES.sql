Here are the tables referenced in the procedure PSM_IR_GENERATE_DATA:

Main Table(s) Used in FROM Clause:
bajaj_due_data (Alias: a)
client_master (Alias: c)
employee_master (Alias: e)



Tables Used in Subqueries:
state_master (For state_name)
bajaj_company_master (For company_name)
favour_master (For favour_name)
branch_master (Used multiple times for branch_name, branch_add1, branch_add2, sourceid, and filtering conditions)
bajaj_plan_master (For plan_name1)
bajaj_ar_head (Used multiple times for sa, pin1)
These tables contribute to constructing the dynamic SQL query used to fetch the required data. Let me know if you need a deeper breakdown of joins or dependencies! 🚀


// TRIGGERS
ALTER TRIGGER WEALTHMAKER.TRGBAJAJDUEDATA DISABLE;



// PROCEDURE 
PSM_IR_GENERATE_DATA

