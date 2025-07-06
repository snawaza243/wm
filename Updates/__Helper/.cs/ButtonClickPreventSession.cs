protected void MyButton_Click(object sender, EventArgs e)
{
    try
    {
        string loggedinUser = Session["LoginId"]?.ToString();

        if (!string.IsNullOrEmpty(loggedinUser))
        {

            psm_controller.ReloadCurrentPage(this);

        }
        else
        {

            psm_controller.RedirectToWelcomePage();
        }
    }
    catch (Exception ex)
    {
        psm_controller.ShowAlert(this, "Error " + ex.Message);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideServerLoader", "hideServerLoader();", true);
        return;
    }
    finally
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideServerLoader", "hideServerLoader();", true);
    }
}