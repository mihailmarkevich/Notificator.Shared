using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notificator.SharedServices.Models;

namespace Notificator.SharedServices
{
    public interface IEventRepository
    {
        /// <summary>
        /// Selects all events
        /// </summary>
        IEnumerable<Models.EventModel> GetAll();

        /// <summary>
        /// Selects an event by id
        /// </summary>
        /// <param name="id"></param>
        Models.EventModel GetById(int id);

        /// <summary>
        /// Selects all current events where scheduled event datetime equals or less than current datetime.
        /// </summary>
        IEnumerable<Models.EventModel> GetAllCurrent();

        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <param name="eventModel"></param>
        void Create(Models.EventModel eventModel);

        /// <summary>
        /// Updates the existed event
        /// </summary>
        /// <param name="eventModel"></param>
        void Update(Models.EventModel eventModel);

        /// <summary>
        /// Updates the event completion status
        /// </summary>
        /// <param name="id"></param>
        void UpdateStatus(int id);

        /// <summary>
        /// Deletes the event
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);
    }
}
