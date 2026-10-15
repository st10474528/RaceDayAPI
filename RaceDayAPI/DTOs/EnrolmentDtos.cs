namespace RaceDayAPI.DTOs;

public record EnrolmentDto(
    int EnrolmentID,
    int ParticipantID,
    int EventID,
    int CategoryID,
    DateTime EnrolmentDate,
    string Status);

public record CreateEnrolmentDto(int EventID, int CategoryID);