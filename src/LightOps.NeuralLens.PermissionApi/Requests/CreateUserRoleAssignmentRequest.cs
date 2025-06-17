namespace LightOps.NeuralLens.PermissionApi.Requests;

public record CreateUserRoleAssignmentRequest(
    string UserId,
    string RoleId,
    string AssignedScope);