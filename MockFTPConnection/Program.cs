using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace MockFTPConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] CodTest = new int[10];

            int opcao;

            do
            {
                Console.WriteLine("Digite uma opção para iniciar os testes..");
                Console.WriteLine();
                Console.WriteLine("[1] Teste de conexão utilizando o protocolo FTP");
                Console.WriteLine("[2] Teste de conexão utilizando o protocolo SFTP");
                Console.WriteLine("[3] Teste de conexão utilizando o protocolo FTPS");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("[0] Encerrar os testes e fechar console.");
                Console.WriteLine("--------------------------------------------------");
                opcao = Int32.Parse(Console.ReadLine());
                switch (opcao)
                {
                    case 1:
                        TestFTP(ref CodTest);
                        break;
                    case 2:
                        TestSFTP(ref CodTest);
                        break;
                    case 3:
                        TestFTPS(ref CodTest);
                        break;
                    default:
                        Sair();
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
            while (opcao != 0);

        }

        public static int TestSFTP(ref int[] CodTest)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Metodo nao implementado.");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();

            return 0;
        }

        public static int TestFTP(ref int[] CodTest)
        {
            //Variaveis de parametros do proxy.
            string EnderecoProxy = string.Empty;
            int PortaProxy = 0;
            string UsernameProxy = string.Empty;
            string SenhaProxy = string.Empty;

            //Variaveis de parametros do SFTP.
            string EnderecoFTP = string.Empty;
            int PortaFTP = 0;
            string UsernameFTP = string.Empty;
            string SenhaFTP = string.Empty;

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Configuração dos parametros de Conexão do Proxy.");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();

            Console.Write("Endereço de Proxy para conexão: ");
            EnderecoProxy = Convert.ToString(Console.ReadLine());

            Console.Write("Porta Proxy: ");
            PortaProxy = Convert.ToInt32(Console.ReadLine());

            Console.Write("Usuario para autenticacao no Proxy: ");
            UsernameProxy = Convert.ToString(Console.ReadLine());

            Console.Write("Senha para autenticacao no Proxy: ");
            SenhaProxy = Convert.ToString(Console.ReadLine());

            Console.WriteLine();

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Configuração dos parametros de Conexão do FTP.");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();

            Console.Write("Endereço de FTP para conexão: ");
            EnderecoFTP = Convert.ToString(Console.ReadLine());

            Console.Write("Porta FTP: ");
            PortaFTP = Convert.ToInt32(Console.ReadLine());

            Console.Write("Usuario para autenticacao no FTP: ");
            UsernameFTP = Convert.ToString(Console.ReadLine());

            Console.Write("Senha para autenticacao no FTP: ");
            SenhaFTP = Convert.ToString(Console.ReadLine());

            Console.WriteLine();

            string FtpUri = EnderecoFTP;
            string ProxyUri = EnderecoProxy;

            try
            {
                Console.WriteLine("Conectando ao FTP utilizando as credencias fornecidas...");
                Console.WriteLine();

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = FtpUri,
                    PortNumber = PortaFTP,
                    UserName = UsernameFTP,
                    Password = SenhaFTP,
                    FtpSecure = FtpSecure.Implicit,
                };

                sessionOptions.AddRawSettings("ProxyMethod", "3");
                sessionOptions.AddRawSettings("ProxyHost", ProxyUri);
                sessionOptions.AddRawSettings("ProxyUsername", UsernameProxy);
                sessionOptions.AddRawSettings("ProxyPassword", SenhaProxy);

                using (Session session = new Session())
                {
                    session.Open(sessionOptions);

                    Console.WriteLine("Conexão estabelecida com Sucesso! Listando diretorio...");
                    Console.WriteLine();

                    RemoteDirectoryInfo directory = session.ListDirectory("/");

                    foreach (RemoteFileInfo fileInfo in directory.Files)
                    {
                        Console.WriteLine(
                            "{0} with size {1}, permissions {2} and last modification at {3}",
                            fileInfo.Name, fileInfo.Length, fileInfo.FilePermissions,
                            fileInfo.LastWriteTime);
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return 1;
            }
        }

        public static int TestFTPS(ref int[] CodTest)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Metodo nao implementado.");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();

            return 0;
        }

        private static void Sair()
        {
            Console.WriteLine();
            Console.WriteLine("Clique em qualquer tecla para Sair..");
        }
    }
}
