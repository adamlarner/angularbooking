import { SiteRoutingModule } from './site-routing.module';

describe('SiteRoutingModule', () => {
  let siteRoutingModule: SiteRoutingModule;

  beforeEach(() => {
    siteRoutingModule = new SiteRoutingModule();
  });

  it('should create an instance', () => {
    expect(siteRoutingModule).toBeTruthy();
  });
});
