namespace LingoHop.Domain.Study;

/// <summary>Lifecycle of a single "Start today's lesson" run.</summary>
public enum StudySessionStatus
{
    InProgress = 0,

    /// <summary>Finished normally - XP and streak have been awarded.</summary>
    Completed = 1,

    /// <summary>Left before answering anything meaningful - no reward.</summary>
    Abandoned = 2,
}
