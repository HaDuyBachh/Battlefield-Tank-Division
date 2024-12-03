using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class ClientCaller : MonoBehaviour
{
    private Process clientProcess;
    public bool IsClick = false;
    void Start()
    {
        StartClientProcess("HelloFromUnityWithRespect");  // Gọi client.exe lần đầu với tham số
    }

    public void Update()
    {
        if (IsClick)
        {
            StartClientProcess("Continue..");
            IsClick = false;
        }    
    }

    // Hàm gọi client.exe với tham số dòng lệnh
    public void StartClientProcess(string arguments)
    {
        // Đảm bảo rằng client.exe nằm trong thư mục StreamingAssets
        string clientExePath = Path.Combine(Application.streamingAssetsPath, "client.exe");

        // Kiểm tra xem file client.exe có tồn tại không
        if (File.Exists(clientExePath))
        {
            if (clientProcess == null || clientProcess.HasExited)
            {
                // Nếu client chưa được khởi động hoặc đã kết thúc, khởi động lại
                clientProcess = new Process();
                clientProcess.StartInfo.FileName = clientExePath;
                clientProcess.StartInfo.Arguments = arguments;  // Truyền tham số dòng lệnh
                clientProcess.Start();
                SendCommandToClient(arguments);
                Debug.Log("Tạo mới --");
            }
            else
            {
                // Gửi các tham số mới vào client (nếu client đang chạy)
                SendCommandToClient(arguments);
                Debug.Log("Dùng sẵn --");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("client.exe không tìm thấy trong thư mục StreamingAssets.");
        }
    }

    // Hàm gửi tham số mới cho client khi client đã được khởi động
    private void SendCommandToClient(string arguments)
    {
        // Có thể sử dụng IPC (Inter-process communication), như socket hoặc file shared
        // Tạo kết nối TCP, UDP, hoặc sử dụng file để gửi lệnh mới
        Debug.Log("Gửi lệnh mới cho client: " + arguments);

        // Ví dụ đơn giản: Ghi vào file để client đọc
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "command.txt"), arguments);
        Debug.Log("Lệnh đã được ghi vào file command.txt");
    }
}
