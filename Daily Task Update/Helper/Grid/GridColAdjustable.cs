using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGridView();
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        BindGridView();
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // Add CSS class to header cells for styling
            foreach (TableCell cell in e.Row.Cells)
            {
                cell.CssClass = "resizable-header";
            }
        }
    }

    private void BindGridView()
    {
        // Sample data - replace with your actual data source
        List<Employee> employees = new List<Employee>
        {
            new Employee { ID = 1, Name = "John Smith", Email = "john@example.com", Department = "IT", JoinDate = DateTime.Now.AddYears(-2), Salary = 75000 },
            new Employee { ID = 2, Name = "Jane Doe", Email = "jane@example.com", Department = "HR", JoinDate = DateTime.Now.AddYears(-1), Salary = 65000 },
            new Employee { ID = 3, Name = "Robert Johnson", Email = "robert@example.com", Department = "Finance", JoinDate = DateTime.Now.AddMonths(-6), Salary = 80000 },
            new Employee { ID = 4, Name = "Emily Davis", Email = "emily@example.com", Department = "Marketing", JoinDate = DateTime.Now.AddYears(-3), Salary = 70000 },
            new Employee { ID = 5, Name = "Michael Wilson", Email = "michael@example.com", Department = "IT", JoinDate = DateTime.Now.AddMonths(-3), Salary = 85000 }
        };

        GridView1.DataSource = employees;
        GridView1.DataBind();
    }
}

public class Employee
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public DateTime JoinDate { get; set; }
    public decimal Salary { get; set; }
}