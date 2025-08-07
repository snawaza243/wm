file_names = [
    "PSM_LOG_CHANNEL_LIST",
    "PSM_LOG_BRANCH_LIST",
    "PSM_LOG_ZONE_LIST",
    "PSM_LOG_REGION_LIST",
    "PSM_LOG_RM_LIST",
    "PSM_LOG_AMC_LIST",
    "PSM_LOG_BRANCH_LIST",
    "PSM_LOG_BRANCH_LIST",
    "PSM_LOG_ZONE_LIST", 
    "PSM_LOG_RM_LIST",
    "PSM_MF_RECO_MANUAL_TR2",
    "PSM_MF_RECO_MANUAL_RTA2",
    "PSM_MF_RECO_M_FIND_TRAN2",
    "PMS_MF_RECO_M_SETREMARK",
    "PSM_MF_RECO_M_RECONCILE2",
    "PSM_MF_RECO_M_PMS_CNF2",
    "PSM_MF_RECO_M_PMS_UNCNF2"
]

for name in file_names:
    with open(f"{name}.sql", "w") as f:
        pass  # Creates an empty file

print(f"Created {len(file_names)} SQL files.")