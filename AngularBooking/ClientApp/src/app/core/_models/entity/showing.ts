import { Event } from "./event";
import { Room } from "./room";
import { PricingStrategy } from "./pricing-strategy";

export class Showing {

  constructor(
    public id?: number,
    public eventId?: number,
    public roomId?: number,
    public startTime?: string,
    public endTime?: string,
    public pricingStrategyId?: number,
    public event?: Event,
    public room?: Room,
    public pricingStrategy?: PricingStrategy
  ) { }
}
