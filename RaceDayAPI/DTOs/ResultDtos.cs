namespace RaceDayAPI.DTOs;

public record ResultDto(
    int ResultID,
    int EnrolmentID,
    string FinishTime,
    int FinishingPosition,
    DateTime RecordedAt);

public record CreateResultDto(int EnrolmentID, string FinishTime, int FinishingPosition);