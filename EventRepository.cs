using Microsoft.Extensions.Logging;
using MySqlConnector;
using Notificator.SharedServices.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notificator.SharedServices
{
    /// <summary>
    /// Represents a repository for performing CRUD operations on the 'events' table.
    /// </summary>
    public class EventRepository: IEventRepository
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRepository"/> class with a provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string for the database.</param>
        public EventRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Retrieves all events from the 'events' table.
        /// </summary>
        /// <returns>A collection of <see cref="EventModel"/> instances representing the events.</returns>
        public IEnumerable<Models.EventModel> GetAll()
        {
            List<Models.EventModel> events = new List<Models.EventModel>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM `events`";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Models.EventModel
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.GetString("description") ?? "",
                            Date = reader.GetDateTime("date"),
                            Status = reader.GetInt32("status") == 1 ? true : false,
                            DateCreated = reader.GetDateTime("date_created"),
                            DateUpdated = reader.IsDBNull("date_updated") ? null : reader.GetDateTime("date_updated"),
                        });
                    }
                }
            }
            return events;
        }

        /// <summary>
        /// Retrieves a specific event by its ID from the 'events' table.
        /// </summary>
        /// <param name="id">The ID of the event to retrieve.</param>
        /// <returns>An <see cref="EventModel"/> instance representing the retrieved event or null if not found.</returns>
        public Models.EventModel GetById(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM `events` WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Models.EventModel
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.IsDBNull("description") ? null : reader.GetString("description"),
                            Date = reader.GetDateTime("date"),
                            Status = reader.GetInt32("status") == 1 ? true : false,
                            DateCreated = reader.GetDateTime("date_created"),
                            DateUpdated = reader.GetDateTime("date_updated")
                        };
                    }
                    return null; // Return null if event with given id is not found
                }
            }
        }

        /// <summary>
        /// Retrieves all current events where scheduled event datetime equals or less than current datetime.
        /// </summary>
        /// <returns>A collection of <see cref="EventModel"/> instances representing the events.</returns>
        public IEnumerable<Models.EventModel> GetAllCurrent()
        {
            List<Models.EventModel> events = new List<Models.EventModel>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                string query = "SELECT * FROM `events` WHERE `status` = 0 AND `date` <= NOW()";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        events.Add(new Models.EventModel
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.GetString("description") ?? "",
                            Date = reader.GetDateTime("date"),
                            Status = reader.GetInt32("status") == 1 ? true : false,
                            DateCreated = reader.GetDateTime("date_created"),
                            DateUpdated = reader.IsDBNull("date_updated") ? null : reader.GetDateTime("date_updated"),
                        });
                    }
                }
            }
            return events;
        }


        /// <summary>
        /// Creates a new event in the 'events' table.
        /// </summary>
        /// <param name="eventModel">The <see cref="EventModel"/> instance representing the new event.</param>
        public void Create(Models.EventModel eventModel)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO `events` (name, description, date, status, date_created) " +
                               "VALUES (@name, @description, @date, @status, @dateCreated)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", eventModel.Name);
                cmd.Parameters.AddWithValue("@description", eventModel.Description);
                cmd.Parameters.AddWithValue("@date", eventModel.Date);
                cmd.Parameters.AddWithValue("@status", eventModel.Status);
                cmd.Parameters.AddWithValue("@dateCreated", eventModel.DateCreated);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates an existing event in the 'events' table.
        /// </summary>
        /// <param name="eventModel">The <see cref="EventModel"/> instance representing the updated event.</param>
        public async void Update(Models.EventModel eventModel)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE `events` SET name = @name, description = @description, date = @date, " +
                               "status = @status WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", eventModel.Id);
                cmd.Parameters.AddWithValue("@name", eventModel.Name);
                cmd.Parameters.AddWithValue("@description", eventModel.Description);
                cmd.Parameters.AddWithValue("@date", eventModel.Date);
                cmd.Parameters.AddWithValue("@status", eventModel.Status ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates the status of an existing event in the 'events' table.
        /// </summary>
        /// <param name="id">The ID of the event to update.</param>
        public void UpdateStatus(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE `events` SET status = 1 WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes an event from the 'events' table by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to delete.</param>
        public void Delete(int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM `events` WHERE id = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

    }
}
