using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class GeneralSystem
{
    /// Type
    public enum Command
    {
        None,
        Move,
        Rotate,
        Login,
    }

    // Function
    public static byte[] Decompress(byte[] compressedData)
    {
        using (var inputStream = new MemoryStream(compressedData))
        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        using (var outputStream = new MemoryStream())
        {
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
    public static byte[] Compress(byte[] data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(data, 0, data.Length);
            }
            return memoryStream.ToArray();
        }
    }
    public static byte[] StringToByte(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
    public static byte[] Vector3ArrayToByte(Vector3[] v3)
    {
        int curr = 0;
        byte[] byteArray = new byte[v3.Length * 3 * sizeof(float)];
        for (int i = 0; i < v3.Length; i++)
        {
            var en = Vector3ToByte(v3[i]);
            Array.Copy(en, 0, byteArray, curr, en.Length);
            curr += en.Length;
        }

        return byteArray;
    }
    public static Vector3[] ByteArrayToVector3Array(byte[] byteArray, int vectorCount)
    {
        // Kiểm tra xem số lượng byte có hợp lệ không với số lượng Vector3 cần giải mã
        if (byteArray.Length != vectorCount * 3 * sizeof(float))
        {
            throw new ArgumentException("Byte array length does not match the expected length for the given number of Vector3 elements");
        }

        Vector3[] vectorArray = new Vector3[vectorCount];
        int curr = 0;

        for (int i = 0; i < vectorCount; i++)
        {
            // Giải mã mỗi Vector3 từ mảng byte
            byte[] vectorBytes = new byte[3 * sizeof(float)];
            Array.Copy(byteArray, curr, vectorBytes, 0, vectorBytes.Length);
            vectorArray[i] = ByteToVector3(vectorBytes);

            // Cập nhật chỉ số cho lần giải mã tiếp theo
            curr += vectorBytes.Length;
        }

        return vectorArray;
    }
    public static byte[] Vector3ToByte(Vector3 vector)
    {
        // Tạo mảng byte để chứa ba thành phần float của Vector3
        byte[] bytes = new byte[3 * sizeof(float)];

        // Chuyển đổi các thành phần float của Vector3 thành byte
        Array.Copy(BitConverter.GetBytes(vector.x), 0, bytes, 0, sizeof(float));
        Array.Copy(BitConverter.GetBytes(vector.y), 0, bytes, sizeof(float), sizeof(float));
        Array.Copy(BitConverter.GetBytes(vector.z), 0, bytes, 2 * sizeof(float), sizeof(float));

        return bytes;
    }
    public static Vector3 ByteToVector3(byte[] bytes)
    {
        if (bytes.Length != 3 * sizeof(float))
            throw new ArgumentException("Byte array length is not valid for a Vector3");

        // Chuyển đổi các byte thành các giá trị float
        float x = BitConverter.ToSingle(bytes, 0);
        float y = BitConverter.ToSingle(bytes, sizeof(float));
        float z = BitConverter.ToSingle(bytes, 2 * sizeof(float));

        return new Vector3(x, y, z);
    }
    public static byte[] EncodeIntTo2Bytes(int value)
    {
        // Kiểm tra giá trị nằm trong phạm vi của 2 byte
        if (value < -32768 || value > 65535)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be between -32768 and 65535 for 2-byte encoding.");

        // Tách byte cao và byte thấp
        byte highByte = (byte)(value >> 8); // Byte cao
        byte lowByte = (byte)(value & 0xFF); // Byte thấp

        return new byte[] { highByte, lowByte };
    }
    public static List<byte> Encode(byte[] messageBytes, byte command, byte id)
    {
        List<byte> data = new List<byte>();

        // Chuyển thông điệp thành byte  --------------------------------------
        ///[command]:   1 byte
        data.Add(command);
        ///[id]:        1 byte
        data.Add(id);
        ///[length]:    2 byte
        data.AddRange(EncodeIntTo2Bytes(messageBytes.Length));
        ///[data]: length byte
        data.AddRange(messageBytes);

        return data;
    }
    public static List<(byte command, int id, int dataLength, byte[] data)> DecodeOnce(byte[] encodedData)
    {
        var result = new List<(byte command, int id, int dataLength, byte[] data)>();
        int offset = 0;

        while (offset < encodedData.Length)
        {
            if (offset + 4 > encodedData.Length)
            {
                Debug.Log("Remaining data is too short to decode.");
                break;
            }

            // Đọc command và id
            byte command = encodedData[offset];
            byte id = encodedData[offset + 1];

            // Đọc length (2 byte)
            int dataLength = BitConverter.ToInt16(new byte[] { encodedData[offset + 3], encodedData[offset + 2] }, 0);

            // Kiểm tra dữ liệu có đủ dài không
            if (offset + 4 + dataLength > encodedData.Length)
            {
                Debug.Log("Invalid data length: not enough bytes for this packet.");
                break;
            }

            // Lấy dữ liệu thực tế
            byte[] data = new byte[dataLength];
            Array.Copy(encodedData, offset + 4, data, 0, dataLength);

            // Thêm vào danh sách kết quả
            result.Add((command, id, dataLength, data));

            // Cập nhật offset
            offset += 4 + dataLength;
        }

        Debug.Log("Decoding complete.");
        return result;
    }
}
