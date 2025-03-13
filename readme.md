**Install**
One should only need a cosmos DB that doesn't have a tickets database already for this to work on run

**Scripts**

***Make a Venue***
PUT {TicketingSystem_HostAddress}/Venues
Content-Type: application/json

{
  "name": "ACoolVenue",
  "description": "Mega Wealthy Room",
  "maxCapacity": 500
}

***Make an Event***
PUT {TicketingSystem_HostAddress}/Events
Content-Type: application/json

{
  "name": "A Good Event",
  "startDate": "2025-03-13",
  "venueId": "The Guid From Above",
  "description": "Lots of fun at tonight's private Taylor Swift Concert",
  "sections": [
    {
      "name": "string",
      "capacity": 500,
      "price": 10000
    }
  ]
}

***Reserve Ticket(s)***
***Event ID = the event id returned from above***
***Section ID is "0" in this case but will essentially be the 0 based order of sections created as a string***
***Seat numbers will be 0 to 499 since 500 were created***
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/reserve
Content-Type: application/json

{
  "tickets": [
    {
      "seatNumber": 0,
      "sectionId": "0"
    },
    {
      "seatNumber": 1,
      "sectionId": "0"
    }
  ]
}

***Cancel Reservation(s)***
***Event ID = the event returned from above***
***Section ID is "0" in this case but will essentially be the 0 based order of sections created as a string***
***Seat numbers will be 0 to 499 since 500 were created***
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/cancelreservation
Content-Type: application/json

{
  "tickets": [
    {
      "seatNumber": 0,
      "sectionId": "0"
    },
    {
      "seatNumber": 1,
      "sectionId": "0"
    }
  ]
}
***Buy Reserved Tickets***
***Event ID = the event id returned from above***
***Section ID is "0" in this case but will essentially be the 0 based order of sections created as a string***
***Seat numbers will be 0 to 499 since 500 were created***
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/buy
Content-Type: application/json

{
  "tickets": [
    {
      "seatNumber": 0,
      "sectionId": "0"
    },
    {
      "seatNumber": 1,
      "sectionId": "0"
    }
  ]
}
***Update an Event***
***Venue id = the venue id Returned from the above***
POST {TicketingSystem_HostAddress}/Events
Content-Type: application/json

{
  "name": "A Spectacular Event",
  "startDate": "2025-03-13",
  "venueId": "The Guid From Above",
  "description": "Lots of fun at tonight's private Taylor Swift Concert",
  "sections": [
    {
      "name": "string",
      "capacity": 500,
      "price": 10000
    }
  ]
}
***Update a Venue***
POST {TicketingSystem_HostAddress}/Venues
Content-Type: application/json

{
  "id": "VenueID",
  "name": "ACoolVenue",
  "description": "An exclusive club with 500 seats for an extraordinary experience",
  "maxCapacity": 500
}
***View available Tickets***
GET {TicketingSystem_HostAddress}/event/{eventId}
***View available Venues***
GET {TicketingSystem_HostAddress}/Venues/Available/2024-03-13
***View Events***
GET {TicketingSystem_HostAddress}/Events
***View a specific Event***
GET {TicketingSystem_HostAddress}/Events?name=AGoodEvent
***Test Conflicts***
Too many to list here ex: if you already ran this once running it again should return a bad request as the venue would no longer be available
PUT {TicketingSystem_HostAddress}/Events
Content-Type: application/json

{
  "name": "A Good Event",
  "startDate": "2025-03-13",
  "venueId": "The Guid From Above",
  "description": "Lots of fun at tonight's private Taylor Swift Concert",
  "sections": [
    {
      "name": "string",
      "capacity": 500,
      "price": 10000
    }
  ]
}
***Test Bad Requests***
Delete necessary data and it will fail

