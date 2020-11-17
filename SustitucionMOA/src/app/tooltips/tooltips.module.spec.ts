import { TooltipsModule } from './tooltips.module';

describe('TooltipsModule', () => {
  let tooltipsModule: TooltipsModule;

  beforeEach(() => {
    tooltipsModule = new TooltipsModule();
  });

  it('should create an instance', () => {
    expect(tooltipsModule).toBeTruthy();
  });
});
