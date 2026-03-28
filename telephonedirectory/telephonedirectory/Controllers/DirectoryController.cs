using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using telephonedirectory.Models;

namespace telephonedirectory.Controllers
{
    public class DirectoryController : Controller
    {

        private readonly string _connectionString;

        public DirectoryController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }



        // GET: Read all entries
        public IActionResult Index()
        {
            List<TelephoneEntry> entries = new List<TelephoneEntry>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"SELECT *
                                 FROM TelephoneEntries";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    entries.Add(new TelephoneEntry
                    {
                        id = (int)dr["Id"],
                        name = dr["Name"].ToString()!,
                        address = dr["Address"].ToString(),
                        telephone = dr["Telephone"].ToString()!,
                        country = dr["Country"].ToString()!,
                        province = dr["Province"].ToString()!,
                        district = dr["District"].ToString()!
                    });
                }
            }

            return View(entries);
        }

        // GET: Show form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Insert into DB
        [HttpPost]
        public IActionResult Create(TelephoneEntry entry)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO TelephoneEntries
                                    (Name, Address, Telephone, Country, Province, District)
                                     VALUES
                                    (@Name, @Address, @Telephone, @Country, @Province, @District)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    //cmd.Parameters.AddWithValue("@Id", entry.id);
                    cmd.Parameters.AddWithValue("@Name", entry.name);
                    cmd.Parameters.AddWithValue("@Address", entry.address ?? (object?)DBNull.Value );
                    cmd.Parameters.AddWithValue("@Telephone", entry.telephone);
                    cmd.Parameters.AddWithValue("@Country", entry.country! ?? (object?)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Province", entry.province! ?? (object?)DBNull.Value);
                    cmd.Parameters.AddWithValue("@District", entry.district! ?? (object?)DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(entry);
        }



        //EDIT

        [HttpGet]
        public IActionResult Edit(int id)
        {
            TelephoneEntry entry = null!;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM TelephoneEntries WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    entry = new TelephoneEntry
                    {
                        id = (int)dr["Id"],
                        name = dr["Name"].ToString()!,
                        address = dr["Address"] == DBNull.Value ? null : dr["Address"].ToString(),
                        telephone = dr["Telephone"].ToString()!,
                        country = dr["Country"] == DBNull.Value ? "Nepal" : dr["Country"].ToString(),
                        province = dr["Province"].ToString(),
                        district = dr["District"].ToString()
                    };
                }

            }
            if (entry == null)
                return NotFound();

            return View(entry);
        }

        [HttpPost]
        public IActionResult Edit(TelephoneEntry entry)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = @"UPDATE TelephoneEntries
                             SET Name=@Name, Address=@Address, Telephone=@Telephone,
                                 Country=@Country, Province=@Province, District=@District
                             WHERE Id=@Id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", entry.id);
                    cmd.Parameters.AddWithValue("@Name", entry.name);
                    cmd.Parameters.AddWithValue("@Address", (object?)entry.address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Telephone", entry.telephone);
                    cmd.Parameters.AddWithValue("@Country", entry.country);
                    cmd.Parameters.AddWithValue("@Province", entry.province);
                    cmd.Parameters.AddWithValue("@District", entry.district);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Entry updated successfully!";
                return RedirectToAction("Index");
            }

            return View(entry);
        }




        // GET: Details
        [HttpGet]
        public IActionResult Details(int id)
        {
            TelephoneEntry entry = null!;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM TelephoneEntries WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    entry = new TelephoneEntry
                    {
                        id = (int)dr["Id"],
                        name = dr["Name"].ToString()!,
                        address = dr["Address"] == DBNull.Value ? null : dr["Address"].ToString(),
                        telephone = dr["Telephone"].ToString()!,
                        country = dr["Country"].ToString(),
                        province = dr["Province"].ToString(),
                        district = dr["District"].ToString()
                    };
                }
            }

            if (entry == null)
                return NotFound();

            return View(entry); // This will use Details.cshtml
        }


        //DELETE
        // GET: Delete confirmation
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TelephoneEntry entry = null!;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM TelephoneEntries WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    entry = new TelephoneEntry
                    {
                        id = (int)dr["Id"],
                        name = dr["Name"].ToString()!,
                        address = dr["Address"] == DBNull.Value ? null : dr["Address"].ToString(),
                        telephone = dr["Telephone"].ToString()!,
                        country = dr["Country"].ToString(),
                        province = dr["Province"].ToString(),
                        district = dr["District"].ToString()
                    };
                }
            }

            if (entry == null)
                return NotFound();

            return View(entry);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM TelephoneEntries WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Entry deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
