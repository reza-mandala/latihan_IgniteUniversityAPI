using Cassandra;
using Cassandra.Mapping;
using Cassandra.Mapping.Attributes;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using ISession = Cassandra.ISession;
using MyIgniteApi.Models;

namespace MyIgniteApi.Services
{
    public class CassandraService
    {
        private string ContactPoint = "";
        private string KeyspaceName = "";
        private Cluster? _cluster;
        private ISession? _session;

        public CassandraService()
        {
            // instalasi cassandra:
            // $ docker pull cassandra
            // 
            // menjalankan cassandra (agar bisa mengakses Cassandra dari aplikasi yang berjalan di luar container):
            // $ docker run --name cassandra-container -d -p 9042:9042 cassandra:latest
            //
            // menampilkan container yang sedang berjalan:
            // $ docker ps -a
            //
            // merestart docker container
            // $ docker restart <container-id>
            // $ docker restart ee7b370dccaa
            //
            // cara mengakses cassandra via terminal:
            // $ docker exec -it cassandra-container cqlsh
            //
            // menghentikan container:
            // $ docker stop cassandra-container
            //
            // menghapus container (setelah menghentikannya):
            // $ docker rm cassandra-container
            // 
            // Handling Emergency Situations with Kill
            // $ docker kill <container-id>
            // $ docker start <container-id>
            //
            // mejalankan Program.cs:
            // $ dotnet run

            OpenConnection();
            EnsureKeyspaceAndTableExist();
        }

        private void OpenConnection()
        {
            // Membaca konfigurasi dari appsettings.json.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            ContactPoint = configuration["Cassandra:ContactPoint"] ?? "";
            KeyspaceName = configuration["Cassandra:KeyspaceName"] ?? "";

            _cluster = Cluster.Builder().AddContactPoint(ContactPoint).Build();
            _session = _cluster.Connect();
        }

        private void EnsureKeyspaceAndTableExist()
        {
            // Membuat keyspace jika belum ada
            var createKeyspaceQuery = $"CREATE KEYSPACE IF NOT EXISTS {KeyspaceName} WITH REPLICATION = {{ 'class' : 'SimpleStrategy', 'replication_factor' : 1 }};";
            _session?.Execute(createKeyspaceQuery);

            // Menggunakan keyspace yang telah dibuat atau sudah ada
            _session?.ChangeKeyspace(KeyspaceName);

            // Membuat tabel university jika belum ada
            var createTableQuery = @"CREATE TABLE IF NOT EXISTS university (
                country TEXT,
                name TEXT,
                web_pages LIST<TEXT>,
                alpha_two_code TEXT,
                domains LIST<TEXT>,
                PRIMARY KEY (country, name)
            );";
            _session?.Execute(createTableQuery);
        }

        public void InsertUniversity(University university)
        {
            var preparedStatement = _session?.Prepare("INSERT INTO university (country, name, web_pages, alpha_two_code, domains) VALUES (?, ?, ?, ?, ?)");
            var boundStatement = preparedStatement?.Bind(university.Country, university.Name, university.WebPages, university.AlphaTwoCode, university.Domains);
            _session?.Execute(boundStatement);
        }
        
        public void DropKeyspace(ISession session, string? keyspaceName)
        {
            try
            {
                session.Execute($"DROP KEYSPACE {keyspaceName};");
                Console.WriteLine($"Keyspace {keyspaceName} dropped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
            _cluster?.Dispose();
        }
    }
}
