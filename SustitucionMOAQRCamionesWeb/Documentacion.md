# Documentación del Proyecto QR Camiones (Angular 20 Demo)

## Índice

- [Descripción General](#descripción-general)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Dependencias y Versiones](#dependencias-y-versiones)
- [Scripts NPM](#scripts-npm)
- [Configuraciones Principales](#configuraciones-principales)
- [Módulos y Componentes](#módulos-y-componentes)
- [Servicios](#servicios)
- [Internacionalización (i18n)](#internacionalización-i18n)
- [Testing](#testing)
- [Estilos y Diseño](#estilos-y-diseño)
- [SSR (Server Side Rendering)](#ssr-server-side-rendering)
- [Ambientes](#ambientes)
- [Recursos y Referencias](#recursos-y-referencias)

---

## Descripción General

Aplicación demo Angular 20 que implementa las últimas características del framework: Signals, Zoneless Change Detection, SSR, i18n, Lazy Loading, arquitectura moderna y gestión de estado reactiva. El objetivo es servir como referencia de buenas prácticas y arquitectura limpia.

---

## Estructura del Proyecto

```
src/
├── app/
│   ├── application/           # Casos de uso y resolvers
│   ├── domain/                # Lógica de dominio y entidades
│   ├── infrastructure/        # Servicios, API, i18n, interceptors
│   ├── shared/                # Componentes y utilidades compartidas
│   ├── views/                 # Componentes de presentación (Home, Posts, Users, Todos, Admin, About)
│   └── app.ts                 # Componente raíz
├── environments/              # Configuración de entornos
├── styles/                    # Utilidades y variables SCSS
├── index.html                 # HTML principal
└── main.ts                    # Bootstrap Angular
```

---

## Dependencias y Versiones

Las principales dependencias se encuentran en [package.json](package.json):

- **Angular**: ^20.3.1 (core, common, compiler, forms, localize, platform-browser, platform-server, router)
- **Angular Material**: ^20.2.4
- **Angular CDK**: ^20.2.4
- **Angular CLI**: ^20.3.2
- **TailwindCSS**: ^4.1.13
- **tw-colors**: ^3.3.2
- **Vitest**: ^3.2.4 (testing)
- **zone.js**: ^0.15.1 (testing)
- **Express**: ^5.1.0 (SSR)
- **TypeScript**: ^5.9.2
- **ESLint**: ^9.36.0
- **angular-eslint**: ^20.3.0
- **typescript-eslint**: ^8.44.0
- **jsdom**: ^27.0.0
- **@types/node**: ^24.5.2

Para ver todas las dependencias y versiones exactas, consulta el archivo [package.json](package.json).

---

## Scripts NPM

- `npm start` — Levanta el servidor de desarrollo Angular.
- `npm run build` — Compila la aplicación para producción.
- `npm test` — Ejecuta los tests unitarios con Vitest.
- `npm run test:ui` — Interfaz web de tests.
- `npm run test:coverage` — Cobertura de tests.
- `npm run serve:ssr:qr-camiones` — Sirve la app con SSR (Express).

---

## Configuraciones Principales

- **Angular CLI**: [angular.json](angular.json) define estilos globales, assets, entornos y budgets.
- **TypeScript**: [tsconfig.json](tsconfig.json) y variantes para app, tests y Vitest.
- **Tailwind**: [tailwind.config.js](tailwind.config.js) con integración de paleta personalizada.
- **Vitest**: [vitest.config.ts](vitest.config.ts) para testing moderno.

---

## Módulos y Componentes

### Componente Principal

- [`App`](src/app/app.ts): Componente raíz, gestiona navegación, idioma, autenticación y loading global.

### Vistas (Presentación)

- [`Home`](src/app/views/home/home.ts): Página principal, muestra features, stats en vivo, demo de signals y acceso rápido.
- [`Posts`](src/app/views/posts/): Gestión de publicaciones, filtros, listado y detalles.
- [`Users`](src/app/views/users/): Gestión de usuarios, cards con info y meta, estados online/offline.
- [`Todos`](src/app/views/todos/): Gestión de tareas, cards con estados, stats, tabs y acciones.
- [`Admin`](src/app/views/admin/): Dashboard solo para admins, muestra control de acceso y stats.
- [`About`](src/app/views/about/about.ts): Página informativa sobre la empresa, misión, visión, stack y contacto.

### Componentes Compartidos

- [`UiCard`](src/app/shared/ui-card/ui-card.ts): Card reutilizable con slots para título, subtítulo y contenido.
- [`material.ts`](src/app/shared/material.ts): Agregador de imports de Angular Material para standalone components.

---

## Servicios

- [`AuthService`](src/app/infrastructure/services/auth.service.ts): Autenticación, roles y estado reactivo.
- [`DataService`](src/app/infrastructure/services/data.service.ts): Gestión de datos (posts, users, todos) usando Signals.
- [`ApiService`](src/app/infrastructure/services/api.service.ts): Abstracción de llamadas HTTP a la API backend.
- [`I18nService`](src/app/infrastructure/services/i18n.service.ts): Internacionalización reactiva con Signals.
- [`LoadingService`](src/app/infrastructure/services/loading.service.ts): Estado global de loading.

---

## Internacionalización (i18n)

- Soporte para inglés y español.
- Cambio de idioma reactivo usando Signals.
- Traducciones centralizadas en [`I18nService`](src/app/infrastructure/services/i18n.service.ts).
- Ejemplo de uso: menú de idioma en el toolbar y textos dinámicos en features.

---

## Testing

- Framework: **Vitest** (más rápido y moderno que Karma/Jasmine).
- Tests ubicados junto a los componentes o en archivos `.spec.ts`.
- Uso de templates inline recomendado para evitar problemas de resolución.
- Mocking de servicios y dependencias para tests unitarios.
- Ver detalles y recomendaciones en [VITEST_MIGRATION.md](VITEST_MIGRATION.md).

---

## Estilos y Diseño

- **SCSS** con variables CSS centralizadas para colores, spacing, tipografía, bordes y sombras.
- **TailwindCSS** para utilidades rápidas y responsive.
- **Material Design** customizado con mejoras visuales y micro-interacciones.
- Utilidades y helpers en [`styles/utilities.scss`](src/styles/utilities.scss).
- Mejoras detalladas en [DESIGN_IMPROVEMENTS.md](DESIGN_IMPROVEMENTS.md).

---

## SSR (Server Side Rendering)

- Implementado con Express y Angular SSR.
- Entrypoint: [`src/server.ts`](src/server.ts).
- Hidratación y event replay para mejor UX.
- Comando: `npm run serve:ssr:qr-camiones`.

---

## Ambientes

- Configuración de endpoints y API keys en [`src/environments/`](src/environments/).
- Soporte para producción, QA, pre-producción y local.
- Cambios de entorno mediante `fileReplacements` en [angular.json](angular.json).

---

## Recursos y Referencias

- [Angular 20 Documentation](https://angular.io)
- [Vitest Documentation](https://vitest.dev/)
- [Angular Material](https://material.angular.io)
- [TailwindCSS](https://tailwindcss.com)
- [Express](https://expressjs.com)

---

## Notas Finales

- Arquitectura basada en Clean Architecture y principios SOLID.
- Uso extensivo de Signals para estado reactivo y desacoplado.
- Código preparado para escalabilidad, mantenibilidad y performance.
- Para detalles de features y ejemplos de código, consulta [ANGULAR20_FEATURES.md](ANGULAR20_FEATURES.md).

---
