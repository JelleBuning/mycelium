using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Organisations.GetAll.v1;

public sealed record GetAllOrganisationsQuery : IQuery<Result<List<OrganisationDto>>>;
