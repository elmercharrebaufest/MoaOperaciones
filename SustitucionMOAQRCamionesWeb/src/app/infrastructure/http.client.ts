// Example HttpClient provider placeholder (no requests in hello world)
import { provideHttpClient, withFetch } from '@angular/common/http';
export const HTTP_PROVIDERS = [provideHttpClient(withFetch())];