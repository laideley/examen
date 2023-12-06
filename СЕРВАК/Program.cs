using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class TCPServer
{
    static void Main()
    {
        // Задайте IP-адрес и порт для прослушивания
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        // Создайте объект TcpListener для прослушивания входящих подключений
        TcpListener listener = new TcpListener(ipAddress, port);

        try
        {
            // Начните прослушивание
            listener.Start();
            Console.WriteLine($"Сервер запущен. Ожидание подключений на {ipAddress}:{port}");

            while (true)
            {
                // Принимайте входящее подключение
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine($"Получено подключение от {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                // Создайте новый поток для обработки клиента
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }


    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        try
        {
            while (true)
            {
                bytesRead = 0;

                // Читайте данные из клиента
                bytesRead = clientStream.Read(message, 0, 4096);

                if (bytesRead == 0)
                    break;

                // Преобразуйте байты в строку и выведите сообщение
                string clientMessage = Encoding.UTF8.GetString(message, 0, bytesRead);
                Console.WriteLine($"Получено сообщение от клиента: {clientMessage}");

                // Проверка данных на сервере
                string responseMessage = ValidateData(clientMessage);

                // Отправьте ответ клиенту
                byte[] reply = Encoding.UTF8.GetBytes(responseMessage);
                clientStream.Write(reply, 0, reply.Length);
                clientStream.Flush();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            tcpClient.Close();
        }
    }

    static string ValidateData(string inputData)
    {
        // Ваша логика проверки данных
        if (int.TryParse(inputData, out _))
        {
            return $"Сервер: Введенное значение '{inputData}' является числом.";
        }
        else
        {
            return $"Сервер: Ошибка! Введенное значение '{inputData}' не является числом.";
        }
    }
}
