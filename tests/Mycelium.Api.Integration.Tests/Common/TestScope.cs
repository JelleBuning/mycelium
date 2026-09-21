using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OtpNet;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Auth.VerifyTotp.v1;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Api.Users.Register.v1;
using Mycelium.Common.DTO.Device;
using DeviceEntity = Mycelium.Api.EntityFramework.Entities.Device;
using OrganisationEntity = Mycelium.Api.EntityFramework.Entities.Organisation;

namespace Mycelium.Api.Integration.Tests.Common;

public sealed class TestScope : IAsyncDisposable
{
    private readonly ApiFixture _fixture;
    private readonly IServiceScope _scope;
    private Guid? _organisationHash = null;

    public HttpClient Client { get; private set; }
    public AppDbContext DbContext { get; }
    public ApiFixture Fixture => _fixture;

    public OrganisationEntity Organisation => DbContext.Organisations
        .Include(x => x.Devices)
        .Include(x => x.Users)
        .Single(x => x.Hash == _organisationHash);

    public SignInResponse? User { get; private set; }

    public TestScope()
    {
        _fixture = new ApiFixture();
        _scope = _fixture.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DbContext.Database.EnsureCreated();
        Client = _fixture.CreateClient();
    }

    public async Task<TestScope> AuthenticateAsUserAsync()
    {
        var (client, user) = await _fixture.CreateAuthenticatedUserAsync();
        _organisationHash = DbContext.Organisations.Single(x => x.Id == user.OrganisationId).Hash;
        Client = client;
        User = user;

        return this;
    }

    public async Task<TestScope> AuthenticateAsDeviceAsync()
    {
        if (_organisationHash == null)
            await AddOrganisationAsync();

        var (client, device) = await _fixture.CreateAuthenticatedDeviceAsync(_organisationHash ?? throw new Exception("organisation is null"));
        Client = client;

        return this;
    }

    public async Task<TestScope> AddOrganisationAsync()
    {
        var hash = Guid.NewGuid();
        await Fixture.AddOrganisationAsync(hash);
        _organisationHash = hash;
        return this;
    }

    public async Task<TestScope> AddDeviceAsync()
    {
        var device = new DeviceEntity
        {
            Name = "Test Device",
            OrganisationId = Organisation?.Id ?? throw new Exception("Organisation is null. Ensure organisation is added."),
        };
        await Fixture.AddDeviceAsync(device);
        return this;
    }

    public async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        await _fixture.DisposeAsync();
    }
}

public static class HttpClientAuthExtensions
{
    extension(HttpClient client)
    {
        public async Task<SignInResponse> AuthenticateUserAsync()
        {
            var signInResponse = await client.SignInUserAsync();
            var totp = new Totp(Base32Encoding.ToBytes(signInResponse.TwoFactorToken), step: 30,
                mode: OtpHashMode.Sha1, totpSize: 6);

            var verifyTotpCommand = new VerifyTotpCommand(
                signInResponse.UserId,
                signInResponse.AuthenticityToken,
                totp.ComputeTotp());

            var verificationResult = await client.PostAsync("/api/v1/auth/users/verify", verifyTotpCommand);
            var userTokenResponse = await verificationResult.Content.DeserializeAsync<TokenDto>()
                ?? throw new Exception("verification result was null");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", $"{userTokenResponse.AccessToken}");

            return signInResponse;
        }

        public async Task<DeviceTokenResponse> RegisterDeviceAsync(Guid organisationHash)
        {
            var verificationResult = await client.PostAsync("/api/v1/devices/register", new RegisterDeviceRequest
            {
                Name = "John Doe",
                OrganisationHash = organisationHash,
            });

            var deviceTokenResponse = await verificationResult.Content.DeserializeAsync<DeviceTokenResponse>()
                ?? throw new Exception("verification result was null");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", $"{deviceTokenResponse.AccessToken}");

            return deviceTokenResponse;
        }

        private async Task<SignInResponse> SignInUserAsync()
        {
            _ = await client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));

            var signInResult = await client.PostAsync("/api/v1/auth/users/sign_in", new
            {
                Email = "test@test.com",
                Password = "password"
            });

            var response = await signInResult.Content.DeserializeAsync<SignInResponse>();
            if (response == null) throw new Exception("sign_in_response is null");

            return response;
        }
    }
}
