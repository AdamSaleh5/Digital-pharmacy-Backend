using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace digital_pharmacy_backend.Services.Firebase;

public sealed class FirebaseNotificationService : IFirebaseNotificationService
{
    private const string DataUpdatedMessage = "تم تحديث البيانات";
    private const int MaxTokensPerBatch = 500;
    private static readonly object FirebaseAppLock = new();

    private readonly FirebaseOptions options;
    private readonly IGenericRepository genericRepository;

    public FirebaseNotificationService(
        IOptions<FirebaseOptions> options,
        IGenericRepository genericRepository)
    {
        this.options = options.Value;
        this.genericRepository = genericRepository;
    }

    public async Task<object> SendDataUpdatedAsync(CancellationToken cancellationToken = default)
    {
        var tokens = genericRepository
            .GetAll<User>()
            .Where(u => u.Role == UserRole.User && !string.IsNullOrWhiteSpace(u.DeviceIp))
            .Select(u => u.DeviceIp!)
            .Distinct()
            .ToList();

        if (tokens.Count == 0)
        {
            throw new InvalidOperationException("No user device tokens found.");
        }

        EnsureFirebaseAppIsInitialized();

        var successCount = 0;
        var failureCount = 0;

        foreach (var batch in tokens.Chunk(MaxTokensPerBatch))
        {
            var multicast = new MulticastMessage
            {
                Tokens = batch.ToList(),
                Notification = new Notification
                {
                    Body = DataUpdatedMessage
                }
            };

            var response = await FirebaseMessaging.DefaultInstance
                .SendEachForMulticastAsync(multicast, cancellationToken);

            successCount += response.SuccessCount;
            failureCount += response.FailureCount;
        }

        return new
        {
            TotalTokens = tokens.Count,
            SuccessCount = successCount,
            FailureCount = failureCount
        };
    }

    private void EnsureFirebaseAppIsInitialized()
    {
        if (FirebaseApp.DefaultInstance is not null)
        {
            return;
        }

        lock (FirebaseAppLock)
        {
            if (FirebaseApp.DefaultInstance is not null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(options.ServiceAccountPath))
            {
                throw new InvalidOperationException("Firebase:ServiceAccountPath is not configured.");
            }

            var serviceAccountPath = Path.GetFullPath(options.ServiceAccountPath);
            if (!File.Exists(serviceAccountPath))
            {
                throw new FileNotFoundException("Firebase service-account JSON file was not found.", serviceAccountPath);
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath)
            });
        }
    }
}
