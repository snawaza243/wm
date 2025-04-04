select EMP_NO,  PITCH_BOOK_NO, guest_cd
from BAJAJ_VENUE_BOOKING 
where EMP_NO is not null
and PITCH_BOOK_NO is not null and telephone is not null;


select telephone from bajaj_venue_booking where guest_cd = '91882357';