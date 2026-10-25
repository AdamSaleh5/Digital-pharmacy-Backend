namespace digital_pharmacy_backend.Services.Firebase;

public interface IFirebaseNotificationService
{
    Task<object> SendDataUpdatedAsync(CancellationToken cancellationToken = default);
}
