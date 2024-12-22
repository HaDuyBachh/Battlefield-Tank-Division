#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <windows.h>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_PORT 8080
#define UNITY_LISTENER_PORT 9999
#define BUFFER_SIZE 1500
#define CHECK_BYTE 0x11 // Giá trị byte đầu cần kiểm tra (ví dụ: 0x11)

typedef struct {
    SOCKET sockfd;
    struct sockaddr_in clientAddr;
    int clientAddrLen;
    char buffer[BUFFER_SIZE];
} ClientData;

DWORD WINAPI handleClient(LPVOID param) {
    ClientData *clientData = (ClientData *)param;
    struct sockaddr_in unityListenerAddr;
    int recvLen;
    SOCKET threadSocket;

    // Tạo socket riêng cho luồng
    if ((threadSocket = socket(AF_INET, SOCK_DGRAM, 0)) == INVALID_SOCKET) {
        printf("Thread socket creation failed. Error Code: %d\n", WSAGetLastError());
        free(clientData);
        return 0;
    }

    // Cấu hình Unity Listener
    unityListenerAddr.sin_family = AF_INET;
    unityListenerAddr.sin_port = htons(UNITY_LISTENER_PORT);
    unityListenerAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

    printf("Processing client: %s:%d\n",
           inet_ntoa(clientData->clientAddr.sin_addr),
           ntohs(clientData->clientAddr.sin_port));

    // Kiểm tra giá trị byte đầu tiên
    if (clientData->buffer[0] != CHECK_BYTE) {
        printf("Invalid first byte: 0x%X. Ignoring packet.\n", clientData->buffer[0] & 0xFF);
        closesocket(threadSocket);
        free(clientData);
        return 0;
    }

    // Gửi dữ liệu tới Unity Listener
    if (sendto(threadSocket, clientData->buffer, BUFFER_SIZE, 0,
               (struct sockaddr *)&unityListenerAddr, sizeof(unityListenerAddr)) == SOCKET_ERROR) {
        printf("sendto to Unity Listener failed. Error Code: %d\n", WSAGetLastError());
        closesocket(threadSocket);
        free(clientData);
        return 0;
    }
    printf("Sent data to Unity Listener.\n");

    // Nhận phản hồi từ Unity Listener
    recvLen = recvfrom(threadSocket, clientData->buffer, BUFFER_SIZE, 0,
                       (struct sockaddr *)&unityListenerAddr, &clientData->clientAddrLen);
    if (recvLen == SOCKET_ERROR) {
        printf("recvfrom from Unity Listener failed. Error Code: %d\n", WSAGetLastError());
        closesocket(threadSocket);
        free(clientData);
        return 0;
    }
    printf("Received %d bytes from Unity Listener.\n", recvLen);

    // Gửi phản hồi lại Unity Sender
    if (sendto(clientData->sockfd, clientData->buffer, recvLen, 0,
               (struct sockaddr *)&clientData->clientAddr, clientData->clientAddrLen) == SOCKET_ERROR) {
        printf("sendto to Unity Sender failed. Error Code: %d\n", WSAGetLastError());
    } else {
        printf("Sent %d bytes back to Unity Sender.\n", recvLen);
    }

    closesocket(threadSocket); // Đóng socket của luồng
    free(clientData);
    return 0;
}


int main() {
    WSADATA wsaData;
    SOCKET sockfd;
    struct sockaddr_in serverAddr, clientAddr;
    char buffer[BUFFER_SIZE];
    int addrLen = sizeof(clientAddr);

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
    serverAddr.sin_addr.s_addr = INADDR_ANY;

    // Bind socket
    if (bind(sockfd, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        printf("Bind failed. Error Code: %d\n", WSAGetLastError());
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    printf("Server is running on port %d...\n", SERVER_PORT);

    while (1) {
        // Nhận dữ liệu từ Unity Sender
        int recvLen = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&clientAddr, &addrLen);
        if (recvLen == SOCKET_ERROR) {
            printf("recvfrom failed. Error Code: %d\n", WSAGetLastError());
            continue;
        }

        printf("Received %d bytes from client %s:%d\n", recvLen,
            inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

        // Tạo dữ liệu client để xử lý trong luồng riêng
        ClientData *clientData = (ClientData *)malloc(sizeof(ClientData));
        if (clientData == NULL) {
            printf("Memory allocation failed.\n");
            continue;
        }

        // Làm sạch buffer của clientData
        memset(clientData->buffer, 0, BUFFER_SIZE);

        // Sao chép đúng số byte nhận được
        memcpy(clientData->buffer, buffer, recvLen);

        // Ghi lại kích thước dữ liệu thực nhận
        clientData->buffer[recvLen] = '\0'; // Nếu xử lý như chuỗi
        clientData->sockfd = sockfd;
        clientData->clientAddr = clientAddr;
        clientData->clientAddrLen = addrLen;

        // Tạo luồng xử lý client
        HANDLE threadHandle = CreateThread(NULL, 0, handleClient, (LPVOID)clientData, 0, NULL);
        if (threadHandle == NULL) {
            printf("CreateThread failed. Error Code: %d\n", GetLastError());
            free(clientData);
        } else {
            CloseHandle(threadHandle);
        }
    }

    // Đóng socket
    closesocket(sockfd);
    WSACleanup();
    return 0;
}
