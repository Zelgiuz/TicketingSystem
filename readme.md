# Ticketing System API Guide

## **Install**  
One should only need a Cosmos DB that doesn't have a tickets database already for this to work on run.  

---

## **Scripts**  

### **Make a Venue**  
```http
PUT {TicketingSystem_HostAddress}/Venues  
Content-Type: application/json  
```
```json
{
  "name": "ACoolVenue",
  "description": "Mega Wealthy Room",
  "maxCapacity": 500
}
```

---

### **Make an Event**  
```http
PUT {TicketingSystem_HostAddress}/Events  
Content-Type: application/json  
```
```json
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
```

---

### **Reserve Ticket(s)**  
- **Event ID** = the event ID returned from above  
- **Section ID** is `"0"` in this case but will essentially be the 0-based order of sections created as a string  
- **Seat numbers** will be `0` to `499` since `500` were created  

```http
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/reserve  
Content-Type: application/json  
```
```json
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
```

---

### **Cancel Reservation(s)**  
- **Event ID** = the event returned from above  
- **Section ID** is `"0"` in this case but will essentially be the 0-based order of sections created as a string  
- **Seat numbers** will be `0` to `499` since `500` were created  

```http
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/cancelreservation  
Content-Type: application/json  
```
```json
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
```

---

### **Buy Reserved Tickets**  
- **Event ID** = the event ID returned from above  
- **Section ID** is `"0"` in this case but will essentially be the 0-based order of sections created as a string  
- **Seat numbers** will be `0` to `499` since `500` were created  

```http
POST {TicketingSystem_HostAddress}/event/{eventId}/tickets/user/1/buy  
Content-Type: application/json  
```
```json
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
```

---

### **Update an Event**  
- **Venue ID** = the venue ID returned from above  

```http
POST {TicketingSystem_HostAddress}/Events  
Content-Type: application/json  
```
```json
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
```

---

### **Update a Venue**  
```http
POST {TicketingSystem_HostAddress}/Venues  
Content-Type: application/json  
```
```json
{
  "id": "VenueID",
  "name": "ACoolVenue",
  "description": "An exclusive club with 500 seats for an extraordinary experience",
  "maxCapacity": 500
}
```

---

### **View Available Tickets for Event**  
```http
GET {TicketingSystem_HostAddress}/event/{eventId}/available
```

---

### **View All Tickets for Event**  
```http
GET {TicketingSystem_HostAddress}/event/{eventId}
```

---


### **View Available Venues**  
```http
GET {TicketingSystem_HostAddress}/Venues/Available/2024-03-13  
```

---

### **View Events**  
```http
GET {TicketingSystem_HostAddress}/Events  
```

---

### **View a Specific Event**  
```http
GET {TicketingSystem_HostAddress}/Events?name=AGoodEvent  
```

---

### **Test Conflicts**  
Too many to list here (e.g., if you already ran this once, running it again should return a bad request as the venue would no longer be available).  

```http
PUT {TicketingSystem_HostAddress}/Events  
Content-Type: application/json  
```
```json
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
```

---

### **Test Bad Requests**  
Delete necessary data and it will fail.  

