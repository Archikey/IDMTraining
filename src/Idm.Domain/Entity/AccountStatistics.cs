namespace Idm.Domain.Entities;

public sealed record AccountStatistics(
    string SystemName,
    AccountType AccountType,
    long Count);