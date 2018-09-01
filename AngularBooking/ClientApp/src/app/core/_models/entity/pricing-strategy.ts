import { PricingStrategyItem } from "./pricing-strategy-item";

export class PricingStrategy {
  constructor(
    public id?: number,
    public name?: string,
    public description?: string,
    public pricingStrategyItems?: PricingStrategyItem[]
  ) { }
}
