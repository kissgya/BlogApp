using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using BlogApi.Models;
using BlogApi.Models.DTOs;

namespace BlogApi.Controllers
{
    [Route("blogger")]
    [ApiController]
    public class BloggerController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog;uid=root;password=;";

        [HttpGet]
        public object GetAllBlogger()
        {
            List<Blogger> bloggers = new List<Blogger>();
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = "SELECT * FROM blogger;";

            var cmd = new MySqlCommand(sql, connector);

            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogger = new Blogger
                {
                    Id = datareader.GetInt32(0),
                    Name = datareader.GetString(1),
                    Email = datareader.GetString(2),
                    Age = datareader.GetInt32(3),
                    Password = datareader.GetString(4),
                    RegistrationTime = datareader.GetDateTime(5),
                };

                bloggers.Add(blogger);
            }

            connector.Close();

            return new {message = "Sikeres lekérdezés", result = bloggers};
        }

        [HttpGet("count")]
        public object GetBloggerCount()
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = "SELECT COUNT(*) FROM blogger;";
            var cmd = new MySqlCommand(sql, connector);

            var count = Convert.ToInt32(cmd.ExecuteScalar());

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = count };
        }

        [HttpGet("contacts")]
        public object GetOrderedContacts()
        {
            var contacts = new List<object>();
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = "SELECT Name, Email FROM blogger ORDER BY Name ASC;";

            var cmd = new MySqlCommand(sql, connector);
            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                contacts.Add(new
                {
                    Name = datareader.GetString(0),
                    Email = datareader.GetString(1)
                });
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = contacts };
        }

        [HttpGet("byId")]
        public object GetBloggerById(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            
            connector.Open();

            string sql = @"SELECT * FROM `blogger` WHERE `id` = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();

            Blogger blogger = null;
            object result = null;

            if (datareader.Read() == true)
            {
                blogger = new Blogger
                {
                    Id = datareader.GetInt32("id"),
                    Name = datareader.GetString("name"),
                    Email = datareader.GetString("email"),
                    Age = datareader.GetInt32("age"),
                    Password = datareader.GetString("password"),
                    RegistrationTime = datareader.GetDateTime("registrationTime")
                };

                result = new { message = "Sikeres lekérdezés", result = blogger };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = blogger }; 
            }
            
            connector.Close();
            return result;            
        }

        [HttpPost]

        public object AddNewBlogger(AddNewBloggerDto addNewBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"INSERT INTO `blogger`(`name`, `email`, `age`, `password`, `registrationTime`) VALUES (@name,@email,@age,@password,@registrationTime)";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", addNewBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", addNewBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", addNewBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", addNewBloggerDto.Password);
            cmd.Parameters.AddWithValue("@registrationTime", DateTime.Now);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres felvétel.", result = addNewBloggerDto };
        }

        [HttpDelete]

        public object DeleteBlogger([FromBody] int id)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"DELETE FROM `blogger` WHERE id = @id";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connector.Close();

            return new { message = "Sikeres törlés.", result = "" };
        }

        [HttpPut]

        public object UpdateBloggerDto(int id, UpdateBloggerDto updateBloggerDto)
        {
            var connector = new MySqlConnection(ConnectionString);

            connector.Open();

            string sql = @"UPDATE `blogger` SET `name`=@name, `email`=@email, `age`=@age, `password`=@password WHERE `id` = @id;";

            var cmd = new MySqlCommand(sql, connector);

            cmd.Parameters.AddWithValue("@name", updateBloggerDto.Name);
            cmd.Parameters.AddWithValue("@email", updateBloggerDto.Email);
            cmd.Parameters.AddWithValue("@age", updateBloggerDto.Age);
            cmd.Parameters.AddWithValue("@password", updateBloggerDto.Password);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

            connector.Close();
            return new { message = "Sikeres frissítés.", result = updateBloggerDto };
        }

        [HttpGet("nameAndEmailById")]
        public object GetBloggerNameAndEmail(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            
            string sql = "SELECT name, email FROM blogger WHERE id = @id;";
            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            object result = null;

            if (datareader.Read())
            {
                result = new
                {
                    message = "Sikeres lekérdezés",
                    result = new
                    {
                        Name = datareader.GetString("name"),
                        Email = datareader.GetString("email")
                    }
                };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = "" };
            }

            connector.Close();
            return result;
        }

        [HttpGet("postsWithBloggerName")]
        public object GetPostsWithBloggerName(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            
            string sql = @"
                SELECT blogger.name, blogpost.Title, blogpost.Content 
                FROM blogger 
                INNER JOIN blogpost ON blogger.id = blogpost.blogId 
                WHERE blogger.id = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            var results = new List<object>();

            while (datareader.Read())
            {
             
                results.Add(new
                {
                    Name = datareader.GetString("name"),
                    Title = datareader.GetString("Title"),
                    Content = datareader.GetString("Content")
                });
            }

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = results };
        }
    };
}
