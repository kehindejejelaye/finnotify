using System;

namespace Finnotify.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}
