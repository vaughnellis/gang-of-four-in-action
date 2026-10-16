namespace CornerStore;

internal sealed record CommandAuditEntry(string CommandName, string Result);
