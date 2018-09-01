import { AgeRatingType } from "./age-rating-type";

export class Event{
  constructor(
    public id?: number,
    public name?: string,
    public description?: string,
    public image?: string,
    public duration?: number,
    public ageRating?: AgeRatingType
  ) { }
}
