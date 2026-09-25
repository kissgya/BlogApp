using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using BlogApi.Models;
using BlogApi.Models.DTOs;

namespace BlogApi.Controllers
{
    [Route("blogpost")]
    [ApiController]
    public class BlogpostController : ControllerBase
    {
        public string ConnectionString = "server=localhost;database=blog;uid=root;password=;";

        [HttpGet]
        public object GetAllBlogpost()
        {
            List<Blogpost> blogposts = new List<Blogpost>();
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = "SELECT * FROM blogpost;";
            var cmd = new MySqlCommand(sql, connector);
            var datareader = cmd.ExecuteReader();

            while (datareader.Read())
            {
                var blogpost = new Blogpost
                {
                    Id = datareader.GetInt32("Id"),
                    Title = datareader.GetString("Title"),
                    Content = datareader.GetString("Content"),
                    postTime = datareader.GetDateTime("postTime"),
                    updateTime = datareader.GetDateTime("updateTime"),
                    blogId = datareader.GetInt32("blogId")
                };
                blogposts.Add(blogpost);
            }

            connector.Close();
            return new { message = "Sikeres lekérdezés", result = blogposts };
        }

        [HttpGet("byId")]
        public object GetBlogpostById(int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = "SELECT * FROM blogpost WHERE Id = @id;";
            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            var datareader = cmd.ExecuteReader();
            Blogpost blogpost = null;
            object result = null;

            if (datareader.Read())
            {
                blogpost = new Blogpost
                {
                    Id = datareader.GetInt32("Id"),
                    Title = datareader.GetString("Title"),
                    Content = datareader.GetString("Content"),
                    postTime = datareader.GetDateTime("postTime"),
                    updateTime = datareader.GetDateTime("updateTime"),
                    blogId = datareader.GetInt32("blogId")
                };
                result = new { message = "Sikeres lekérdezés", result = blogpost };
            }
            else
            {
                result = new { message = "Nincs ilyen Id.", result = blogpost };
            }

            connector.Close();
            return result;
        }

        [HttpPost]
        public object AddNewBlogpost(AddNewBlogpostDto addNewBlogpostDto)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"INSERT INTO blogpost (Title, Content, postTime, updateTime, blogId) 
                           VALUES (@title, @content, @postTime, @updateTime, @blogId);";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@title", addNewBlogpostDto.Title);
            cmd.Parameters.AddWithValue("@content", addNewBlogpostDto.Content);
            cmd.Parameters.AddWithValue("@postTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", addNewBlogpostDto.blogId);

            cmd.ExecuteNonQuery();
            connector.Close();

            return new { message = "Sikeres felvétel.", result = addNewBlogpostDto };
        }

        [HttpPut]
        public object UpdateBlogpost(int id, UpdateBlogpostDto updateBlogpostDto)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = @"UPDATE blogpost SET Title=@title, Content=@content, 
                           updateTime=@updateTime, blogId=@blogId WHERE Id = @id;";

            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@title", updateBlogpostDto.Title);
            cmd.Parameters.AddWithValue("@content", updateBlogpostDto.Content);
            cmd.Parameters.AddWithValue("@updateTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@blogId", updateBlogpostDto.blogId);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            connector.Close();

            return new { message = "Sikeres frissítés.", result = updateBlogpostDto };
        }

        [HttpDelete]
        public object DeleteBlogpost([FromBody] int id)
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            string sql = "DELETE FROM blogpost WHERE Id = @id;";
            var cmd = new MySqlCommand(sql, connector);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            connector.Close();

            return new { message = "Sikeres törlés.", result = "" };
        }

        [HttpGet("count")]
        public object GetBlogpostCount()
        {
            var connector = new MySqlConnection(ConnectionString);
            connector.Open();

            
            string sql = "SELECT COUNT(*) FROM blogpost;";
            var cmd = new MySqlCommand(sql, connector);

            
            var count = Convert.ToInt32(cmd.ExecuteScalar());

            connector.Close();

            return new { message = "Sikeres lekérdezés", result = count };
        }
    }
}