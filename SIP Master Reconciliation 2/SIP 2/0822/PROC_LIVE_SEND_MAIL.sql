CREATE OR REPLACE PROCEDURE WEALTHMAKER.SEND_MAIL (
   p_recp     VARCHAR2,   -- multiple recipients separated by ;
   p_from     VARCHAR2,
   p_msg      CLOB,
   p_msg1     CLOB,
   p_msg2     CLOB,
   p_msg3     CLOB,
   p_sub      VARCHAR2
)
IS
   l_conn       UTL_SMTP.connection;
   l_mailhost   VARCHAR2(100) := 'email-smtp.ap-south-1.amazonaws.com'; -- AWS SES
   l_smtp_user  VARCHAR2(255) := 'AKIAXKPU2BH7BU5WPFTG';
   l_smtp_pass  VARCHAR2(255) := 'BJTtxarxHwpF2dpJ9DbTnZdgzU5VCe6IO6xIp1NkIaAi';
   l_crlf       VARCHAR2(2)   := UTL_TCP.crlf;
   l_header     VARCHAR2(2000);
   l_recp       VARCHAR2(500);
   l_pos        PLS_INTEGER;
   l_piece      VARCHAR2(32767);
   l_offset     PLS_INTEGER := 1;
BEGIN
   -- Open connection
   l_conn := utl_smtp.open_connection(
                host => l_mailhost,
                port => 587,
                wallet_path => 'file:/rdsdbdata/userdirs/01',
                wallet_password => '',
                secure_connection_before_smtp => FALSE);

   -- Handshake
   utl_smtp.ehlo(l_conn, 'bajajcapital.com');
   utl_smtp.starttls(l_conn);
   utl_smtp.ehlo(l_conn, 'bajajcapital.com');

   -- Authenticate
   utl_smtp.auth(
      c        => l_conn,
      username => l_smtp_user,
      password => l_smtp_pass,
      schemes  => 'PLAIN LOGIN');

   -- Envelope
   utl_smtp.mail(l_conn, p_from);

   -- Support multiple recipients (semicolon separated)
   l_recp := p_recp;
   WHILE l_recp IS NOT NULL LOOP
      l_pos := INSTR(l_recp, ';');
      IF l_pos > 0 THEN
         utl_smtp.rcpt(l_conn, TRIM(SUBSTR(l_recp, 1, l_pos - 1)));
         l_recp := SUBSTR(l_recp, l_pos + 1);
      ELSE
         utl_smtp.rcpt(l_conn, TRIM(l_recp));
         l_recp := NULL;
      END IF;
   END LOOP;

   -- Message headers
   l_header :=
         'Date: ' || TO_CHAR(SYSDATE, 'Dy, DD Mon YYYY HH24:MI:SS') || l_crlf ||
         'From: ' || p_from || l_crlf ||
         'To: ' || p_recp || l_crlf ||
         'Subject: ' || p_sub || l_crlf ||
         'MIME-Version: 1.0' || l_crlf ||
         'Content-Type: text/html; charset=UTF-8' || l_crlf ||
         l_crlf;

   -- Open message
   utl_smtp.open_data(l_conn);
   utl_smtp.write_data(l_conn, l_header);

   -- Write body in chunks (for big CLOBs)
   FOR part IN 1..4 LOOP
      CASE part
         WHEN 1 THEN l_piece := p_msg;
         WHEN 2 THEN l_piece := p_msg1;
         WHEN 3 THEN l_piece := p_msg2;
         WHEN 4 THEN l_piece := p_msg3;
      END CASE;

      IF l_piece IS NOT NULL THEN
         l_offset := 1;
         WHILE l_offset <= LENGTH(l_piece) LOOP
            utl_smtp.write_data(l_conn, SUBSTR(l_piece, l_offset, 10000));
            l_offset := l_offset + 10000;
         END LOOP;
      END IF;
   END LOOP;

   utl_smtp.write_data(l_conn, '<br><br>-- End of Mail --<br>');

   -- Close
   utl_smtp.close_data(l_conn);
   utl_smtp.quit(l_conn);

EXCEPTION
   WHEN OTHERS THEN
      BEGIN
         utl_smtp.quit(l_conn);
      EXCEPTION WHEN OTHERS THEN NULL; END;
      RAISE;
END;
/