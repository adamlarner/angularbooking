import { Showing } from "./showing";

export class Room{
  constructor(
    public id?: number,
    public venueId?: number,
    public name?: string,
    public description?: string,
    public rows?: number,
    public columns?: number,
    public isles?: string,
    public showings?: Showing[]
  ) { }
}
