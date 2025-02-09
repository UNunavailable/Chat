using Npgsql;

namespace MessageService.Models
{
    public class MessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(string connectionString, ILogger<MessageRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                var command = new NpgsqlCommand(@"
                    CREATE TABLE IF NOT EXISTS Messages (
                        Id SERIAL PRIMARY KEY,
                        Content VARCHAR(128) NOT NULL,
                        Timestamp TIMESTAMP NOT NULL,
                        ClientNumber INT NOT NULL
                    )", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed");
                throw;
            }
        }

        public async Task AddMessageAsync(Message message)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new NpgsqlCommand(@"
                    INSERT INTO Messages (Content, Timestamp, ClientNumber)
                    VALUES (@content, @timestamp, @clientNumber)", connection);

                command.Parameters.AddWithValue("@content", message.Content);
                command.Parameters.AddWithValue("@timestamp", message.Timestamp);
                command.Parameters.AddWithValue("@clientNumber", message.ClientNumber);

                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Message added: {Content}", message.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message");
                throw;
            }
        }

        public async Task<List<Message>> GetMessagesAsync(DateTime start, DateTime end)
        {
            var messages = new List<Message>();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new NpgsqlCommand(@"
                    SELECT Id, Content, Timestamp, ClientNumber 
                    FROM Messages 
                    WHERE Timestamp BETWEEN @start AND @end", connection);

                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messages.Add(new Message
                    {
                        Id = reader.GetInt32(0),
                        Content = reader.GetString(1),
                        Timestamp = reader.GetDateTime(2),
                        ClientNumber = reader.GetInt32(3)
                    });
                }

                _logger.LogInformation("Retrieved {Count} messages", messages.Count);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages");
                throw;
            }
        }
    }

}
