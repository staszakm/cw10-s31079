using cw10.Data;
using cw10.DTOs;
using cw10.Exceptions;
using cw10.Models;
using Microsoft.EntityFrameworkCore;

namespace cw10.Services;

public interface IDbService
{
    public Task<PagesDto<TripDto>> GetTripsAsync(int page, int pageSize);
    public Task<bool> DeleteClientAsync(int clientId);
    public Task<bool> JoinClientTripAsync(int tripId, ClientTripPostDto clientTrip);
}

public class Service(MasterContext context) : IDbService
{
    public async Task<PagesDto<TripDto>> GetTripsAsync(int page, int pageSize)
    {
        var tripsRet = await context.Trips.Include(tr => tr.CountryTrips).ThenInclude(c => c.IdCountryNavigation)
            .Include(tr => tr.ClientTrips).ThenInclude(c => c.IdClientNavigation)
            .OrderByDescending(trip => trip.DateFrom).Skip((page - 1) * pageSize).Take(pageSize).Select(tr =>
                new TripDto
                {
                    Name = tr.Name,
                    DateFrom = tr.DateFrom,
                    DateTo = tr.DateTo,
                    MaxPeople = tr.MaxPeople,
                    Countries = tr.CountryTrips.Select(c => new CountryDto
                    {
                        Name = c.IdCountryNavigation.Name,
                    }),
                    Clients = tr.ClientTrips.Select(c => new ClientDto
                    {
                        FirstName = c.IdClientNavigation.FirstName,
                        LastName = c.IdClientNavigation.LastName,
                    })
                }).ToListAsync();
        
        
        var trips = await context.Trips.CountAsync();
        var pages = (int)trips/pageSize;

        return new PagesDto<TripDto>()
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = pages,
            Trips = tripsRet
        };
    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        var client = await context.Clients.FindAsync(clientId);

        if (client == null)
        {
            throw new NotFoundException($"Client {clientId} not found");
        }
        
        var trips = await context.ClientTrips.AnyAsync(t => t.IdClient == clientId);

        if (trips)
        {
            throw new InvalidOperationException($"Client {clientId} has alread booked trips");
        }
        
        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> JoinClientTripAsync(int tripId, ClientTripPostDto clientTrip)
    {
        var client = await context.Clients.FirstOrDefaultAsync(p=>p.Pesel == clientTrip.Pesel);
        var trips = await context.Trips.FindAsync(tripId);
            
        if (trips == null)
        {
            throw new NotFoundException("There is no such trip");
        }
        if (trips.DateFrom <= DateTime.Now)
        {
            throw new ArgumentException("The trip has expired");
        }
        if (client != null)
        {
            throw new InvalidOperationException($"Client with PESEL {clientTrip.Pesel} has already joined a trip");
        }

        var addClient = new Client
        {
            FirstName = clientTrip.FirstName,
            LastName = clientTrip.LastName,
            Email = clientTrip.Email,
            Telephone = clientTrip.Telephone,
            Pesel = clientTrip.Pesel,
        };
        
        context.Clients.Add(addClient);
        await context.SaveChangesAsync();

        var addNewClientTrip = new ClientTrip
        {
            IdClient = addClient.IdClient,
            IdTrip = tripId,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientTrip.PaymentDate,
        };
        
        context.ClientTrips.Add(addNewClientTrip);
        await context.SaveChangesAsync();
        
        return true;

    }

}