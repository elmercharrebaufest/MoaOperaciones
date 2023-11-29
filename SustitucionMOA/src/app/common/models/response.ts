export interface ApiResponse<T, U = {}> {
  data?: T,
  info?: string;
  error?: string;
  filtros?: U;
  logout?: boolean;
}