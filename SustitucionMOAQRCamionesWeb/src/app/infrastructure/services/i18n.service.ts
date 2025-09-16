import { Injectable, signal, computed, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type SupportedLocale = 'en' | 'es';

export interface TranslationKey {
  [key: string]: string | TranslationKey;
}

const TRANSLATIONS: Record<SupportedLocale, TranslationKey> = {
  en: {
    common: {
      loading: 'Loading...',
      error: 'Error',
      success: 'Success',
      cancel: 'Cancel',
      save: 'Save',
      delete: 'Delete',
      edit: 'Edit',
      create: 'Create',
      search: 'Search',
      filter: 'Filter',
      clear: 'Clear'
    },
    navigation: {
      home: 'Home',
      about: 'About',
      posts: 'Posts',
      users: 'Users',
      todos: 'Todos',
      login: 'Login',
      logout: 'Logout'
    },
    home: {
      welcome: 'Welcome to Angular 20',
      subtitle: 'Modern Angular with Signals and Zoneless Change Detection',
      features: {
        title: 'New Features',
        signals: 'Signal-based State Management',
        zoneless: 'Zoneless Change Detection',
        i18n: 'Built-in Internationalization',
        lazyLoading: 'Advanced Lazy Loading',
        ssr: 'Server-Side Rendering'
      }
    },
    posts: {
      title: 'Posts Management',
      create: 'Create New Post',
      noPosts: 'No posts available',
      author: 'Author'
    },
    auth: {
      login: 'Login',
      logout: 'Logout',
      email: 'Email',
      password: 'Password',
      loginSuccess: 'Login successful',
      loginError: 'Login failed'
    }
  },
  es: {
    common: {
      loading: 'Cargando...',
      error: 'Error',
      success: 'Éxito',
      cancel: 'Cancelar',
      save: 'Guardar',
      delete: 'Eliminar',
      edit: 'Editar',
      create: 'Crear',
      search: 'Buscar',
      filter: 'Filtrar',
      clear: 'Limpiar'
    },
    navigation: {
      home: 'Inicio',
      about: 'Acerca de',
      posts: 'Publicaciones',
      users: 'Usuarios',
      todos: 'Tareas',
      login: 'Iniciar Sesión',
      logout: 'Cerrar Sesión'
    },
    home: {
      welcome: 'Bienvenido a Angular 20',
      subtitle: 'Angular Moderno con Signals y Detección de Cambios sin Zone',
      features: {
        title: 'Nuevas Características',
        signals: 'Gestión de Estado basada en Signals',
        zoneless: 'Detección de Cambios sin Zone',
        i18n: 'Internacionalización Integrada',
        lazyLoading: 'Carga Diferida Avanzada',
        ssr: 'Renderizado del Lado del Servidor'
      }
    },
    posts: {
      title: 'Gestión de Publicaciones',
      create: 'Crear Nueva Publicación',
      noPosts: 'No hay publicaciones disponibles',
      author: 'Autor'
    },
    auth: {
      login: 'Iniciar Sesión',
      logout: 'Cerrar Sesión',
      email: 'Correo Electrónico',
      password: 'Contraseña',
      loginSuccess: 'Inicio de sesión exitoso',
      loginError: 'Error al iniciar sesión'
    }
  }
};

@Injectable({
  providedIn: 'root'
})
export class I18nService {
  private readonly _currentLocale = signal<SupportedLocale>('en');
  private readonly _translations = signal(TRANSLATIONS);

  readonly currentLocale = this._currentLocale.asReadonly();
  readonly availableLocales: SupportedLocale[] = ['en', 'es'];
  
  readonly currentTranslations = computed(() => 
    this._translations()[this._currentLocale()]
  );

  constructor(@Inject(PLATFORM_ID) private readonly platformId: Object) {
    // Detect browser language or use stored preference only in browser
    if (isPlatformBrowser(this.platformId)) {
      const storedLocale = localStorage.getItem('app_locale') as SupportedLocale;
      const browserLocale = navigator.language.slice(0, 2) as SupportedLocale;
      
      if (storedLocale && this.availableLocales.includes(storedLocale)) {
        this._currentLocale.set(storedLocale);
      } else if (this.availableLocales.includes(browserLocale)) {
        this._currentLocale.set(browserLocale);
      }
    }
  }

  setLocale(locale: SupportedLocale): void {
    if (this.availableLocales.includes(locale)) {
      this._currentLocale.set(locale);
      // Solo guardar en localStorage en el navegador
      if (isPlatformBrowser(this.platformId)) {
        localStorage.setItem('app_locale', locale);
      }
    }
  }

  translate(key: string): string {
    const keys = key.split('.');
    let translation: any = this.currentTranslations();

    for (const k of keys) {
      if (translation && typeof translation === 'object' && k in translation) {
        translation = translation[k];
      } else {
        console.warn(`Translation key not found: ${key}`);
        return key; // Return the key if translation not found
      }
    }

    return typeof translation === 'string' ? translation : key;
  }

  // Método helper para templates
  t(key: string): string {
    return this.translate(key);
  }
}
