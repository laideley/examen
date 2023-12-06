using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SimpleHTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // создание TCP сокета
            TcpListener server = new TcpListener(IPAddress.Any, 80);

            try
            {
                // запуск сервера
                server.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    // ожидание подключения клиента
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Клиент подключен.");

                    // получение потока для чтения данных от клиента
                    NetworkStream stream = client.GetStream();

                    // чтение запроса от клиента
                    StreamReader reader = new StreamReader(stream);
                    string request = reader.ReadLine();

                    // проверка запроса на наличие данных
                    if (request != null)
                    {
                        // разбиение запроса на части
                        string[] requestData = request.Split(' ');

                        // получение метода запроса и пути к файлу
                        string method = requestData[0];
                        string filePath = requestData[1];

                        // проверка метода на GET запрос
                        if (method == "GET")
                        {
                            // проверка наличия файла на сервере
                            if (File.Exists(filePath))
                            {
                                // чтение файла в байтовый массив
                                byte[] fileData = File.ReadAllBytes(filePath);

                                // отправка заголовка ответа клиенту
                                string responseHeader = "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: " + fileData.Length.ToString() + "\r\n\r\n";
                                byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(responseHeader);
                                stream.Write(headerBytes, 0, headerBytes.Length);

                                // отправка файла клиенту
                                stream.Write(fileData, 0, fileData.Length);
                                Console.WriteLine("Файл успешно отправлен.");
                            }
                            else
                            {
                                // отправка заголовка ошибки клиенту
                                string responseHeader = "HTTP/1.1 404 Not Found\r\nContent-Type: text/html\r\nContent-Length: 0\r\n\r\n";
                                byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(responseHeader);
                                stream.Write(headerBytes, 0, headerBytes.Length);
                                Console.WriteLine("Ошибка: файл не найден.");
                            }
                        }
                        else
                        {
                            // отправка заголовка ошибки клиенту
                            string responseHeader = "HTTP/1.1 405 Method Not Allowed\r\nContent-Type: text/html\r\nContent-Length: 0\r\n\r\n";
                            byte[] headerBytes = System.Text.Encoding.ASCII.GetBytes(responseHeader);
                            stream.Write(headerBytes, 0, headerBytes.Length);
                            Console.WriteLine("Ошибка: неподдерживаемый метод запроса.");
                        }
                    }

                    // закрытие соединения с клиентом
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // остановка сервера
                server.Stop();
            }
        }
    }
}