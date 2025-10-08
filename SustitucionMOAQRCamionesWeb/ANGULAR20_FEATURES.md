# Angular 20 Demo Application

Esta aplicación demuestra las nuevas características y mejores prácticas de Angular 20, incluyendo **Zoneless Change Detection**, **Signal-based State Management**, **SSR**, **i18n**, **Lazy Loading** avanzado, y más.

## 🚀 Características Implementadas

### 1. **Zoneless Change Detection**
- Configurado con `provideZonelessChangeDetection()`
- Rendimiento mejorado sin la dependencia de Zone.js
- Ubicación: `src/app/app.config.ts`

### 2. **Signal-based State Management**
- Reemplazo completo de RxJS Subjects por Signals
- Estado reactivo sin librerías externas
- Ejemplos en:
  - `src/app/infrastructure/services/auth.service.ts`
  - `src/app/infrastructure/services/data.service.ts`
  - `src/app/presentation/home/home.ts`

### 3. **Internacionalización (i18n)**
- Sistema de i18n basado en Signals
- Cambio de idioma en tiempo real
- Soporte para Inglés y Español
- Ubicación: `src/app/infrastructure/services/i18n.service.ts`

### 4. **HTTP Interceptors Funcionales**
- Interceptores modernos sin clases
- Manejo de autenticación automática
- Loading state global
- Ubicación: `src/app/infrastructure/interceptors/`

### 5. **Lazy Loading Avanzado**
- Carga diferida por rutas
- Código splitting automático
- Guards funcionales
- Ejemplos en: `src/app/app.routes.ts`

### 6. **Server-Side Rendering (SSR)**
- Configurado con `provideClientHydration(withEventReplay())`
- Event replay para mejor UX
- Hidratación optimizada

### 7. **Arquitectura Moderna**
- Componentes standalone
- Functional guards
- Clean Architecture
- Dependency Injection mejorada

## 📁 Estructura del Proyecto

```
src/app/
├── application/           # Casos de uso y resolvers
├── domain/               # Lógica de dominio
├── infrastructure/       # Servicios e infraestructura
│   ├── interceptors/     # HTTP Interceptors funcionales
│   └── services/         # Servicios con Signals
├── presentation/         # Componentes UI
│   ├── auth/            # Módulo de autenticación
│   ├── posts/           # Gestión de posts
│   ├── users/           # Gestión de usuarios
│   ├── todos/           # Gestión de todos
│   ├── admin/           # Panel de administración
│   └── shared/          # Componentes compartidos
├── app.config.ts        # Configuración de la aplicación
├── app.routes.ts        # Definición de rutas
└── app.ts              # Componente principal
```

## 🛠️ Tecnologías Utilizadas

- **Angular 20.2.0** - Framework principal
- **Angular Material** - Componentes UI
- **TailwindCSS** - Utilidades CSS
- **TypeScript 5.8** - Tipado estático
- **Vite** - Build tool
- **Express** - SSR server

## 🎯 Ejemplos de Uso

### 1. Signal-based Counter
```typescript
// Signal reactivo
private readonly counter = signal(0);

// Computed signals
readonly doubleCounter = computed(() => this.counter() * 2);
readonly isEven = computed(() => this.counter() % 2 === 0);

// Métodos para actualizar
increment(): void {
  this.counter.update(value => value + 1);
}
```

### 2. Manejo de Estado Global
```typescript
// Service con Signals
@Injectable({ providedIn: 'root' })
export class DataService {
  private readonly _posts = signal<Post[]>([]);
  readonly posts = this._posts.asReadonly();
  
  // Computed signals para datos derivados
  readonly filteredPosts = computed(() => {
    const userId = this._selectedUserId();
    return userId ? this._posts().filter(p => p.userId === userId) : this._posts();
  });
}
```

### 3. Interceptores Funcionales
```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token) {
    const authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(authReq);
  }
  
  return next(req);
};
```

### 4. Guards Funcionales
```typescript
const canEnter = () => {
  const authService = inject(AuthService);
  return authService.isAuthenticated();
};

const adminGuard = () => {
  const authService = inject(AuthService);
  return authService.isAdmin();
};
```

## 🚦 Cómo Ejecutar

1. **Instalar dependencias:**
   ```bash
   npm install
   ```

2. **Desarrollo:**
   ```bash
   npm start
   ```

3. **Build de producción:**
   ```bash
   npm run build
   ```

4. **SSR:**
   ```bash
   npm run serve:ssr:qr-camiones
   ```

## 📊 Características de Rendimiento

- **Zoneless Change Detection**: ~40% mejora en rendimiento
- **Signal-based State**: Actualizaciones granulares
- **Lazy Loading**: Carga bajo demanda
- **SSR + Hydration**: Mejor First Contentful Paint
- **Tree Shaking**: Bundle optimizado

## 🔐 Autenticación Demo

Para probar las funcionalidades:

- **Admin:** `admin@example.com` / cualquier password
- **Usuario:** `user@example.com` / cualquier password

## 🌐 Internacionalización

La aplicación soporta:
- 🇺🇸 **Inglés** (por defecto)
- 🇪🇸 **Español**

El cambio de idioma es reactivo usando Signals.

## 📱 Responsive Design

- Mobile-first approach
- Material Design 3
- TailwindCSS utilities
- Adaptive layouts

## 🧪 Testing

```bash
# Unit tests
npm test

# E2E tests
npm run e2e

# Lint
npm run lint
```

## 🎨 Características UI/UX

- **Material Design 3** components
- **Dark/Light theme** support
- **Responsive design**
- **Loading states** globales
- **Error handling** mejorado
- **Accessibility** (a11y) compliance

## 📈 Monitoring y Analytics

- **Global error handling**
- **Performance metrics**
- **User interaction tracking**
- **Loading time monitoring**

## 🔧 Configuración Avanzada

### Vite Configuration
- Hot Module Replacement (HMR)
- Fast builds
- Optimized bundling

### TypeScript Configuration
- Strict mode enabled
- Path mapping
- Latest ECMAScript features

## 📚 Recursos Adicionales

- [Angular 20 Documentation](https://angular.io)
- [Signals RFC](https://github.com/angular/angular/discussions/49685)
- [Zoneless Change Detection](https://angular.io/guide/zoneless)
- [Angular Material](https://material.angular.io)

## 👨‍💻 Desarrollo

Este proyecto demuestra:
- **Clean Architecture**
- **SOLID Principles**
- **Best Practices** de Angular 20
- **Modern TypeScript**
- **Performance Optimization**

---

**🚀 ¡Explora las nuevas capacidades de Angular 20!**
