namespace MpegMetadata;

public class ProgrammeInformation {

    public string Category { get; set; } = "";
    public string ChannelImageURL { get; set; } = "";
    public string ChannelName { get; set; } = "";
    public string ChannelAffiliate { get; set; } = "";
    public string ChannelNumber { get; set; } = "";
    public int EndTime { get; set; }
    public string EpisodeNumber { get; set; } = "";
    public string EpisodeTitle { get; set; } = "";
    public string ImageURL { get; set; } = "";
    public int OriginalAirdate { get; set; }
    public string ProgramID { get; set; } = "";
    public int RecordEndTime { get; set; }
    public int RecordStartTime { get; set; }
    public int RecordSuccess { get; set; }
    public int Resume { get; set; }
    public string SeriesID { get; set; } = "";
    public int StartTime { get; set; }
    public string Synopsis { get; set; } = "";
    public string Title { get; set; } = "";

}
