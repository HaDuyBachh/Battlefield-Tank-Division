#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <winsock2.h>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_PORT 8888

int main(int argc, char *argv[]) {
    WSADATA wsa;
    SOCKET clientSocket;
    struct sockaddr_in serverAddr;
    int serverAddrLen = sizeof(serverAddr);

    // Kiểm tra tham số dòng lệnh
    if (argc < 2) {
        printf("Usage: %s <message>\n", argv[0]);
        return 1;
    }

    const char *message = argv[1]; // Lấy tham số đầu tiên (message)

    // Khởi tạo Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0) {
        printf("Failed to initialize Winsock. Error Code: %d\n", WSAGetLastError());
        return 1;
    }

    // Tạo socket
    if ((clientSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) == INVALID_SOCKET) {
        printf("Could not create socket. Error Code: %d\n", WSAGetLastError());
        WSACleanup();
        return 1;
    }

    // Thiết lập thông tin server
    memset(&serverAddr, 0, sizeof(serverAddr));
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverAddr.sin_port = htons(SERVER_PORT);

    // Gửi thông điệp tới server
    if (sendto(clientSocket, message, strlen(message), 0, (struct sockaddr *)&serverAddr, serverAddrLen) == SOCKET_ERROR) {
        printf("sendto failed. Error Code: %d\n", WSAGetLastError());
    } else {
        printf("Sent message: %s\n", message);
    }

    // Đóng socket
    closesocket(clientSocket);
    WSACleanup();
    return 0;
}
