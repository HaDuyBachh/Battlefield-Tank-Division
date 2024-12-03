#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>

#define BUFF_SIZE 1024
int client_sock;

int read_input(char *buff, size_t size)
{
    if (fgets(buff, size, stdin) == NULL)
    {
        perror("Error reading input");
        close(client_sock);
        exit(1);
    }
    if (strlen(buff) > 0 && buff[strlen(buff) - 1] == '\n')
    {
        buff[strlen(buff) - 1] = '\0';
    }
    if (strlen(buff) == 0)
    {
        close(client_sock);
        exit(0);
    }
    return 1;
}


int main(int argc, char *argv[])
{
    if (argc != 3)
    {
        fprintf(stderr, "Usage: %s <IPAddress> <PortNumber>\n", argv[0]);
        exit(0);
    }

    return 0;
    char *server_ip = argv[1];
    int port = atoi(argv[2]);
    struct sockaddr_in server_addr;
    socklen_t sin_size;
    char buff[BUFF_SIZE];
    int bytes_sent, bytes_received;

    client_sock = socket(AF_INET, SOCK_DGRAM, 0);
    if (client_sock < 0)
    {
        perror("Socket error");
        exit(1);
    }

    server_addr.sin_family = AF_INET;
    server_addr.sin_port = htons(port);
    server_addr.sin_addr.s_addr = inet_addr(server_ip);
    memset(&(server_addr.sin_zero), '\0', 8);

    sin_size = sizeof(struct sockaddr);

    printf("Connected to server\n");

    while (1)
    {
        printf("Enter username: ");
        int result = read_input(buff, BUFF_SIZE);
        char username[BUFF_SIZE];
        strcpy(username, buff);

        printf("Enter password: ");
        result = read_input(buff, BUFF_SIZE);
        if (result == -1)
        {
            close(client_sock);
            exit(1);
        }
        char password[BUFF_SIZE];
        strcpy(password, buff);

        snprintf(buff, 2*BUFF_SIZE, "%s %s", username, password);
        bytes_sent = sendto(client_sock, buff, strlen(buff), 0, (struct sockaddr *)&server_addr, sin_size);
        if (bytes_sent < 0)
        {
            perror("Send error");
            close(client_sock);
            return 0;
        }

        bytes_received = recvfrom(client_sock, buff, BUFF_SIZE - 1, 0, (struct sockaddr *)&server_addr, &sin_size);
        if (bytes_received < 0)
        {
            perror("Receive error");
            close(client_sock);
            exit(1);
        }
        buff[bytes_received] = '\0';

        if (strcmp(buff, "OK") == 0)
        {
            printf("Login successful\n");

            while (1)
            {
                printf("Enter new password (or 'bye' to sign out): ");
                result = read_input(buff, BUFF_SIZE);
                if (result == -1)
                {
                    close(client_sock);
                    exit(1);
                }
                else if (strcmp(buff, "bye") == 0)
                {
                    // sendto(client_sock, "bye", 3, 0, (struct sockaddr *)&server_addr, sin_size); // Inform the server about signout
                    printf("Goodbye %s\n", username);
                    break;
                }

                bytes_sent = sendto(client_sock, buff, strlen(buff), 0, (struct sockaddr *)&server_addr, sin_size);
                if (bytes_sent < 0)
                {
                    perror("Send error");
                    close(client_sock);
                    exit(1);
                }

                bytes_received = recvfrom(client_sock, buff, BUFF_SIZE - 1, 0, (struct sockaddr *)&server_addr, &sin_size);
                if (bytes_received < 0)
                {
                    perror("Receive error");
                    close(client_sock);
                    exit(1);
                }
                buff[bytes_received] = '\0';
                printf("%s\n", buff);
            }
        }
        else
        {
            printf("%s\n", buff);
        }
    }

    close(client_sock);
    return 0;
}

// int maincheck() {
//     int sock;
//     struct sockaddr_in server_addr;
//     char buffer[BUFFER_SIZE];

//     sock = socket(AF_INET, SOCK_DGRAM, 0);
//     server_addr.sin_family = AF_INET;
//     server_addr.sin_port = htons(atoi(argv[2]));
//     inet_pton(AF_INET, argv[1], &server_addr.sin_addr);

//     // printf("Enter username: ");
//     // fgets(buffer, sizeof(buffer), stdin);
//     // buffer[strcspn(buffer, "\n")] = 0; 

//     // sendto(sock, buffer, strlen(buffer), 0, (struct sockaddr *)&server_addr, sizeof(server_addr));
//     // recvfrom(sock, buffer, BUFFER_SIZE, 0, NULL, NULL);
//     // printf("%s\n", buffer);

//     // printf("Enter password: ");
//     // fgets(buffer, sizeof(buffer), stdin);
//     // buffer[strcspn(buffer, "\n")] = 0; 
//     // sendto(sock, buffer, strlen(buffer), 0, (struct sockaddr *)&server_addr, sizeof(server_addr));
//     // recvfrom(sock, buffer, BUFFER_SIZE, 0, NULL, NULL);
//     // printf("%s\n", buffer);

//     close(sock);
//     return 0;
// }   