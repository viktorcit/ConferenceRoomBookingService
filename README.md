# Conference Room Booking Service

Conference Room Booking Service is an ASP.NET Core Web API for managing
conference rooms, room amenities, availability checks, and bookings.

The project was created as a technical assignment for a Junior ASP.NET Core
Developer position. The main goal is to provide a simple and clear booking
workflow: manage rooms and amenities, search for available rooms, book a room
for a selected time interval, and calculate the final booking price.

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core 8
- SQLite
- Swagger / OpenAPI

## Features

- Create, read, update, and delete amenities
- Create, update, and delete conference rooms
- Assign amenities to conference rooms
- Search available rooms by date, time, and capacity
- Book a room for a selected time interval
- Prevent booking conflicts for overlapping time intervals
- Prevent duplicate rooms and amenities by name
- Calculate booking price based on:
  - room base price
  - selected amenities
  - time-based price modifiers
- Return appropriate HTTP responses for validation errors, missing resources, and conflicts

## Initial Data

The project contains a SQLite database with the required initial data.

### Rooms

| Room | Capacity | Base Price |
| --- | ---: | ---: |
| Зал А | 50 people | 2000 UAH/hour |
| Зал B | 100 people | 3500 UAH/hour |
| Зал C | 30 people | 1500 UAH/hour |

### Amenities

| Amenity | Price |
| --- | ---: |
| Проєктор | 500 UAH |
| Wi-Fi | 300 UAH |
| Звук | 700 UAH |

### Room Amenities

| Room | Amenities |
| --- | --- |
| Зал C | Wi-Fi |
| Зал А | Wi-Fi, Проєктор |
| Зал B | Wi-Fi, Проєктор, Звук |

## Price Modifiers

The final booking price is calculated for each booked hour. The base room price
may be modified depending on the selected time interval:

| Time | Modifier |
| --- | ---: |
| 06:00-09:00 | -10% |
| 09:00-18:00 | standard price |
| 12:00-14:00 | +15% |
| 18:00-23:00 | -20% |

Selected amenities are added to the booking price.

## API Endpoints

### Amenities

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/api/amenity` | Get all amenities |
| GET | `/api/amenity/{amenityId}` | Get amenity by id |
| POST | `/api/amenity` | Create a new amenity |
| PATCH | `/api/amenity/{amenityId}` | Update an existing amenity |
| DELETE | `/api/amenity/{amenityId}` | Delete an amenity |

### Rooms

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/room/available` | Get available rooms for selected date, time, and capacity |
| POST | `/api/room/create` | Create a new conference room |
| POST | `/api/room/book/{roomId}` | Book a conference room |
| PATCH | `/api/room/{roomId}` | Update room price or amenities |
| DELETE | `/api/room/{roomId}` | Delete a conference room |

## Example Requests

### Create Amenity

```json
{
  "name": "Whiteboard",
  "price": 250
}
```

### Create Room

```json
{
  "name": "Зал D",
  "capacity": 20,
  "basePrice": 1200,
  "amenities": ["Wi-Fi"]
}
```

### Search Available Rooms

```json
{
  "date": "2026-09-10",
  "startTime": "10:00:00",
  "endTime": "12:00:00",
  "capacity": 30
}
```

### Book Room

```json
{
  "date": "2026-09-10",
  "startTime": "10:00:00",
  "endTime": "12:00:00",
  "amenities": ["Wi-Fi", "Проєктор"]
}
```

### Update Room

```json
{
  "basePrice": 2200,
  "amenities": ["Wi-Fi", "Проєктор"]
}
```

## Validation and Business Rules

- Room capacity must be between 1 and 300.
- Room base price must be greater than zero.
- Amenity price must be greater than zero.
- Booking date cannot be in the past.
- Booking start time must be earlier than end time.
- A room cannot be booked for an already occupied time interval.
- Room names and amenity names should be unique.
- Requested amenities must exist before they can be assigned to a room or selected for booking.

## How to Run

### Prerequisites

- .NET 8 SDK
- EF Core tools, if you want to recreate the database manually

Install EF Core tools if they are not installed:

```bash
dotnet tool install --global dotnet-ef
```

### Run the Project

Clone the repository and run:

```bash
dotnet restore
dotnet run --project ConferenceRoomBookingService/ConferenceRoomBookingService.csproj
```

Swagger UI will be available at:

```text
http://localhost:5170/swagger
```

or, depending on the selected launch profile:

```text
https://localhost:7006/swagger
```

## Database

The project uses SQLite. The connection string is configured in:

```text
ConferenceRoomBookingService/appsettings.json
```

Default connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ConferenceRoomBooking.db"
  }
}
```

To recreate the database from migrations:

```bash
dotnet ef database update --project ConferenceRoomBookingService/ConferenceRoomBookingService.csproj
```

## Project Structure

```text
ConferenceRoomBookingService/
  Controllers/     API controllers
  Data/            DbContext, DTOs, and contracts
  Entity/          Entity models
  Enums/           Response types
  Helpers/         Shared helper classes
  Migrations/      EF Core migrations
  Services/        Business logic and service interfaces
```

## Notes

- Time values should be sent in `HH:mm:ss` format.
- Swagger is enabled in the Development environment.
- The included SQLite database contains the initial assignment data.
- The application is focused on API behavior and business logic rather than authentication or UI.
