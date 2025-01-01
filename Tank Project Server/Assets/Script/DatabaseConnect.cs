using System.Collections;
using System.Collections.Generic;
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

    public async Task<(bool success, string message)> Register(string username, string password)
    {
        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                // Kiểm tra username đã tồn tại chưa
                using (var checkCmd = new NpgsqlCommand())
                {
                    checkCmd.Connection = conn;
                    checkCmd.CommandText = @"
                    SELECT EXISTS (
                        SELECT 1 FROM public.""User""
                        WHERE username = @username
                    );";
                    checkCmd.Parameters.AddWithValue("username", username);

                    bool userExists = (bool)await checkCmd.ExecuteScalarAsync();
                    if (userExists)
                    {
                        Debug.Log("Username da ton tai");
                        return (false, "Username đã tồn tại!");
                    }
                }

                // Thêm user mới
                using (var insertCmd = new NpgsqlCommand())
                {
                    insertCmd.Connection = conn;
                    insertCmd.CommandText = @"
                    INSERT INTO public.""User"" (username, password, info, rank, kills)
                    VALUES (@username, @password, @info, @rank, @kills)";

                    insertCmd.Parameters.AddWithValue("username", username);
                    insertCmd.Parameters.AddWithValue("password", password);
                    insertCmd.Parameters.AddWithValue("info", "Info for player");
                    insertCmd.Parameters.AddWithValue("rank", "Bronze");
                    insertCmd.Parameters.AddWithValue("kills", 0);

                    int rowsAffected = await insertCmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        return (true, "Đăng ký thành công!");
                    }
                    return (false, "Đăng ký thất bại!");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Lỗi đăng ký: {ex.Message}");
            return (false, "Lỗi khi đăng ký: " + ex.Message);
        }
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
