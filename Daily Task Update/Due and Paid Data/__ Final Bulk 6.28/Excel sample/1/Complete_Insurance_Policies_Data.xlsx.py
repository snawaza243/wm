import pandas as pd

# Extended data parsed from user-provided text
data_full = [
    ["343900831", "BAJAJ A", 14529, "Paid", "20-Mar-25", 14209, "", "20-Mar-25", 14528.67, "20-Sep-25", "Half Yearly", "ECS", "20-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["343940696", "BAJAJ A", 10479, "Paid", "20-Mar-25", 10248.44, "", "20-Mar-25", 10479.02, "20-Sep-25", "Half Yearly", "ECS", "20-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["343996822", "BAJAJ A", 29353, "Paid", "21-Mar-25", 28707, "", "21-Mar-25", 29353.19, "21-Mar-25", "Annual", "ECS", "21-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["344122374", "BAJAJ A", 47945, "Paid", "21-Mar-25", 46890, "", "21-Mar-25", 47945.02, "21-Mar-25", "Annual", "ECS", "21-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["343992036", "BAJAJ A", 25440, "Paid", "21-Mar-25", 24880, "", "21-Mar-25", 25440.2, "21-Mar-25", "Annual", "ECS", "21-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["359236932", "BAJAJ A", 5186, "Paid", "21-Mar-25", 5072, "", "08-Mar-25", 5186, "08-Mar-25", "Annual", "Non ECS", "08-Mar-19", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["360052832", "BAJAJ A", 12231, "Paid", "20-Mar-25", 11962, "", "20-Mar-25", 12231.15, "20-Mar-26", "Annual", "ECS", "20-Mar-19", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["359648176", "BAJAJ A", 48919, "Paid", "20-Mar-25", 47843, "", "20-Mar-25", 48919.39, "20-Mar-26", "Annual", "ECS", "20-Mar-19", "22-Mar-25", "LIFELONG_ASSURE"],
    ["359985082", "BAJAJ A", 4893, "Paid", "20-Mar-25", 4785, "", "20-Mar-25", 4892.66, "20-Mar-26", "Annual", "ECS", "20-Mar-19", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["358707982", "BAJAJ A", 6563, "Paid", "20-Mar-25", 6419, "", "04-Mar-25", 6563, "04-Jun-25", "Quarterly", "Non ECS", "04-Mar-19", "22-Mar-25", "LIFELONG_ASSURE"],
    ["349542166", "BAJAJ A", 12725, "Paid", "20-Mar-25", 12445, "", "11-Mar-25", 12724.8, "11-Sep-25", "Half Yearly", "ECS", "11-Sep-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["356414808", "BAJAJ A", 20548, "Paid", "20-Mar-25", 20095.46, "", "21-Jan-25", 20548, "21-Jan-26", "Annual", "Non ECS", "21-Jan-19", "22-Mar-25", "LIFELONG_ASSURE"],
    ["361190090", "BAJAJ A", 19882, "Paid", "20-Mar-25", 19444.93, "", "28-Mar-25", 19882, "28-Mar-25", "Annual", "Non ECS", "28-Mar-19", "22-Mar-25", "LIFELONG_ASSURE"],
    ["359907850", "BAJAJ A", 18234, "Paid", "20-Mar-25", 17833, "", "19-Mar-25", 18234.24, "19-Mar-26", "Annual", "ECS", "19-Mar-19", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["357632116", "BAJAJ A", 29353, "Paid", "21-Mar-25", 28707, "", "26-Feb-25", 29353, "26-Feb-25", "Annual", "Non ECS", "26-Feb-19", "22-Mar-25", "LIFELONG_ASSURE"],
    ["390205923", "BAJAJ A", 3170, "Paid", "21-Mar-25", 3100, "", "19-Mar-25", 3169.8, "19-Mar-25", "Monthly", "ECS", "19-Feb-20", "22-Mar-25", "LIFE_INCOME"],
    ["457081702", "BAJAJ A", 897, "Paid", "20-Mar-25", 877.17, "", "13-Mar-25", 896.91, "13-Apr-25", "Monthly", "ECS", "13-May-21", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["488391761", "BAJAJ A", 250000, "Paid", "21-Mar-25", 250000, "", "17-Dec-24", 250000, "17-Dec-24", "Annual", "Non ECS", "17-Dec-21", "22-Mar-25", "FUTURE_WEALTH_PLUS"],
    ["475208248", "BAJAJ A", 2642, "Paid", "20-Mar-25", 2583.74, "", "20-Mar-25", 2641.87, "20-Apr-25", "Monthly", "ECS", "20-Sep-21", "22-Mar-25", "LIFELONG_ASSURE"],
    ["393999538", "BAJAJ A", 19571, "Paid", "20-Mar-25", 19140, "", "20-Mar-25", 19570.66, "20-Mar-26", "Annual", "ECS", "20-Mar-20", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["394569184", "BAJAJ A", 3068, "Paid", "21-Mar-25", 3000, "", "21-Mar-25", 3067.5, "21-Mar-25", "Monthly", "ECS", "21-Mar-20", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["343998776", "BAJAJ A", 12524, "Paid", "21-Mar-25", 12248.75, "", "21-Mar-25", 12524.35, "21-Mar-25", "Half Yearly", "ECS", "21-Mar-18", "22-Mar-25", "LIFELONG_ASSURE"],
    ["394517680", "BAJAJ A", 17123, "Paid", "20-Mar-25", 16746, "", "21-Mar-25", 17123, "21-Mar-25", "Annual", "Non ECS", "21-Mar-20", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["408012671", "BAJAJ A", 12679, "Paid", "20-Mar-25", 12400.05, "", "27-Mar-25", 12679.05, "27-Mar-25", "Monthly", "ECS", "27-Jul-20", "22-Mar-25", "LUMP_SUM_BENEFIT"],
    ["427247037", "BAJAJ A", 3068, "Paid", "20-Mar-25", 3000, "", "20-Mar-25", 3067.5, "20-Apr-25", "Monthly", "ECS", "20-Nov-20", "22-Mar-25", "POS_GOAL_SURAKSHA"],
    ["507902007", "BAJAJ A", 10000, "Paid", "20-Mar-25", 10000, "", "20-Mar-25", 10000, "20-Apr-25", "Monthly", "ECS", "20-Apr-22", "22-Mar-25", "FUTURE_WEALTH_PLUS"],
    ["511143267", "BAJAJ A", 11515, "Paid", "20-Mar-25", 3226, "", "11-Jan-25", 11515, "11-Apr-25", "Monthly", "Non ECS", "11-Aug-22", "22-Mar-25", "SMART_LIFE_REG_RISK"],
    ["507900410", "BAJAJ A", 11000, "Paid", "20-Mar-25", 11000, "", "20-Mar-25", 11000, "20-Apr-25", "Monthly", "ECS", "20-Apr-22", "22-Mar-25", "FUTURE_WEALTH_PLUS"],
    ["503262206", "BAJAJ A", 511250, "Paid", "21-Mar-25", 500000, "", "22-Mar-25", 511250, "22-Mar-25", "Annual", "Non ECS", "22-Mar-22", "22-Mar-25", "ANNUITY_GUARANTEE_PENSION_B"],
    ["502538835", "BAJAJ A", 204541, "Paid", "20-Mar-25", 200040, "", "23-Mar-25", 204541, "23-Mar-25", "Annual", "Non ECS", "23-Mar-22", "22-Mar-25", "FLEXI_INCOME_SINGLE_LIFE"],
    ["416304338", "BAJAJ A", 15655, "Paid", "21-Mar-25", 15311, "", "21-Mar-25", 15655.01, "21-Mar-25", "Half Yearly", "ECS", "21-Sep-20", "22-Mar-25", "LIFELONG_ASSURE"],
    ["501686282", "BAJAJ A", 24461, "Paid", "21-Mar-25", 23923.05, "", "11-Mar-25", 24461.31, "11-Mar-25", "Annual", "ECS", "11-Mar-22", "22-Mar-25", "LIFELONG_ASSURE"],
    ["554205794", "BAJAJ A", 54796, "Paid", "20-Mar-25", 53590.14, "", "20-Mar-25", 54795.92, "20-Mar-25", "Annual", "ECS", "20-Mar-23", "22-Mar-25", "LIFELONG_ASSURE"],
    ["557654876", "BAJAJ A", 5000, "Paid", "20-Mar-25", 5000, "", "20-Mar-25", 5000, "20-Apr-25", "Monthly", "ECS", "20-Apr-23", "22-Mar-25", "FUTURE_WEALTH_PLUS"],
    ["554627216", "BAJAJ A", 102250, "Paid", "21-Mar-25", 100000, "", "24-Mar-25", 102250, "24-Mar-25", "Annual", "Non ECS", "24-Mar-23", "22-Mar-25", "SECOND_INCOME_WITH_ROP"],
    ["C249104466", "TATAAIG", 102250, "Paid", "22-Mar-25", 100000, "", "", 102250, "", "", "", "", "22-Mar-25", ""],
    ["C263199305", "TATAAIG", 102250, "Paid", "22-Mar-25", 100000, "", "", 102250, "", "", "", "", "22-Mar-25", ""],
    ["C239644279", "TATAAIG", 47554, "Paid", "22-Mar-25", 40300, "", "", 47554, "", "", "", "", "22-Mar-25", ""],
    ["C239652203", "TATAAIG", 13688, "Paid", "22-Mar-25", 11600, "", "", 13688, "", "", "", "", "22-Mar-25", ""],
    ["C265343269", "TATAAIG", 204500, "Paid", "22-Mar-25", 200000, "", "", 204500, "", "", "", "", "22-Mar-25", ""],
    ["C166639742", "TATAAIG", 3666, "Paid", "22-Mar-25", 3107, "", "", 3666, "", "", "", "", "22-Mar-25", ""],
    ["C275536516", "TATAAIG", 16048, "Paid", "22-Mar-25", 13600, "", "", 16048, "", "", "", "", "22-Mar-25", ""],
    ["C677270357", "TATAAIG", 14278, "Paid", "22-Mar-25", 12100, "", "", 14278, "", "", "", "", "22-Mar-25", ""],
    ["C677270399", "TATAAIG", 24898, "Paid", "22-Mar-25", 21100, "", "", 24898, "", "", "", "", "22-Mar-25", ""],
    ["C677265074", "TATAAIG", 15812, "Paid", "22-Mar-25", 13400, "", "", 15812, "", "", "", "", "22-Mar-25", ""],
    ["C676591082", "TATAAIG", 24171, "Paid", "22-Mar-25", 23639, "", "", 24171, "", "", "", "", "22-Mar-25", ""],
    ["C674995538", "TATAAIG", 14337, "Paid", "22-Mar-25", 12150, "", "", 14337, "", "", "", "", "22-Mar-25", ""],
    ["C675286453", "TATAAIG", 3828, "Paid", "22-Mar-25", 3744, "", "", 3828, "", "", "", "", "22-Mar-25", ""],
    ["C675321758", "TATAAIG", 20546, "Paid", "22-Mar-25", 20094, "", "", 20546, "", "", "", "", "22-Mar-25", ""],
    ["C243508783", "TATAAIG", 10798, "Paid", "22-Mar-25", 10560, "", "", 10798, "", "", "", "", "22-Mar-25", ""],
    ["C117778694", "TATAAIG", 17228, "Paid", "22-Mar-25", 14600, "", "", 17228, "", "", "", "", "22-Mar-25", ""]
]

# Define columns
columns_full = ["Policy No", "Code", "Total Premium Amount", "Status", "Last Payment Date", "Net Amount", 
                "Premium Type", "Due Date", "Modal Amount", "Next Due Date", "Frequency", "Mode", 
                "DOC", "Received date", "Plan Name"]

# Create DataFrame with all data
df_full = pd.DataFrame(data_full, columns=columns_full)

# Save to Excel
file_path_full = "Complete_Insurance_Policies_Data.xlsx"
df_full.to_excel(file_path_full, index=False)

file_path_full