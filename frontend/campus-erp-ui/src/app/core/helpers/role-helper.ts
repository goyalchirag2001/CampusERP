import { RoleConstants } from '../models/role-constants';

export class RoleHelper {
  static display(role?: string): string {
    switch (role) {
      case RoleConstants.SuperAdmin:
        return 'Super Admin';

      case RoleConstants.PlatformAdmin:
        return 'Platform Admin';

      case RoleConstants.InstitutionAdmin:
        return 'Institution Admin';

      case RoleConstants.CampusAdmin:
        return 'Campus Admin';

      case RoleConstants.Student:
        return 'Student';

      case RoleConstants.Teacher:
        return 'Teacher';

      default:
        return role ?? '';
    }
  }
}
