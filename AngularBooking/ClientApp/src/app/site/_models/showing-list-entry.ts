import { ShowingListEntryRoom } from "./showing-list-entry-room";
import { AgeRatingType } from "../../core/_models/entity/age-rating-type";
import { FacilityFlags } from "../../core/_models/entity/facility-flags";

export interface ShowingListEntry {
  id?: number,
  type?: string,
  name?: string,
  description?: string,
  image?: string,
         
  // venue
  facilities?: FacilityFlags,
  address1?: string,
  address2?: string,
  address3?: string,
  address4?: string,
  address5?: string,
  contact?: string,

  // event
  ageRating?: AgeRatingType,
  duration?: string

  // rooms
  rooms?: ShowingListEntryRoom[]

}
