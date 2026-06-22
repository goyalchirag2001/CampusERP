export interface CreateRoleRequest {
  name: string;

  description?: string;

  permissionIds: string[];
}
