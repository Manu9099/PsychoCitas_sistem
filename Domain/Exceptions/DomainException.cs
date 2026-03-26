namespace PsychoCitas.Domain.Exceptions;

public class DomainException(string message) : Exception(message) { }
public class NotFoundException(string entity, object key) 
    : Exception($"{entity} con id '{key}' no fue encontrado.") { }
public class ConflictoAgendaException(DateTime inicio, DateTime fin)
    : Exception($"Existe una cita que se solapa con el horario {inicio:HH:mm} - {fin:HH:mm}.") { }
