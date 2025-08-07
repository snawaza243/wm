CREATE OR REPLACE PROCEDURE WEALTHMAKER.SEND_MAIL (
   recp      VARCHAR2,
   from_id   VARCHAR2,
   msg       CLOB,
   msg1      CLOB,
   msg2      CLOB,
   msg3      CLOB,
   sub       VARCHAR2
)
IS
   tmpvar        NUMBER;
   sender        VARCHAR2 (50);
   recipient     VARCHAR2 (500);
   ccrecipient   VARCHAR2 (50);
   subject       VARCHAR2 (500);
   MESSAGE       VARCHAR2 (1500);
   crlf          VARCHAR2 (2):= UTL_TCP.crlf;
   connection    UTL_SMTP.connection;
   mailhost      VARCHAR2 (50):= 'smtp-relay.gmail.com';    
   header        VARCHAR2 (1000);
   error_msg     VARCHAR2 (4000);
   msg_count     NUMBER (10);
   pos           NUMBER (10);
   norecp        NUMBER;
   recp1         VARCHAR2 (200);
   i             NUMBER;
   l_smtp_user   VARCHAR2(255) := 'wealthmaker@bajajcapital.com';
   l_smtp_pass   VARCHAR2(255) := 'Em@!l@321';
BEGIN
   tmpvar := 0;
   norecp := 0;
   i := 0;
   
   RETURN;
   /* OLD CONFIGURATION
   connection := UTL_SMTP.open_connection (mailhost,587);
   --Authenticate with SMTP server
   UTL_SMTP.starttls(connection);
   UTL_SMTP.helo(connection, 'bajajcapital.com');
   UTL_SMTP.command(connection, 'AUTH LOGIN');
   UTL_SMTP.command(connection, l_smtp_user, l_smtp_pass);  
   UTL_SMTP.command(connection, UTL_ENCODE.base64_encode(UTL_RAW.cast_to_raw(l_smtp_user)));
   UTL_SMTP.command(connection, UTL_ENCODE.base64_encode(UTL_RAW.cast_to_raw(l_smtp_pass)));
   */
   
   /*----------------------- CHANGES DONE BY ACODUMMY---------------------*/
   connection := utl_smtp.open_connection(
      host => mailhost,
      port => 587,
      wallet_path => 'file:/home/oracle/oracle-wallet-email-new',
      wallet_password => 'oracle@123',
      secure_connection_before_smtp => FALSE);
    UTL_SMTP.ehlo(connection, 'bajajcapital.com');
    UTL_SMTP.starttls(connection); 
    
    UTL_SMTP.AUTH(
       c => connection,
       username => l_smtp_user,
       password => l_smtp_pass,
       schemes  => 'PLAIN LOGIN');
    -------------------------------------------------------------------------   
       
   sender := from_id;
   recp1 := recp;

   WHILE recp1 IS NOT NULL AND INSTR (recp1, ';') > 0
   LOOP
      IF INSTR (recp1, ';') > 0
      THEN
         recp1 := SUBSTR (recp1, INSTR (recp1, ';') + 1);
         norecp := norecp + 1;
      END IF;
   END LOOP;

   norecp := norecp + 1;
   recipient := recp;
   recp1 := recp;
   subject := sub;
   header :=
         'Date: '
      || TO_CHAR (SYSDATE, 'dd Mon yy hh24:mi:ss')
      || crlf
      || 'From: '
      || sender
      || crlf
      || 'Subject: '
      || subject
      || crlf
      || '';
   header := header || 'To: ' || recipient || crlf || '';
   header := header || 'CC: ' || ccrecipient || crlf;
   --UTL_SMTP.helo (connection, mailhost);
    UTL_SMTP.helo (connection, mailhost);
   
   UTL_SMTP.mail (connection, sender);

   WHILE i < norecp
   LOOP
      recipient := SUBSTR (recp1, 1, INSTR (recp1, ';') - 1);

      IF recipient IS NOT NULL
      THEN
         
         UTL_SMTP.rcpt (connection, TRIM (recipient));
      END IF;

      recp1 := SUBSTR (recp1, INSTR (recp1, ';') + 1);
      i := i + 1;
   END LOOP;

   UTL_SMTP.rcpt (connection, TRIM (recp1));
   
  --UTL_SMTP.rcpt (connection, ccrecipient);
   UTL_SMTP.open_data (connection);
   UTL_SMTP.write_data (connection,
                           'MIME-Version: 1.0'
                        || CHR (13)
                        || CHR (10)
                        || 'Content-type: text/html'
                        || CHR (13)
                        || CHR (10)
                        || 'X-Priority:1'
                        || CHR (13)
                        || CHR (10)
                        || header
                       );
   pos := 0;
   msg_count := LENGTH (msg2);

   IF msg_count > 10000
   THEN
      UTL_SMTP.write_data (connection, crlf || msg || msg1);

      WHILE msg_count > 0
      LOOP
         UTL_SMTP.write_data (connection, SUBSTR (msg2, pos, 10000));
         msg_count := msg_count - 10000;
         pos := pos + 10000 + 1;
      END LOOP;

      UTL_SMTP.write_data (connection,
                           msg3 || '<br><br><br><br><br><br><br><Br><br>'
                          );
   ELSE
      UTL_SMTP.write_data (connection,
                              crlf
                           || msg
                           || msg1
                           || msg2
                           || msg3
                           || '<br><br><br><br><br><br><br><Br><br>'
                          );
   END IF;

   UTL_SMTP.close_data (connection);
   UTL_SMTP.quit (connection);
--EXCEPTION
--   WHEN NO_DATA_FOUND
--   THEN
--      NULL;
--   WHEN OTHERS
--   THEN
--       --utl_smtp.close_data(connection);
--      -- utl_smtp.quit(connection);
--      error_msg := REPLACE (SQLERRM (SQLCODE), '''', '');

--     DBMS_OUTPUT.put_line(error_msg);

--      COMMIT;
--      -- Consider logging the error and then re-raise
--      RAISE;
END;
/
