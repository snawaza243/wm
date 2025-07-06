       select common_id --nvl(count(common_id),0) into dt_count 
    from tb_doc_upload 
    where  verification_flag='1' 
    and rejection_status='0' 
    and tran_type='AC' 
    and punching_flag='0' 
    and common_id=TRIM(dt_number);