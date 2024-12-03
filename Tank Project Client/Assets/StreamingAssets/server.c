#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <winsock2.h>

#pragma comment(lib, "ws2_32.lib")

#define SERVER_PORT 8080
#define UNITY_LISTENER_PORT 9999
#define BUFFER_SIZE 5012

int main()
{
    WSADATA wsaData;
    SOCKET sockfd;
    struct sockaddr_in serverAddr, unityListenerAddr, clientAddr;
    char buffer[BUFFER_SIZE];
    int addrLen = sizeof(clientAddr);

    // Khởi tạo Winsock
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
    {
        printf("WSAStartup failed. Error Code: %d\n", WSAGetLastError());
        return 1;
    }

    // Tạo socket UDP
    if ((sockfd = socket(AF_INET, SOCK_DGRAM, 0)) == INVALID_SOCKET)
    {
        printf("Socket creation failed. Error Code: %d\n", WSAGetLastError());
        WSACleanup();
        return 1;
    }

    // Cấu hình server
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(SERVER_PORT);
    serverAddr.sin_addr.s_addr = INADDR_ANY;

    // Bind socket
    if (bind(sockfd, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
    {
        printf("Bind failed. Error Code: %d\n", WSAGetLastError());
        closesocket(sockfd);
        WSACleanup();
        return 1;
    }

    printf("Server is running on port %d...\n", SERVER_PORT);

    while (1)
    {
        buffer[0] = '\0';
        // Nhận dữ liệu từ Unity Sender (block cho đến khi nhận được dữ liệu)
        int recvLen = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&clientAddr, &addrLen);
        if (recvLen == SOCKET_ERROR)
        {
            printf("recvfrom failed. Error Code: %d\n", WSAGetLastError());
            break;
        }

        if (buffer[0] == '^')
        {
            buffer[recvLen] = '\0';
            printf("Received from Unity Sender: %s\n", buffer);

            // Gửi dữ liệu tới Unity Listener
            unityListenerAddr.sin_family = AF_INET;
            unityListenerAddr.sin_port = htons(UNITY_LISTENER_PORT);
            unityListenerAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

            if (sendto(sockfd, buffer, strlen(buffer), 0, (struct sockaddr *)&unityListenerAddr, sizeof(unityListenerAddr)) == SOCKET_ERROR)
            {
                printf("sendto to Unity Listener failed. Error Code: %d\n", WSAGetLastError());
                break;
            }
            printf("Sent to Unity Listener: %s\n", buffer);

            //Bắt đầu nhận dữ liệu --------------------------------------------------------------------------------------------------------------
            
            buffer[0] = '\0';
            // Nhận phản hồi từ Unity Listener (block đến khi nhận dữ liệu)
            int addrLenTemp = sizeof(unityListenerAddr);
            recvLen = recvfrom(sockfd, buffer, BUFFER_SIZE, 0, (struct sockaddr *)&unityListenerAddr, &addrLenTemp);
            if (recvLen == SOCKET_ERROR)
            {
                printf("recvfrom failed. Error Code: %d\n", WSAGetLastError());
                break;
            }
            buffer[recvLen] = '\0';
            printf("Received from Unity Listener: %s\n", buffer);

            // Gửi phản hồi lại Unity Sender
            if (sendto(sockfd, buffer, strlen(buffer), 0, (struct sockaddr *)&clientAddr, addrLen) == SOCKET_ERROR)
            {
                printf("sendto to Unity Sender failed. Error Code: %d\n", WSAGetLastError());
                break;
            }
            printf("Sent to Unity Sender: %s\n", buffer);
            }
    }

    // Đóng socket
    closesocket(sockfd);
    WSACleanup();
    return 0;
}
