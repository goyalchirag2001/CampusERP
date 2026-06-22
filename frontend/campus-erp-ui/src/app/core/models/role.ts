import { Permission } from './permission';

export interface Role {
  id: string;

  name: string;

  description?: string;

  isSystemRole: boolean;

  permissionCount: number;

  isActive: boolean;

  permissionIds: string[];

  permissions: Permission[];
}
