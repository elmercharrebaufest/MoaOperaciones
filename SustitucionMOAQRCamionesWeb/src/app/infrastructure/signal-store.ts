// Minimal Signal Store pattern (framework-agnostic)
import { signal, computed } from '@angular/core';


export function createThemeStore() {
    const mode = signal<'light' | 'dark'>('light');
    const isDark = computed(() => mode() === 'dark');
    const toggle = () => mode.update(m => (m === 'light' ? 'dark' : 'light'));
    return { mode, isDark, toggle } as const;
}