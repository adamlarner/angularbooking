import { FacilityFlags } from "./facility-flags";
import { Room } from "./room";

export class Venue {
  constructor(
    public id?: number,
    public name?: string,
    public description?: string,
    public image?: string,
    public address1?: string,
    public address2?: string,
    public address3?: string,
    public address4?: string,
    public address5?: string,
    public contactPhone?: string,
    public latLong?: string,
    public website?: string,
    public facebook?: string,
    public twitter?: string,
    public instagram?: string,
    public facilities?: FacilityFlags,
    public rooms?: Room[]
  ) { }
}
