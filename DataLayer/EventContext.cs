using BusinessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class EventContext : IDb<Event, int>
    {
        private readonly EventManagerDbContext context;

        public EventContext(EventManagerDbContext context_)
        {
            this.context = context_;
        }

        public async Task CreateAsync(Event item)
        {
            try
            {
                context.Events.Add(item);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;   
            }
        }

        public async Task DeleteAsync(int key)
        {
            try
            {
                Event eventFromDb = context.Events.Find(key);
                if (eventFromDb != null)
                {
                    context.Events.Remove(eventFromDb);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ICollection<Event>> ReadAllAsync(bool useNavigationalProperties = false)
        {
            try
            {
                IQueryable<Event> query = context.Events;
                if (useNavigationalProperties)
                {
                    query = context.Events.Include(e => e.Creator);
                }

                return await query.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Event> ReadAsync(int key, bool useNavigationalProperties = false)
        {
            try
            {
                Event eventFromDb = context.Events.Find(key);
                if (eventFromDb != null)
                {
                    if (useNavigationalProperties)
                    {
                        eventFromDb = await context.Events
                                                .Include(e => e.Creator)
                                                .FirstAsync(x => x.Id == key);
                    }

                    return eventFromDb;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAsync(Event item, bool useNavigationalProperties = false)
        {
            try
            {
                Event eventFromDb = await context.Events.FindAsync(item.Id);

                if (eventFromDb != null)
                {
                    eventFromDb.Name = item.Name;
                    eventFromDb.Description = item.Description;
                    eventFromDb.Location = item.Location;
                    eventFromDb.EventStart = item.EventStart;
                    eventFromDb.MaxAttendees = item.MaxAttendees;
                }

                await context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
