# PsychoCitas — Clean Architecture en .NET 8

## Estructura del proyecto

```
PsychoCitas/
├── Domain/                         # Núcleo — sin dependencias externas
│   ├── Entities/                   # Entidades de dominio con lógica
│   │   ├── BaseEntity.cs           # Id, timestamps, DomainEvents
│   │   ├── Paciente.cs             # Paciente + factory method + eventos
│   │   ├── Cita.cs                 # Cita + reglas de negocio + eventos
│   │   ├── HistoriaClinica.cs      # Historia clínica del paciente
│   │   ├── NotaSesion.cs           # Nota por sesión + evaluación de riesgo
│   │   ├── Pago.cs                 # Pago de cita
│   │   └── Notificacion.cs         # Notificación (email/whatsapp/sms)
│   ├── Enums/                      # EstadoCita, TipoSesion, Modalidad, etc.
│   ├── Exceptions/                 # DomainException, NotFoundException, ConflictoAgenda
│   ├── Interfaces/                 # IRepository<T>, ICitaRepository, IUnitOfWork
│   └── ValueObjects/               # (extensible: Telefono, Email tipado, etc.)
│
├── Application/                    # Casos de uso — depende solo de Domain
│   ├── Common/
│   │   ├── Behaviors/              # Pipeline MediatR: Logging, Validation
│   │   ├── Exceptions/             # ValidationException
│   │   └── Interfaces/             # ICurrentUser, INotificationService, IStorageService
│   ├── DTOs/                       # CitaDto, PacienteDetalleDto, NotaSesionDto, etc.
│   └── Features/                   # CQRS por feature (Vertical Slices)
│       ├── Citas/Commands/         # AgendarCita, CancelarCita, CompletarCita, MarcarNoAsistio
│       ├── Citas/Queries/          # GetCitaById, etc.
│       ├── Agenda/Queries/         # GetAgendaHoy
│       ├── Pacientes/Commands/     # RegistrarPaciente
│       ├── Pacientes/Queries/      # GetPacienteDetalle, BuscarPacientes
│       └── Notas/Commands/         # GuardarNota
│
├── Infrastructure/                 # Implementaciones concretas
│   ├── Persistence/
│   │   ├── AppDbContext.cs         # EF Core + auto-timestamps
│   │   ├── UnitOfWork.cs           # Transacciones + domain events dispatch
│   │   ├── Configurations/         # IEntityTypeConfiguration por entidad
│   │   └── Repositories/           # Implementaciones de interfaces del Domain
│   ├── Services/
│   │   ├── Notifications/          # Email (SendGrid), WhatsApp (Twilio/Meta), SMS
│   │   └── Storage/                # Azure Blob Storage
│   └── Identity/                   # JWT generation, CurrentUser
│
├── API/                            # ASP.NET Core Web API
│   ├── Controllers/                # CitasController, PacientesController, NotasController
│   ├── Middleware/                 # ExceptionMiddleware (manejo global de errores)
│   ├── Extensions/                 # ServiceCollectionExtensions (DI wiring)
│   ├── Program.cs                  # Configuración JWT, Swagger, CORS, pipeline
│   └── appsettings.json
│
└── Tests/
    ├── Domain.Tests/               # Unit tests — entidades y reglas de dominio
    └── Application.Tests/          # Unit tests — handlers con mocks
```

## Paquetes NuGet por proyecto

### Domain
- (ninguno — zero dependencies)

### Application
```
MediatR
FluentValidation
FluentValidation.DependencyInjectionExtensions
```

### Infrastructure
```
Microsoft.EntityFrameworkCore
Npgsql.EntityFrameworkCore.PostgreSQL
Microsoft.EntityFrameworkCore.Design
```

### API
```
Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.IdentityModel.Tokens
Swashbuckle.AspNetCore
```

## Setup rápido

```bash
# 1. Restaurar paquetes
dotnet restore

# 2. Crear base de datos (con el schema.sql del step anterior)
psql -U postgres -c "CREATE DATABASE psychocitas;"
psql -U postgres -d psychocitas -f schema.sql

# 3. O usar EF Migrations
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
dotnet ef database update --project Infrastructure --startup-project API

# 4. Correr
dotnet run --project API
# Swagger en: https://localhost:5001/swagger
```

## Principios aplicados

- **Clean Architecture**: dependencias siempre hacia adentro (API → Application → Domain)
- **CQRS** con MediatR: cada caso de uso es un Command o Query aislado
- **Domain Events**: las entidades emiten eventos, Infrastructure los despacha
- **Unit of Work**: transacciones controladas, domain events flushed antes de SaveChanges
- **Pipeline behaviors**: logging y validación automática antes de cada handler
- **Factory methods**: las entidades no se crean con `new` directo — usan `Crear()` / `Create()`
- **Guard clauses en el dominio**: reglas de negocio dentro de las entidades, no en los handlers
- **Repositorio tipado**: `ICitaRepository` con queries específicas, no solo CRUD genérico

## Próximos pasos sugeridos

1. `ICurrentUser` implementation en Infrastructure/Identity
2. Background Service para envío de recordatorios (Hangfire o .NET Worker)
3. `INotificationService` con Twilio (WhatsApp/SMS) y SendGrid (Email)
4. `IStorageService` con Azure Blob para documentos del paciente
5. Endpoint de autoagenda pública (sin auth)
6. Docker + docker-compose para desarrollo local
