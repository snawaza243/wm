  public void newFun()
  {

      npsEcsDdlCompany.Items.Clear();
      string sql1 = ("select iss_name,iss_code from iss_master where iss_code='IS02520'");
      npsEcsDdlCompany.Items.Clear();

      DataTable dtSql1 = pc.ExecuteCurrentQuery(sql1);

      if (dtSql1.Rows.Count > 0)
      {
          foreach (DataRow dr in dtSql1.Rows)
          {
              npsEcsDdlCompany.Items.Add(Convert.ToString(dr["iss_name"]) + new string(' ', 60) + "#" + Convert.ToString(dr["iss_code"]));
          }
      }

      npsEcsDdlCompany.SelectedIndex = 0;

      string sql2 = (" select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status");

      DataTable dtSql2 = pc.ExecuteCurrentQuery(sql2);
      npExsDdlStatus.Items.Clear();

      if (dtSql2.Rows.Count > 0)
      {
          foreach (DataRow dr in dtSql2.Rows)
          {
              npExsDdlStatus.Items.Add(Convert.ToString(dr["status"]) + new string(' ', 60) + "#" + Convert.ToString(dr["status_cd"]));
          }
      }
      if (npExsDdlStatus.Items.Count > 0)
      {
          npExsDdlStatus.SelectedIndex = 0;
      }
 
  }
