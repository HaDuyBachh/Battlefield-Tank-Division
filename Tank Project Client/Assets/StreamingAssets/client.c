#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <windows.h>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_IP "127.0.0.1"
#define SERVER_PORT 8080
#define UNITY_LISTENER_PORT 9999
#define BUFFER_SIZE 1500

void sendData(SOCKET sockfd, struct sockaddr_in *serverAddr, char *data, int dataLen) {
    // Gửi dữ liệu đến server
    if (sendto(sockfd, data, dataLen, 0, (struct sockaddr *)serverAddr, sizeof(*serverAddr)) == SOCKET_ERROR) {
        printf("sendto failed. Error Code: %d\n", WSAGetLastError());
        return;
    }
    printf("Sent data to server.\n");
}

void receiveData(SOCKET sockfd) {
    char buffer[BUFFER_SIZE];
    struct sockaddr_in serverAddr;
    int addrLen = sizeof(serverAddr);
    
    // Nhận dữ liệu từ server
    int recvLen = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&serverAddr, &addrLen);
    if (recvLen == SOCKET_ERROR) {
        printf("recvfrom failed. Error Code: %d\n", WSAGetLastError());
        return;
    }

    printf("Received %d bytes from server: %s\n", recvLen, buffer);
}

int main() {
    WSADATA wsaData;
    SOCKET sockfd;
    struct sockaddr_in serverAddr;
    char message[] = "hello from client!";
    
    // Khởi tạo Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        printf("WSAStartup failed. Error Code: %d\n", WSAGetLastError());
        return 1;
    }

    // Tạo socket UDP
    if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) == INVALID_SOCKET) {
        printf("Socket creation failed. Error Code: %d\n", WSAGetLastError());
        WSACleanup();
        return 1;
    }

    // Cấu hình server
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(SERVER_PORT);
    serverAddr.sin_addr.s_addr = inet_addr(SERVER_IP);

    // Gửi dữ liệu tới server
    sendData(sockfd, &serverAddr, message, sizeof(message));

    // Nhận phản hồi từ server
    receiveData(sockfd);

    // Đóng socket
    closesocket(sockfd);
    WSACleanup();
    return 0;
}
