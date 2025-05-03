using System.Diagnostics.Metrics;

namespace Notes.API.Diagnostics;

public static class DiagnosticsConfig
{
    public const string Name = "Notes";

    public static Meter Meter = new(Name);

    public static Counter<int> NotesCounter = Meter.CreateCounter<int>("notes.count");
    public static Counter<int> NotesDescriptionTooLong = Meter.CreateCounter<int>("notes.description.too_long");
}
