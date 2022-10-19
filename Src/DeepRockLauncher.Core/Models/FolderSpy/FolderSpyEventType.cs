namespace DeepRockLauncher.Core.Models.FolderSpy;

public enum FolderSpyEventType
{
    Unknown,
    Error,
    // Object was changed
    Changed,
    // Object was created
    Created,
    // Object was deleted
    Deleted,
    // Object was Renamed - Note: Objects being moved around will register as Renamed
    Renamed
}