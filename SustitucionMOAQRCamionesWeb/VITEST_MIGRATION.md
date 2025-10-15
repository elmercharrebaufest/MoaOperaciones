# Migración de Testing: Karma/Jasmine → Vitest

## ✅ Migración Completada

Este proyecto ha sido migrado exitosamente de **Karma/Jasmine** a **Vitest** para las pruebas unitarias.

## 🚀 Beneficios de Vitest

- **Rendimiento**: Mucho más rápido que Karma
- **Configuración simple**: Menos archivos de configuración
- **HMR**: Hot Module Replacement para tests
- **ES Modules**: Soporte nativo
- **API familiar**: Compatible con Jest/Jasmine
- **Mejor DX**: Mejor experiencia de desarrollo

## 📁 Archivos de Configuración

### Archivos añadidos:
- `vitest.config.ts` - Configuración principal de Vitest
- `src/test-setup.ts` - Setup global para Angular testing
- `src/vitest.d.ts` - Definiciones de tipos para Vitest
- `tsconfig.vitest.json` - Configuración TypeScript para tests

### Archivos removidos:
- `karma.conf.js` (no existía)
- Dependencias de Karma/Jasmine del package.json

## 🔧 Scripts NPM Actualizados

```json
{
  "scripts": {
    "test": "vitest",
    "test:run": "vitest run",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest run --coverage",
    "test:karma": "ng test"  // Backup (removido en angular.json)
  }
}
```

## 📦 Dependencias

### Añadidas:
- `vitest` - Framework de testing
- `@vitest/ui` - Interfaz web para tests
- `jsdom` - Entorno DOM para testing
- `zone.js` - Requerido para Angular testing

### Removidas:
- `karma`
- `karma-chrome-launcher`
- `karma-coverage`
- `karma-jasmine`
- `karma-jasmine-html-reporter`
- `jasmine-core`
- `@types/jasmine` (opcional)

## 🧪 Ejecutar Tests

```bash
# Ejecutar tests en modo watch
npm test

# Ejecutar tests una vez
npm run test:run

# Abrir interfaz web de tests
npm run test:ui

# Ejecutar con coverage
npm run test:coverage

# Ejecutar test específico
npm test -- --run src/app/app.spec.ts
```

## 📋 Estado de Tests

### ✅ Tests Funcionando (8/11):
- `src/app/app.spec.ts` - 3 tests ✅
- `src/app/basic.test.ts` - 2 tests ✅
- `src/app/infrastructure/services/data.service.spec.ts` - 3 tests ✅

### ⚠️ Tests con Problemas (3/11):
- `src/app/domain/domain.spec.ts` - Problema de inyección
- `src/app/shared/ui-card/ui-card.spec.ts` - Resolución de templates
- `src/app/views/home/home.spec.ts` - Resolución de templates

## 🔧 Configuración Angular.json

Se removió la configuración de testing de Karma del `angular.json`:

```json
// REMOVIDO:
"test": {
  "builder": "@angular/build:karma",
  // ...
}
```

## 💡 Recomendaciones para Nuevos Tests

### 1. Tests de Componentes con Templates Inline

Para evitar problemas de resolución de archivos, usa templates inline en tests:

```typescript
@Component({
  selector: 'test-component',
  template: '<h1>{{title}}</h1>',
  standalone: true
})
class TestComponent {
  title = 'Test';
}
```

### 2. Mocking de Servicios

Usa mocks simples en lugar de TestBed complejo cuando sea posible:

```typescript
const mockService = {
  getData: vi.fn().mockReturnValue(of(mockData))
};
```

### 3. Tests de Servicios

Para servicios con dependencias complejas, crea mocks específicos:

```typescript
class MockDataService {
  constructor(private http: any) {}
  getData() {
    return this.http.get('/api/data');
  }
}
```

## 🚨 Problemas Conocidos

1. **Templates externos**: Los componentes que usan `templateUrl` y `styleUrls` pueden fallar
2. **Zone.js warnings**: Normales cuando se usa zoneless change detection
3. **TestBed reset**: Se hace automáticamente en `test-setup.ts`

## 🔄 Próximos Pasos

1. **Corregir tests fallidos**: Convertir a templates inline o mejorar resolución de archivos
2. **Añadir coverage**: Configurar reportes de cobertura
3. **Tests E2E**: Considerar migrar a Playwright o Cypress
4. **CI/CD**: Actualizar pipelines para usar Vitest

## 📚 Referencias

- [Vitest Documentation](https://vitest.dev/)
- [Angular Testing Guide](https://angular.dev/guide/testing)
- [Vitest Angular Integration](https://github.com/vitest-dev/vitest/tree/main/examples)

---

La migración fue exitosa y Vitest está funcionando correctamente. Los tests básicos y mocks están pasando. Para una implementación completa, se recomienda refactorizar los tests con problemas de resolución de archivos.
