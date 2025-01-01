using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Npgsql;
using System;
using System.Threading.Tasks;

public class DatabaseConnect : MonoBehaviour
{
    private string connString;
    void Start()
    {
        // Thay đổi các thông số kết nối theo thông tin của bạn
        string host = "localhost";
        string port = "5432";
        string database = "tankdivision";
        string username = "postgres";
        string password = "bachbeo30";

        connString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
    }

    public async Task<bool> Login(string username, string password)
    {
        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                    SELECT EXISTS (
                        SELECT 1 FROM public.""User"" 
                        WHERE username = @username 
                        AND password = @password
                    );";

                    // Thêm parameters để tránh SQL injection
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    // Thực thi và lấy kết quả
                    bool exists = (bool)await cmd.ExecuteScalarAsync();
                    return exists;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Lỗi kết nối: {ex.Message}");
            return false;
        }
    }
}
