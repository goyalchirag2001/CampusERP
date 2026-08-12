export interface Room {
  id: string;

  institutionId: string;

  campusId: string;

  campusName: string;

  building: string;

  floor: string;

  roomNumber: string;

  roomName: string;

  roomType: string;

  capacity: number;

  hasProjector: boolean;

  hasSmartBoard: boolean;

  hasAirConditioning: boolean;

  hasComputers: boolean;

  hasInternet: boolean;

  description: string | null;

  locationCode: string | null;

  displayOrder: number;

  isAccessible: boolean;

  isActive: boolean;
}