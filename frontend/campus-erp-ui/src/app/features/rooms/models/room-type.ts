export enum RoomType {
  Classroom = 1,

  ComputerLab = 2,

  ScienceLab = 3,

  ElectronicsLab = 4,

  MechanicalLab = 5,

  CivilLab = 6,

  LanguageLab = 7,

  SeminarHall = 8,

  Auditorium = 9,

  ConferenceRoom = 10,

  StaffRoom = 11,

  Library = 12,

  ExaminationHall = 13,

  SportsGround = 14,

  Other = 15,
}

export const ROOM_TYPES = [
  { value: RoomType.Classroom, name: 'Classroom' },
  { value: RoomType.ComputerLab, name: 'Computer Lab' },
  { value: RoomType.ScienceLab, name: 'Science Lab' },
  { value: RoomType.ElectronicsLab, name: 'Electronics Lab' },
  { value: RoomType.MechanicalLab, name: 'Mechanical Lab' },
  { value: RoomType.CivilLab, name: 'Civil Lab' },
  { value: RoomType.LanguageLab, name: 'Language Lab' },
  { value: RoomType.SeminarHall, name: 'Seminar Hall' },
  { value: RoomType.Auditorium, name: 'Auditorium' },
  { value: RoomType.ConferenceRoom, name: 'Conference Room' },
  { value: RoomType.StaffRoom, name: 'Staff Room' },
  { value: RoomType.Library, name: 'Library' },
  { value: RoomType.ExaminationHall, name: 'Examination Hall' },
  { value: RoomType.SportsGround, name: 'Sports Ground' },
  { value: RoomType.Other, name: 'Other' },
];