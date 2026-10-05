using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface INotificationService
{
    Task NotifyClientAsync(string connectionId, string eventName, object payload, CancellationToken cancellationToken = default);
}
