using System.Text.RegularExpressions;
using LightOps.NeuralLens.Component.ServiceDefaults;

namespace LightOps.NeuralLens.PermissionApi.Domain.Services;

public class AssignedScopeBuilder
{
    private static readonly Regex OrganizationScopeRegex = new(@"^\/organizations\/([a-zA-Z0-9-]*)", RegexOptions.Compiled);
    private static readonly Regex WorkspaceScopeRegex = new(@"^\/organizations\/([a-zA-Z0-9-]*)\/workspaces\/([a-zA-Z0-9-]*)$", RegexOptions.Compiled);

    public List<string> BuildUpstreamAssignedScopes(params string[] assignedScopes)
    {
        // Add global, organization, and workspace scopes by default, along with provided scopes
        var scopes = new List<string>
        {
            AuthAssignableScopes.Global,
            AuthAssignableScopes.Organization,
            AuthAssignableScopes.Workspace,
        };
        scopes.AddRange(assignedScopes);

        foreach (var assignedScope in assignedScopes)
        {
            // Extend assigned scopes nested under organizations to include organizations
            // Eg. /organization/org1/workspace/wsp1 to /organization/org1)
            var organizationMatch = OrganizationScopeRegex.Match(assignedScope);
            if (organizationMatch is { Success: true, Captures.Count: > 0 })
            {
                scopes.Add(AuthAssignedScopes.Organization(organizationMatch.Groups[1].Value));
            }
        }

        return scopes
            .Distinct()
            .ToList();
    }
}