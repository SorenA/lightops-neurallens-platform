using LightOps.Mapping.Api.Mappers;
using LightOps.NeuralLens.PermissionApi.Domain.Models;
using LightOps.NeuralLens.PermissionApi.Models;

namespace LightOps.NeuralLens.PermissionApi.Domain.Mappers;

public class UserRoleAssignmentViewModelMapper : IMapper<UserRoleAssignment, UserRoleAssignmentViewModel>
{
    public UserRoleAssignmentViewModel Map(UserRoleAssignment source)
    {
        return new UserRoleAssignmentViewModel(
            source.Id,
            source.UserId,
            source.RoleId,
            source.AssignedScope,
            source.CreatedAt);
    }
}