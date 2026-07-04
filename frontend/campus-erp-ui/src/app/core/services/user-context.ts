import { Injectable, inject, signal } from '@angular/core';
import { computed } from '@angular/core';
import { Profile } from '../../features/profile/models/profile';
import { ProfileService } from '../../features/profile/services/profile';
import { RoleConstants } from '../models/role-constants';
import { RoleHelper } from '../helpers/role-helper';

@Injectable({
  providedIn: 'root',
})
export class UserContextService {
  private readonly profileService = inject(ProfileService);

  readonly profile = signal<Profile | null>(null);

  readonly loading = signal(false);

  readonly loaded = signal(false);

  load(force = false): void {
    if (!force && this.loaded()) {
      return;
    }

    if (this.loading()) {
      return;
    }

    this.loading.set(true);

    this.profileService.getMyProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);

        this.loaded.set(true);

        this.loading.set(false);
      },

      error: () => {
        this.loading.set(false);

        this.loaded.set(false);

        this.profile.set(null);
      },
    });
  }

  refresh(): void {
    this.load(true);
  }

  update(profile: Profile): void {
    this.profile.set(profile);

    this.loaded.set(true);
  }

  clear(): void {
    this.profile.set(null);

    this.loaded.set(false);

    this.loading.set(false);
  }

  isLoaded(): boolean {
    return this.loaded();
  }

  readonly displayName = computed(() => this.profile()?.fullName ?? '');

  readonly fullName = computed(() => this.profile()?.fullName ?? '');

  readonly email = computed(() => this.profile()?.email ?? '');

  readonly role = computed(() => this.profile()?.role ?? '');

  readonly roleDisplay = computed(() => RoleHelper.display(this.profile()?.role));

  readonly institution = computed(() => this.profile()?.institutionName ?? '');

  readonly campus = computed(() => this.profile()?.campusName ?? '');

  readonly avatarInitials = computed(() => this.profile()?.avatarInitials ?? '');

  readonly profilePhoto = computed(() => this.profile()?.profilePhotoUrl);

  readonly hasProfilePhoto = computed(() => !!this.profile()?.profilePhotoUrl);

  isStudent(): boolean {
    return this.role() === RoleConstants.Student;
  }

  isTeacher(): boolean {
    return this.role() === RoleConstants.Teacher;
  }

  isAdmin(): boolean {
    return (
      this.role() === RoleConstants.SuperAdmin ||
      this.role() === RoleConstants.PlatformAdmin ||
      this.role() === RoleConstants.InstitutionAdmin ||
      this.role() === RoleConstants.CampusAdmin
    );
  }
}
