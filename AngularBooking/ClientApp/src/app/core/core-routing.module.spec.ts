import { CoreRoutingModule } from './core-routing.module';

describe('CoreRoutingModule', () => {
  let coreRoutingModule: CoreRoutingModule;

  beforeEach(() => {
    coreRoutingModule = new CoreRoutingModule();
  });

  it('should create an instance', () => {
    expect(coreRoutingModule).toBeTruthy();
  });
});
